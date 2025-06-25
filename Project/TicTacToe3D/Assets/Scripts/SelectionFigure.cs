using UnityEngine;

public class SelectionFigure : MonoBehaviour {
    private Vector3 coordinates = new();
    public Vector3 Coordinates {
        get => coordinates;
        set {
            coordinates = value;
            transform.position = value;
        }
    }

    private FigureSide attachedFigureSide = null;

    public void MoveSelectionFigure(FigureSide figureSide, Vector3 figureSideCoordinates, Vector3 figureSideDirection) {
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
}
