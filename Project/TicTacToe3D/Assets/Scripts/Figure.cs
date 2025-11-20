using System;
using System.Collections;
using UnityEngine;

public class Figure : MonoBehaviour {
    public const int NUMBER_OF_FIGURE_TYPES = 2;
    public enum FigureType {
        SPHERE,
        CROSS
    }

    [field: SerializeField] public FigureType Type { get; private set; } = FigureType.SPHERE;
    [SerializeField] IdleAnimation idleAnimation;
    [SerializeField] MeshRenderer body;
    [SerializeField] ParticleSystem fallVfx;
    [SerializeField] ParticleSystem destructionVfx;
    [SerializeField] Material defaultBodyMaterial;
    [SerializeField] Material ditheredBodyMaterial;

    public Coordinates coordinates;
    public bool Visible {
        get => body.gameObject.activeSelf;
        set => body.gameObject.SetActive(value);
    }

    public bool involvedInPointEffect = false;

    private void Start() {
        PointsEffector.OnStartNextEffectEvent += HandleOnStartNextEffectEvent;
        PointsEffector.OnEndEffectsEvent += HandleOnEndEffectsEvent;
    }

    private void OnDestroy() {
        PointsEffector.OnStartNextEffectEvent -= HandleOnStartNextEffectEvent;
        PointsEffector.OnEndEffectsEvent -= HandleOnEndEffectsEvent;
    }

    private void HandleOnStartNextEffectEvent() {
        if (involvedInPointEffect) {
            body.material = defaultBodyMaterial;
        } else {
            body.material = ditheredBodyMaterial;
        }
    }

    private void HandleOnEndEffectsEvent() {
        body.material = defaultBodyMaterial;
    }

    public void FallTo(int y, Action callback, bool playFallVfx = true) {
        idleAnimation.enabled = false;
        StartCoroutine(Fall(y, callback, playFallVfx));
    }

    private IEnumerator Fall(int y, Action callback, bool playFallVfx) {
        float speed = 0.0f;
        float acceleration = 0.03f;
        while (transform.position.y > y) {
            transform.position += speed * Time.deltaTime * Vector3.down;
            speed += acceleration;
            yield return null;
        }
        StopFalling(callback, playFallVfx);
    }

    private void StopFalling(Action callback, bool playFallVfx) {
        if (playFallVfx) {
            fallVfx.Play();
            SfxPlayer.PlaySound(SfxPlayer.Sound.FIGURE_FALL);
        }
        idleAnimation.enabled = true;
        callback();
    }

    public void PlayDestructionSfx() {
        SfxPlayer.PlaySound(SfxPlayer.Sound.DESTROY_FIGURE);
        Instantiate(destructionVfx, transform.position, Quaternion.identity);
    }
}
