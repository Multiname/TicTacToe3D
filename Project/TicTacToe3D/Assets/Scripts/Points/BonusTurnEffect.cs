using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BonusTurnEffect : MonoBehaviour {
    [SerializeField] GameSettings gameSettings;
    [SerializeField] RectTransform bonusTurnArea;
    [SerializeField] Image bonusTurnIcon;

    [SerializeField] Sprite[] bonusTurnSprites = new Sprite[Figure.NUMBER_OF_FIGURE_TYPES];

    [SerializeField] float maxY = 0.5f;
    [SerializeField] float speed = 1.0f;

    private RectTransform bonusTurnIconTransform;

    private float minY;
    private float deltaY;

    private void Start() {
        bonusTurnIconTransform = bonusTurnIcon.GetComponent<RectTransform>();

        minY = bonusTurnArea.anchorMin.y;
        deltaY = maxY - minY;
    }

    public async UniTask StartEffect(Figure.FigureType figureType) {
        SfxPlayer.PlaySound(SfxPlayer.Sound.BONUS_TURN);

        bonusTurnIcon.sprite = bonusTurnSprites[(int)figureType];

        var anchorMin = bonusTurnArea.anchorMin;
        var color = Color.white;
        color.a = 0;

        float finalSpeed = speed * gameSettings.EffectsSpeed;

        float x = -1.0f;
        while (x <= 1.0f) {
            var y = Mathf.Sqrt(1 - Mathf.Pow(x, 2));

            anchorMin.y = minY + deltaY * y;
            bonusTurnArea.anchorMin = anchorMin;

            if (x <= 0.0f) {
                bonusTurnIconTransform.localEulerAngles = new Vector3(0, 0, -180 * y);
            } else {
                bonusTurnIconTransform.localEulerAngles = new Vector3(0, 0, -180 - 135 * (1 - y));
            }

            color.a = y;
            bonusTurnIcon.color = color;

            x += finalSpeed * Time.deltaTime;
            await UniTask.Yield();
        }

        anchorMin.y = minY;
        bonusTurnArea.anchorMin = anchorMin;

        bonusTurnIconTransform.localEulerAngles = Vector3.zero;

        color.a = 0;
        bonusTurnIcon.color = color;
    }
}
