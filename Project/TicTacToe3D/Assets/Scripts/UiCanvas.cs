using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiCanvas : MonoBehaviour {
    [SerializeField] Board board;

    [SerializeField] TextMeshProUGUI[] scoreTexts = new TextMeshProUGUI[Figure.NUMBER_OF_FIGURE_TYPES];

    [SerializeField] TextMeshProUGUI[] basePointsTexts = new TextMeshProUGUI[Figure.NUMBER_OF_FIGURE_TYPES];
    [SerializeField] GameObject[] basePointsContainers = new GameObject[Figure.NUMBER_OF_FIGURE_TYPES];

    [SerializeField] TextMeshProUGUI[] points2DTexts = new TextMeshProUGUI[Figure.NUMBER_OF_FIGURE_TYPES];
    [SerializeField] GameObject[] points2DContainers = new GameObject[Figure.NUMBER_OF_FIGURE_TYPES];

    [SerializeField] TextMeshProUGUI[] points3DTexts = new TextMeshProUGUI[Figure.NUMBER_OF_FIGURE_TYPES];
    [SerializeField] GameObject[] points3DContainers = new GameObject[Figure.NUMBER_OF_FIGURE_TYPES];

    [SerializeField] TextMeshProUGUI[] fallPointsTexts = new TextMeshProUGUI[Figure.NUMBER_OF_FIGURE_TYPES];
    [SerializeField] GameObject[] fallPointsContainers = new GameObject[Figure.NUMBER_OF_FIGURE_TYPES];

    [SerializeField] TextMeshProUGUI[] heightPointsTexts = new TextMeshProUGUI[Figure.NUMBER_OF_FIGURE_TYPES];
    [SerializeField] GameObject[] heightPointsContainers = new GameObject[Figure.NUMBER_OF_FIGURE_TYPES];

    [SerializeField] TextMeshProUGUI[] comboPointsTexts = new TextMeshProUGUI[Figure.NUMBER_OF_FIGURE_TYPES];
    [SerializeField] GameObject[] comboPointsContainers = new GameObject[Figure.NUMBER_OF_FIGURE_TYPES];

    private readonly int[] currentScores = new int[Figure.NUMBER_OF_FIGURE_TYPES];
    private readonly int[] upcomingScores = new int[Figure.NUMBER_OF_FIGURE_TYPES];
    private readonly List<GameObject> upcomingPointsContainers = new();

    private void PreparePoints(int[] gainedPoints, TextMeshProUGUI[] pointsTexts, GameObject[] pointsContainers) {
        for (int type = 0; type < Figure.NUMBER_OF_FIGURE_TYPES; ++type) {
            if (gainedPoints[type] > 0) {
                pointsTexts[type].text = gainedPoints[type].ToString();
                upcomingPointsContainers.Add(pointsContainers[type]);
                upcomingScores[type] += gainedPoints[type];
            }
        }
    }

    public void PrepareBasePoints() {
        PreparePoints(board.gainedBasePoints, basePointsTexts, basePointsContainers);
        PrepareComboPoints();
    }

    public void Prepare2DPoints() {
        PreparePoints(board.gained2DPoints, points2DTexts, points2DContainers);
    }

    public void Prepare3DPoints() {
        PreparePoints(board.gained3DPoints, points3DTexts, points3DContainers);
    }

    public void PrepareFallPoints() {
        PreparePoints(board.gainedFallPoints, fallPointsTexts, fallPointsContainers);
    }

    public void PrepareHeightPoints() {
        PreparePoints(board.gainedHeightPoints, heightPointsTexts, heightPointsContainers);
    }

    private void PrepareComboPoints() {
        PreparePoints(board.gainedComboPoints, comboPointsTexts, comboPointsContainers);
    }

    public void SetNewPointsVisibility(bool visibility) {
        upcomingPointsContainers.ForEach(c => c.SetActive(visibility));
        if (visibility) {
            for (Figure.FigureType type = 0; (int)type < Figure.NUMBER_OF_FIGURE_TYPES; ++type) {
                scoreTexts[(int)type].text = upcomingScores[(int)type].ToString();
            }
        } else {
            for (Figure.FigureType type = 0; (int)type < Figure.NUMBER_OF_FIGURE_TYPES; ++type) {
                scoreTexts[(int)type].text = currentScores[(int)type].ToString();
            }
        }
    }

    public void AcceptNewPoints() {
        SetNewPointsVisibility(true);
        for (Figure.FigureType type = 0; (int)type < Figure.NUMBER_OF_FIGURE_TYPES; ++type) {
            currentScores[(int)type] = upcomingScores[(int)type];
        }
        upcomingPointsContainers.Clear();
    }

    public void HidePointsContainers() {
        for (Figure.FigureType type = 0; (int)type < Figure.NUMBER_OF_FIGURE_TYPES; ++type) {
            basePointsContainers[(int)type].SetActive(false);
            points2DContainers[(int)type].SetActive(false);
            points3DContainers[(int)type].SetActive(false);
            fallPointsContainers[(int)type].SetActive(false);
            heightPointsContainers[(int)type].SetActive(false);
            comboPointsContainers[(int)type].SetActive(false);
        }
    }
}
