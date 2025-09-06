using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BasePointParticle : MonoBehaviour {
    [SerializeField] float rotationSpeed = 10.0f;
    [SerializeField] float appearanceEffectSpeed = 0.2f;
    [SerializeField] float initialMagnification = 2.0f;
    [SerializeField] float particleAcceleration = 10.0f;
    [SerializeField] float pauseDuration = 1.0f;

    [SerializeField] Image image;
    [SerializeField] RectTransform trailEffectPrefab;

    private RectTransform rt;
    private RectTransform trailEffect;

    private float scaleStep;

    private void Awake() {
        rt = GetComponent<RectTransform>();

        scaleStep = (initialMagnification - 1.0f) * appearanceEffectSpeed;
    }

    private void Update() {
        image.transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));
    }

    public async UniTask Appear(Vector2 startPosition, Vector2 targetPosition, RectTransform cameraCanvas, Action callback) {
        rt.anchoredPosition = startPosition;

        trailEffect = Instantiate(trailEffectPrefab, cameraCanvas);
        trailEffect.anchoredPosition = rt.anchoredPosition;

        transform.localScale = initialMagnification * Vector3.one;

        while (image.color.a < 1.0f) {
            var color = image.color;
            color.a += appearanceEffectSpeed * Time.deltaTime;
            image.color = color;

            transform.localScale -= scaleStep * Time.deltaTime * Vector3.one;

            await UniTask.Yield();
        }

        transform.localScale = Vector3.one;

        await UniTask.WaitForSeconds(pauseDuration);

        Vector2 direction = (targetPosition - rt.anchoredPosition).normalized;
        float speed = 0.0f;

        while (Vector3.Distance(rt.anchoredPosition, targetPosition) > 10.0f) {
            speed += particleAcceleration;
            rt.anchoredPosition += speed * Time.deltaTime * direction;
            trailEffect.anchoredPosition = rt.anchoredPosition;
            await UniTask.Yield();
        }

        callback();
        Destroy(trailEffect.gameObject);
        Destroy(gameObject);
    }
}
