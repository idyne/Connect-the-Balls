using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using DG.Tweening;
using System.Linq;

public class DrawTutorial : FateMonoBehaviour
{
    [SerializeField] private List<int> levels;
    [SerializeField] private SaveDataVariable saveData;
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem effect;
    [SerializeField] public BallRuntimeSet ballsRuntimeSet;
    [SerializeField] public BallRuntimeSet freeBallsRuntimeSet;
    private LevelData.Solution.Path currentPath = null;
    private IEnumerator tutorialRoutine = null;
    private Tween tutorialTween = null;
    [SerializeField] private Canvas canvas;

    public void StartTutorial()
    {
        if (!levels.Contains(saveData.Value.Level) || freeBallsRuntimeSet.Items.Count == 0)
        {
            Hide();
            return;
        }
        Cancel();
        IEnumerator routine()
        {
            for (int i = 0; i < Board.Instance.LevelData.solution.paths.Length; i++)
            {
                LevelData.Solution.Path path = Board.Instance.LevelData.solution.paths[i];
                LevelData.Solution.Coordinates coordinates = path.path[0];
                Ball pathBall = ballsRuntimeSet.Items.Where((ball) => ball.Grid.i == coordinates.col && ball.Grid.j == coordinates.row).ToArray()[0];
                yield return new WaitUntil(() => pathBall.IsHead);
                if (!BallController.Instance.IsPathSolved(path))
                {
                    currentPath = path;
                    transform.position = new Vector3(0.5f + path.path[0].col, 0, 0.5f + path.path[0].row);
                    animator.SetTrigger("TouchDown");
                    yield return new WaitForSeconds(1f);
                    for (int j = 1; j < path.path.Length; j++)
                    {
                        bool finished = false;
                        tutorialTween = transform.DOMove(new Vector3(0.5f + path.path[j].col, 0, 0.5f + path.path[j].row), 0.5f).OnComplete(() => { tutorialTween = null; finished = true; });
                        yield return new WaitUntil(() => finished);
                    }
                    animator.SetTrigger("TouchUp");
                    yield return new WaitForSeconds(1f);
                    tutorialRoutine = routine();
                    yield return tutorialRoutine;
                    tutorialRoutine = null;
                    yield break;
                }
            }

        }
        tutorialRoutine = routine();
        StartCoroutine(tutorialRoutine);
    }

    public void Hide()
    {
        animator.enabled = false;
        canvas.enabled = false;
    }
    public void Show()
    {
        if (!levels.Contains(saveData.Value.Level))
        {
            return;
        }
        animator.enabled = true;
        canvas.enabled = true;
    }

    public void Cancel()
    {
        if (tutorialRoutine != null)
        {
            StopCoroutine(tutorialRoutine);
            tutorialRoutine = null;
        }
        if (tutorialTween != null)
        {
            tutorialTween.Kill();
            tutorialTween = null;
        }
    }

    public void StartOver()
    {
        if (!levels.Contains(saveData.Value.Level))
        {
            Hide();
            return;
        }
        if (!BallController.Instance.IsPathSolved(currentPath)) return;
        Cancel();
        StartTutorial();
    }

    private void Reset()
    {
        animator = GetComponentInChildren<Animator>();
    }
    public void PlayEffect()
    {
        

        effect.Play();
    }
}
