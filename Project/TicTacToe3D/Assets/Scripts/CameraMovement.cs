using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour {
    [SerializeField] GameSettings gameSettings;

    [SerializeField] SelectionFigure selectionFigure;

    [SerializeField] float rotationSpeed = 1.0f;
    [SerializeField] float transitionSpeed = 2.0f;

    private const float LOCAL_POSITION_STEP = 0.5f;
    private const float BASE_LOCAL_POSITION = 2.0f;
    private const float ORTHOGRAPHIC_SIZE_STEP = 0.4f;
    private const float BASE_ORTHOGRAPHIC_SIZE = 1.6f;

    public bool ready = true;

    public static event Action<int, float> OnUpdateFieldOfViewEvent;

    private void Update() {
        if (Mouse.current.rightButton.isPressed && ready) {
            selectionFigure.CameraIsRotating = true;
            transform.Rotate(Mouse.current.delta.x.ReadValue() * rotationSpeed * gameSettings.sensitivity * Vector3.up);
        } else if (Mouse.current.rightButton.wasReleasedThisFrame) {
            selectionFigure.CameraIsRotating = false;
        }
    }

    public void UpdateFieldOfView(int maxHeight, Action callback) {
        if (maxHeight < 1) {
            maxHeight = 1;
        } else if (maxHeight > Board.MAX_Y - 1) {
            maxHeight = Board.MAX_Y - 1;
        }

        StartCoroutine(
            TransitToFieldOfView(
                new(0.0f, BASE_LOCAL_POSITION + LOCAL_POSITION_STEP * maxHeight, -3.0f),
                BASE_ORTHOGRAPHIC_SIZE + ORTHOGRAPHIC_SIZE_STEP * maxHeight,
                maxHeight,
                callback
            )
        );
    }

    private IEnumerator TransitToFieldOfView(Vector3 targetLocalPosition, float targetOrtographicSize, int maxHeight, Action callback) {
        float finalTransitionSpeed = transitionSpeed * gameSettings.EffectsSpeed;

        OnUpdateFieldOfViewEvent?.Invoke(maxHeight, MathF.Abs(targetOrtographicSize - Camera.main.orthographicSize) / (ORTHOGRAPHIC_SIZE_STEP * finalTransitionSpeed));

        float sign = Mathf.Sign(targetOrtographicSize - Camera.main.orthographicSize);
        while (sign * (targetOrtographicSize - Camera.main.orthographicSize) > 0) {
            Camera.main.transform.localPosition += new Vector3(0.0f, sign * LOCAL_POSITION_STEP * finalTransitionSpeed * Time.deltaTime, 0.0f);
            Camera.main.orthographicSize += sign * ORTHOGRAPHIC_SIZE_STEP * finalTransitionSpeed * Time.deltaTime;
            yield return null;
        }
        Camera.main.transform.localPosition = targetLocalPosition;
        Camera.main.orthographicSize = targetOrtographicSize;
        callback();
    }
}