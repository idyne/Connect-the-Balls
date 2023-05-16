using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using UnityEngine.UI;
using DG.Tweening;

public class Ball : FateMonoBehaviour
{
    [Header("Properties")]
    private bool isHead = false;
    [SerializeField] private BallType ballType = null;
    [SerializeField] private Ball previous = null, next = null, root = null, pair = null;
    [Header("Components")]
    [SerializeField] private MeshRenderer m_Renderer;
    [SerializeField] private Animator animator;
    [Header("Assets")]
    [SerializeField] private SoundEntity splashSound;
    [SerializeField] private SoundEntity explodeSound;
    [SerializeField] private Transform link;
    [SerializeField] private MeshRenderer linkMeshRenderer;
    [SerializeField] private GridHolder grid;
    [SerializeField] private Material headMaterial;
    [SerializeField] private BallRuntimeSet ballsRuntimeSet, bondBallsRuntimeSet, freeBallsRuntimeSet;
    [SerializeField] private ParticleSystem splashEffect;
    [SerializeField] private Image canvasImage;
    [SerializeField] private Transform canvasImageTransform;
    [SerializeField] private Canvas canvas;

    public int LinkLength = 1;

    private Tween canvasImageScaleTween = null;

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
    public void AnimatedStart()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutQuad);
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
        ParticleSystem.MainModule main = splashEffect.main;
        main.startColor = color;
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

    public void Bind(Ball nextBall, bool playEffect = true)
    {
        if (!CanBind(nextBall)) return;
        nextBall.SetBallType(ballType);
        SetNext(nextBall);
        freeBallsRuntimeSet.Remove(this);
        freeBallsRuntimeSet.Remove(nextBall);
        bondBallsRuntimeSet.Add(this);
        bondBallsRuntimeSet.Add(nextBall);
        int count = 0;
        Ball next = root;
        while (next)
        {
            next = next.next;
            count++;
        }
        LinkLength = count;
        nextBall.LinkLength = count;
        if (playEffect)
        {
            //GameManager.Instance.PlaySound(splashSound, 0.5f + LinkLength * 0.1f);
            nextBall.PlaySplashEffect();
            nextBall.PlayInLinkAnimation();
        }
    }
    public void PlaySplashSound()
    {
        GameManager.Instance.PlaySound(splashSound, 0.5f + LinkLength * 0.1f);
    }
    public void PlayInLinkAnimation()
    {
        canvas.enabled = true;
        Color color = ballType.color * 0.8f;
        color.a = 1;
        canvasImage.color = color;
        if (canvasImageScaleTween != null)
        {
            canvasImageScaleTween.Kill();
            canvasImageScaleTween = null;
        }
        canvasImageScaleTween = canvasImageTransform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutQuint).OnComplete(() =>
        {
            canvasImageScaleTween = null;
        });
        animator.SetBool("InLink", true);
    }
    public void StopInLinkAnimation()
    {
        if (canvasImageScaleTween != null)
        {
            canvasImageScaleTween.Kill();
            canvasImageScaleTween = null;
        }
        canvasImageScaleTween = canvasImageTransform.DOScale(Vector3.zero, 0.4f).OnComplete(() =>
         {
             canvasImageScaleTween = null;
             canvas.enabled = false;
         });

        animator.SetBool("InLink", false);

    }
    public void PlaySplashEffect()
    {
        splashEffect.Play();
    }
    public void Unbind(bool playEffect = true)
    {
        if (next == null) return;
        Debug.Log("Unbind: " + this, this);
        next.LinkLength = 1;
        next.ClearBallType();
        next.StopInLinkAnimation();
        int linkLength = LinkLength - 1;
        if (playEffect)
        {
            //GameManager.Instance.PlaySound(splashSound, 0.5f + LinkLength * 0.1f);
            PlaySplashEffect();
        }
        ClearNext();
        if (isHead)
        {
            bondBallsRuntimeSet.Remove(this);
            freeBallsRuntimeSet.Add(this);
        }
        Ball otherNext = root;
        while (otherNext)
        {
            otherNext.LinkLength = linkLength;
            otherNext = otherNext.next;
        }
    }

    public void Explode()
    {
        Deactivate();
        PooledEffect pooledEffect = BallExplosionPool.Instance.EffectPool.Get();
        pooledEffect.transform.position = transform.position;
        ParticleSystem effect = pooledEffect.Effect;
        ParticleSystem.MainModule main = effect.main;
        pooledEffect.ParticleSystemRenderer.material.color = ballType.color;
        main.startColor = ballType.color;
        
    }
    public void PlayExplodeSound(float pitch)
    {
        GameManager.Instance.PlaySound(explodeSound, pitch:pitch);
    }
    public bool CanBind(Ball ball)
    {
        return !next && ballType != null && ball && grid.IsAdjacent(ball.Grid);
    }
}
