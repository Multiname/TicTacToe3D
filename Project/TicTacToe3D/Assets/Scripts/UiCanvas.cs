using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UiCanvas : MonoBehaviour {
    [SerializeField] GameManager gameManager;

    [SerializeField] TextMeshProUGUI sphereScore;
    [SerializeField] TextMeshProUGUI crossScore;

    [SerializeField] RectTransform sphereBasePointsIcon;
    [SerializeField] RectTransform crossBasePointsIcon;

    [SerializeField] TextMeshProUGUI sphereBasePointsText;
    [SerializeField] TextMeshProUGUI crossBasePointsText;

    [SerializeField] BasePointParticle sphereBasePointParticlePrefab;
    [SerializeField] BasePointParticle crossBasePointParticlePrefab;

    private RectTransform rt;
    private Vector2 sphereBasePointsIconPosition;
    private Vector2 crossBasePointsIconPosition;

    private int _sphereGainedBasePoints = 0;
    private int SphereGainedBasePoints {
        get => _sphereGainedBasePoints;
        set {
            _sphereGainedBasePoints = value;
            sphereBasePointsText.text = _sphereGainedBasePoints.ToString();
        }
    }

    private int _crossGainedBasePoints = 0;
    private int CrossGainedBasePoints {
        get => _crossGainedBasePoints;
        set {
            _crossGainedBasePoints = value;
            crossBasePointsText.text = _crossGainedBasePoints.ToString();
        }
    }

    private void Start() {
        rt = GetComponent<RectTransform>();

        Canvas.ForceUpdateCanvases();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rt,
            RectTransformUtility.WorldToScreenPoint(Camera.main, sphereBasePointsIcon.position) + sphereBasePointsIcon.rect.center,
            Camera.main,
            out sphereBasePointsIconPosition
        );

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rt,
            RectTransformUtility.WorldToScreenPoint(Camera.main, crossBasePointsIcon.position) + crossBasePointsIcon.rect.center,
            Camera.main,
            out crossBasePointsIconPosition
        );
    }

    public void SetScore(Figure.FigureType figureType, int score) {
        if (figureType == Figure.FigureType.SPHERE) {
            sphereScore.text = score.ToString();
        } else {
            crossScore.text = score.ToString();
        }
    }

    public async UniTask GainBasePoints(ICollection<Figure> blewFigures) {
        var particleTasks = new List<UniTask>();

        foreach (var figure in blewFigures) {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(figure.transform.position + 0.85f / 2 * Vector3.up);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rt,
                screenPoint,
                Camera.main,
                out Vector2 anchoredPosition
            );

            if (figure.Type == Figure.FigureType.SPHERE) {
                var particle = Instantiate(sphereBasePointParticlePrefab, transform);
                particleTasks.Add(particle.Appear(anchoredPosition, sphereBasePointsIconPosition, () => ++SphereGainedBasePoints));
            } else {
                var particle = Instantiate(crossBasePointParticlePrefab, transform);
                particleTasks.Add(particle.Appear(anchoredPosition, crossBasePointsIconPosition, () => ++CrossGainedBasePoints));
            }
        }

        await UniTask.WhenAll(particleTasks);
    }
}
