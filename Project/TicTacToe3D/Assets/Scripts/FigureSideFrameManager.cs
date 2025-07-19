using System;
using UnityEngine;

public class FigureSideFrameManager : MonoBehaviour {
    [SerializeField] Material sphereMaterial;
    [SerializeField] Material crossMaterial;

    private static Material[] materials;

    public static event Action OnStartNextTurn;

    private void Start() {
        materials = new Material[2] {
            sphereMaterial,
            crossMaterial
        };
    }

    public static void SetMaterial(FigureSideFrame frame) {
        frame.Inner.material = materials[(int)GameManager.CurrentPlayer];
        frame.Outer.material = materials[(int)GameManager.CurrentPlayer];
    }

    public static void InvokeStartNextTurn() {
        OnStartNextTurn.Invoke();
    }
}
