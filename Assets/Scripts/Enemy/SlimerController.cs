using System.Collections;
using UnityEngine;
using UnityEngine.Audio;


public class SlimerController : MonoBehaviour
{
    private float speed;
    private int direction;

    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    Rigidbody2D rigidbody2d;
    private SpriteRenderer spriteRenderer;

    //AUDIO
    public AudioClip deathClip;
    AudioSource audioSource;


    //ANIMATION
    Animator animator;
    private bool isDead;
    private bool isResting;
    private bool isMoving;
    float timeToSwitch = 5.0f;


    private void Awake()
    {
        //Audio
        audioSource = GetComponent<AudioSource>();
        audioSource.enabled = GameManager.Instance.settings.IsSfxOn;

    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        speed = 1.0f;
        spriteRenderer = GetComponent<SpriteRenderer>();
        direction = Random.value > 0.5f ? 1 : -1;
        spriteRenderer.flipX = direction == 1;
        animator = GetComponent<Animator>();
        isMoving = true;
        

        //audioSource.enabled = GameManager.Instance.IsSoundEffectsOn;

    }

    // Update is called once per frame
    void Update()
    {
        timeToSwitch -= Time.deltaTime;
        isMoving = timeToSwitch > 0;
        timeToSwitch = timeToSwitch < -5.0f ? 5.0f : timeToSwitch;
    }

    private void FixedUpdate()
    {
        animator.SetBool("IsMoving", isMoving);
        if (isMoving)
        {
            Vector2 position = rigidbody2d.position;
            position.x = position.x + speed * direction * Time.deltaTime;
            rigidbody2d.MovePosition(position);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("VerticalSensor"))
        {
            direction = -direction;
            spriteRenderer.flipX = direction == 1;
        }
        if (other.gameObject.CompareTag("Player"))
        {

            if (rigidbody2d.position.y < other.attachedRigidbody.position.y )
            {
                isDead = true;
                StartCoroutine(IsDead());
            }
            else
            {
                PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.ChangeHealth(-1);
                }
            }
        }
    }


    /// <summary>
    /// Para que la animación de la destrucción se ejecute debemos esperar un poco
    /// antes de destruir el gameObject
    /// </summary>
    /// <returns></returns>
    IEnumerator IsDead()
    {
        audioSource.Stop();
        animator.SetBool("IsDead", isDead);
        audioSource.PlayOneShot(deathClip);
        gameObject.layer = 9;
        yield return new WaitForSeconds(deathClip.length);
        Destroy(gameObject);
    }


}
