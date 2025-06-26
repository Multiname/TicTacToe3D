using UnityEngine;

public class Board : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] SelectionFigure selectionFigure;

    [SerializeField] Figure[] figurePrefabs = new Figure[2];

    private readonly Figure[,,] placedFigures = new Figure[3, 3, 8];

    public void PlaceFigure(Vector3Int coordinates) {
        Figure figure = Instantiate(figurePrefabs[(int)gameManager.CurrentPlayer], coordinates, Quaternion.identity);
        placedFigures[coordinates.x, coordinates.z, coordinates.y] = figure;

        gameManager.StartNextTurn();
        selectionFigure.SwitchForm();
    }
}
