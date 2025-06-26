using UnityEngine;

public class Figure : MonoBehaviour {
    public enum FigureType {
        SPHERE,
        CROSS
    }

    [SerializeField] FigureType type = FigureType.SPHERE;
}
