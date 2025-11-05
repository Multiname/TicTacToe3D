using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UiPanel : MonoBehaviour {
    protected UIDocument uiDocument;
    protected VisualElement panel;

    public bool Visible {
        get => panel.visible;
        set => panel.visible = value;
    }

    protected virtual void Awake() {
        uiDocument = GetComponent<UIDocument>();
        panel = uiDocument.rootVisualElement.Q<VisualElement>("Panel");
    }
}
