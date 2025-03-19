using System;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Esta clase controla la UI que aparece cuando el juego finaliza
/// Los elementos son invisibles y en función del resultado se tornan visibles
/// </summary>
public class UIGameOver : MonoBehaviour
{
    public PlayerController player;
    private UIDocument UIDoc;
    VisualElement root;
    const string winClassname = "win";
    const string loseClassname = "lose";
    const string invisibleClassname = "invisible";
    const string visibleClassname = "visible";
    const string labelResult = "LabelResult";
    const string labelCollectibles = "LabelCollectibles";
    const string labelLives = "LabelLives";
    const string youwin = "You win";
    const string youlose = "You lose";
    const string restartButtonName = "ButtonRestart";
    const string menuButtonName = "ButtonMenu";

    Button restartButton;
    Button menuButton;



    private void Awake()
    {
        UIDoc = GetComponent<UIDocument>();
        root = UIDoc.rootVisualElement;
        //Nos suscribimos al GameManager pa saber cuando termina el juego
        GameManager.Instance.GameStateChanged += OnGameStateChanged;
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnEnable()
    {
        restartButton = root.Q<Button>(restartButtonName);
        restartButton.RegisterCallback<ClickEvent>(Restart);
        menuButton = root.Q<Button>(menuButtonName);
        menuButton.RegisterCallback<ClickEvent>(Menu);
        
    }

    private void Menu(ClickEvent evt)
    {
        GameManager.Instance.Menu();
    }

    private void Restart(ClickEvent evt)
    {
        GameManager.Instance.RestartGame();
    }

    /// <summary>
    /// Si cambia el estado del GameManager pues mostramos unos elementos u otros
    /// </summary>
    /// <param name="state"></param>
    private void OnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Win:
                ShowWinElements();
                break;
                case GameState.Lose:
                ShowLoseElements();
                break;
        }
    }

  
 
    private void ShowWinElements()
    {
        root.Query<VisualElement>(className: winClassname).ForEach(element => { 
            element.RemoveFromClassList(invisibleClassname);
            element.AddToClassList(visibleClassname);

        });

        root.Q<Label>(labelResult).text = youwin;
        root.Q<Label>(labelCollectibles).text += player.Coins.ToString();
        root.Q<Label>(labelLives).text += player.CurrentHealth.ToString();

    } 
    private void ShowLoseElements()
    {
        root.Query<VisualElement>(className: loseClassname).ForEach(element => { 
            element.RemoveFromClassList(invisibleClassname);
            element.AddToClassList(visibleClassname);
        });
        root.Q<Label>(labelResult).text = youlose;

    }

   
}
