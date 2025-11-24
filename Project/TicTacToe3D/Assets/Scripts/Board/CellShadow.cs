using UnityEngine;

public class CellShadow : MonoBehaviour {
    [SerializeField] GameObject crossShadow;
    [SerializeField] GameObject sphereShadow;

    private readonly GameObject[] shadows = new GameObject[2];

    private void Start() {
        shadows[(int)Figure.FigureType.SPHERE] = sphereShadow;
        shadows[(int)Figure.FigureType.CROSS] = crossShadow;
    }

    public void HideShadow() {
        sphereShadow.SetActive(false);
        crossShadow.SetActive(false);
    }

    public void ShowShadow(Figure.FigureType figureType) {
        HideShadow();
        shadows[(int)figureType].SetActive(true);
    }
}
