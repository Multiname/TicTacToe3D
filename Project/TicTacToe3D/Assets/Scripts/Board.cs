using UnityEngine;

public class Board : MonoBehaviour {
    [SerializeField] FigureSidesBuilder figureSidesBuilder;
    [SerializeField] SelectionFigure selectionFigure;

    [SerializeField] Figure[] figurePrefabs = new Figure[2];

    private readonly Figure[,,] placedFigures = new Figure[3, 3, 8];
    private readonly int[,] cellsHeight = new int[3, 3];

    public void PlaceFigure(Vector3Int coordinates) {
        Figure figure = Instantiate(figurePrefabs[(int)GameManager.CurrentPlayer], coordinates, Quaternion.identity);
        figure.coordinates.coordinates = coordinates;
        figureSidesBuilder.BuildSides(figure.transform, figure.coordinates);

        int currentCellHeight = cellsHeight[coordinates.x, coordinates.z];
        if (coordinates.y > currentCellHeight) {
            selectionFigure.Active = false;
            figure.FallTo(cellsHeight[coordinates.x, coordinates.z], HandleFigureFall);
            figure.coordinates.coordinates.y = currentCellHeight;
        } else {
            StartNextTurn();
        }

        placedFigures[figure.coordinates.coordinates.x, figure.coordinates.coordinates.z, figure.coordinates.coordinates.y] = figure;
        cellsHeight[coordinates.x, coordinates.z]++;
    }

    private void HandleFigureFall() {
        selectionFigure.Active = true;
        StartNextTurn();
    }

    private void StartNextTurn() {
        GameManager.StartNextTurn();
        selectionFigure.SwitchForm();
    }

    public bool CheckFigureOn(Vector3Int coordinates) {
        return placedFigures[coordinates.x, coordinates.z, coordinates.y] != null;
    }
}
