using UnityEngine;

public class IdleAnimation : MonoBehaviour {
    [SerializeField] float speed = 1.0f;
    [SerializeField] float distance = 0.0f;

    private Vector3 initialPoisiton;
    private float sinInput = 0.0f;

    private void Start() {
        initialPoisiton = transform.localPosition;
    }

    private void Update() {
        transform.localPosition = initialPoisiton + Mathf.Sin(sinInput) * distance * Vector3.up;
        sinInput += Time.deltaTime * speed;
    }
}
