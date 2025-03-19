using UnityEngine;

public class WaterController : MonoBehaviour
{
  AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.enabled = GameManager.Instance.settings.IsSfxOn;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }

    }
}
