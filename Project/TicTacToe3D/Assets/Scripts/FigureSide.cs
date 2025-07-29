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

    [field: SerializeField] public FigureSideType Type { private get; set; } = FigureSideType.Y_PLUS;

    [field: SerializeField] public Coordinates Coordinates { private get; set; }

    private static SelectionFigure selectionFigure = null;

    private void Start() {
        if (selectionFigure == null) {
            selectionFigure = GameObject.FindWithTag("SelectionFigure").GetComponent<SelectionFigure>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        selectionFigure.MoveSelectionFigure(this, Coordinates, Type);
    }

    public void OnPointerExit(PointerEventData eventData) {
        selectionFigure.Detach(this);
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            selectionFigure.ConfirmSelection();
        }
    }

    private void OnDestroy() {
        selectionFigure.Detach(this);
    }
}
