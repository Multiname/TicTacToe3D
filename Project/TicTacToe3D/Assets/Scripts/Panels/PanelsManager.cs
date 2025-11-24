using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PanelsManager : MonoBehaviour {
    [SerializeField] MainMenu mainMenu;
    [SerializeField] NewGamePanel newGamePanel;
    [SerializeField] SettingsPanel settingsPanel;
    [SerializeField] UiPanel tutorialPanel;
    [SerializeField] WinnerPanel winnerPanel;
    [SerializeField] MenuButton menuButton;
    [SerializeField] SelectionFigure selectionFigure;

    [SerializeField] bool runningFromStartScene = false;

    public enum MainMenuOption {
        NEW_GAME,
        SETTINGS,
        TUTORIAL
    }
    public const int NUMBER_OF_MAIN_MENU_OPTIONS = 3;

    private UiPanel[] menuPanels;

    public bool MenuPanelIsVisible { get; private set; }
    private bool gameIsOngoing;

    private void Start() {
        menuPanels = new UiPanel[NUMBER_OF_MAIN_MENU_OPTIONS] {
            newGamePanel,
            settingsPanel,
            tutorialPanel
        };

        gameIsOngoing = !runningFromStartScene;
        menuButton.gameObject.SetActive(!runningFromStartScene);

        mainMenu.gameObject.SetActive(true);
        newGamePanel.gameObject.SetActive(true);
        settingsPanel.gameObject.SetActive(true);
        tutorialPanel.gameObject.SetActive(true);
        if (winnerPanel != null) winnerPanel.gameObject.SetActive(true);

        mainMenu.Visible = runningFromStartScene;
        newGamePanel.Visible = false;
        settingsPanel.Visible = false;
        tutorialPanel.Visible = false;
        if (winnerPanel != null) winnerPanel.Visible = false;

        if (!runningFromStartScene) {
            SetMenuButtonToShowMainMenu();
        }
    }

    private void Update() {
        if (Keyboard.current.escapeKey.wasReleasedThisFrame) {
            menuButton.handleClick();
        }
    }

    private void SetMenuButtonToShowMainMenu() {
        menuButton.handleClick = () => {
            mainMenu.shownFromScene = true;
            mainMenu.Visible = true;

            if (selectionFigure != null) selectionFigure.MenuIsVisible = true;
            MenuPanelIsVisible = true;
            menuButton.SetGoBackSprite();

            SetMenuButtonToHideMainMenu();
        };
    }

    private void SetMenuButtonToHideMainMenu() {
        menuButton.handleClick = () => {
            mainMenu.shownFromScene = false;
            mainMenu.Visible = false;

            if (selectionFigure != null) selectionFigure.MenuIsVisible = false;
            menuButton.SetShowMenuSprite();

            UniTask.Void(async () => {
                await UniTask.Yield();
                MenuPanelIsVisible = false;
            });

            SetMenuButtonToShowMainMenu();
        };
    }

    public void ShowWinnerPanel(GameManager.Winner winner) {
        if (winner == GameManager.Winner.Sphere) {
            winnerPanel.ShowSphereWinner();
            SfxPlayer.PlaySound(SfxPlayer.Sound.GAME_END);
        } else if (winner == GameManager.Winner.Cross) {
            winnerPanel.ShowCrossWinner();
            SfxPlayer.PlaySound(SfxPlayer.Sound.GAME_END);
        } else {
            winnerPanel.ShowDraw();
            SfxPlayer.PlaySound(SfxPlayer.Sound.GAME_DRAW);
        }

        selectionFigure.MenuIsVisible = true;
        menuButton.gameObject.SetActive(false);
        winnerPanel.Visible = true;
        gameIsOngoing = false;
        menuButton.handleClick = () => { };
    }

    public void ProceedToPanel(MainMenuOption mainMenuOption) {
        mainMenu.shownFromScene = false;
        mainMenu.Visible = false;
        
        menuPanels[(int)mainMenuOption].Visible = true;
        menuButton.gameObject.SetActive(true);

        menuButton.handleClick = () => {
            mainMenu.Visible = true;
            menuPanels[(int)mainMenuOption].Visible = false;

            if (gameIsOngoing) {
                SetMenuButtonToHideMainMenu();
            } else {
                menuButton.gameObject.SetActive(false);
                menuButton.handleClick = () => { };
            }
        };
    }

    public void GoBackToMainMenuFromWinnerPanel() {
        winnerPanel.Visible = false;
        mainMenu.Visible = true;
        menuButton.SetGoBackSprite();
    }
}
