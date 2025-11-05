using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Board : MonoBehaviour {
    public const int MAX_X = 3;
    public const int MAX_Z = 3;
    public const int MAX_Y = 5;
    private const int NUMBER_OF_LINE_TYPES = 13;
    private const int LINE_1D_BONUS_POINTS = 0;
    private const int LINE_2D_BONUS_POINTS = 1;
    private const int LINE_3D_BONUS_POINTS = 2;

    private enum LineDimension {
        LINE_1D,
        LINE_2D,
        LINE_3D
    }

    private enum LineType {
        LINE_1D_X,
        LINE_1D_Z,
        LINE_1D_Y,
        LINE_2D_X_ASC,
        LINE_2D_X_DESC,
        LINE_2D_Z_ASC,
        LINE_2D_Z_DESC,
        LINE_2D_Y_ASC,
        LINE_2D_Y_DESC,
        LINE_3D_X0_Z0,
        LINE_3D_X0_Z1,
        LINE_3D_X1_Z0,
        LINE_3D_X1_Z1
    }

    private readonly int[] lineDimensionsBonusPoints = new int[3] {
        LINE_1D_BONUS_POINTS,
        LINE_2D_BONUS_POINTS,
        LINE_3D_BONUS_POINTS
    };

    private readonly int[][] lineDimensionGainedBonusPoints = new int[3][];

    private readonly LineDimension[] lineTypeDimensions = new LineDimension[NUMBER_OF_LINE_TYPES] {
        LineDimension.LINE_1D,
        LineDimension.LINE_1D,
        LineDimension.LINE_1D,

        LineDimension.LINE_2D,
        LineDimension.LINE_2D,
        LineDimension.LINE_2D,
        LineDimension.LINE_2D,
        LineDimension.LINE_2D,
        LineDimension.LINE_2D,

        LineDimension.LINE_3D,
        LineDimension.LINE_3D,
        LineDimension.LINE_3D,
        LineDimension.LINE_3D
    };

    private readonly LineType[] line2dYTranslation = new LineType[2] {
        LineType.LINE_2D_Y_ASC,
        LineType.LINE_2D_Y_DESC
    };
    private readonly LineType[,] line3dTranslation = new LineType[2, 2] {
        {
            LineType.LINE_3D_X0_Z0,
            LineType.LINE_3D_X0_Z1
        },
        {
            LineType.LINE_3D_X1_Z0,
            LineType.LINE_3D_X1_Z1
        }
    };

    private readonly Func<int, int, int, Vector3Int>[,] linesRelativeCoordinates = new Func<int, int, int, Vector3Int>[NUMBER_OF_LINE_TYPES, 3] {
        // LINE_1D_X
        {
            (x, z, y) => new(0, y, z),
            (x, z, y) => new(2, y, z),
            (x, z, y) => new(1, y, z)
        },
        // LINE_1D_Z
        {
            (x, z, y) => new(x, y, 0),
            (x, z, y) => new(x, y, 2),
            (x, z, y) => new(x, y, 1)
        },
        // LINE_1D_Y
        {
            (x, z, y) => new(x, y, z),
            (x, z, y) => new(x, y - 2, z),
            (x, z, y) => new(x, y - 1, z)
        },

        // LINE_2D_X_ASC
        {
            (x, z, y) => new(0, y - 1, z),
            (x, z, y) => new(2, y + 1, z),
            (x, z, y) => new(1, y, z)
        },
        // LINE_2D_X_DESC
        {
            (x, z, y) => new(0, y + 1, z),
            (x, z, y) => new(2, y - 1, z),
            (x, z, y) => new(1, y, z)
        },
        // LINE_2D_Z_ASC
        {
            (x, z, y) => new(x, y - 1, 0),
            (x, z, y) => new(x, y + 1, 2),
            (x, z, y) => new(x, y, 1)
        },
        // LINE_2D_Z_DESC
        {
            (x, z, y) => new(x, y + 1, 0),
            (x, z, y) => new(x, y - 1, 2),
            (x, z, y) => new(x, y, 1)
        },
        // LINE_2D_Y_ASC
        {
            (x, z, y) => new(0, y, 0),
            (x, z, y) => new(2, y, 2),
            (x, z, y) => new(1, y, 1)
        },
        // LINE_2D_Y_DESC
        {
            (x, z, y) => new(0, y, 2),
            (x, z, y) => new(2, y, 0),
            (x, z, y) => new(1, y, 1)
        },
        
        // LINE_3D_X0_Z0
        {
            (x, z, y) => new(0, y - 1, 0),
            (x, z, y) => new(2, y + 1, 2),
            (x, z, y) => new(1, y, 1)
        },
        // LINE_3D_X0_Z1
        {
            (x, z, y) => new(0, y - 1, 2),
            (x, z, y) => new(2, y + 1, 0),
            (x, z, y) => new(1, y, 1)
        },
        // LINE_3D_X1_Z0
        {
            (x, z, y) => new(2, y - 1, 0),
            (x, z, y) => new(0, y + 1, 2),
            (x, z, y) => new(1, y, 1)
        },
        // LINE_3D_X1_Z1
        {
            (x, z, y) => new(2, y - 1, 2),
            (x, z, y) => new(0, y + 1, 0),
            (x, z, y) => new(1, y, 1)
        }
    };

    private readonly Action<int>[] enemyBasePointsGainHandlers = new Action<int>[5];
    private readonly Action<int, int[]>[] enemyBonusPointsGainHandlers = new Action<int, int[]>[5];

    [SerializeField] FigureSidesBuilder figureSidesBuilder;
    [SerializeField] SelectionFigure selectionFigure;
    [SerializeField] CameraMovement cameraMovement;
    [SerializeField] GameManager gameManager;
    [SerializeField] GameSettings gameSettings;
    [SerializeField] PointsEffector pointsEffector;
    [SerializeField] BonusTurnEffect bonusTurnEffect;

    [SerializeField] Figure[] figurePrefabs = new Figure[Figure.NUMBER_OF_FIGURE_TYPES];

    private readonly Figure[,,] placedFigures = new Figure[MAX_X, MAX_Z, MAX_Y+2];
    private readonly bool[][,,] placingState = new bool[][,,] {
        new bool[MAX_X, MAX_Z, MAX_Y+2],
        new bool[MAX_X, MAX_Z, MAX_Y+2]
    };
    private readonly int[,] cellsHeight = new int[MAX_X, MAX_Z];

    private Action<Figure.FigureType, int, int, int, int, int>[] singleFigureLineFinders;
    private Action<Figure.FigureType, int, int, int>[] multipleFigureLineFinders;
    private readonly HashSet<Vector3Int>[][] detectedLines = new HashSet<Vector3Int>[Figure.NUMBER_OF_FIGURE_TYPES][];

    private readonly List<Figure> figuresToBlowList = new();
    private readonly HashSet<Figure> figuresToBlowSet = new();
    private readonly Dictionary<Figure, int> figuresToFall = new();
    private int figuresToFallCount = 0;

    private readonly int[] gainedPoints = new int[Figure.NUMBER_OF_FIGURE_TYPES];
    public readonly int[] gainedBasePoints = new int[Figure.NUMBER_OF_FIGURE_TYPES];
    public readonly int[] gained2DPoints = new int[Figure.NUMBER_OF_FIGURE_TYPES];
    public readonly int[] gained3DPoints = new int[Figure.NUMBER_OF_FIGURE_TYPES];
    public readonly int[] gainedFallPoints = new int[Figure.NUMBER_OF_FIGURE_TYPES];
    public readonly int[] gainedHeightPoints = new int[Figure.NUMBER_OF_FIGURE_TYPES];
    public readonly int[] gainedComboPoints = new int[Figure.NUMBER_OF_FIGURE_TYPES];
    private bool playerBlewLine = false;
    private int comboCount = 1;

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
        ResetDetectedLines();

        Action<int, int, int, int, int, int, bool, LineDimension> buildLineCheckerForSingleFigure(Figure.FigureType figureType, Figure placedFigure) {
            return (x_0, z_0, y_0, x_1, z_1, y_1, anchorIsFirst, lineDimension) => {
                if (CheckLine(
                    placingState[(int)figureType],
                    x_0, z_0, y_0,
                    x_1, z_1, y_1
                )) {
                    if (lineDimension == LineDimension.LINE_2D && gameSettings.enabledModifiers[(int)GameSettings.Modifier.LINE_2D] ||
                        lineDimension == LineDimension.LINE_3D && gameSettings.enabledModifiers[(int)GameSettings.Modifier.LINE_3D]) {
                        gainedPoints[(int)figureType] += lineDimensionsBonusPoints[(int)lineDimension];
                        lineDimensionGainedBonusPoints[(int)lineDimension][(int)figureType] += lineDimensionsBonusPoints[(int)lineDimension];
                    }

                    if (anchorIsFirst) {
                        pointsEffector.AddLine(new Figure[] {
                            placedFigures[x_1, z_1, y_1],
                            placedFigures[x_0, z_0, y_0],
                            placedFigure
                        });
                    } else {
                        pointsEffector.AddLine(new Figure[] {
                            placedFigures[x_0, z_0, y_0],
                            placedFigure,
                            placedFigures[x_1, z_1, y_1]
                        });
                    }
                }
            };
        }

        singleFigureLineFinders = new Action<Figure.FigureType, int, int, int, int, int>[3] {
            (t, x, x_, z, z_, y) => {
                int y_m = y - 1;
                int y_p = y + 1;

                var checkLine = buildLineCheckerForSingleFigure(t, placedFigures[x, z, y]);

                checkLine(0, 1, y, 2, 1, y, false, LineDimension.LINE_1D);
                checkLine(1, 0, y, 1, 2, y, false, LineDimension.LINE_1D);
                checkLine(1, 1, y_m, 1, 1, y-2, true, LineDimension.LINE_1D);

                checkLine(0, 1, y_p, 2, 1, y_m, false, LineDimension.LINE_2D);
                checkLine(0, 1, y_m, 2, 1, y_p, false, LineDimension.LINE_2D);
                checkLine(1, 0, y_p, 1, 2, y_m, false, LineDimension.LINE_2D);
                checkLine(1, 0, y_m, 1, 2, y_p, false, LineDimension.LINE_2D);
                checkLine(0, 0, y, 2, 2, y, false, LineDimension.LINE_2D);
                checkLine(0, 2, y, 2, 0, y, false, LineDimension.LINE_2D);

                checkLine(0, 0, y_m, 2, 2, y_p, false, LineDimension.LINE_3D);
                checkLine(0, 0, y_p, 2, 2, y_m, false, LineDimension.LINE_3D);
                checkLine(0, 2, y_m, 2, 0, y_p, false, LineDimension.LINE_3D);
                checkLine(0, 2, y_p, 2, 0, y_m, false, LineDimension.LINE_3D);

                if (figuresToBlowList.Count > 0) {
                    figuresToBlowList.Add(placedFigures[x, z, y]);
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

                var checkLine = buildLineCheckerForSingleFigure(t, placedFigures[x, z, y]);

                checkLine(x_m_z, z_m_x, y, x_p_z, z_p_x, y, false, LineDimension.LINE_1D);
                checkLine(1, 1, y, abs_x, abs_z, y, true, LineDimension.LINE_1D);
                checkLine(x, z, y_m, x, z, y_m_2, true, LineDimension.LINE_1D);

                checkLine(x_m_z, z_m_x, y_m, x_p_z, z_p_x, y_p, false, LineDimension.LINE_2D);
                checkLine(x_m_z, z_m_x, y_p, x_p_z, z_p_x, y_m, false, LineDimension.LINE_2D);
                checkLine(1, 1, y_p, abs_x, abs_z, y+2, true, LineDimension.LINE_2D);
                checkLine(1, 1, y_m, abs_x, abs_z, y_m_2, true, LineDimension.LINE_2D);

                if (figuresToBlowList.Count > 0) {
                    figuresToBlowList.Add(placedFigures[x, z, y]);
                }
            },
            (t, x, x_, z, z_, y) => {
                int abs_x = Mathf.Abs(x - 2);
                int abs_z = Mathf.Abs(z - 2);

                int y_m = y - 1;
                int y_p = y + 1;
                int y_m_2 = y - 2;
                int y_p_2 = y + 2;

                var checkLine = buildLineCheckerForSingleFigure(t, placedFigures[x, z, y]);

                checkLine(1, z, y, abs_x, z, y, true, LineDimension.LINE_1D);
                checkLine(x, 1, y, x, abs_z, y, true, LineDimension.LINE_1D);
                checkLine(x, z, y_m, x, z, y_m_2, true, LineDimension.LINE_1D);

                checkLine(1, z, y_p, abs_x, z, y_p_2, true, LineDimension.LINE_2D);
                checkLine(1, z, y_m, abs_x, z, y_m_2, true, LineDimension.LINE_2D);
                checkLine(x, 1, y_p, x, abs_z, y_p_2, true, LineDimension.LINE_2D);
                checkLine(x, 1, y_m, x, abs_z, y_m_2, true, LineDimension.LINE_2D);
                checkLine(1, 1, y, abs_x, abs_z, y, true, LineDimension.LINE_2D);

                checkLine(1, 1, y_p, abs_x, abs_z, y_p_2, true, LineDimension.LINE_3D);
                checkLine(1, 1, y_m, abs_x, abs_z, y_m_2, true, LineDimension.LINE_3D);

                if (figuresToBlowList.Count > 0) {
                    figuresToBlowList.Add(placedFigures[x, z, y]);
                }
            }
        };

        multipleFigureLineFinders = new Action<Figure.FigureType, int, int, int>[3] {
            (t, x, z, y) => {
                detectedLines[(int)t][(int)LineType.LINE_1D_X].Add(new(0, y, 1));
                detectedLines[(int)t][(int)LineType.LINE_1D_Z].Add(new(1, y, 0));
                detectedLines[(int)t][(int)LineType.LINE_1D_Y].Add(new(1, y, 1));

                detectedLines[(int)t][(int)LineType.LINE_2D_X_ASC].Add(new(0, y, 1));
                detectedLines[(int)t][(int)LineType.LINE_2D_X_DESC].Add(new(0, y, 1));
                detectedLines[(int)t][(int)LineType.LINE_2D_Z_ASC].Add(new(1, y, 0));
                detectedLines[(int)t][(int)LineType.LINE_2D_Z_DESC].Add(new(1, y, 0));
                detectedLines[(int)t][(int)LineType.LINE_2D_Y_ASC].Add(new(0, y, 0));
                detectedLines[(int)t][(int)LineType.LINE_2D_Y_DESC].Add(new(0, y, 0));

                detectedLines[(int)t][(int)LineType.LINE_3D_X0_Z0].Add(new(0, y, 0));
                detectedLines[(int)t][(int)LineType.LINE_3D_X0_Z1].Add(new(0, y, 0));
                detectedLines[(int)t][(int)LineType.LINE_3D_X1_Z0].Add(new(0, y, 0));
                detectedLines[(int)t][(int)LineType.LINE_3D_X1_Z1].Add(new(0, y, 0));
            },
            (t, x, z, y) => {
                int x_m = x - 1;
                int z_m = z - 1;

                detectedLines[(int)t][(int)LineType.LINE_1D_X].Add(new(0, y, z));
                detectedLines[(int)t][(int)LineType.LINE_1D_Z].Add(new(x, y, 0));
                detectedLines[(int)t][(int)LineType.LINE_1D_Y].Add(new(x, y, z));

                detectedLines[(int)t][(int)LineType.LINE_2D_X_ASC].Add(new(0, y-x_m, z));
                detectedLines[(int)t][(int)LineType.LINE_2D_X_DESC].Add(new(0, y+x_m, z));
                detectedLines[(int)t][(int)LineType.LINE_2D_Z_ASC].Add(new(x, y-z_m, 0));
                detectedLines[(int)t][(int)LineType.LINE_2D_Z_DESC].Add(new(x, y+z_m, 0));
            },
            (t, x, z, y) => {
                int x_m = x - 1;
                int z_m = z - 1;

                detectedLines[(int)t][(int)LineType.LINE_1D_X].Add(new(0, y, z));
                detectedLines[(int)t][(int)LineType.LINE_1D_Z].Add(new(x, y, 0));
                detectedLines[(int)t][(int)LineType.LINE_1D_Y].Add(new(x, y, z));

                detectedLines[(int)t][(int)LineType.LINE_2D_X_ASC].Add(new(0, y-x_m, z));
                detectedLines[(int)t][(int)LineType.LINE_2D_X_DESC].Add(new(0, y+x_m, z));
                detectedLines[(int)t][(int)LineType.LINE_2D_Z_ASC].Add(new(x, y-z_m, 0));
                detectedLines[(int)t][(int)LineType.LINE_2D_Z_DESC].Add(new(x, y+z_m, 0));
                detectedLines[(int)t][(int)line2dYTranslation[(x + z) / 2 % 2]].Add(new(0, y, 0));

                detectedLines[(int)t][(int)line3dTranslation[x / 2, z / 2]].Add(new(0, y+1, 0));
                detectedLines[(int)t][(int)line3dTranslation[(x + 1) % 3, (z + 1) % 3]].Add(new(0, y-1, 0));
            }
        };

        enemyBasePointsGainHandlers[(int)GameSettings.InterceptionRule.NO_RESTRICTION] = (points) => {
            gainedPoints[(int)gameManager.EnemyPlayer] += points;
            gainedBasePoints[(int)gameManager.EnemyPlayer] += points;
        };
        enemyBasePointsGainHandlers[(int)GameSettings.InterceptionRule.NO_POINTS] = (points) => { };
        enemyBasePointsGainHandlers[(int)GameSettings.InterceptionRule.NO_BONUS] = (points) => {
            gainedPoints[(int)gameManager.EnemyPlayer] += points;
            gainedBasePoints[(int)gameManager.EnemyPlayer] += points;
        };
        enemyBasePointsGainHandlers[(int)GameSettings.InterceptionRule.STEAL_BASE_POINTS] = (points) => {
            gainedPoints[(int)gameManager.CurrentPlayer] += points;
            gainedBasePoints[(int)gameManager.CurrentPlayer] += points;
        };
        enemyBasePointsGainHandlers[(int)GameSettings.InterceptionRule.STEAL_ALL_POINTS] = (points) => {
            gainedPoints[(int)gameManager.CurrentPlayer] += points;
            gainedBasePoints[(int)gameManager.CurrentPlayer] += points;
        };

        enemyBonusPointsGainHandlers[(int)GameSettings.InterceptionRule.NO_RESTRICTION] = (points, gainedBonusPoints) => {
            gainedPoints[(int)gameManager.EnemyPlayer] += points;
            gainedBonusPoints[(int)gameManager.EnemyPlayer] += points;
        };
        enemyBonusPointsGainHandlers[(int)GameSettings.InterceptionRule.NO_POINTS] = (points, gainedBonusPoints) => { };
        enemyBonusPointsGainHandlers[(int)GameSettings.InterceptionRule.NO_BONUS] = (points, gainedBonusPoints) => { };
        enemyBonusPointsGainHandlers[(int)GameSettings.InterceptionRule.STEAL_BASE_POINTS] = (points, gainedBonusPoints) => { };
        enemyBonusPointsGainHandlers[(int)GameSettings.InterceptionRule.STEAL_ALL_POINTS] = (points, gainedBonusPoints) => {
            gainedPoints[(int)gameManager.CurrentPlayer] += points;
            gainedBonusPoints[(int)gameManager.CurrentPlayer] += points;
        };

        lineDimensionGainedBonusPoints[(int)LineDimension.LINE_1D] = null;
        lineDimensionGainedBonusPoints[(int)LineDimension.LINE_2D] = gained2DPoints;
        lineDimensionGainedBonusPoints[(int)LineDimension.LINE_3D] = gained3DPoints;
    }

    private void ResetDetectedLines() {
        for (int i = 0; i < Figure.NUMBER_OF_FIGURE_TYPES; ++i) {
            detectedLines[i] = new HashSet<Vector3Int>[NUMBER_OF_LINE_TYPES];
            for (int j = 0; j < NUMBER_OF_LINE_TYPES; ++j) {
                detectedLines[i][j] = new();
            }
        }
    }

    private void ResetGainedPoints() {
        for (int i = 0; i < Figure.NUMBER_OF_FIGURE_TYPES; ++i) {
            gainedPoints[i] = 0;
        }
    }

    private void ResetParticularPoints() {
        for (int i = 0; i < Figure.NUMBER_OF_FIGURE_TYPES; ++i) {
            gainedBasePoints[i] = 0;
            gained2DPoints[i] = 0;
            gained3DPoints[i] = 0;
            gainedFallPoints[i] = 0;
            gainedHeightPoints[i] = 0;
            gainedComboPoints[i] = 0;
        }
    }

    private bool CheckLine(bool[,,] figurePlacingState, int x_0, int z_0, int y_0, int x_1, int z_1, int y_1) {
        if (y_0 >= 0 &&
            y_1 >= 0 &&
            figurePlacingState[x_0, z_0, y_0] &&
            figurePlacingState[x_1, z_1, y_1]
        ) {
            figuresToBlowList.Add(placedFigures[x_0, z_0, y_0]);
            figuresToBlowList.Add(placedFigures[x_1, z_1, y_1]);
            return true;
        }
        return false;
    }

    private bool CheckLine(bool[,,] figurePlacingState, Vector3Int coord_0, Vector3Int coord_1, Vector3Int anchor) {
        if (coord_0.y >= 0 &&
            coord_1.y >= 0 &&
            anchor.y >= 0 &&
            figurePlacingState[coord_0.x, coord_0.z, coord_0.y] &&
            figurePlacingState[coord_1.x, coord_1.z, coord_1.y] &&
            figurePlacingState[anchor.x, anchor.z, anchor.y]
        ) {
            figuresToBlowSet.Add(placedFigures[coord_0.x, coord_0.z, coord_0.y]);
            figuresToBlowSet.Add(placedFigures[coord_1.x, coord_1.z, coord_1.y]);
            figuresToBlowSet.Add(placedFigures[anchor.x, anchor.z, anchor.y]);
            return true;
        }
        return false;
    }

    private void CheckDetectedLines() {
        for (Figure.FigureType figureType = 0; (int)figureType < Figure.NUMBER_OF_FIGURE_TYPES; ++figureType) {
            HashSet<Vector3Int>[] lines = detectedLines[(int)figureType];
            bool foundLine = false;

            for (LineType lineType = 0; (int)lineType < NUMBER_OF_LINE_TYPES; ++lineType) {
                foreach (Vector3Int lineAnchor in lines[(int)lineType]) {
                    int x = lineAnchor.x;
                    int z = lineAnchor.z;
                    int y = lineAnchor.y;

                    Vector3Int coord_0 = linesRelativeCoordinates[(int)lineType, 0](x, z, y);
                    Vector3Int coord_1 = linesRelativeCoordinates[(int)lineType, 1](x, z, y);
                    Vector3Int anchor = linesRelativeCoordinates[(int)lineType, 2](x, z, y);

                    if (CheckLine(placingState[(int)figureType], coord_0, coord_1, anchor)) {
                        foundLine = true;

                        LineDimension lineDimension = lineTypeDimensions[(int)lineType];
                        int lineDimensionBonusPoints = lineDimensionsBonusPoints[(int)lineDimension];
                        if (figureType == gameManager.CurrentPlayer) {
                            if (lineDimension == LineDimension.LINE_2D && gameSettings.enabledModifiers[(int)GameSettings.Modifier.LINE_2D] ||
                                lineDimension == LineDimension.LINE_3D && gameSettings.enabledModifiers[(int)GameSettings.Modifier.LINE_3D]) {
                                gainedPoints[(int)figureType] += lineDimensionBonusPoints;
                                lineDimensionGainedBonusPoints[(int)lineDimension][(int)figureType] += lineDimensionBonusPoints;
                            }

                            gainedPoints[(int)figureType] += comboCount;
                        } else {
                            if (lineDimension == LineDimension.LINE_2D && gameSettings.enabledModifiers[(int)GameSettings.Modifier.LINE_2D] ||
                                lineDimension == LineDimension.LINE_3D && gameSettings.enabledModifiers[(int)GameSettings.Modifier.LINE_3D]) {
                                enemyBonusPointsGainHandlers[(int)gameSettings.enabledInterceptionRule](lineDimensionBonusPoints, lineDimensionGainedBonusPoints[(int)lineDimension]);
                            }

                            enemyBonusPointsGainHandlers[(int)gameSettings.enabledInterceptionRule](comboCount, new int[2]);
                        }

                        pointsEffector.AddLine(new Figure[] {
                            placedFigures[coord_0.x, coord_0.z, coord_0.y],
                            placedFigures[anchor.x, anchor.z, anchor.y],
                            placedFigures[coord_1.x, coord_1.z, coord_1.y]
                        });

                        if (gameSettings.enabledModifiers[(int)GameSettings.Modifier.FALL]) {
                            pointsEffector.TryToAddHeightPoint(placedFigures[coord_0.x, coord_0.z, coord_0.y]);
                            pointsEffector.TryToAddHeightPoint(placedFigures[anchor.x, anchor.z, anchor.y]);
                            pointsEffector.TryToAddHeightPoint(placedFigures[coord_1.x, coord_1.z, coord_1.y]);
                        }
                    }
                }
            }

            if (foundLine && gameSettings.enabledModifiers[(int)GameSettings.Modifier.Combo]) {
                if (figureType == gameManager.CurrentPlayer) {
                    gainedPoints[(int)figureType] += comboCount;
                    gainedComboPoints[(int)figureType] += comboCount;
                } else {
                    enemyBonusPointsGainHandlers[(int)gameSettings.enabledInterceptionRule](comboCount, gainedComboPoints);
                }
            }
        }
    }

    public void PlaceFigure(Vector3Int coordinates) {
        Figure figure = Instantiate(figurePrefabs[(int)gameManager.CurrentPlayer], coordinates, Quaternion.identity);
        figure.coordinates.coordinates = coordinates;
        figureSidesBuilder.BuildSides(figure.transform, figure.coordinates);

        int currentCellHeight = cellsHeight[coordinates.x, coordinates.z]++;
        LogCellsHeight();
        if (currentCellHeight == 0) {
            CellsShadows.ShowShadow(figure.coordinates.coordinates.x, figure.coordinates.coordinates.z, figure.Type);
        }
        if (coordinates.y > currentCellHeight) {
            figure.coordinates.coordinates.y = currentCellHeight;
            UpdateFigureMatrices(figure);

            selectionFigure.FiguresFall = true;
            figure.FallTo(currentCellHeight, () => HandleFigureFall(figure, true));
        } else {
            UpdateFigureMatrices(figure);
            HandleFigureFall(figure, false);
        }
    }

    private async void HandleFigureFall(Figure figure, bool fell) {
        int x = figure.coordinates.coordinates.x;
        int z = figure.coordinates.coordinates.z;
        int y = figure.coordinates.coordinates.y;

        int x_ = Mathf.Abs(x - 1);
        int z_ = Mathf.Abs(z - 1);

        singleFigureLineFinders[x_ + z_](figure.Type, x, x_, z, z_, y);

        if (fell && figuresToBlowList.Contains(figure) && gameSettings.enabledModifiers[(int)GameSettings.Modifier.FALL]) {
            ++gainedPoints[(int)figure.Type];
            ++gainedFallPoints[(int)figure.Type];
            pointsEffector.AddFallPoint(figure);
        }

        MarkFiguresToFall(figuresToBlowList);
        LogFiguresToFall();

        await BlowFigures(figuresToBlowList);
        DropFigures();
    }

    private void MarkFiguresToFall(ICollection<Figure> figuresToBlowCollection) {
        figuresToFall.Clear();

        foreach (var f in figuresToBlowCollection) {
            for (int y = f.coordinates.coordinates.y + 1; y < cellsHeight[f.coordinates.coordinates.x, f.coordinates.coordinates.z]; ++y) {
                var figure = placedFigures[f.coordinates.coordinates.x, f.coordinates.coordinates.z, y];
                if (!figuresToBlowCollection.Contains(figure)) {
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

    private async UniTask BlowFigures(ICollection<Figure> figuresToBlowCollection) {
        foreach (var figure in figuresToBlowCollection) {
            if (figure.Type == gameManager.CurrentPlayer) {
                if (gameSettings.enabledModifiers[(int)GameSettings.Modifier.BASE_POINTS]) {
                    ++gainedPoints[(int)figure.Type];
                    ++gainedBasePoints[(int)figure.Type];
                }

                if (gameSettings.enabledModifiers[(int)GameSettings.Modifier.HEIGHT]) {
                    gainedPoints[(int)figure.Type] += figure.coordinates.coordinates.y;
                    gainedHeightPoints[(int)figure.Type] += figure.coordinates.coordinates.y;
                    pointsEffector.TryToAddHeightPoint(figure);
                }
            } else {
                if (gameSettings.enabledModifiers[(int)GameSettings.Modifier.BASE_POINTS]) {
                    enemyBasePointsGainHandlers[(int)gameSettings.enabledInterceptionRule](1);
                }

                if (gameSettings.enabledModifiers[(int)GameSettings.Modifier.HEIGHT]) {
                    enemyBonusPointsGainHandlers[(int)gameSettings.enabledInterceptionRule](figure.coordinates.coordinates.y, gainedHeightPoints);
                    pointsEffector.TryToAddHeightPoint(figure);
                }
            }
        }

        if (figuresToBlowCollection.Count > 0) {
            selectionFigure.EffectIsPlaying = true;
            await pointsEffector.StartEffects();
            selectionFigure.EffectIsPlaying = false;
        }

        ResetParticularPoints();

        foreach (var figure in figuresToBlowCollection) {
            int newHeight = --cellsHeight[figure.coordinates.coordinates.x, figure.coordinates.coordinates.z];
            LogCellsHeight();

            if (newHeight == 0) {
                CellsShadows.HideShadow(figure.coordinates.coordinates.x, figure.coordinates.coordinates.z);
            }

            if (figure.Type == gameManager.CurrentPlayer) {
                playerBlewLine = true;
            }

            RemoveFigureFromMatrices(figure.coordinates.coordinates.x, figure.coordinates.coordinates.z, figure.coordinates.coordinates.y);
            Destroy(figure.gameObject);
        }
        figuresToBlowCollection.Clear();
    }

    private void DropFigures() {
        foreach (var figure in figuresToFall.Keys) {
            var coords = figure.coordinates.coordinates;
            RemoveFigureFromMatrices(coords.x, coords.z, coords.y);
        }
        foreach (var (figure, newHeight) in figuresToFall) {
            if (newHeight == 0) {
                CellsShadows.ShowShadow(
                    figure.coordinates.coordinates.x,
                    figure.coordinates.coordinates.z,
                    figure.Type
                );
            }

            figure.FallTo(newHeight, HandleFiguresFall);
            figure.coordinates.coordinates.y = newHeight;
            UpdateFigureMatrices(figure);
        }
        if (figuresToFall.Count > 0) {
            selectionFigure.FiguresFall = true;
        } else {
            selectionFigure.FiguresFall = false;
            StartNextTurn();
        }
    }

    private async void HandleFiguresFall() {
        figuresToFallCount--;
        if (figuresToFallCount > 0) {
            return;
        }

        foreach (var (figure, _) in figuresToFall) {
            int x = figure.coordinates.coordinates.x;
            int z = figure.coordinates.coordinates.z;
            int y = figure.coordinates.coordinates.y;

            int x_ = Mathf.Abs(x - 1);
            int z_ = Mathf.Abs(z - 1);

            multipleFigureLineFinders[x_ + z_](figure.Type, x, z, y);
        }

        CheckDetectedLines();
        ++comboCount;

        CheckBlowingAmongFallen();
        ResetDetectedLines();
        MarkFiguresToFall(figuresToBlowSet);

        await BlowFigures(figuresToBlowSet);
        DropFigures();
    }

    private void CheckBlowingAmongFallen() {
        foreach (Figure figure in figuresToBlowSet) {
            if (figuresToFall.Keys.Contains(figure)) {
                if (gameSettings.enabledModifiers[(int)GameSettings.Modifier.FALL]) {
                    if (figure.Type == gameManager.CurrentPlayer) {
                        ++gainedPoints[(int)figure.Type];
                        ++gainedFallPoints[(int)figure.Type];
                    } else {
                        enemyBonusPointsGainHandlers[(int)gameSettings.enabledInterceptionRule](1, gainedFallPoints);
                    }

                    pointsEffector.AddFallPoint(figure);
                }
            }
        }
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

    private int GetCurrentMaxHeight() {
        int maxHeight = 0;
        foreach (var height in cellsHeight) {
            if (height > maxHeight) {
                maxHeight = height;
            }
        }
        return maxHeight;
    }

    private void StartNextTurn() {
        gameManager.AddPointsToPlayers(gainedPoints);
        ResetGainedPoints();
        comboCount = 1;

        selectionFigure.CameraIsInTransition = true;
        cameraMovement.UpdateFieldOfView(GetCurrentMaxHeight(), async () => {
            if (!playerBlewLine || !gameSettings.enabledModifiers[(int)GameSettings.Modifier.BONUS_TURN]) {
                gameManager.StartNextTurn();
                selectionFigure.SwitchForm();
            } else {
                await bonusTurnEffect.StartEffect(gameManager.CurrentPlayer);
            }
            playerBlewLine = false;
            selectionFigure.CameraIsInTransition = false;
        });
    }

    public bool CheckFigureOn(Vector3Int coordinates) {
        return placedFigures[coordinates.x, coordinates.z, coordinates.y] != null;
    }
}
