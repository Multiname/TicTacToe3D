using UnityEngine;

public class GameManager : MonoBehaviour {
    public Figure.FigureType CurrentPlayer { get; private set; } = Figure.FigureType.SPHERE;

    public void StartNextTurn() {
        if (CurrentPlayer == Figure.FigureType.SPHERE) {
            CurrentPlayer = Figure.FigureType.CROSS;
        } else {
            CurrentPlayer = Figure.FigureType.SPHERE;
        }
    }
}
