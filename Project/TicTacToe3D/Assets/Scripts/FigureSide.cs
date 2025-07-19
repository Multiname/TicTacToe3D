using UnityEngine;
using UnityEngine.EventSystems;

public class FigureSide : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    public enum FigureSideType {
        Y_PLUS,
        X_PLUS,
        X_MINUS,
        Z_PLUS,
        Z_MINUS
    }
    private static readonly Vector3Int[] directions = new Vector3Int[5] {
        new(0, 1, 0),
        new(1, 0, 0),
        new(-1, 0, 0),
        new(0, 0, 1),
        new(0, 0, -1)
    };

    [SerializeField] FigureSideType type = FigureSideType.Y_PLUS;
    private Vector3Int direction = new();

    [SerializeField] FigureSideFrame frame;

    [field: SerializeField] public Coordinates Coordinates { private get; set; }
    public bool Active { private get; set; } = true;

    private SelectionFigure selectionFigure = null;

    private void Start() {
        direction = directions[(int)type];

        selectionFigure = GameObject.FindWithTag("SelectionFigure").GetComponent<SelectionFigure>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (Active) {
            frame.SetVisibility(true);
            selectionFigure.MoveSelectionFigure(this, Coordinates, direction);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (Active) {
            frame.SetVisibility(false);
            selectionFigure.Detach(this);
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (Active) {
            selectionFigure.ConfirmSelection();
        }
    }
}
