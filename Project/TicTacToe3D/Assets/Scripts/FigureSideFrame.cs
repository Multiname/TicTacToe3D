using System;
using UnityEngine;

public class FigureSideFrame : MonoBehaviour {
    [field: SerializeField] public LineRenderer Outer { get; private set; }
    [field: SerializeField] public LineRenderer Inner { get; private set; }

    private Vector3[] corners = new Vector3[4];
    private float animationSpeed = 0.5f;

    private Action setMaterial;

    private void Awake() {
        setMaterial = () => FigureSideFrameManager.SetMaterial(this);
        FigureSideFrameManager.OnStartNextTurn += setMaterial;
    }

    private void Start() {
        Inner.GetPositions(corners);
    }

    private void OnDestroy() {
        FigureSideFrameManager.OnStartNextTurn -= setMaterial;
    }

    private void Update() {
        var scale = Time.time * animationSpeed % 1;
        for (int i = 0; i < 4; ++i) {
            Inner.SetPosition(i, corners[i] * scale);
        }
    }

    public void SetVisibility(bool visible) {
        Outer.gameObject.SetActive(visible);
        Inner.gameObject.SetActive(visible);
    }
}