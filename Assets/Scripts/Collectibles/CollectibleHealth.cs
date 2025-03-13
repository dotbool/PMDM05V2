using System.Collections;
using UnityEngine;

public class CollectibleHealth : MonoBehaviour
{

    Animator animator;
    public AudioClip collectedClip;
    AudioSource audioSource;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController controller = collision.GetComponent<PlayerController>();

        if (controller != null)
        {
            controller.ChangeHealth(1);
            StartCoroutine(IsCollected());
        }
    }

    IEnumerator IsCollected()
    {
        animator.SetBool("IsOpen", true); // disparamos la animación de open
        audioSource.PlayOneShot(collectedClip);
        gameObject.layer = 9;
        yield return new WaitForSeconds(collectedClip.length);
        Destroy(gameObject);
    }
}
