using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;

public class Ball : FateMonoBehaviour
{
    [Header("Properties")]
    private bool isHead = false;
    [SerializeField] private BallType ballType = null;
    [SerializeField] private Ball previous = null, next = null, root = null, pair = null;
    [Header("Components")]
    [SerializeField] private MeshRenderer m_Renderer;
    [Header("Assets")]
    [SerializeField] private Transform link;
    [SerializeField] private MeshRenderer linkMeshRenderer;
    [SerializeField] private GridHolder grid;
    [SerializeField] private Material headMaterial;
    [SerializeField] private BallRuntimeSet ballsRuntimeSet,bondBallsRuntimeSet, freeBallsRuntimeSet;

    public BallType BallType { get => ballType; }
    public Ball Previous { get => previous; }
    public Ball Next { get => next; }
    public Ball Root { get => root; }
    public GridHolder Grid { get => grid; }
    public bool IsHead { get => isHead; }
    public Ball Pair { get => pair; }

    private void OnEnable()
    {
        ballsRuntimeSet.Add(this);
        freeBallsRuntimeSet.Add(this);
    }
    private void OnDisable()
    {
        ballsRuntimeSet.Remove(this);
        freeBallsRuntimeSet.Remove(this);
        bondBallsRuntimeSet.Remove(this);
    }

    private void Start()
    {
        isHead = ballType != null;
        if (isHead)
        {
            root = this;
            m_Renderer.transform.localScale *= 1.25f;
            m_Renderer.material = headMaterial;
            SetColor(ballType.color);

        }
    }
    private void Reset()
    {
        m_Renderer = GetComponentInChildren<MeshRenderer>();
    }

    public void SetBallType(BallType ballType)
    {
        if (isHead || ballType == null || ballType == this.ballType) return;
        this.ballType = ballType;
        if (Application.isPlaying)
            SetColor(ballType.color);
    }

    public void SetGrid(GridHolder grid)
    {
        if (grid == null || grid == this.grid) return;
        this.grid = grid;
        grid.SetBall(this);
    }

    public void SetPair(Ball pair)
    {
        if (pair == null || this.pair == pair) return;
        this.pair = pair;
        pair.SetPair(this);
    }

    public void ClearBallType()
    {
        bondBallsRuntimeSet.Remove(this);
        freeBallsRuntimeSet.Add(this);
        if (isHead || ballType == null) return;
        ballType = null;
        SetColor(Color.white);
    }
    public void SetColor(Color color)
    {
        m_Renderer.material.color = color;
        linkMeshRenderer.material.color = color;
    }
    public void SetNext(Ball next)
    {
        if (next == null) return;
        this.next = next;
        next.SetRoot(root);
        if (next.Previous != this) next.SetPrevious(this);
        link.gameObject.SetActive(true);
        link.localScale = new Vector3(1, 1, Vector3.Distance(next.transform.position, transform.position));
        link.rotation = Quaternion.LookRotation(next.transform.position - transform.position);
    }

    public void SetPrevious(Ball previous)
    {
        if (previous == null) return;
        this.previous = previous;
        if (previous.Next != this) previous.SetNext(this);
    }
    public void ClearNext()
    {
        if (this.next == null) return;
        Ball next = this.next;
        this.next = null;
        next.ClearPrevious();
        link.gameObject.SetActive(false);
    }

    public void ClearPrevious()
    {
        if (this.previous == null) return;
        Ball previous = this.previous;
        this.previous = null;
        previous.ClearNext();
        ClearRoot();
    }

    private void ClearRoot()
    {
        if (root == null) return;
        if (isHead) root = this;
        else root = null;
    }
    public void SetRoot(Ball root)
    {
        if (root == null) return;
        this.root = root;
    }

    public void ClearForward()
    {
        Ball head = this;
        while (head.Next != null)
        {
            head = head.Next;
        }
        Ball currentBall = head;
        while (currentBall != this)
        {
            Ball temp = currentBall;
            currentBall = currentBall.Previous;
            temp.ClearPrevious();
            temp.ClearBallType();
        }
        if (isHead)
        {
            bondBallsRuntimeSet.Remove(this);
            freeBallsRuntimeSet.Add(this);
        }
    }

    public void Bind(Ball nextBall)
    {
        if (!CanBind(nextBall)) return;
        nextBall.SetBallType(ballType);
        SetNext(nextBall);
        freeBallsRuntimeSet.Remove(this);
        freeBallsRuntimeSet.Remove(nextBall);
        bondBallsRuntimeSet.Add(this);
        bondBallsRuntimeSet.Add(nextBall);
    }
    public void Unbind()
    {
        if (next == null) return;
        next.ClearBallType();
        ClearNext();
        if (isHead)
        {
            bondBallsRuntimeSet.Remove(this);
            freeBallsRuntimeSet.Add(this);
        }

    }
    public bool CanBind(Ball ball)
    {
        return !next && ballType != null && ball && grid.IsAdjacent(ball.Grid);
    }
}
