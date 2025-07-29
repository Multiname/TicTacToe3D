using UnityEngine;

public class SelectionFigure : MonoBehaviour {
    [SerializeField] Board board;

    [SerializeField] GameObject body;
    [SerializeField] GameObject sphereForm;
    [SerializeField] GameObject crossForm;
    [SerializeField] FigureSideFrame[] figureSideFrames = new FigureSideFrame[5];
    [SerializeField] Material sphereFrameMaterial;
    [SerializeField] Material crossFrameMaterial;
    private static Material[] frameMaterials;

    private static readonly Vector3Int[] figureSideDirections = new Vector3Int[5] {
        new(0, 1, 0),
        new(1, 0, 0),
        new(-1, 0, 0),
        new(0, 0, 1),
        new(0, 0, -1)
    };

    private bool ReadyToPlace {
        get => !FiguersFall && !CameraIsMoving;
    }
    private bool _figuresFall = false;
    public bool FiguersFall {
        private get => _figuresFall;
        set {
            _figuresFall = value;
            if (attachedFigureSide != null) {
                body.SetActive(ReadyToPlace);
            }
        }
    }
    private bool _cameraIsMoving = false;
    public bool CameraIsMoving {
        private get => _cameraIsMoving;
        set {
            _cameraIsMoving = value;
            if (attachedFigureSide != null) {
                body.SetActive(ReadyToPlace);
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
    private FigureSideFrame visibleFigureSideFrame = null;

    private void Start() {
        frameMaterials = new Material[2] {
            sphereFrameMaterial,
            crossFrameMaterial
        };
    }

    public void MoveSelectionFigure(FigureSide figureSide, Coordinates figureSideCoordinates, FigureSide.FigureSideType figureSideType) {
        Vector3Int newCoordinates = figureSideCoordinates.coordinates + figureSideDirections[(int)figureSideType];
        if (!board.CheckFigureOn(newCoordinates) && newCoordinates.y < Board.MAX_Y) {
            body.SetActive(ReadyToPlace);

            if (visibleFigureSideFrame != null) {
                visibleFigureSideFrame.SetVisibility(false);
            }
            visibleFigureSideFrame = figureSideFrames[(int)figureSideType];
            visibleFigureSideFrame.SetVisibility(true);

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
        visibleFigureSideFrame.SetVisibility(false);
        visibleFigureSideFrame = null;
        body.SetActive(false);
    }

    public void ConfirmSelection() {
        if (ReadyToPlace && body.activeSelf) {
            board.PlaceFigure(Coordinates);
            if (board.CheckFigureOn(Coordinates)) {
                Detach();
            }
        }
    }

    public void SwitchForm() {
        sphereForm.SetActive(!sphereForm.activeSelf);
        crossForm.SetActive(!crossForm.activeSelf);

        foreach (var frame in figureSideFrames) {
            frame.SetMeterial(frameMaterials[(int)GameManager.CurrentPlayer]);
        }
    }
}
