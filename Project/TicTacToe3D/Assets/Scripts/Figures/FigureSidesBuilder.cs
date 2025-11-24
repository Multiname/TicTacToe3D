using System;
using UnityEngine;

public class FigureSidesBuilder : MonoBehaviour {
    [SerializeField] GameObject SideYPlus;
    [SerializeField] GameObject SideXPlus;
    [SerializeField] GameObject SideXMinus;
    [SerializeField] GameObject SideZPlus;
    [SerializeField] GameObject SideZMinus;

    private Action<Transform, Coordinates>[,] sidesBuilders;

    private void Start() {
        sidesBuilders = new Action<Transform, Coordinates>[2, 3] {
            {
                (parent, coordinates) => {
                    AttachFigureSideComponent(
                        CreateFigureSideObject(SideXPlus, parent),
                        FigureSide.FigureSideType.X_PLUS,
                        coordinates
                    );

                    CreateFigureSideObject(SideXMinus, parent);
                },
                (parent, coordinates) => {
                    AttachFigureSideComponent(
                        CreateFigureSideObject(SideXPlus, parent),
                        FigureSide.FigureSideType.X_PLUS,
                        coordinates
                    );

                    AttachFigureSideComponent(
                        CreateFigureSideObject(SideXMinus, parent),
                        FigureSide.FigureSideType.X_MINUS,
                        coordinates
                    );
                },
                (parent, coordinates) => {
                    AttachFigureSideComponent(
                        CreateFigureSideObject(SideXMinus, parent),
                        FigureSide.FigureSideType.X_MINUS,
                        coordinates
                    );

                    CreateFigureSideObject(SideXPlus, parent);
                }
            },
            {
                (parent, coordinates) => {
                    AttachFigureSideComponent(
                        CreateFigureSideObject(SideZPlus, parent),
                        FigureSide.FigureSideType.Z_PLUS,
                        coordinates
                    );

                    CreateFigureSideObject(SideZMinus, parent);
                },
                (parent, coordinates) => {
                    AttachFigureSideComponent(
                        CreateFigureSideObject(SideZPlus, parent),
                        FigureSide.FigureSideType.Z_PLUS,
                        coordinates
                    );

                    AttachFigureSideComponent(
                        CreateFigureSideObject(SideZMinus, parent),
                        FigureSide.FigureSideType.Z_MINUS,
                        coordinates
                    );
                },
                (parent, coordinates) => {
                    AttachFigureSideComponent(
                        CreateFigureSideObject(SideZMinus, parent),
                        FigureSide.FigureSideType.Z_MINUS,
                        coordinates
                    );

                    CreateFigureSideObject(SideZPlus, parent);
                }
            }
        };
    }

    private GameObject CreateFigureSideObject(GameObject side, Transform parent) {
        return Instantiate(side, parent);
    }

    private void AttachFigureSideComponent(GameObject side, FigureSide.FigureSideType type, Coordinates coordinates) {
        var figureSide = side.AddComponent<FigureSide>();
        figureSide.Type = type;
        figureSide.Coordinates = coordinates;
    }

    public void BuildSides(Transform parent, Coordinates coordinates) {
        AttachFigureSideComponent(
            CreateFigureSideObject(SideYPlus, parent),
            FigureSide.FigureSideType.Y_PLUS,
            coordinates
        );

        sidesBuilders[0, coordinates.coordinates.x](parent, coordinates);
        sidesBuilders[1, coordinates.coordinates.z](parent, coordinates);
    }
}
