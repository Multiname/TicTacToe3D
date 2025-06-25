using UnityEngine;
using UnityEngine.EventSystems;

public class FigureSide : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    private enum FigureSideType {
        Y_PLUS,
        X_PLUS,
        X_MINUS,
        Z_PLUS,
        Z_MINUS
    }
    private static readonly Vector3[] directions = new Vector3[5] {
        new(0, 1, 0),
        new(1, 0, 0),
        new(-1, 0, 0),
        new(0, 0, 1),
        new(0, 0, -1)
    };

    [SerializeField] SelectionFigure selectionFigure;

    [SerializeField] Vector3 coordinates = new();
    [SerializeField] FigureSideType type = FigureSideType.Y_PLUS;
    private Vector3 direction = new();

    private void Start() {
        direction = directions[(int)type];
    }

    public void OnPointerEnter(PointerEventData eventData) {
        selectionFigure.MoveSelectionFigure(this, coordinates, direction);
    }

    public void OnPointerExit(PointerEventData eventData) {
        selectionFigure.Detach(this);
    }
}
