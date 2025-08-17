using TMPro;
using UnityEngine;

public class UiCanvas : MonoBehaviour {
    [SerializeField] GameManager gameManager;

    [SerializeField] TextMeshProUGUI sphereScore;
    [SerializeField] TextMeshProUGUI crossScore;

    public void SetScore(Figure.FigureType figureType, int score) {
        if (figureType == Figure.FigureType.SPHERE) {
            sphereScore.text = score.ToString();
        } else {
            crossScore.text = score.ToString();
        }
    }
}
