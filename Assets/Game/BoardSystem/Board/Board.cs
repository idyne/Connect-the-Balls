using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Board : FateMonoBehaviour
{
    private static Board instance = null;
    public static Board Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<Board>();
            return instance;
        }
    }
    [SerializeField] private SaveDataVariable saveData;
    [SerializeField] private LevelDataTable levelDataTable;
    [SerializeField] private LevelData currentLevelData;
    [SerializeField] private GameObject gridPrefab, ballGridPrefab, obstaclePrefab;
    [SerializeField] private GridHolder[,] gridMatrix;
    [SerializeField] private Transform gridContainer;
    [SerializeField] private Vector3 startPosition = new Vector3(0.5f, 0, 0.5f);
    [SerializeField] private UnityEvent onBuilt;
    [SerializeField] private UnityEvent onBuildAnimationFinished;
    [SerializeField] private BoardPlate boardPlate;

    public LevelData LevelData { get => levelData; }
    private LevelData levelData { get => levelDataTable.levelDatas[(saveData.Value.Level - 1) % levelDataTable.levelDatas.Count]; }
    public Vector3 StartPosition { get => startPosition; }

    private void Awake()
    {
        currentLevelData = levelData;
        Build();
    }

    public void Build(bool editor = false)
    {

        int ballCount = 0;
        ClearGrids();
        boardPlate.Adjust(new Vector3(levelData.width / 2f, 0, levelData.length / 2f), new Vector2(levelData.width, levelData.length), false);
        gridMatrix = new GridHolder[levelData.width, levelData.length];
        // Create grids and their holders
        for (int i = 0; i < levelData.width; i++)
        {
            for (int j = 0; j < levelData.length; j++)
            {
                GridHolder gridHolder = null;
                if (editor)
                {
#if UNITY_EDITOR
                    gridHolder = (PrefabUtility.InstantiatePrefab(ballGridPrefab) as GameObject).GetComponent<GridHolder>();
#endif
                }
                else
                {
                    gridHolder = Instantiate(ballGridPrefab).GetComponent<GridHolder>();
                }
                gridHolder.i = i;
                gridHolder.j = j;
                gridMatrix[i, j] = gridHolder;
                gridHolder.transform.SetParent(gridContainer);
                gridHolder.transform.position = startPosition + new Vector3(i, 0, j);
                Ball ball = gridHolder.GetComponentInChildren<Ball>();
                ball.SetGrid(gridHolder);
                ball.name = "Ball " + ballCount++;
            }
        }
        // Set adjacents
        for (int i = 0; i < levelData.width; i++)
        {
            for (int j = 0; j < levelData.length; j++)
            {
                GridHolder grid = gridMatrix[i, j];
                SetAdjacents(grid);
            }
        }
        for (int i = 0; i < levelData.obstacles.Length; i++)
        {
            LevelData.Obstacle obstacle = levelData.obstacles[i];
            PlaceObstacle(gridMatrix[obstacle.col, obstacle.row], obstacle.direction);
        }
        /*PlaceObstacle(gridMatrix[1, 1], Direction.Up);
        //PlaceObstacle(gridMatrix[1, 1], Direction.Down);
        PlaceObstacle(gridMatrix[1, 1], Direction.Right);
        PlaceObstacle(gridMatrix[1, 1], Direction.Left);*/
        for (int i = 0; i < levelData.headPairs.Length; i++)
        {
            Ball a = gridMatrix[levelData.headPairs[i].a.col, levelData.headPairs[i].a.row].GetComponentInChildren<Ball>();
            Ball b = gridMatrix[levelData.headPairs[i].b.col, levelData.headPairs[i].b.row].GetComponentInChildren<Ball>();
            a.SetBallType(levelData.headPairs[i].type);
            b.SetBallType(levelData.headPairs[i].type);
            a.SetPair(b);
        }
        if (editor)
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
        onBuilt.Invoke();
    }

    public void BuildAnimated()
    {
        if (!Application.isPlaying) return;
        WaitForSeconds waitForSeconds = new(0.7f / (levelData.width * levelData.length));
        IEnumerator routine()
        {
            List<Ball> balls = new();
            int ballCount = 0;
            ClearGrids();
            boardPlate.Adjust(new Vector3(levelData.width / 2f, 0, levelData.length / 2f), new Vector2(levelData.width, levelData.length));
            gridMatrix = new GridHolder[levelData.width, levelData.length];
            // Create grids and their holders
            for (int i = 0; i < levelData.width; i++)
            {
                for (int j = 0; j < levelData.length; j++)
                {
                    GridHolder gridHolder = Instantiate(ballGridPrefab).GetComponent<GridHolder>();

                    gridHolder.i = i;
                    gridHolder.j = j;
                    gridMatrix[i, j] = gridHolder;
                    gridHolder.transform.SetParent(gridContainer);
                    gridHolder.transform.position = startPosition + new Vector3(i, 0, j);
                    Ball ball = gridHolder.GetComponentInChildren<Ball>();
                    ball.SetGrid(gridHolder);
                    ball.name = "Ball " + ballCount++;
                    balls.Add(ball);
                    ball.transform.localScale = Vector3.zero;
                }
            }
            // Set adjacents
            for (int i = 0; i < levelData.width; i++)
            {
                for (int j = 0; j < levelData.length; j++)
                {
                    GridHolder grid = gridMatrix[i, j];
                    SetAdjacents(grid);
                }
            }
            for (int i = 0; i < levelData.obstacles.Length; i++)
            {
                LevelData.Obstacle obstacle = levelData.obstacles[i];
                PlaceObstacle(gridMatrix[obstacle.col, obstacle.row], obstacle.direction);
            }
            for (int i = 0; i < levelData.headPairs.Length; i++)
            {
                Ball a = gridMatrix[levelData.headPairs[i].a.col, levelData.headPairs[i].a.row].GetComponentInChildren<Ball>();
                Ball b = gridMatrix[levelData.headPairs[i].b.col, levelData.headPairs[i].b.row].GetComponentInChildren<Ball>();
                a.SetBallType(levelData.headPairs[i].type);
                b.SetBallType(levelData.headPairs[i].type);
                a.SetPair(b);
            }
            onBuilt.Invoke();
            foreach (Ball ball in balls.OrderBy((ball) => ball.transform.position.z).ThenBy((ball) => ball.transform.position.x))
            {
                ball.AnimatedStart();
                yield return waitForSeconds;
            }
            onBuildAnimationFinished.Invoke();
        }
        StartCoroutine(routine());
    }
    private void PlaceObstacle(GridHolder a, Direction direction, bool editor = false)
    {
        Vector3 obstaclePosition = Vector3.zero;
        Vector3 obstacleRotation = Vector3.zero;
        switch (direction)
        {
            case Direction.Left:
                if (a.left == null) return;
                obstaclePosition = new Vector3((a.i + a.left.i) / 2f, 0, a.j);
                obstacleRotation.y = 90;
                a.left.right = null;
                a.left = null;
                break;
            case Direction.Right:
                if (a.right == null) return;
                obstaclePosition = new Vector3((a.i + a.right.i) / 2f, 0, a.j);
                obstacleRotation.y = 90;
                a.right.left = null;
                a.right = null;
                break;
            case Direction.Up:
                if (a.up == null) return;
                obstaclePosition = new Vector3(a.i, 0, (a.j + a.up.j) / 2f);
                a.up.down = null;
                a.up = null;
                break;
            case Direction.Down:
                if (a.down == null) return;
                obstaclePosition = new Vector3(a.i, 0, (a.j + a.down.j) / 2f);
                a.down.up = null;
                a.down = null;
                break;
        }
        ObstacleHolder obstacle = null;
        if (editor)
        {
#if UNITY_EDITOR
            obstacle = (PrefabUtility.InstantiatePrefab(obstaclePrefab) as GameObject).GetComponent<ObstacleHolder>();
#endif
        }
        else
        {
            obstacle = Instantiate(obstaclePrefab).GetComponent<ObstacleHolder>();
        }
        obstacle.transform.SetParent(gridContainer);
        obstacle.transform.position = obstaclePosition + startPosition;
        obstacle.transform.eulerAngles = obstacleRotation;
    }

    private void SetAdjacents(GridHolder grid)
    {
        if (grid.i != 0)
            grid.left = gridMatrix[grid.i - 1, grid.j];
        if (grid.i != gridMatrix.GetLength(0) - 1)
            grid.right = gridMatrix[grid.i + 1, grid.j];
        if (grid.j != 0)
            grid.down = gridMatrix[grid.i, grid.j - 1];
        if (grid.j != gridMatrix.GetLength(1) - 1)
            grid.up = gridMatrix[grid.i, grid.j + 1];
    }

    private void ClearGrids()
    {
        while (gridContainer.childCount > 0)
        {
            DestroyImmediate(gridContainer.GetChild(0).gameObject);
        }
    }

    private void Reset()
    {
        gridContainer = transform.Find("Grids");
        if (gridContainer == null)
        {
            gridContainer = new GameObject("Grids").transform;
            gridContainer.SetParent(transform);
        }
    }
}
public enum Direction { None, Left, Right, Up, Down }
#if UNITY_EDITOR

[CustomEditor(typeof(Board))]
public class BoardEditor : Editor
{
    private Board board;
    private void OnEnable()
    {
        board = (Board)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Build"))
        {
            board.Build(true);
        }
    }
}
#endif