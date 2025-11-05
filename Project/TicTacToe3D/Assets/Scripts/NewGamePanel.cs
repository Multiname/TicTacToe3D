using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class NewGamePanel : UiPanel {
    private const int START_SCENE_INDEX = 1;

    [SerializeField] GameSettings gameSettings;

    private Label pointsLabel;
    private Label ruleLabel;
    private Button basePointsModifierButton;

    private int points = 1;
    private int Points {
        get => points;
        set {
            if (value < 1) {
                points = 1;
            } else if (value > gameSettings.MaxWinPointsValue) {
                points = gameSettings.MaxWinPointsValue;
            } else {
                points = value;
            }
            pointsLabel.text = points.ToString();
        }
    }

    private class Modifier {
        public string name;
        public bool enabled;
    }
    
    private readonly Modifier[] modifiers = new Modifier[GameSettings.NUMBER_OF_MODIFIERS] {
        new() { name = "BasePointsModifier", enabled = true },
        new() { name = "ComboModifier", enabled = false },
        new() { name = "2DModifier", enabled = false },
        new() { name = "3DModifier", enabled = false },
        new() { name = "FallModifier", enabled = false },
        new() { name = "HeightModifier", enabled = false },
        new() { name = "BonusTurnModifier", enabled = false }
    };

    private int selectedRuleIndex = 0;
    private int SelectedRuleIndex {
        get => selectedRuleIndex;
        set {
            if (value < 0) {
                selectedRuleIndex = GameSettings.NUMBER_OF_RULES - 1;
            } else if (value >= GameSettings.NUMBER_OF_RULES) {
                selectedRuleIndex = 0;
            } else {
                selectedRuleIndex = value;
            }

            ruleLabel.text = rules[selectedRuleIndex];
        }
    }
    private readonly string[] rules = new string[GameSettings.NUMBER_OF_RULES] {
        "отключено",
        "ничего",
        "только базовые",
        "отнимать базовые",
        "отнимать все"
    };

    protected override void Awake() {
        base.Awake();

        pointsLabel = uiDocument.rootVisualElement.Q<Label>("Points");
        ruleLabel = uiDocument.rootVisualElement.Q<Label>("Rule");

        ConfigureButton("LeftPointsSelectorArrow", (_) => --Points);
        ConfigureButton("RightPointsSelectorArrow", (_) => ++Points);

        ConfigureButton("LeftRuleSelectorArrow", (_) => --SelectedRuleIndex);
        ConfigureButton("RightRuleSelectorArrow", (_) => ++SelectedRuleIndex);

        basePointsModifierButton = ConfigureButton(modifiers[0].name, (button) => ToggleModifier(button, 0));
        for (int modifierIndex = 1; modifierIndex < GameSettings.NUMBER_OF_MODIFIERS; ++modifierIndex) {
            int buttonModifierIndex = modifierIndex;
            ConfigureButton(modifiers[buttonModifierIndex].name, (button) => ToggleModifier(button, buttonModifierIndex));
        }

        ConfigureButton("StartButton", (_) => StartGame());

        InitializeInput();
    }

    private Button ConfigureButton(string buttonName, Action<Button> clickAction) {
        var button = uiDocument.rootVisualElement.Q<Button>(buttonName);

        button.RegisterCallback<ClickEvent>(evt => clickAction(button));

        button.RegisterCallback<PointerDownEvent>(e => button.AddToClassList("pressed"), TrickleDown.TrickleDown);
        button.RegisterCallback<PointerUpEvent>(e => button.RemoveFromClassList("pressed"));

        return button;
    }

    private void ToggleModifier(Button button, int modifierIndex) {
        bool modifierEnabled = !modifiers[modifierIndex].enabled;
        modifiers[modifierIndex].enabled = modifierEnabled;

        if (modifierEnabled) {
            button.AddToClassList("selected");
        } else {
            button.RemoveFromClassList("selected");
        }

        if (!modifierEnabled) {
            bool allPointModifiersDisabled = true;
            for (int i = 0; i < GameSettings.NUMBER_OF_MODIFIERS - 1; ++i) {
                if (modifiers[i].enabled) {
                    allPointModifiersDisabled = false;
                    break;
                }
            }
            if (allPointModifiersDisabled) {
                modifiers[(int)GameSettings.Modifier.BASE_POINTS].enabled = true;
                basePointsModifierButton.AddToClassList("selected");
            }
        }
    }

    private void StartGame() {
        gameSettings.winPoints = Points;

        for (int modifierIndex = 0; modifierIndex < GameSettings.NUMBER_OF_MODIFIERS; ++modifierIndex) {
            gameSettings.enabledModifiers[modifierIndex] = modifiers[modifierIndex].enabled;
        }

        gameSettings.enabledInterceptionRule = (GameSettings.InterceptionRule)SelectedRuleIndex;

        SceneManager.LoadScene(START_SCENE_INDEX);
    }

    private void InitializeInput() {
        points = gameSettings.winPoints;
        pointsLabel.text = points.ToString();

        selectedRuleIndex = (int)gameSettings.enabledInterceptionRule;
        ruleLabel.text = rules[selectedRuleIndex];

        for (int modifierIndex = 0; modifierIndex < GameSettings.NUMBER_OF_MODIFIERS; ++modifierIndex) {
            var button = uiDocument.rootVisualElement.Q<Button>(modifiers[modifierIndex].name);

            modifiers[modifierIndex].enabled = gameSettings.enabledModifiers[modifierIndex];
            if (modifiers[modifierIndex].enabled) {
                button.AddToClassList("selected");
            } else {
                button.RemoveFromClassList("selected");
            }
        }
    }
}
