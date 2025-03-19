using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Editor Window es la clase que tiene el rootVisualElement
/// </summary>
public class TrainingEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Window/UI Toolkit/TrainingEditor")]
    public static void ShowExample()
    {
        TrainingEditor wnd = GetWindow<TrainingEditor>();
        wnd.titleContent = new GUIContent("TrainingEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        //Primero hay que instanciar los widgets tipo new Label("esto es una label");
        //y después añadirlos el rootElement
        VisualElement root = rootVisualElement;

        //--------------------------- a través de cs --------------------------------------------
        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("These controls were created using C# code.");
        root.Add(label);
            //La label incial 

        //AQUI ES INSTANCIADO LO CREADO CON CS
        Button button = new Button();
        button.name = "button3";
        button.text = "This is button3.";
        root.Add(button);

        Toggle toggle = new Toggle();
        toggle.name = "toggle3";
        toggle.label = "Number?";
        root.Add(toggle);
        //---------------------------------------------------------------------------------------------------


        // Instantiate UXML EL modo 1 UIBuilder. Aquí es instanciado lo creado con el UIBUILder
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);
        //--------------------------------------------------------------


        //Aqu´es INSTANCIADO LO CREADO CON UXML
        //Para crear basándonos en uxml -> Create > UIToolkit > UIDOcument
        //Editamos el inline style derivado del xml. Es decir, aquí lo que hacemos es escribir el xml y 
        //después importarlo e instanciarlo
        // Import UXML created manually.
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UITraining/TrainingEditor_uxml.uxml");
        VisualElement labelFromUXML2 = visualTree.Instantiate();
        root.Add(labelFromUXML2);

    }

    private void SetupButtonHandler()
    {
        VisualElement root = rootVisualElement;

        var buttons = root.Query<Button>();
    }

}
