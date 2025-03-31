using System;
using UnityEngine;
using UnityEngine.UIElements;

public enum ButtonsNames { Play, Settings, Exit, Music, Sfx }

public class UIMenuSceneController : MonoBehaviour
{

    private UIDocument UIDoc;
    VisualElement root;
    VisualElement UIContainer;
    const string sceneOutClassname = "scene-out";
    const string sceneOutLeftClassName = "scene-out-left";
    const string sceneInFromRightClassName = "scene-in-from-right";
    const string toggleContainerClassName = "ToggleContainer";

    Button buttonX;
    Button playButton;
    Button settingsButton;
    Button exitButton;
    VisualElement ToggleContainer;
    Toggle musicToggle;
    Toggle sfxToggle;

    const string playButtonName ="ButtonPlay";
    const string settingsButtonName ="ButtonSettings";
    const string exitButtonName ="ButtonExit";
    const string UIContainerName ="UIContainer";
    const string ToggleContainerName = "ToggleContainer";
    const string buttonXName = "ButtonX";
    const string toggleMusicName = "ToggleMusic";
    const string toggleSfxName = "ToggleSfx";


    public Func<ButtonsNames, Settings> ButtonClicked;
    AudioSource audioSource;



    private void Awake()
    {
        UIDoc = GetComponent<UIDocument>();
        root = UIDoc.rootVisualElement;
        audioSource = GetComponent<AudioSource>();

    }


    private void OnSfxChanged(bool isSfxOn)
    {
        audioSource.enabled = isSfxOn;
    }

    /// <summary>
    ///En el PlayerController podemos asignarlo en Awake porque el
    ///GameManager se construye antes que el player, ya que éste 
    ///sólo despierta cuando se carga la scene 1
    /// </summary>
    private void Start()
    {
        GameManager.Instance.UIMenuSceneController = this;
        audioSource.enabled = GameManager.Instance.settings.IsSfxOn;
        GameManager.Instance.SfxChanged += OnSfxChanged;


    }

    private void OnEnable()
    {

        musicToggle = root.Q<Toggle>(toggleMusicName);
        sfxToggle = root.Q<Toggle>(toggleSfxName);
        buttonX = root.Q<Button>(buttonXName);
        ToggleContainer = root.Q<VisualElement>(ToggleContainerName);
        UIContainer = root.Q<VisualElement>(UIContainerName);
        playButton = root.Q<Button>(playButtonName);
        settingsButton = root.Q<Button>(settingsButtonName);
        exitButton = root.Q<Button>(exitButtonName);

        root.Query<Toggle>().ForEach(toggle => toggle.RegisterCallback<ClickEvent>(Clicked));
        root.Query<Button>().ForEach(button => button.RegisterCallback<ClickEvent>(Clicked));
        root.Query<Button>().ForEach(button => button.RegisterCallback<MouseOverEvent>(MouseOver));

    }

    private void MouseOver(MouseOverEvent evt)
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    private void Clicked(ClickEvent evt)
    {
        String targetName = ((VisualElement) evt.target).name;
        switch(targetName)
        {
            case toggleSfxName:
                ButtonClicked.Invoke(ButtonsNames.Sfx);
                break;
            case toggleMusicName:
                ButtonClicked.Invoke(ButtonsNames.Music);
                break;
            case playButtonName:
                PlayTransition(evt);
                break;
            case settingsButtonName:
                SettingsTransitionIn(evt);
                break;
            case exitButtonName:
                ButtonClicked.Invoke(ButtonsNames.Exit);
                break;
            case buttonXName:
                SettingsTransitionOut(evt);
                break;
        }
    }

    private void SettingsTransitionOut(ClickEvent evt)
    {
       
        ToggleContainer.RemoveFromClassList(sceneInFromRightClassName);
        ToggleContainer.AddToClassList(toggleContainerClassName);
        UIContainer.RemoveFromClassList(className: sceneOutLeftClassName);

    }

    private void SettingsTransitionIn(ClickEvent evt)
    {
        Settings settings = ButtonClicked.Invoke(ButtonsNames.Settings);
        musicToggle.value = settings.IsMusicOn;
        sfxToggle.value = settings.IsSfxOn;
        ToggleContainer.RemoveFromClassList(toggleContainerClassName);
        ToggleContainer.AddToClassList(className: sceneInFromRightClassName);
        UIContainer.AddToClassList(className: sceneOutLeftClassName);
    }

    //Query returns a list of elements that match the selection rules.
    //You can filter the return results of Query with the public methods
    //of UQueryBuilder, such as First, Last, AtIndex, Children, and Where.

    //Q is the shorthand for Query<T>.First(). It returns the first element
    //    that matches the selection rules.
    private void PlayTransition(ClickEvent evt)
    {
        root.Query<Button>().ForEach(b => b.AddToClassList(className: sceneOutClassname));
        if (evt.target == playButton)
        {
            playButton.RegisterCallback<TransitionEndEvent>(PlayGame);
        }
    }
    
    private void PlayGame(TransitionEndEvent evt)
    { 
        ButtonClicked.Invoke(ButtonsNames.Play);
    }

    private void OnDisable()
    {
        //root.Query<Button>().ForEach(b => b.UnregisterCallback<ClickEvent>(PlayTransition));
        root.Query<Toggle>().ForEach(toggle => toggle.UnregisterCallback<ClickEvent>(Clicked));
        root.Query<Button>().ForEach(button => button.UnregisterCallback<ClickEvent>(Clicked));
        playButton.UnregisterCallback<TransitionEndEvent>(PlayGame);
        root.Query<Button>().ForEach(button => button.UnregisterCallback<MouseOverEvent>(MouseOver));

    }

    private void OnDestroy()
    {
        GameManager.Instance.SfxChanged -= OnSfxChanged;
    }

}
