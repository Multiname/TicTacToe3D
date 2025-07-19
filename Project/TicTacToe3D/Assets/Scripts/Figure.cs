using System;
using System.Collections;
using UnityEngine;

public class Figure : MonoBehaviour {
    public enum FigureType {
        SPHERE,
        CROSS
    }

    [SerializeField] FigureType type = FigureType.SPHERE;
    [SerializeField] IdleAnimation idleAnimation;

    public Coordinates coordinates;

    public void FallTo(int y, Action callback) {
        idleAnimation.enabled = false;
        StartCoroutine(Fall(y, callback));
    }

    private IEnumerator Fall(int y, Action callback) {
        float speed = 0.0f;
        float acceleration = 0.03f;
        while (transform.position.y > y) {
            transform.position += speed * Time.deltaTime * Vector3.down;
            speed += acceleration;
            yield return null;
        }
        StopFalling(callback);
    }

    private void StopFalling(Action callback) {
        idleAnimation.enabled = true;
        callback();
    }
}
