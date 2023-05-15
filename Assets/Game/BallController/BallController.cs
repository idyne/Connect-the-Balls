using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using UnityEngine.InputSystem.EnhancedTouch;
using System.Linq;

public class BallController : FateMonoBehaviour
{
    [SerializeField] private LayerMask ballLayerMask;
    [SerializeField] public BallRuntimeSet freeBallsRuntimeSet;
    [SerializeField] public BallRuntimeSet ballsRuntimeSet;
    [SerializeField] private GameStateVariable gameState;
    private static bool enableLog = false;
    private Camera mainCamera;
    public Ball selectedBall;
    public Stack<ICommand> littleCommands = new();
    private Stack<Stack<ICommand>> commands = new();

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private int[,] CreateAdjacencyMatrix()
    {
        int[,] adjacencyMatrix = new int[ballsRuntimeSet.Items.Count, ballsRuntimeSet.Items.Count];
        //string lines = "";
        for (int i = 0; i < ballsRuntimeSet.Items.Count; i++)
        {
            //string line = "";
            for (int j = 0; j < ballsRuntimeSet.Items.Count; j++)
            {
                Ball a = ballsRuntimeSet.Items[i];
                Ball b = ballsRuntimeSet.Items[j];
                //adjacencyMatrix[i, j] = a.Grid.IsAdjacent(b.Grid) && (a.BallType == selectedBall.BallType || a.BallType == null) && (b.BallType == selectedBall.BallType || b.BallType == null) ? 1 : DijkstrasAlgorithm.NOT_REACHABLE_DISTANCE;
                adjacencyMatrix[i, j] = a.Grid.IsAdjacent(b.Grid) && (!a.IsHead) && (!b.IsHead) ? 1 : DijkstrasAlgorithm.NOT_REACHABLE_DISTANCE;
                if (a.IsHead) adjacencyMatrix[i, j] = DijkstrasAlgorithm.NOT_REACHABLE_DISTANCE;
                //line += adjacencyMatrix[i, j] + " ";
            }
            //lines += line + "\n";
        }
        //Debug.Log(lines);
        return adjacencyMatrix;
    }

    private void Update()
    {
        if (gameState.Value != GameState.IN_GAME) return;
        if (Input.touchSupported)
        {
            CheckTouchInput();
        }
        else
        {
            CheckMouseInput();
        }
    }

    private bool IsPathSolved(LevelData.Solution.Path path)
    {
        LevelData.Solution.Coordinates coordinates = path.path[0];
        Ball pathBall = ballsRuntimeSet.Items.Where((ball) => ball.Grid.i == coordinates.col && ball.Grid.j == coordinates.row).ToArray()[0];
        if (!pathBall.IsHead)
        {
            Debug.LogError("Not head!");
            return false;
        }
        bool reverse = pathBall.Previous != null;
        if (!reverse && pathBall.Pair.Previous == null) return false;
        Debug.Log(pathBall, pathBall);
        Ball current = pathBall;
        for (int i = 1; i < path.path.Length; i++)
        {
            coordinates = path.path[i];
            Ball ball = ballsRuntimeSet.Items.Where((ball) => ball.Grid.i == coordinates.col && ball.Grid.j == coordinates.row).ToArray()[0];
            Debug.Log(ball, ball);
            if (ball != current.Next) return false;
            current = ball;
        }
        return true;
    }

    public void Hint()
    {
        LevelData levelData = Board.Instance.LevelData;
        LevelData.Solution.Path path = levelData.solution.paths[0];
        bool found = false;
        if (IsPathSolved(path))
        {
            for (int i = 1; i < levelData.solution.paths.Length; i++)
            {
                path = levelData.solution.paths[i];
                if (!IsPathSolved(path))
                {
                    found = true;
                    break;
                }
            }
        }
        else
        {
            found = true;
        }
        if (!found) return;
        LevelData.Solution.Coordinates coordinates = path.path[0];
        Ball pathBall = ballsRuntimeSet.Items.Where((ball) => ball.Grid.i == coordinates.col && ball.Grid.j == coordinates.row).ToArray()[0];
        SelectBall(pathBall);

        for (int i = 1; i < path.path.Length; i++)
        {
            coordinates = path.path[i];
            pathBall = ballsRuntimeSet.Items.Where((ball) => ball.Grid.i == coordinates.col && ball.Grid.j == coordinates.row).ToArray()[0];
            if (pathBall.IsHead) break;
            MoveTo(pathBall);
        }
    }

