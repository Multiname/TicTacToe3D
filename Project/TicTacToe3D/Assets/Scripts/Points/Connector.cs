using UnityEngine;

public class Connector : MonoBehaviour {
    [SerializeField] GameObject body;

    public void RotateBody(Vector3 angle) {
        body.transform.Rotate(angle);
    }
}
