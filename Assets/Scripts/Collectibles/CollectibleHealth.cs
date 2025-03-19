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
        audioSource.enabled = GameManager.Instance.settings.IsSfxOn;

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController controller = collision.GetComponent<PlayerController>();

        if (controller != null && controller.CurrentHealth < controller.MaxHealth)
        {
            controller.ChangeHealth(1);
            StartCoroutine(IsCollected());
        }
    }

    IEnumerator IsCollected()
    {
        animator.SetBool("IsOpen", true); // disparamos la animación de open
        audioSource.PlayOneShot(collectedClip); //reproducimos sonido
        gameObject.layer = 9;//sacamos al objeto del layer del player para que no interactúe de nuevo
        yield return new WaitForSeconds(collectedClip.length); //esperamos a que acabe el sonido antes de destruir el objeto
        Destroy(gameObject);
    }
}
