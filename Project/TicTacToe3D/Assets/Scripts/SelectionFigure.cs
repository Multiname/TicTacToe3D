using UnityEngine;

public class SelectionFigure : MonoBehaviour {
    [SerializeField] Board board;

    [SerializeField] GameObject body;
    [SerializeField] GameObject sphereForm;
    [SerializeField] GameObject crossForm;

    private bool _acitve = true;
    public bool Active {
        private get => _acitve;
        set {
            _acitve = value;
            if (_acitve) {
                if (attachedFigureSide != null) {
                    body.SetActive(true);
                }
            } else {
                body.SetActive(false);
            }
        }
    }

    private Vector3Int _coordinates = new();
    private Vector3Int Coordinates {
        get => _coordinates;
        set {
            _coordinates = value;
            transform.position = value;
        }
    }

    private FigureSide attachedFigureSide = null;

    public void MoveSelectionFigure(FigureSide figureSide, Coordinates figureSideCoordinates, Vector3Int figureSideDirection) {
        Vector3Int newCoordinates = figureSideCoordinates.coordinates + figureSideDirection;
        if (!board.CheckFigureOn(newCoordinates)) {
            body.SetActive(Active);
            attachedFigureSide = figureSide;
            Coordinates = newCoordinates;
        }
    }

    public void Detach(FigureSide figureSide) {
        if (attachedFigureSide == figureSide) {
            Detach();
        }
    }

    private void Detach() {
        attachedFigureSide = null;
        body.SetActive(false);
    }

    public void ConfirmSelection() {
        if (Active) {
            board.PlaceFigure(Coordinates);
            if (board.CheckFigureOn(Coordinates)) {
                Detach();
            }
        }
    }

    public void SwitchForm() {
        sphereForm.SetActive(!sphereForm.activeSelf);
        crossForm.SetActive(!crossForm.activeSelf);
    }
}
