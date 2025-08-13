using UnityEngine;

public class CellsShadows : MonoBehaviour {
    private static readonly CellShadow[,] cellShadows = new CellShadow[Board.MAX_X, Board.MAX_Z];

    private void Start() {
        int index = 0;
        for (int i = 0; i < Board.MAX_X; ++i) {
            for (int j = 0; j < Board.MAX_Z; ++j) {
                cellShadows[j, i] = transform.GetChild(index++).GetComponent<CellShadow>();
            }
        }
    }

    public static void HideShadow(int x, int z) {
        cellShadows[x, z].HideShadow();
    }

    public static void ShowShadow(int x, int z, Figure.FigureType figureType) {
        cellShadows[x, z].ShowShadow(figureType);
    }
}
