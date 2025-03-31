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

    /// <summary>
    /// En Start el audio del background estará activo en función de la 
    /// propiedad IsMusicOn del game manager. Y será habilitado o inhabilitado
    /// si la propiedad cambia
    /// </summary>
    void Start()
    {
        music.enabled = GameManager.Instance.settings.IsMusicOn;

    }
    private void OnDestroy()
    {
        GameManager.Instance.MusicChanged -= OnMusicChanged;
    }


}
