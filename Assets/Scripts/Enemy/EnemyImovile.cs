using UnityEngine;

public class EnemyImovile : MonoBehaviour
{

    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        audioSource.enabled = GameManager.Instance.settings.IsSfxOn;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.collider.GetComponent<PlayerController>();
        if(player != null)
        {
            player.ChangeHealth(-1);
            player.rigidbody2d.AddForce(Vector2.up + Vector2.left * 4f, ForceMode2D.Impulse);

        }
    }
}
