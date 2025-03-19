using System.Collections;
using TMPro;
using UnityEngine;

public class CollectibleCoin : MonoBehaviour
{

    AudioSource audioSource;
    Animator animator;
    public TextMeshPro textPoint;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        audioSource.enabled = GameManager.Instance.settings.IsSfxOn;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Player"){
            StartCoroutine(Collected());
        }
    }

    IEnumerator Collected()
    {
        gameObject.layer = 9;
        textPoint.gameObject.SetActive(true);
        if (audioSource.enabled)
        {
            audioSource.Play();
        }
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);


    }
}
