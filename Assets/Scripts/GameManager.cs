using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Unity.VisualScripting;

public enum GameState { Lose, Menu, Play, Win }


public class GameManager : MonoBehaviour
{

    public static GameManager Instance; //Queremos mantener los valores de las variables, por lo que hacemos la variable static
    BackGroundMusicController BackGroundMusicController;

    public Settings settings = new Settings();

    private PlayerController player;
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


    private readonly int coinsToWin = 4;
    private int coinsCollected;
    private readonly int livesToLose = 0;
    private int currentLives;
    private GameState gameState;

    public event Action<GameState> GameStateChanged;
    public event Action<bool> MusicChanged;


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

    private void OnDestroy()
    {
        if (player != null)
        {
            player.CoinCollected -= OnCoinCollected;
        }
        if (uIMenuSceneController != null)
        {
            uIMenuSceneController.ButtonClicked -= OnUIMenuButtonClicked;
        }

    }

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

    public void Win()
    {

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
        if(uIMenuSceneController!= null)
        {
            uIMenuSceneController.ButtonClicked -= OnUIMenuButtonClicked;
        }

    }
}