    public void Undo()
    {
        if (this.commands.Count == 0) return;
        Stack<ICommand> commands = this.commands.Pop();
        while (commands.Count > 0)
        {
            ICommand command = commands.Pop();
            command.Undo();
        }

    }

    public void Restart()
    {
        while (commands.Count > 0)
            Undo();
    }

    public void ExecuteLittleCommand(ICommand command)
    {
        command.Execute();
        littleCommands.Push(command);
    }
    public ICommand UndoLittleCommand()
    {
        ICommand command = littleCommands.Pop();
        command.Undo();
        return command;
    }

    private void CheckTouchInput()
    {
        if (Input.touchCount > 0)
        {
            UnityEngine.Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    SelectBall(touch.position);
                    break;
                case TouchPhase.Moved:
                    MoveSelectedBall(touch.position);
                    break;
                case TouchPhase.Stationary:
                    break;
                case TouchPhase.Ended:
                    ReleaseSelectedBall();
                    break;
                case TouchPhase.Canceled:
                    ReleaseSelectedBall();
                    break;
                default:
                    break;
            }
        }
    }
    private void CheckMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectBall(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            MoveSelectedBall(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
            ReleaseSelectedBall();
    }

    private void SelectBall(Vector2 mousePosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ballLayerMask, QueryTriggerInteraction.Collide))
        {
            Ball ball = hit.transform.GetComponent<Ball>();
            if (ball.BallType == null) return;
            // bir rengin hiçbir linki yoktur ve headlerden herhangi biri seçilmiştir
            if (ball.IsHead && ball.Next == null && ball.Pair.Next == null)
            {
                AssignSelectedBallCommand assignSelectedBallCommand = new(this, ball);
                ExecuteLittleCommand(assignSelectedBallCommand);
            }
            // bir rengin linki vardır ve linkin en ucu seçilmiştir
            else if (!ball.IsHead && ball.Next == null)
            {
                AssignSelectedBallCommand assignSelectedBallCommand = new(this, ball);
                ExecuteLittleCommand(assignSelectedBallCommand);
            }
            // bir rengin linki vardır ve linkin ucu olmayan bir üyesi seçilmiştir
            else if (ball.Next != null)
            {

                AssignSelectedBallCommand assignSelectedBallCommand = new(this, ball);
                ExecuteLittleCommand(assignSelectedBallCommand);
                ClearForwardCommand clearForwardCommand = new(ball);
                ExecuteLittleCommand(clearForwardCommand);
            }
            // bir rengin linki vardır ama linkte olmayan head seçilmiştir
            else if (ball.IsHead && ball.Pair.Next != null)
            {

                AssignSelectedBallCommand assignSelectedBallCommand = new(this, ball);
                ExecuteLittleCommand(assignSelectedBallCommand);
                ClearForwardCommand clearForwardCommand = new(ball.Pair);
                ExecuteLittleCommand(clearForwardCommand);
            }

        }
    }
    private void SelectBall(Ball ball)
    {
        if (ball.BallType == null) return;
        // bir rengin hiçbir linki yoktur ve headlerden herhangi biri seçilmiştir
        if (ball.IsHead && ball.Next == null && ball.Pair.Next == null)
        {
            AssignSelectedBallCommand assignSelectedBallCommand = new(this, ball);
            ExecuteLittleCommand(assignSelectedBallCommand);
        }
        // bir rengin linki vardır ve linkin en ucu seçilmiştir
        else if (!ball.IsHead && ball.Next == null)
        {
            AssignSelectedBallCommand assignSelectedBallCommand = new(this, ball);
            ExecuteLittleCommand(assignSelectedBallCommand);
        }
        // bir rengin linki vardır ve linkin ucu olmayan bir üyesi seçilmiştir
        else if (ball.Next != null)
        {

            AssignSelectedBallCommand assignSelectedBallCommand = new(this, ball);
            ExecuteLittleCommand(assignSelectedBallCommand);
            ClearForwardCommand clearForwardCommand = new(ball);
            ExecuteLittleCommand(clearForwardCommand);
        }
        // bir rengin linki vardır ama linkte olmayan head seçilmiştir
        else if (ball.IsHead && ball.Pair.Next != null)
        {

            AssignSelectedBallCommand assignSelectedBallCommand = new(this, ball);
            ExecuteLittleCommand(assignSelectedBallCommand);
            ClearForwardCommand clearForwardCommand = new(ball.Pair);
            ExecuteLittleCommand(clearForwardCommand);
        }
    }
    private void MoveTo(Ball ball)
    {
        if (ball == selectedBall) return;
        if (selectedBall.Grid.IsAdjacent(ball.Grid))
        {
            // diğer top başka bir renktir ve head değildir
            if (!(selectedBall.IsHead && selectedBall.Previous != null) && ball.BallType != selectedBall.BallType && ball.BallType != null && !ball.IsHead)
            {
                Ball previousSelectedBall = selectedBall;
                AssignSelectedBallCommand assignSelectedBallCommand = new(this, ball);
                ExecuteLittleCommand(assignSelectedBallCommand);
                Ball previousPreviousBall = ball.Previous;
                ClearForwardCommand clearForwardCommand = new(ball.Previous);
                ExecuteLittleCommand(clearForwardCommand);
                BindIfNearHead(previousPreviousBall);
                BindCommand bindCommand = new(previousSelectedBall, ball);
                ExecuteLittleCommand(bindCommand);
            }
            // diğer top renksizdir
            else if (!(selectedBall.IsHead && selectedBall.Previous != null) && ball.BallType == null)
            {
                Ball previousSelectedBall = selectedBall;
                AssignSelectedBallCommand assignSelectedBallCommand = new(this, ball);
                ExecuteLittleCommand(assignSelectedBallCommand);
                BindCommand bindCommand = new(previousSelectedBall, ball);
                ExecuteLittleCommand(bindCommand);

            }
            // diğer top selectedBallun rootunun pairidir
            else if (!(selectedBall.IsHead && selectedBall.Previous != null) && ball == selectedBall.Root.Pair)
            {
                Ball previousSelectedBall = selectedBall;
                AssignSelectedBallCommand assignSelectedBallCommand = new(this, ball);
                ExecuteLittleCommand(assignSelectedBallCommand);
                BindCommand bindCommand = new(previousSelectedBall, ball);
                ExecuteLittleCommand(bindCommand);
            }
            // diğer top selectedBallun linkindedir
            else if (ball.BallType == selectedBall.BallType && ball.Root == selectedBall.Root)
            {
                while (littleCommands.Count > 1 && selectedBall != ball)
                {
                    UndoLittleCommand();
                }
                while (selectedBall.Next)
                {
                    AssignSelectedBallCommand assignSelectedBallCommand = new(this, selectedBall.Next);
                    ExecuteLittleCommand(assignSelectedBallCommand);
                }
                while (selectedBall != ball)
                {
                    Ball previous = selectedBall.Previous;
                    AssignSelectedBallCommand assignSelectedBallCommand = new(this, previous);
                    ExecuteLittleCommand(assignSelectedBallCommand);
                    UnbindCommand unbindCommand = new(previous);
                    ExecuteLittleCommand(unbindCommand);
                }

            }
        }
        else
        {
            // komşu olmayan bir yerdeki top seçiliyse

            if (ball.BallType != null && ball.BallType != selectedBall.BallType) return;
            int index;
            if (selectedBall.IsHead && selectedBall.Root != selectedBall)
                index = ballsRuntimeSet.Items.IndexOf(selectedBall.Previous);
            else
                index = ballsRuntimeSet.Items.IndexOf(selectedBall);
            int ballIndex = ballsRuntimeSet.Items.IndexOf(ball);
            int[] parents = DijkstrasAlgorithm.dijkstra(CreateAdjacencyMatrix(), index, out int[] shortestDistances);
            int distance = shortestDistances[ballIndex];
            if (distance >= DijkstrasAlgorithm.NOT_REACHABLE_DISTANCE) return;

            int[] path = DijkstrasAlgorithm.path(ballIndex, parents);
            for (int i = 0; i < path.Length; i++)
            {
                Ball pathBall = ballsRuntimeSet.Items[path[i]];
                if (pathBall.IsHead) break;
                MoveTo(pathBall);
            }



        }
    }
    private void MoveSelectedBall(Vector2 mousePosition)
    {
        if (!selectedBall) return;
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ballLayerMask, QueryTriggerInteraction.Collide))
        {
            Ball ball = hit.transform.GetComponent<Ball>();
            MoveTo(ball);
        }
    }

    public void Bind(Ball ball)
    {
        if (!selectedBall || !ball || !selectedBall.CanBind(ball)) return;
        selectedBall.Bind(ball);
        selectedBall = ball;
        if (freeBallsRuntimeSet.Items.Count == 0)
        {
            ReleaseSelectedBall();
            GameManager.Instance.FinishLevel(true);
        }
    }

    public void CheckFinish()
    {
        if (freeBallsRuntimeSet.Items.Count == 0)
        {
            ReleaseSelectedBall();
            GameManager.Instance.FinishLevel(true);
        }
    }

    public static void LogColored(string message, Color color, Object context = null)
    {
        if (!enableLog) return;
        Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message), context);
    }

    public void BindIfNearHead(Ball currentBall)
    {
        if (!currentBall) return;

        if (!currentBall.IsHead)
        {
            Ball ball = null;
            if (currentBall.Grid.left)
            {
                ball = currentBall.Grid.left.ball;
                if (!(ball.IsHead && ball.BallType == currentBall.BallType && ball != currentBall.Root))
                    ball = null;
            }
            if (!ball && currentBall.Grid.right)
            {
                ball = currentBall.Grid.right.ball;
                if (!(ball.IsHead && ball.BallType == currentBall.BallType && ball != currentBall.Root))
                    ball = null;
            }
            if (!ball && currentBall.Grid.up)
            {
                ball = currentBall.Grid.up.ball;
                if (!(ball.IsHead && ball.BallType == currentBall.BallType && ball != currentBall.Root))
                    ball = null;
            }
            if (!ball && currentBall.Grid.down)
            {
                ball = currentBall.Grid.down.ball;
                if (!(ball.IsHead && ball.BallType == currentBall.BallType && ball != currentBall.Root))
                    ball = null;
            }
            if (ball)
            {
                BindCommand bindCommand = new(currentBall, ball);
                ExecuteLittleCommand(bindCommand);
            }
        }

    }

    public void ReleaseSelectedBall()
    {
        if (!selectedBall) return;


        if (!selectedBall.IsHead)
        {
            Ball ball = null;
            if (selectedBall.Grid.left)
            {
                ball = selectedBall.Grid.left.ball;
                if (!(ball.IsHead && ball.BallType == selectedBall.BallType && ball != selectedBall.Root))
                    ball = null;
            }
            if (!ball && selectedBall.Grid.right)
            {
                ball = selectedBall.Grid.right.ball;
                if (!(ball.IsHead && ball.BallType == selectedBall.BallType && ball != selectedBall.Root))
                    ball = null;
            }
            if (!ball && selectedBall.Grid.up)
            {
                ball = selectedBall.Grid.up.ball;
                if (!(ball.IsHead && ball.BallType == selectedBall.BallType && ball != selectedBall.Root))
                    ball = null;
            }
            if (!ball && selectedBall.Grid.down)
            {
                ball = selectedBall.Grid.down.ball;
                if (!(ball.IsHead && ball.BallType == selectedBall.BallType && ball != selectedBall.Root))
                    ball = null;
            }
            if (ball)
            {
                Ball previousSelectedBall = selectedBall;
                AssignSelectedBallCommand assignSelectedBallCommand = new(this, ball);
                ExecuteLittleCommand(assignSelectedBallCommand);
                BindCommand bindCommand = new(previousSelectedBall, ball);
                ExecuteLittleCommand(bindCommand);
            }
        }
        AssignSelectedBallCommand assignNullToSelectedBallCommand = new(this, null);
        ExecuteLittleCommand(assignNullToSelectedBallCommand);
        FinalizeLittleCommands();
    }
    private void FinalizeLittleCommands()
    {
        Debug.Log("FinalizeLittleCommands");
        commands.Push(littleCommands);
        littleCommands = new();
        CheckFinish();
    }

    public class AssignSelectedBallCommand : ICommand
    {
        private readonly BallController ballController;
        private readonly Ball ball;
        private Ball previousSelectedBall = null;
        public bool Done { get; private set; }


        public AssignSelectedBallCommand(BallController ballController, Ball ball)
        {
            this.ballController = ballController;
            this.ball = ball;
        }

        public bool Execute()
        {
            if (Done) return false;
            previousSelectedBall = ballController.selectedBall;
            LogColored("AssignSelectedBallCommand: " + previousSelectedBall + " => " + ball, Color.cyan, ball);
            ballController.selectedBall = ball;
            Done = true;
            return true;
        }

        public bool Undo()
        {
            if (!Done) return false;
            LogColored("AssignSelectedBallCommand: " + previousSelectedBall + " => " + ball, Color.green, ball);
            ballController.selectedBall = previousSelectedBall;
            previousSelectedBall = null;
            Done = false;
            return true;
        }
    }

    public class ClearForwardCommand : ICommand
    {
        private readonly Ball ball;
        private Queue<Ball> forwardBalls = new();
        public bool Done { get; private set; }


        public ClearForwardCommand(Ball ball)
        {
            this.ball = ball;
        }

        public bool Execute()
        {
            if (Done) return false;
            LogColored("ClearForwardCommand: " + ball, Color.cyan, ball);
            Ball next = ball.Next;
            while (next)
            {
                forwardBalls.Enqueue(next);
                next = next.Next;
            }
            ball.ClearForward();
            Done = true;
            return true;
        }

        public bool Undo()
        {
            if (!Done) return false;
            LogColored("ClearForwardCommand: " + ball, Color.green, ball);
            Ball current = ball;
            while (forwardBalls.Count > 0)
            {
                current.Bind(forwardBalls.Dequeue());
                current = current.Next;
            }
            Done = false;
            return true;
        }
    }
    public class BindCommand : ICommand
    {
        private readonly Ball ball;
        private readonly Ball nextBall;
        public bool Done { get; private set; }


        public BindCommand(Ball ball, Ball nextBall)
        {
            this.ball = ball;
            this.nextBall = nextBall;
        }

        public bool Execute()
        {
            if (Done) return false;
            LogColored("BindCommand: " + ball + " => " + nextBall, Color.cyan, ball);
            ball.Bind(nextBall);
            Done = true;
            return true;
        }

        public bool Undo()
        {
            if (!Done) return false;
            LogColored("BindCommand: " + ball + " => " + nextBall, Color.green, ball);
            ball.Unbind();
            Done = false;
            return true;
        }
    }
    public class UnbindCommand : ICommand
    {
        private readonly Ball ball;
        private Ball unbondBall = null;
        public bool Done { get; private set; }


        public UnbindCommand(Ball ball)
        {
            this.ball = ball;
        }

        public bool Execute()
        {
            if (Done) return false;
            LogColored("UnbindCommand: " + ball, Color.cyan, ball);
            unbondBall = ball.Next;
            ball.Unbind();
            Done = true;
            return true;
        }

        public bool Undo()
        {
            if (!Done) return false;
            LogColored("UnbindCommand: " + ball, Color.green, ball);
            ball.Bind(unbondBall);
            unbondBall = null;
            Done = false;
            return true;
        }
    }

}

