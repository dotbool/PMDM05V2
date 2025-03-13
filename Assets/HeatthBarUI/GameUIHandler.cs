using UnityEngine;
using UnityEngine.UIElements;

public class GameUIHandler : MonoBehaviour
{
    public PlayerController PlayerController;
    public UIDocument UIDoc;
    private Label m_HealthLabel;
    private VisualElement m_HealthBarMask;


    private void Start()
    {
        PlayerController.HealthChange += OnHealthChanged;
        m_HealthLabel = UIDoc.rootVisualElement.Q<Label>("HealthLabel");
        m_HealthBarMask = UIDoc.rootVisualElement.Q<VisualElement>("HealthBarMask");
        OnHealthChanged(); //lo llamamos aquí para establecer el texto de la label

    }


    void OnHealthChanged()
    {
        m_HealthLabel.text = PlayerController.CurrentHealth + "/" + PlayerController.MaxHealth;
        float healthRatio = (float)PlayerController.CurrentHealth / PlayerController.MaxHealth;
        float healthPercent = Mathf.Lerp(8, 88, healthRatio);
        m_HealthBarMask.style.width = Length.Percent(healthPercent);

    }

}