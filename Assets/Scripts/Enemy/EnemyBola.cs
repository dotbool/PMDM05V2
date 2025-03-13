using System.Collections;
using UnityEngine;

public class EnemyBola : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(PlayAnimation());
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
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


