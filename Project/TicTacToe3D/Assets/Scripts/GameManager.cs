using UnityEngine;

public class GameManager : MonoBehaviour {
    public Figure.FigureType CurrentPlayer { get; private set; } = Figure.FigureType.SPHERE;

    [SerializeField] UiCanvas uiCanvas;

    private readonly int[] playersScores = new int[Figure.NUMBER_OF_FIGURE_TYPES] { 0, 0 };

    public void StartNextTurn() {
        if (CurrentPlayer == Figure.FigureType.SPHERE) {
            CurrentPlayer = Figure.FigureType.CROSS;
        } else {
            CurrentPlayer = Figure.FigureType.SPHERE;
        }
    }

    public void AddPointsToPlayer(Figure.FigureType figureType, int points) {
        playersScores[(int)figureType] += points;
        uiCanvas.SetScore(figureType, playersScores[(int)figureType]);
    }
}
