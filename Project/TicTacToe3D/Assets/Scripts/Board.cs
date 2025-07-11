using UnityEngine;

public class Board : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] SelectionFigure selectionFigure;

    [SerializeField] FigureSide[] cellSelectionSides_0 = new FigureSide[3];
    [SerializeField] FigureSide[] cellSelectionSides_1 = new FigureSide[3];
    [SerializeField] FigureSide[] cellSelectionSides_2 = new FigureSide[3];
    private FigureSide[,] cellSelectionSides = new FigureSide[3, 3];

    [SerializeField] Figure[] figurePrefabs = new Figure[2];

    private readonly Figure[,,] placedFigures = new Figure[3, 3, 8];
    private readonly int[,] cellsHeight = new int[3, 3];

    private void Start() {
        for (int i = 0; i < 3; ++i) {
            cellSelectionSides[i, 0] = cellSelectionSides_0[i];
            cellSelectionSides[i, 1] = cellSelectionSides_1[i];
            cellSelectionSides[i, 2] = cellSelectionSides_2[i];
        }
    }

    public void PlaceFigure(Vector3Int coordinates) {
        Figure figure = Instantiate(figurePrefabs[(int)gameManager.CurrentPlayer], coordinates, Quaternion.identity);
        placedFigures[coordinates.x, coordinates.z, coordinates.y] = figure;
        Coordinates figureCoordinates = figure.GetComponent<Coordinates>();
        figureCoordinates.coordinates = coordinates;

        int currentCellHeight = cellsHeight[coordinates.x, coordinates.z];
        if (coordinates.y > currentCellHeight) {
            selectionFigure.Active = false;
            figure.FallTo(cellsHeight[coordinates.x, coordinates.z], HandleFigureFall);
            figureCoordinates.coordinates.y = currentCellHeight;
        } else {
            StartNextTurn();
        }

        if (figureCoordinates.coordinates.y == 0) {
            cellSelectionSides[coordinates.x, coordinates.z].gameObject.SetActive(false);
        }

        cellsHeight[coordinates.x, coordinates.z]++;
    }

    private void HandleFigureFall() {
        selectionFigure.Active = true;
        StartNextTurn();
    }

    private void StartNextTurn() {
        gameManager.StartNextTurn();
        selectionFigure.SwitchForm();
    }
}
