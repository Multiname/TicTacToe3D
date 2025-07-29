using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour {
    private enum State {
        NONE,
        ROTATING,
        MOVING
    }

    private State state = State.NONE;
    private Action[] handlers;

    private bool clicked = false;
    private Vector2 clickPosition;

    [SerializeField] SelectionFigure selectionFigure;

    [SerializeField] float rotationSpeed = 1.0f;
    [SerializeField] float moveSpeed = 1.0f;
    [SerializeField] float activationThreshold = 0.0f;
    [SerializeField] float maxHeight = 1.0f;

    private void Start() {
        handlers = new Action[3] {
            () => {
                if (!clicked) {
                    clicked = true;
                    clickPosition = Mouse.current.position.ReadValue();
                } else if (Vector2.Distance(clickPosition, Mouse.current.position.ReadValue()) > activationThreshold) {
                    if (Mathf.Abs(Mouse.current.delta.x.ReadValue()) > Mathf.Abs(Mouse.current.delta.y.ReadValue())) {
                        state = State.ROTATING;
                    } else {
                        state = State.MOVING;
                    }
                }
            },
            () => {
                selectionFigure.CameraIsMoving = true;
                transform.Rotate(Mouse.current.delta.x.ReadValue() * rotationSpeed * Vector3.up);
            },
            () => {
                selectionFigure.CameraIsMoving = true;
                float newHeight = transform.position.y - Mouse.current.delta.y.ReadValue() * moveSpeed;
                if (newHeight > maxHeight) {
                    newHeight = maxHeight;
                } else if (newHeight < 0) {
                    newHeight = 0;
                }
                transform.position = new(1, newHeight, 1);
            }
        };
    }

    private void Update() {
        if (Mouse.current.rightButton.isPressed) {
            handlers[(int)state]();
        } else {
            state = State.NONE;
            clicked = false;
            selectionFigure.CameraIsMoving = false;
        }
    }
}