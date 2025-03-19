using System;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundMusicController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private AudioSource music;


    public bool IsMusicOn
    {
        set { music.enabled = value; }
    }


    private void Awake()
    {
        music = GetComponent<AudioSource>();
        GameManager.Instance.MusicChanged += OnMusicChanged;
    }

    private void OnMusicChanged(bool isMusicOn)
    {
        music.enabled = isMusicOn;
    }

    void Start()
    {
        music.enabled = GameManager.Instance.settings.IsMusicOn;

    }
    private void OnDestroy()
    {
        GameManager.Instance.MusicChanged -= OnMusicChanged;
    }


}
