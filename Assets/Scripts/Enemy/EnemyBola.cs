using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class EnemyBola : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(PlayAnimation());
        audioSource.enabled = GameManager.Instance.settings.IsSfxOn;

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Colisionando");
        PlayerController player = collision.collider.GetComponent<PlayerController>();
        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }

    public IEnumerator PlayAnimation()
    {
        float start = Random.Range(0, 6);
        yield return new WaitForSeconds(start);
        animator.SetTrigger("Start");
    }
}


