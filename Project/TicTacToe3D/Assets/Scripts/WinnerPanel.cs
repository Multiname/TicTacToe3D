using UnityEngine;
using UnityEngine.UIElements;

public class WinnerPanel : UiPanel {
    [SerializeField] PanelsManager panelsManager;

    private Button restartButton;
    private VisualElement oneWinnerContainer;
    private VisualElement drawContainer;
    private VisualElement sphereWinner;
    private VisualElement crossWinner;

    protected override void Awake() {
        base.Awake();

        oneWinnerContainer = uiDocument.rootVisualElement.Q<VisualElement>("OneWinnerContainer");
        drawContainer = uiDocument.rootVisualElement.Q<VisualElement>("DrawContainer");
        sphereWinner = uiDocument.rootVisualElement.Q<VisualElement>("SphereWinner");
        crossWinner = uiDocument.rootVisualElement.Q<VisualElement>("CrossWinner");

        restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");
        restartButton.RegisterCallback<ClickEvent>(evt => panelsManager.GoBackToMainMenuFromWinnerPanel());

        restartButton.RegisterCallback<PointerDownEvent>(e => restartButton.AddToClassList("pressed"), TrickleDown.TrickleDown);
        restartButton.RegisterCallback<PointerUpEvent>(e => restartButton.RemoveFromClassList("pressed"));
        
        restartButton.RegisterCallback<PointerEnterEvent>(e => {
            if (enabledVisibilityFrameCount != Time.frameCount) {
                restartButton.AddToClassList("hover");
            }
        });
        restartButton.RegisterCallback<PointerOutEvent>(e => restartButton.RemoveFromClassList("hover"));
    }

    public void ShowSphereWinner() {
        oneWinnerContainer.style.display = DisplayStyle.Flex;
        drawContainer.style.display = DisplayStyle.None;
        sphereWinner.style.display = DisplayStyle.Flex;
        crossWinner.style.display = DisplayStyle.None;
    }

    public void ShowCrossWinner() {
        oneWinnerContainer.style.display = DisplayStyle.Flex;
        drawContainer.style.display = DisplayStyle.None;
        sphereWinner.style.display = DisplayStyle.None;
        crossWinner.style.display = DisplayStyle.Flex;
    }

    public void ShowDraw() {
        oneWinnerContainer.style.display = DisplayStyle.None;
        drawContainer.style.display = DisplayStyle.Flex;
    }
}
