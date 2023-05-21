using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using DG.Tweening;

public class BoardPlate : FateMonoBehaviour
{
    [SerializeField] private RectTransform imageTransform;
    [SerializeField] private Canvas canvas;
    private Camera mainCamera;
    private bool initialized = false;

    private void Awake()
    {
        Initialize();
    }
    private void Initialize()
    {
        if (initialized) return;
        mainCamera = Camera.main;
        canvas.worldCamera = mainCamera;
        canvas.planeDistance = 100;
        initialized = true;
    }
    public void Adjust(Vector3 center, Vector2 size, bool animated = true)
    {
        Initialize();
        //center = mainCamera.WorldToScreenPoint(center + Vector3.forward);
        //center.y = -1;
        /*size.x += 1;
        size.y += 1;
        if (animated)
        {
            //imageTransform.DOMove(center, 1).SetEase(Ease.OutBack);
            DOTween.To(() => imageTransform.sizeDelta, (Vector2 x) => imageTransform.sizeDelta = x, size * 100, 1).SetEase(Ease.OutBack);
        }
        else
        {
            //imageTransform.position = center;
            imageTransform.sizeDelta = size * 100;
        }*/
    }

    private void Reset()
    {
        canvas = GetComponentInChildren<Canvas>();
    }
}
