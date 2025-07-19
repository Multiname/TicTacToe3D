using System;
using UnityEngine;

public class FigureSidesBuilder : MonoBehaviour {
    [SerializeField] FigureSide SideYPlus;
    [SerializeField] FigureSide SideXPlus;
    [SerializeField] FigureSide SideXMinus;
    [SerializeField] FigureSide SideZPlus;
    [SerializeField] FigureSide SideZMinus;

    private Action<Transform, Coordinates>[,] sidesBuilders;

    private void Start() {
        sidesBuilders = new Action<Transform, Coordinates>[2, 3] {
            {
                (parent, coordinates) => {
                    Instantiate(SideXPlus, parent).Coordinates = coordinates;
                    var side = Instantiate(SideXMinus, parent);
                    side.Coordinates = coordinates;
                    side.Active = false;
                },
                (parent, coordinates) => {
                    Instantiate(SideXPlus, parent).Coordinates = coordinates;
                    Instantiate(SideXMinus, parent).Coordinates = coordinates;
                },
                (parent, coordinates) => {
                    Instantiate(SideXMinus, parent).Coordinates = coordinates;
                    var side = Instantiate(SideXPlus, parent);
                    side.Coordinates = coordinates;
                    side.Active = false;
                }
            },
            {
                (parent, coordinates) => {
                    Instantiate(SideZPlus, parent).Coordinates = coordinates;
                    var side = Instantiate(SideZMinus, parent);
                    side.Coordinates = coordinates;
                    side.Active = false;
                },
                (parent, coordinates) => {
                    Instantiate(SideZPlus, parent).Coordinates = coordinates;
                    Instantiate(SideZMinus, parent).Coordinates = coordinates;
                },
                (parent, coordinates) => {
                    Instantiate(SideZMinus, parent).Coordinates = coordinates;
                    var side = Instantiate(SideZPlus, parent);
                    side.Coordinates = coordinates;
                    side.Active = false;
                }
            }
        };
    }

    public void BuildSides(Transform parent, Coordinates coordinates) {
        Instantiate(SideYPlus, parent).Coordinates = coordinates;
        sidesBuilders[0, coordinates.coordinates.x](parent, coordinates);
        sidesBuilders[1, coordinates.coordinates.z](parent, coordinates);
    }
}
