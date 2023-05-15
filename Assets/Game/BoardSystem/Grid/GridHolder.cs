using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;

public class GridHolder : FateMonoBehaviour
{
    public int i, j;

    public GridHolder left, right, up, down;
    [SerializeField] public Ball ball;

    public bool IsAdjacent(GridHolder other)
    {
        return other == left || other == right || other == up || other == down;
    }

    public void SetBall(Ball ball)
    {
        if (ball == null && ball == this.ball) return;
        this.ball = ball;
        ball.SetGrid(this);
    }

}
