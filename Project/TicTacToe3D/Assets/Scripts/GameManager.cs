using System;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [Flags]
    public enum Winner {
        None = 0,
        Sphere = 1,
        Cross = 2,
        Draw = 3
    }
    private Winner winner = Winner.None;

    [SerializeField] GameSettings gameSettings;
    [SerializeField] PanelsManager panelsManager;
    [SerializeField] SelectionFigure selectionFigure;

    public static event Action<Figure.FigureType> OnStartNextTurnEvent;

    public Figure.FigureType CurrentPlayer { get; private set; } = Figure.FigureType.SPHERE;
    public Figure.FigureType EnemyPlayer {
        get {
            if (CurrentPlayer == Figure.FigureType.SPHERE) {
                return Figure.FigureType.CROSS;
            }
            return Figure.FigureType.SPHERE;
        }
    }

    private readonly int[] playersScores = new int[Figure.NUMBER_OF_FIGURE_TYPES];

    private void Start() {
        for (int i = 0; i < Figure.NUMBER_OF_FIGURE_TYPES; ++i) {
            playersScores[i] = gameSettings.winPoints;
        }

        SfxPlayer.PlaySound(SfxPlayer.Sound.GAME_START);
    }

    public void StartNextTurn() {
        if (CurrentPlayer == Figure.FigureType.SPHERE) {
            CurrentPlayer = Figure.FigureType.CROSS;
        } else {
            CurrentPlayer = Figure.FigureType.SPHERE;
        }

        OnStartNextTurnEvent?.Invoke(CurrentPlayer);
    }

    public void AddPointsToPlayers(int[] points) {
        for (int figureType = 0; figureType < Figure.NUMBER_OF_FIGURE_TYPES; ++figureType) {
            playersScores[figureType] -= points[figureType];
            if (playersScores[figureType] <= 0) {
                winner |= (Winner)(figureType + 1);
            }
        }

        if (winner != Winner.None) {
            panelsManager.ShowWinnerPanel(winner);
        }
    }
}
