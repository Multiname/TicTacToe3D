using UnityEngine;

public class GameManager : MonoBehaviour {
    public static Figure.FigureType CurrentPlayer { get; private set; } = Figure.FigureType.SPHERE;

    public static void StartNextTurn() {
        if (CurrentPlayer == Figure.FigureType.SPHERE) {
            CurrentPlayer = Figure.FigureType.CROSS;
        } else {
            CurrentPlayer = Figure.FigureType.SPHERE;
        }

        FigureSideFrameManager.InvokeStartNextTurn();
    }
}
