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

    private SelectionFigure selectionFigure = null;

    private void Start() {
        selectionFigure = GameObject.FindWithTag("SelectionFigure").GetComponent<SelectionFigure>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        selectionFigure.MoveSelectionFigure(this, Coordinates, Type);
    }

    public void OnPointerExit(PointerEventData eventData) {
        selectionFigure.Detach(this);
    }

    public void OnPointerClick(PointerEventData eventData) {
        selectionFigure.ConfirmSelection();
    }
}
