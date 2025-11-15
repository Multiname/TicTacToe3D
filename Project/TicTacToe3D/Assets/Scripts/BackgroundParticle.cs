using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(IdleAnimation))]
public class BackgroundParticle : MonoBehaviour {
    private const float MIN_Y = -2.5f;
    private const float MAX_Y = -1.0f;
    private const float DELTA_XZ = 0.3f;
    private const float MIN_SCALE = 0.3f;
    private const float MAX_SCALE = 0.7f;
    private const float MIN_DISTANCE = 0.1f;
    private const float MAX_DISTANCE = 0.3f;
    private const float MIN_SPEED = 0.1f;
    private const float MAX_SPEED = 0.5f;

    [SerializeField] Material sphereMaterial;
    [SerializeField] Material crossMaterial;

    [SerializeField] int minFigureHeight = 0;

    private MeshRenderer meshRenderer;
    private IdleAnimation idleAnimation;

    private float MasterAlpha {
        get => meshRenderer.material.color.a;
        set {
            var color = meshRenderer.material.color;
            color.a = value;
            meshRenderer.material.color = color;

            idleAnimation.enabled = value > 0;
        }
    }
    
    private float _heightAlpha = 1.0f;
    private float HeightAlpha {
        get => _heightAlpha;
        set {
            _heightAlpha = value;
            CalculateMasterAlpha();
        }
    }

    private float _colorAlpha = 1.0f;
    private float ColorAlpha {
        get => _colorAlpha;
        set {
            _colorAlpha = value;
            CalculateMasterAlpha();
        }
    }

    private void CalculateMasterAlpha() {
        MasterAlpha = _heightAlpha * _colorAlpha;
    }

    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        idleAnimation = GetComponent<IdleAnimation>();

        GameManager.OnStartNextTurnEvent += SwitchMaterial;
        CameraMovement.OnUpdateFieldOfViewEvent += ChangeVisibility;

        Vector3 basePosition = transform.position;
        transform.localPosition = new(
            basePosition.x + Random.Range(-DELTA_XZ, DELTA_XZ),
            Random.Range(MIN_Y, MAX_Y),
            basePosition.z + Random.Range(-DELTA_XZ, DELTA_XZ)
        );

        float randomScale = Random.Range(MIN_SCALE, MAX_SCALE);
        transform.localScale = Vector3.one * randomScale;

        transform.Rotate(new(Random.Range(0.0f, 90.0f), Random.Range(0.0f, 90.0f), Random.Range(0.0f, 90.0f)));

        idleAnimation.distance = Random.Range(MIN_DISTANCE, MAX_DISTANCE);
        idleAnimation.speed = Random.Range(MIN_SPEED, MAX_SPEED);

        if (minFigureHeight != 0) {
            HeightAlpha = 0;
        }
    }

    private void OnDestroy() {
        GameManager.OnStartNextTurnEvent -= SwitchMaterial;
        CameraMovement.OnUpdateFieldOfViewEvent -= ChangeVisibility;
    }

    private void SwitchMaterial(Figure.FigureType figureType) {
        targetColor = figureType;

        if (colorTransitionIsRunning) {
            targetColorAlpha = (targetColorAlpha + 1) % 2;
            stepSign *= -1;
        } else {
            targetColorAlpha = 0;
            stepSign = -1;
            StartCoroutine(TransformToTargetMaterial());
        }
    }

    private bool colorTransitionIsRunning = false;
    private int targetColorAlpha = 0;
    private int stepSign = -1;
    private Figure.FigureType targetColor = Figure.FigureType.SPHERE;
    private Figure.FigureType currentColor = Figure.FigureType.SPHERE;
    private const float COLOR_TRANSITION_SPEED = 3.0f;
    public static float settingsEffectsSpeed = 1.0f;
    private IEnumerator TransformToTargetMaterial() {
        colorTransitionIsRunning = true;

        float finalSpeed = COLOR_TRANSITION_SPEED * settingsEffectsSpeed;

        while (currentColor != targetColor || ColorAlpha != targetColorAlpha) {
            ColorAlpha += stepSign * finalSpeed * Time.deltaTime;
            yield return null;

            if (stepSign * ColorAlpha >= stepSign * targetColorAlpha) {
                ColorAlpha = targetColorAlpha;

                if (currentColor != targetColor) {
                    if (targetColor == Figure.FigureType.SPHERE) {
                        meshRenderer.material = sphereMaterial;
                    } else {
                        meshRenderer.material = crossMaterial;
                    }
                    CalculateMasterAlpha();
                    currentColor = targetColor;

                    targetColorAlpha = (targetColorAlpha + 1) % 2;
                    stepSign *= -1;
                }
            }
        }

        colorTransitionIsRunning = false;
    }

    private void ChangeVisibility(int newMaxHeight, float transitionDuration) {
        bool newVisibility = newMaxHeight >= minFigureHeight;

        if (newVisibility && HeightAlpha == 0) {
            StartCoroutine(Show(transitionDuration));
        } else if (!newVisibility && HeightAlpha == 1) {
            StartCoroutine(Hide(transitionDuration));
        }
    }

    private IEnumerator Show(float transitionDuration) {
        float step = 1 / transitionDuration;

        while (HeightAlpha < 1) {
            HeightAlpha += step * Time.deltaTime;
            yield return null;
        }
        HeightAlpha = 1;
    }

    private IEnumerator Hide(float transitionDuration) {
        float step = 1 / transitionDuration;

        while (HeightAlpha > 0) {
            HeightAlpha -= step * Time.deltaTime;
            yield return null;
        }
        HeightAlpha = 0;
    }
}
