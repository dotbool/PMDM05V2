using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //MOVIMIENTO
    public Rigidbody2D rigidbody2d;
    private Vector2 move;
    private float speed;

    float timeToJump;
    float jumpCoolDown = .3f;
    bool canJump;

    [SerializeField] private Move leftMove;
    [SerializeField] private Move rightMove;
    [SerializeField] private Move jumpMove;

    private bool isIdle;
    private bool isPushLeft;
    public bool IsPushLeft
    {
        get { return isPushLeft; }
        set { isPushLeft = value; }
    }

    private bool isPushRight;
    public bool IsPushRight
    {
        get { return isPushRight; }
        set { isPushRight = value; }
    }

    private bool isPushjump;
    public bool IsPushJump
    {
        get { return isPushjump; }
        set { isPushjump = value; }
    }

    public bool IsFacingRight { get; set; }
    public Vector2 Move {
        get { return move; }
        set { move = value; }
    }
    public bool IsGrounded { get; set; }
    public bool IsFalling { get; set; }
    public bool IsJumping { get; set; }
    public float LastInAirY { get; set; }




    private PlayerStateMachine playerStateMachine;
    public PlayerStateMachine PlayerStateMachine => playerStateMachine;


    //---------------------ANIMACIONES--------------------------------------

    private SpriteRenderer spriteRenderer;
    public Animator Animator { get; set; }



    //-----------------------HEALTH-----------------------------------------
    public int MaxHealth { get; set; } = 3;
    public int CurrentHealth { get; set; } = 3;
    public bool IsHurt { get; set; }

    //Cooldown para cuando es herido
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float damageCooldown;


    //Avisa a la UI de cambios en el health
    public event Action HealthChange;

    //--------------------------------------------------------------------------
    public InputAction MoveAction;



    //----------------------------AUDIO------------------------------------------

    private AudioSource audioSource;



    //----------------------- COLLECTIBLES ---------------------------------------
    public event Action<int> CoinCollected;
    public int Coins { get; set; }


    private void Awake()
    {
        //Move. Asignamos los callBacks para cuando los botones
        //sean pulsados. Establecen las variables isMoving y Jump
        //if (leftMove != null)
        //{
        //    leftMove.MoveHappened += OnLeftMoveHappened;
        //}
        //if (rightMove != null)
        //{
        //    rightMove.MoveHappened += OnRightMoveHappened;
        //}
        //if (jumpMove != null)
        //{
        //    jumpMove.MoveHappened += OnJumpMoveHappened;
        //}
        audioSource = GetComponent<AudioSource>();
        playerStateMachine = new PlayerStateMachine(this);
        GameManager.Instance.Player = this;
        GameManager.Instance.GameStateChanged += OnGameStateChanged;

    }

    private void OnGameStateChanged(GameState state)
    {
        MoveAction.Disable();
    }

    /// <summary>
    /// Las acciones de movimiento son establecidas cada vez
    /// que un botón es pulsado (true) o no pulsado(false)
    /// </summary>
    /// <param name="moveLeft"></param>
    private void OnLeftMoveHappened(bool moveLeft)
    {
        isPushLeft = moveLeft;
    }
    private void OnRightMoveHappened(bool moveRight)
    {
        IsPushRight = moveRight;
    }
    private void OnJumpMoveHappened(bool moveJump)
    {
        IsPushJump = moveJump;

    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        speed = 3.0f;
        IsFacingRight = true;

        //ANIMACIONES
        spriteRenderer = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();


        playerStateMachine.Initialize(playerStateMachine.idleState);
        MoveAction.Enable();

        //AUDIO
        audioSource.enabled = GameManager.Instance.settings.IsSfxOn;


    }


    // Update is called once per frame
    /// <summary>
    /// Averiguamos que teclas están siendo pulsadas y determinamos
    /// el estado del player en cada frame
    /// </summary>
    void Update()
    {
        Move = MoveAction.ReadValue<Vector2>();
        isPushLeft = Move.x < 0;
        isPushRight = Move.x > 0;
        IsPushJump = Move.y > 0;

        if (IsPushRight)
        {
            IsFacingRight = true;
        }
        if (IsPushLeft)
        {
            IsFacingRight = false;
        }

        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown < 0)
            {
                isInvincible = false;
            }
        }

        if (!canJump)
        {
            timeToJump -= Time.deltaTime;
            if (timeToJump <= 0)
            {
                canJump = true;
            }
        }
        playerStateMachine.Execute();

    }

    /// <summary>
    /// Hacemos el movimiento físico en función del estado del player
    /// FixedUpdate should be used instead of Update when dealing with Rigidbody.
    /// For example when adding a force to a rigidbody, you have to apply the 
    /// force every fixed frame inside FixedUpdate instead of every frame inside Update.
    /// </summary>
    private void FixedUpdate()
    {
        //CalcularClimbing();
        CalcularSalto();
        DoMove();
    }
    void DoMove()
    {

        //Climbing no es un estado en sí sino una variante de running. Cómo sólo
        //cambia el Move lo ejecutamos en fixedupdate
        Vector2 position;
        if (canJump && IsPushJump && IsGrounded)
        {
            Jump();
            position = (Vector2)rigidbody2d.position + speed * Time.deltaTime * Move;

        }
        else
        {
            position = (Vector2)rigidbody2d.position + speed * Time.deltaTime * new Vector2(Move.x,0);

        }
        rigidbody2d.position = position;
        spriteRenderer.flipX = !IsFacingRight;
    }

    /// <summary>
    /// Averiguamos si el player está en tierra o en el aire
    /// </summary>
    public void CalcularSalto()
    {
        RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position, Vector2.down, .5f, LayerMask.GetMask("Ground"));
        //RaycastHit2D hitHill = Physics2D.Raycast(rigidbody2d.position, Vector2.down, .5f, LayerMask.GetMask("Hill"));

        IsGrounded = hit.collider != null;
        if (IsGrounded)
        {
            Debug.DrawRay(rigidbody2d.position, Vector2.down * .5f, Color.green);

            if (hit.collider.gameObject.CompareTag("Hill"))
            {
                if (isPushRight || isPushLeft)
                {
                    Move *= 2.2f;
                }
            
            }
            else
            {
                LastInAirY = rigidbody2d.position.y;
            }
            IsFalling = false;
            IsJumping = false;
        }
      
        else
        {
            IsJumping = LastInAirY < rigidbody2d.position.y;
            IsFalling = LastInAirY > rigidbody2d.position.y;
            LastInAirY = rigidbody2d.position.y;

            //Debug.DrawRay(rigidbody2d.position, Vector2.down * .4f, Color.red);
        }
    }
  

    private void Jump()
    {
        rigidbody2d.AddForce(Vector2.up * 8f, ForceMode2D.Impulse);
        canJump = false;
        timeToJump = jumpCoolDown;
    }


   
    public void ChangeHealth(int amount)
    {

        if (amount < 0)
        {
            if (isInvincible)
            {
                return;
            }
            isInvincible = true;
            damageCooldown = timeInvincible;
            IsHurt = true;
            StartCoroutine(IsBeingHurt());
        }
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
        

        //avisamos a los suscriptores, que ahora mismo son la barra del health y el game manager
        HealthChange?.Invoke();

    }

    /// <summary>
    /// Corutina para cuando el player ha sido herido
    /// </summary>
    /// <returns></returns>
    IEnumerator IsBeingHurt()
    {
        while (IsHurt)
        {
            speed = 0; //inmovilizamos al player 
            gameObject.layer = 9; //y lo sacamos de la layer por defecto para que no sea dañado de nuevo
            yield return new WaitForSeconds(1.0f); 
            gameObject.layer = 0;
            IsHurt = false;
            speed = 3.0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("WaterSensor"))
        {
            ChangeHealth(-3);
        }
        else if (other.gameObject.CompareTag("CollectibleCoin")){
            Coins += 1;
            CoinCollected?.Invoke(Coins); //Avisamos a los observers de que se cogió coin y enviámos el total
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (audioSource.enabled)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    public void StopSound()
    {
        if (audioSource.enabled)
        {
            audioSource.Stop();
        }
    }



}
