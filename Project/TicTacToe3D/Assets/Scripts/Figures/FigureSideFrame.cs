using UnityEngine;

public class FigureSideFrame : MonoBehaviour {
    [SerializeField] private LineRenderer outer;
    [SerializeField] private LineRenderer inner;

    private readonly Vector3[] corners = new Vector3[4];
    private const float animationSpeed = 0.5f;

    private void Start() {
        inner.GetPositions(corners);
    }

    private void Update() {
        var scale = Time.time * animationSpeed % 1;
        for (int i = 0; i < 4; ++i) {
            inner.SetPosition(i, corners[i] * scale);
        }
    }

    public void SetVisibility(bool visible) {
        outer.gameObject.SetActive(visible);
        inner.gameObject.SetActive(visible);
    }

    public void SetMeterial(Material material) {
        outer.material = material;
        inner.material = material;
    }
}