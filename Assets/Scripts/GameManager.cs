using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Unity.VisualScripting;
using System.Collections.Generic;

public enum GameState { Lose, Menu, Play, Win }


public class GameManager : MonoBehaviour
{
    public static GameManager Instance; //Queremos mantener los valores de las variables, por lo que la hacemos static

    public Settings settings = new(); //esta clase sostiene los valores del sonido

    private PlayerController player;

    //El player está en la Main Scene por lo que sólo
    //obtendremos una referencia a ese gameObject  cuando 
    //sea construido. Es por ello que la dependencia vendrá
    //vía setter. Una vez exista dependencia le añadimos
    //los listeners para saber cuando coge moneda o cuando
    //cambia el health
    public PlayerController Player
    {
        get { return player; }
        set
        { 
            player = value;
            player.CoinCollected += OnCoinCollected;
            player.HealthChange += OnHealthChange;
        }
    }

    private UIMenuSceneController uIMenuSceneController;

    
    public UIMenuSceneController UIMenuSceneController
    {
        get { return uIMenuSceneController; }
        set { 
            uIMenuSceneController = value;
            uIMenuSceneController.ButtonClicked += OnUIMenuButtonClicked;
        }
    }


    private readonly int coinsToWin = 50;
    private int coinsCollected;
    //private readonly int livesToLose = 0;
    //private int currentLives;
    private GameState gameState;

    public event Action<GameState> GameStateChanged;
    public event Action<bool> MusicChanged;
    public event Action<bool> SfxChanged;


    /// <summary>
    /// Awake es llamado cada vez que se carga la scene. Si ya existe una Instance,
    /// se destruye la que se fuera a crear y permanece la que hubiera
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();

        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gameState = GameState.Menu;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    /// <summary>
    /// Liberamos memoria
    /// </summary>
    private void OnDestroy()
    {
        if (player != null)
        {
            player.CoinCollected -= OnCoinCollected;
            player.HealthChange -= OnHealthChange;
        }
        if (uIMenuSceneController != null)
        {
            uIMenuSceneController.ButtonClicked -= OnUIMenuButtonClicked;
        }

    }

    /// <summary>
    /// Este método recibe el nombre del botón de la UI del menú
    /// Si es un toggle  de los del sonido, se avisa a los suscriptores
    /// </summary>
    /// <param name="buttonName"></param>
    /// <returns></returns>
    private Settings OnUIMenuButtonClicked(ButtonsNames buttonName)
    {

        switch (buttonName)
        {
            case ButtonsNames.Music:
                settings.IsMusicOn = !settings.IsMusicOn;
                MusicChanged.Invoke(settings.IsMusicOn);
                break;

            case ButtonsNames.Sfx:
                settings.IsSfxOn = !settings.IsSfxOn;
                SfxChanged.Invoke(settings.IsSfxOn);
                break;

            case ButtonsNames.Exit:
                Exit();
                break;
            
            case ButtonsNames.Play:
                Play();
                break;

            default:
                break;
        }

        return settings;
    }




    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void Menu()
    {
        SceneManager.LoadScene(0);
    }

   
    public void Exit()
    {
        SaveData();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit(); 
#endif
    }

    /// <summary>
    /// Salva en Player prefs
    /// </summary>
    void SaveData()
    {
        PlayerPrefs.SetInt("music", settings.IsMusicOn ? 1 : 0);
        PlayerPrefs.SetInt("sound", settings.IsSfxOn ? 1 : 0);
    }

    void LoadData()
    {
        settings.IsMusicOn = PlayerPrefs.GetInt("music", 1) > 0;
        settings.IsSfxOn = PlayerPrefs.GetInt("sound", 1) > 0;
    }

    /// <summary>
    /// El listener que se le asigna al player para saber cuando coge monedas
    /// </summary>
    /// <param name="coins"></param>
    private void OnCoinCollected(int coins)
    {
        if(coins == coinsToWin)
        {
            UpdateGameState(GameState.Win);
        }
    }
    private void OnHealthChange()
    {
        if(player.CurrentHealth <= 0)
        {
            UpdateGameState(GameState.Lose);
        }
    }

    /// <summary>
    /// Cómo el juego acaba se gane o se pierda,
    /// siempre lo reseteamos si hay un cambio de estado
    /// </summary>
    /// <param name="newState"></param>
    private void UpdateGameState(GameState newState)
    {
        gameState = newState;
        ResetGame();
        GameStateChanged?.Invoke(gameState);

    }

    /// <summary>
    /// Quitamos las suscripciones en el player
    /// Si se reinicia el juego
    /// </summary>
    private void ResetGame() {
        if (player != null)
        {
            player.CoinCollected -= OnCoinCollected;
            player.HealthChange -= OnHealthChange;

        }
        coinsCollected = 0;
        if(uIMenuSceneController != null)
        {
            uIMenuSceneController.ButtonClicked -= OnUIMenuButtonClicked;
        }

    }
}
