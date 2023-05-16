using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using DG.Tweening;
public class AnimatedUIElement : FateMonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Transform container;

    public void Show()
    {
        canvas.enabled = true;
        container.localScale = Vector3.zero;
        container.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }

    public void Hide()
    {

        container.localScale = Vector3.one;
        container.DOScale(Vector3.zero, 0.3f).OnComplete(() => { canvas.enabled = false; });
    }

    private void Reset()
    {
        canvas = GetComponentInChildren<Canvas>();
        container = canvas.transform.Find("Container");
    }

}
