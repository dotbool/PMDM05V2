using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //MOVIMIENTO
    public Rigidbody2D rigidbody2d;
    private Vector2 move;
    private float speed;

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
    public bool IsAscending { get; set; }




    private PlayerStateMachine playerStateMachine;
    public PlayerStateMachine PlayerStateMachine => playerStateMachine;


    //---------------------ANIMACIONES--------------------------------------

    private SpriteRenderer spriteRenderer;
    public Animator Animator { get; set; }



    //-----------------------HEALTH-----------------------------------------
    public int MaxHealth { get; set; } = 3000;
    public int CurrentHealth { get; set; } = 3000;

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
    public AudioClip audioClipJump;
    public AudioClip audioClipRun;
    public AudioClip audioClipHurt;


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

        playerStateMachine = new PlayerStateMachine(this);

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
        audioSource = GetComponent<AudioSource>();

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
    /// Cómo puede intervenir el botón de saltar, ejecutamos este
    /// método en cada update, en el state
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
    float timeToJump;
    float jumpCoolDown = .3f;
    bool canJump;

    private void Jump()
    {
        rigidbody2d.AddForce(Vector2.up * 8f, ForceMode2D.Impulse);
        canJump = false;
        timeToJump = jumpCoolDown;
    }


    ///// <summary>
    ///// Determinamos aquí si el player está sobre una colina
    ///// Este método no depende de pulsaciones de teclas (es running). Sólo
    ///// cambia el valor de Move que es físico, por lo que será ejecutado en
    ///// Fixedupdate
    ///// </summary>
    //void CalcularClimbing()
    //{
    //    RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position, Vector2.down, .5f, LayerMask.GetMask("Hill"));

    //    if (hit.collider != null)
    //    {
        
    //        Debug.DrawRay(rigidbody2d.position, Vector2.down * .4f, Color.blue);

    //        if (isPushRight || isPushLeft)
    //        {
    //            Move *= 2.2f;
    //        }
    //    }
    //    else
    //    {
    //        Debug.DrawRay(rigidbody2d.position, Vector2.down * .4f, Color.yellow);

    //    }
    //}


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
        }
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);

        //avisamos a la UI de que el Health ha cambiado
        HealthChange?.Invoke();

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("WaterSensor"))
        {
            ChangeHealth(-3);
            Destroy(gameObject);

        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    public void StopSound()
    {
        audioSource.Stop();
    }



}
