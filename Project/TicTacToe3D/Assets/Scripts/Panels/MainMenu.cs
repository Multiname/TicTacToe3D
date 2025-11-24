using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : UiPanel {
    [SerializeField] PanelsManager panelsManager;

    public bool shownFromScene = false;

    protected override void Awake() {
        base.Awake();

        ConfigureButton("NewGameButton", () => panelsManager.ProceedToPanel(PanelsManager.MainMenuOption.NEW_GAME));
        ConfigureButton("SettingsButton", () => panelsManager.ProceedToPanel(PanelsManager.MainMenuOption.SETTINGS));
        ConfigureButton("InfoButton", () => panelsManager.ProceedToPanel(PanelsManager.MainMenuOption.TUTORIAL));
    }

    private void ConfigureButton(string buttonName, Action clickAction) {
        var button = uiDocument.rootVisualElement.Q<Button>(buttonName);

        button.RegisterCallback<ClickEvent>(evt => {
            clickAction();
            SfxPlayer.PlaySound(SfxPlayer.Sound.UI_BUTTON_CLICK);
        });

        button.RegisterCallback<PointerDownEvent>(e => button.AddToClassList("pressed"), TrickleDown.TrickleDown);
        button.RegisterCallback<PointerUpEvent>(e => button.RemoveFromClassList("pressed"));

        button.RegisterCallback<PointerEnterEvent>(e => {
            if (enabledVisibilityFrameCount != Time.frameCount || !shownFromScene) {
                button.AddToClassList("hover");
                SfxPlayer.PlaySound(SfxPlayer.Sound.UI_BUTTON_HOVER);
            }
        });
        button.RegisterCallback<PointerOutEvent>(e => button.RemoveFromClassList("hover"));
    }
}
