using UnityEngine;
using UnityEngine.EventSystems;

public class FigureSide : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    private enum FigureSideType {
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

    [SerializeField] SelectionFigure selectionFigure;

    [SerializeField] Vector3Int coordinates = new();
    [SerializeField] FigureSideType type = FigureSideType.Y_PLUS;
    private Vector3Int direction = new();

    private void Start() {
        direction = directions[(int)type];
    }

    public void OnPointerEnter(PointerEventData eventData) {
        selectionFigure.MoveSelectionFigure(this, coordinates, direction);
    }

    public void OnPointerExit(PointerEventData eventData) {
        selectionFigure.Detach(this);
    }

    public void OnPointerClick(PointerEventData eventData) {
        selectionFigure.ConfirmSelection();
    }
}
