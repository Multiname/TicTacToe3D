using UnityEngine;

public class SelectionFigure : MonoBehaviour {
    [SerializeField] Board board;

    [SerializeField] GameObject sphereForm;
    [SerializeField] GameObject crossForm;

    private Vector3Int _coordinates = new();
    private Vector3Int Coordinates {
        get => _coordinates;
        set {
            _coordinates = value;
            transform.position = value;
        }
    }

    private FigureSide attachedFigureSide = null;

    public void MoveSelectionFigure(FigureSide figureSide, Vector3Int figureSideCoordinates, Vector3Int figureSideDirection) {
        gameObject.SetActive(true);
        attachedFigureSide = figureSide;
        Coordinates = figureSideCoordinates + figureSideDirection;
    }

    public void Detach(FigureSide figureSide) {
        if (attachedFigureSide == figureSide) {
            attachedFigureSide = null;
            gameObject.SetActive(false);
        }
    }

    public void ConfirmSelection() {
        board.PlaceFigure(Coordinates);
    }

    public void SwitchForm() {
        sphereForm.SetActive(!sphereForm.activeSelf);
        crossForm.SetActive(!crossForm.activeSelf);
    }
}
