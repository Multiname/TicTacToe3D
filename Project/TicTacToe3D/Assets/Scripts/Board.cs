using System;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
    [SerializeField] FigureSidesBuilder figureSidesBuilder;
    [SerializeField] SelectionFigure selectionFigure;

    [SerializeField] Figure[] figurePrefabs = new Figure[2];

    private readonly Figure[,,] placedFigures = new Figure[3, 3, 10];
    private readonly bool[][,,] placingState = new bool[][,,] {
        new bool[3, 3, 10],
        new bool[3, 3, 10],
    };
    private readonly int[,] cellsHeight = new int[3, 3];

    private Action<Figure.FigureType, int, int, int, int, int>[] singleFigureLineFinders;
    [SerializeField] private List<Figure> figuresToBlow = new();
    private readonly Dictionary<Figure, int> figuresToFall = new();
    private int figuresToFallCount = 0;

    private void LogPlacedFigures() {
        var log = "placedFigures [ ";
        for (int i = 0; i < 3; ++i) {
            for (int j = 0; j < 3; ++j) {
                if (placedFigures[i, j, 0] == null) {
                    log += "_ ";
                } else if (placedFigures[i, j, 0].Type == Figure.FigureType.SPHERE) {
                    log += "O ";
                } else {
                    log += "X ";
                }
            }
            log += "| ";
        }
        log += "]";
        Debug.Log(log);
    }

    private void LogPlacingState() {
        var log = "placingState O [ ";
        for (int i = 0; i < 3; ++i) {
            for (int j = 0; j < 3; ++j) {
                if (placingState[0][i, j, 0] == false) {
                    log += "_ ";
                } else {
                    log += "O ";
                }
            }
            log += "| ";
        }
        log += "]";
        Debug.Log(log);
 
        log = "placingState X [ ";
        for (int i = 0; i < 3; ++i) {
            for (int j = 0; j < 3; ++j) {
                if (placingState[1][i, j, 0] == false) {
                    log += "_ ";
                } else {
                    log += "X ";
                }
            }
            log += "| ";
        }
        log += "]";
        Debug.Log(log);
    }

    private void LogCellsHeight() {
        var log = "cellsHeight [ ";
        for (int i = 0; i < 3; ++i) {
            for (int j = 0; j < 3; ++j) {
                log += cellsHeight[i, j] + " ";
            }
            log += "| ";
        }
        log += "]";
        Debug.Log(log);
    }

    private void LogFiguresToFall() {
        var log = "figuresToFall [ ";
        foreach (var (figure, height) in figuresToFall) {
            log += $"{figure.coordinates.coordinates.x} {figure.coordinates.coordinates.z} {figure.coordinates.coordinates.y} {height} | ";
        }
        log += "]";
        Debug.Log(log);
    }

    private void Start() {
        singleFigureLineFinders = new Action<Figure.FigureType, int, int, int, int, int>[3] {
            (t, x, x_, z, z_, y) => {
                int y_m = y - 1;
                int y_p = y + 1;

                var f = placingState[(int)t];

                CheckLine(f, 0, 1, y, 2, 1, y);
                CheckLine(f, 1, 0, y, 1, 2, y);
                CheckLine(f, 1, 1, y_m, 1, 1, y-2);

                CheckLine(f, 0, 1, y_p, 2, 1, y_m);
                CheckLine(f, 0, 1, y_m, 2, 1, y_p);
                CheckLine(f, 1, 0, y_p, 1, 2, y_m);
                CheckLine(f, 1, 0, y_m, 1, 2, y_p);
                CheckLine(f, 0, 0, y, 2, 2, y);
                CheckLine(f, 0, 2, y, 2, 0, y);

                CheckLine(f, 0, 0, y_m, 2, 2, y_p);
                CheckLine(f, 0, 0, y_p, 2, 2, y_m);
                CheckLine(f, 0, 2, y_m, 2, 0, y_p);
                CheckLine(f, 0, 2, y_p, 2, 0, y_m);

                if (figuresToBlow.Count > 0) {
                    figuresToBlow.Add(placedFigures[x, z, y]);
                }
            },
            (t, x, x_, z, z_, y) => {
                int x_m_z = x - z_;
                int x_p_z = x + z_;
                int z_m_x = z - x_;
                int z_p_x = z + x_;

                int abs_x = Mathf.Abs(x - 2*x_);
                int abs_z = Mathf.Abs(z - 2*z_);

                int y_m = y - 1;
                int y_p = y + 1;
                int y_m_2 = y - 2;

                var f = placingState[(int)t];

                CheckLine(f, x_m_z, z_m_x, y, x_p_z, z_p_x, y);
                CheckLine(f, 1, 1, y, abs_x, abs_z, y);
                CheckLine(f, x, z, y_m, x, z, y_m_2);

                CheckLine(f, x_m_z, z_m_x, y_m, x_p_z, z_p_x, y_p);
                CheckLine(f, x_m_z, z_m_x, y_p, x_p_z, z_p_x, y_m);
                CheckLine(f, 1, 1, y_p, abs_x, abs_z, y+2);
                CheckLine(f, 1, 1, y_m, abs_x, abs_z, y_m_2);

                if (figuresToBlow.Count > 0) {
                    figuresToBlow.Add(placedFigures[x, z, y]);
                }
            },
            (t, x, x_, z, z_, y) => {
                int abs_x = Mathf.Abs(x - 2);
                int abs_z = Mathf.Abs(z - 2);

                int y_m = y - 1;
                int y_p = y + 1;
                int y_m_2 = y - 2;
                int y_p_2 = y + 2;

                var f = placingState[(int)t];

                CheckLine(f, abs_x, z, y, 1, z, y);
                CheckLine(f, x, abs_z, y, x, 1, y);
                CheckLine(f, x, z, y_m, x, z, y_m_2);

                CheckLine(f, abs_x, z, y_p_2, 1, z, y_p);
                CheckLine(f, abs_x, z, y_m_2, 1, z, y_m);
                CheckLine(f, x, abs_z, y_p_2, x, 1, y_p);
                CheckLine(f, x, abs_z, y_m_2, x, 1, y_m);
                CheckLine(f, abs_x, abs_z, y, 1, 1, y);

                CheckLine(f, abs_x, abs_z, y_p_2, 1, 1, y_p);
                CheckLine(f, abs_x, abs_z, y_m_2, 1, 1, y_m);

                if (figuresToBlow.Count > 0) {
                    figuresToBlow.Add(placedFigures[x, z, y]);
                }
            }
        };
    }

    private void CheckLine(bool[,,] figurePlacingState, int x_0, int z_0, int y_0, int x_1, int z_1, int y_1) {
        if (y_0 >= 0 &&
            y_1 >= 0 &&
            figurePlacingState[x_0, z_0, y_0] &&
            figurePlacingState[x_1, z_1, y_1]
        ) {
            figuresToBlow.Add(placedFigures[x_0, z_0, y_0]);
            figuresToBlow.Add(placedFigures[x_1, z_1, y_1]);
        }
    }

    public void PlaceFigure(Vector3Int coordinates) {
        Figure figure = Instantiate(figurePrefabs[(int)GameManager.CurrentPlayer], coordinates, Quaternion.identity);
        figure.coordinates.coordinates = coordinates;
        figureSidesBuilder.BuildSides(figure.transform, figure.coordinates);

        int currentCellHeight = cellsHeight[coordinates.x, coordinates.z]++;
        LogCellsHeight();
        if (coordinates.y > currentCellHeight) {
            figure.coordinates.coordinates.y = currentCellHeight;
            UpdateFigureMatrices(figure);

            selectionFigure.Active = false;
            figure.FallTo(currentCellHeight, () => BlowLinesAroundFigure(figure.Type, figure.coordinates.coordinates));
        } else {
            UpdateFigureMatrices(figure);
            BlowLinesAroundFigure(figure.Type, figure.coordinates.coordinates);
        }
    }

    private void BlowLinesAroundFigure(Figure.FigureType type, Vector3Int coordinates) {
        int x = coordinates.x;
        int z = coordinates.z;
        int y = coordinates.y;

        int x_ = Mathf.Abs(x - 1);
        int z_ = Mathf.Abs(z - 1);

        singleFigureLineFinders[x_ + z_](type, x, x_, z, z_, y);
        MarkFiguresToFall();
        LogFiguresToFall();

        foreach (var figure in figuresToBlow) {
            cellsHeight[figure.coordinates.coordinates.x, figure.coordinates.coordinates.z]--;
            LogCellsHeight();

            RemoveFigureFromMatrices(figure.coordinates.coordinates.x, figure.coordinates.coordinates.z, figure.coordinates.coordinates.y);
            Destroy(figure.gameObject);
        }
        figuresToBlow.Clear();

        foreach (var figure in figuresToFall.Keys) {
            var coords = figure.coordinates.coordinates;
            RemoveFigureFromMatrices(coords.x, coords.z, coords.y);
        }
        foreach (var (figure, newHeight) in figuresToFall) {
            figure.FallTo(newHeight, BlowLinesAroundFigures);
            figure.coordinates.coordinates.y = newHeight;
            UpdateFigureMatrices(figure);
        }
        if (figuresToFall.Count > 0) {
            selectionFigure.Active = false;
        } else {
            selectionFigure.Active = true;
            StartNextTurn();
        }
        figuresToFall.Clear();
    }

    private void MarkFiguresToFall() {
        foreach (var f in figuresToBlow) {
            f.gameObject.SetActive(false);
        }

        foreach (var f in figuresToBlow) {
            for (int y = f.coordinates.coordinates.y + 1; y < cellsHeight[f.coordinates.coordinates.x, f.coordinates.coordinates.z]; ++y) {
                var figure = placedFigures[f.coordinates.coordinates.x, f.coordinates.coordinates.z, y];
                if (figure.gameObject.activeSelf) {
                    figuresToFall.ContainsKey(figure);
                    if (figuresToFall.ContainsKey(figure)) {
                        figuresToFall[figure]--;
                    } else {
                        figuresToFall[figure] = figure.coordinates.coordinates.y - 1;
                    }
                }
            }
        }

        figuresToFallCount = figuresToFall.Count;
    }

    private void BlowLinesAroundFigures() {
        figuresToFallCount--;
        if (figuresToFallCount > 0) {
            return;
        }

        selectionFigure.Active = true;
        StartNextTurn();

        Debug.Log("BlowLinesAroundFigures");
    }

    private void UpdateFigureMatrices(Figure figure) {
        int x = figure.coordinates.coordinates.x;
        int z = figure.coordinates.coordinates.z;
        int y = figure.coordinates.coordinates.y;

        placedFigures[x, z, y] = figure;
        placingState[(int)figure.Type][x, z, y] = true;

        LogPlacedFigures();
        LogPlacingState();
    }

    private void RemoveFigureFromMatrices(int x, int z, int y) {
        var type = placedFigures[x, z, y].Type;
        placedFigures[x, z, y] = null;
        placingState[(int)type][x, z, y] = false;

        LogPlacedFigures();
        LogPlacingState();
    }

    private void StartNextTurn() {
        GameManager.StartNextTurn();
        selectionFigure.SwitchForm();
    }

    public bool CheckFigureOn(Vector3Int coordinates) {
        return placedFigures[coordinates.x, coordinates.z, coordinates.y] != null;
    }
}
