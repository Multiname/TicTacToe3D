using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PointsEffector : MonoBehaviour {
    private class AnglesForDirections {
        private readonly Vector3[,,] anglesForDirections = new Vector3[3, 3, 3];

        public AnglesForDirections() {
            // 1D
            SetAngle(
                1, 0, 0,
                0, 0, 0
            );
            SetAngle(
                0, 0, 1,
                0, -90, 0
            );
            SetAngle(
                0, 1, 0,
                0, 0, 90
            );

            // 2D
            SetAngle(
                1, 0, 1,
                0, 0, 0
            );
            SetAngle(
                -1, 0, 1,
                0, -90, 0
            );
            SetAngle(
                1, 1, 0,
                -90, 0, 0
            );
            SetAngle(
                -1, 1, 0,
                90, 0, 0
            );
            SetAngle(
                0, 1, 1,
                0, 0, 90
            );
            SetAngle(
                0, 1, -1,
                0, 0, -90
            );

            // 3D
            SetAngle(
                1, 1, 1,
                0, 0, 0
            );
            SetAngle(
                -1, 1, 1,
                0, -90, 0
            );
            SetAngle(
                1, 1, -1,
                0, 90, 0
            );
            SetAngle(
                -1, 1, -1,
                0, 180, 0
            );
        }

        private void SetAngle(int x, int y, int z, float xAngle, float yAngle, float zAngle) {
            var angle = new Vector3(xAngle, yAngle, zAngle);
            anglesForDirections[x + 1, y + 1, z + 1] = angle;
            anglesForDirections[-x + 1, -y + 1, -z + 1] = angle;
        }
        public Vector3 GetAngle(Vector3Int direction) => anglesForDirections[direction.x + 1, direction.y + 1, direction.z + 1];
    }
    private static readonly AnglesForDirections anglesForDirections = new();

    private interface IEffect {
        public void SetVisibility(bool visibility);
    }

    private class Line : IEffect {
        private readonly GameObject[] points = new GameObject[3];
        private readonly LineExtraObjects lineExtraObjects;
        private readonly GameObject[,,] pointsCollection;

        public Line(LineExtraObjects lineExtraObjects, GameObject pointPrefab, GameObject[,,] pointsCollection) {
            this.lineExtraObjects = lineExtraObjects;
            this.pointsCollection = pointsCollection;
            for (int i = 0; i < 3; ++i) {
                Vector3Int pointPosition = lineExtraObjects.correspondingFigures[i].coordinates.coordinates;
                if (this.pointsCollection[pointPosition.x, pointPosition.z, pointPosition.y] != null) {
                    points[i] = this.pointsCollection[pointPosition.x, pointPosition.z, pointPosition.y];
                } else {
                    GameObject point = Instantiate(
                        pointPrefab,
                        pointPosition,
                        Quaternion.identity
                    );
                    points[i] = point;
                    this.pointsCollection[pointPosition.x, pointPosition.z, pointPosition.y] = point;
                    point.SetActive(false);
                }
            }
        }

        public void SetVisibility(bool visibility) {
            foreach (var point in points) {
                point.SetActive(visibility);
            }
            lineExtraObjects.SetVisibility(visibility);
        }

        public class LineExtraObjects {
            public readonly Figure[] correspondingFigures = new Figure[3];
            private readonly Connector[] connectors = new Connector[2];
            public int Dimension { get; private set; }
            public Figure.FigureType FigureType { get; private set; }

            public LineExtraObjects(Figure[] line) {
                line.CopyTo(correspondingFigures, 0);

                var anchorPostion = line[1].coordinates.coordinates;
                var direction = line[0].coordinates.coordinates - anchorPostion;
                Dimension = (int)(MathF.Abs(direction.x) + MathF.Abs(direction.y) + MathF.Abs(direction.z));
                FigureType = line[1].Type;

                Connector connector;
                var connectorPosition = anchorPostion + 0.5f * (Vector3)direction;
                if (placedConnectors.ContainsKey(connectorPosition)) {
                    connector = placedConnectors[connectorPosition];
                    connectors[0] = placedConnectors[connectorPosition];
                } else {
                    connector = Instantiate(connectorsPrefabs[Dimension - 1][(int)FigureType]);
                    connector.transform.position = connectorPosition;
                    connector.RotateBody(anglesForDirections.GetAngle(direction));
                    connectors[0] = connector;
                    placedConnectors[connectorPosition] = connector;
                    connector.gameObject.SetActive(false);
                }

                direction = line[2].coordinates.coordinates - anchorPostion;
                connectorPosition = anchorPostion + 0.5f * (Vector3)direction;
                if (placedConnectors.ContainsKey(connectorPosition)) {
                    connectors[1] = placedConnectors[connectorPosition];
                } else {
                    connector = Instantiate(connector);
                    connector.transform.position = connectorPosition;
                    connectors[1] = connector;
                    placedConnectors[connectorPosition] = connector;
                }
            }

            public void SetVisibility(bool visibility) {
                foreach (var connector in connectors) {
                    connector.gameObject.SetActive(visibility);
                }
                foreach (var figure in correspondingFigures) {
                    figure.gameObject.SetActive(!visibility);
                }
            }
        }
    }

    private class EffectPoint : IEffect {
        private readonly Figure correspondingFigure;
        private readonly GameObject point;

        public EffectPoint(Figure figure, GameObject[] pointsPrefabs) {
            correspondingFigure = figure;

            point = Instantiate(pointsPrefabs[(int)figure.Type], figure.coordinates.coordinates, Quaternion.identity);
            point.SetActive(false);
        }

        public void SetVisibility(bool visibility) {
            point.SetActive(visibility);
            correspondingFigure.gameObject.SetActive(!visibility);
        }

        public void Destroy() {
            UnityEngine.Object.Destroy(point);
        }
    }

    [SerializeField] UiCanvas uiCanvas;
    [SerializeField] PanelsManager panelsManager;
    [SerializeField] GameManager gameManager;
    [SerializeField] GameSettings gameSettings;

    [SerializeField] float switchPeriod = 1.0f;

    [SerializeField] GameObject[] basePoints = new GameObject[Figure.NUMBER_OF_FIGURE_TYPES];
    [SerializeField] GameObject[] points2D = new GameObject[Figure.NUMBER_OF_FIGURE_TYPES];
    [SerializeField] GameObject[] points3D = new GameObject[Figure.NUMBER_OF_FIGURE_TYPES];
    [SerializeField] GameObject[] fallPoints = new GameObject[Figure.NUMBER_OF_FIGURE_TYPES];
    [SerializeField] Connector[] connectors1d = new Connector[Figure.NUMBER_OF_FIGURE_TYPES];
    [SerializeField] Connector[] connectors2d = new Connector[Figure.NUMBER_OF_FIGURE_TYPES];
    [SerializeField] Connector[] connectors3d = new Connector[Figure.NUMBER_OF_FIGURE_TYPES];
    private static Connector[][] connectorsPrefabs;
    [SerializeField] GameObject[] heightPoint_1 = new GameObject[Figure.NUMBER_OF_FIGURE_TYPES];
    [SerializeField] GameObject[] heightPoint_2 = new GameObject[Figure.NUMBER_OF_FIGURE_TYPES];
    [SerializeField] GameObject[] heightPoint_3 = new GameObject[Figure.NUMBER_OF_FIGURE_TYPES];
    [SerializeField] GameObject[] heightPoint_4 = new GameObject[Figure.NUMBER_OF_FIGURE_TYPES];
    private GameObject[][] heightPoints;

    private static Dictionary<Vector3, Connector> placedConnectors = new();
    private static GameObject[,,] placedBasePoints = new GameObject[Board.MAX_X, Board.MAX_Z, Board.MAX_Y];
    private static GameObject[,,] placed2DPoints = new GameObject[Board.MAX_X, Board.MAX_Z, Board.MAX_Y];
    private static GameObject[,,] placed3DPoints = new GameObject[Board.MAX_X, Board.MAX_Z, Board.MAX_Y];
    private List<IEffect> placedFallPoints = new();
    private List<IEffect> placedHeightPoints = new();
    private List<IEffect> linesOfBasePoints = new();
    private List<IEffect> linesOf2DPoints = new();
    private List<IEffect> linesOf3DPoints = new();

    private CancellationTokenSource effectCts;
    private bool effectIsPlaying = false;

    private void Start() {
        connectorsPrefabs = new Connector[3][] {
            connectors1d,
            connectors2d,
            connectors3d
        };

        heightPoints = new GameObject[4][] {
            heightPoint_1,
            heightPoint_2,
            heightPoint_3,
            heightPoint_4
        };
    }

    private void Update() {
        if (Mouse.current.leftButton.wasReleasedThisFrame && effectIsPlaying && !panelsManager.MenuPanelIsVisible) {
            effectCts.Cancel();
        }
    }

    private void OnDestroy() {
        effectCts?.Cancel();
        effectCts?.Dispose();
    }

    private bool CheckNecessityOfBonusPointsVisualization(Figure.FigureType figureType) {
        return gameManager.CurrentPlayer == figureType ||
            gameSettings.enabledInterceptionRule == GameSettings.InterceptionRule.NO_RESTRICTION ||
            gameSettings.enabledInterceptionRule == GameSettings.InterceptionRule.STEAL_ALL_POINTS;
    }

    public void AddLine(Figure[] line) {
        Line.LineExtraObjects lineExtraObjects = new(line);
        
        linesOfBasePoints.Add(new Line(lineExtraObjects, basePoints[(int)lineExtraObjects.FigureType], placedBasePoints));

        if (CheckNecessityOfBonusPointsVisualization(lineExtraObjects.FigureType)) {
            if (lineExtraObjects.Dimension == 2 && gameSettings.enabledModifiers[(int)GameSettings.Modifier.LINE_2D]) {
                linesOf2DPoints.Add(new Line(lineExtraObjects, points2D[(int)lineExtraObjects.FigureType], placed2DPoints));
            }
            if (lineExtraObjects.Dimension == 3 && gameSettings.enabledModifiers[(int)GameSettings.Modifier.LINE_3D]) {
                linesOf3DPoints.Add(new Line(lineExtraObjects, points3D[(int)lineExtraObjects.FigureType], placed3DPoints));
            }
        }
    }

    public void AddFallPoint(Figure figure) {
        if (CheckNecessityOfBonusPointsVisualization(figure.Type)) {
            var point = new EffectPoint(figure, fallPoints);
            placedFallPoints.Add(point);
        }
    }

    public void TryToAddHeightPoint(Figure figure) {
        if (CheckNecessityOfBonusPointsVisualization(figure.Type)) {
            int height = figure.coordinates.coordinates.y;
            if (height > 0) {
                var point = new EffectPoint(figure, heightPoints[height - 1]);
                placedHeightPoints.Add(point);
            }
        }
    }

    public async UniTask StartEffects() {
        await UniTask.Yield();
        effectIsPlaying = true;

        await StartNextEffect(linesOfBasePoints, uiCanvas.PrepareBasePoints);
        await TryToStartNextEffect(linesOf2DPoints, uiCanvas.Prepare2DPoints);
        await TryToStartNextEffect(linesOf3DPoints, uiCanvas.Prepare3DPoints);
        await TryToStartNextEffect(placedFallPoints, uiCanvas.PrepareFallPoints);
        await TryToStartNextEffect(placedHeightPoints, uiCanvas.PrepareHeightPoints);

        DisposePlaced();
        uiCanvas.HidePointsContainers();

        effectIsPlaying = false;
    }

    private async UniTask StartNextEffect(ICollection<IEffect> effectsCollection, Action prepareUi) {
        effectCts = new();
        prepareUi();
        await PlayEffect(effectsCollection);
        effectCts.Dispose();
        effectCts = null;
    }

    private async UniTask TryToStartNextEffect(ICollection<IEffect> effectsCollection, Action prepareUi) {
        if (effectsCollection.Count > 0) {
            await StartNextEffect(effectsCollection, prepareUi);
        }
    }

    private void SetEffectsVisibility(ICollection<IEffect> effectsCollection, bool visibility) {
        foreach (var line in effectsCollection) {
            line.SetVisibility(visibility);
        }
    }

    private async UniTask PlayEffect(ICollection<IEffect> effectsCollection) {
        float finalSwitchPeriod = switchPeriod / gameSettings.effectsSpeed;

        bool effectVisibility = true;
        while (!effectCts.Token.IsCancellationRequested) {
            SetEffectsVisibility(effectsCollection, effectVisibility);
            uiCanvas.SetNewPointsVisibility(effectVisibility);
            effectVisibility = !effectVisibility;
            await UniTask.WaitForSeconds(finalSwitchPeriod, cancellationToken: effectCts.Token).SuppressCancellationThrow();
        }
        SetEffectsVisibility(effectsCollection, false);
        uiCanvas.AcceptNewPoints();
    }

    private void DisposePlaced() {
        foreach (var connector in placedConnectors.Values) {
            Destroy(connector.gameObject);
        }
        placedConnectors = new();

        for (int x = 0; x < Board.MAX_X; ++x) {
            for (int z = 0; z < Board.MAX_X; ++z) {
                for (int y = 0; y < Board.MAX_X; ++y) {
                    GameObject point = placedBasePoints[x, z, y];
                    if (point != null) {
                        Destroy(point);
                    }

                    point = placed2DPoints[x, z, y];
                    if (point != null) {
                        Destroy(point);
                    }

                    point = placed3DPoints[x, z, y];
                    if (point != null) {
                        Destroy(point);
                    }
                }
            }
        }
        placedBasePoints = new GameObject[Board.MAX_X, Board.MAX_Z, Board.MAX_Y];
        placed2DPoints = new GameObject[Board.MAX_X, Board.MAX_Z, Board.MAX_Y];
        placed3DPoints = new GameObject[Board.MAX_X, Board.MAX_Z, Board.MAX_Y];

        placedFallPoints.ForEach(point => ((EffectPoint)point).Destroy());
        placedFallPoints = new();

        placedHeightPoints.ForEach(point => ((EffectPoint)point).Destroy());
        placedHeightPoints = new();

        linesOfBasePoints = new();
        linesOf2DPoints = new();
        linesOf3DPoints = new();
    }
}
