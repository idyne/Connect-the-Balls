using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundImage : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    private void Awake()
    {
        canvas.worldCamera = Camera.main;
        canvas.planeDistance = 100;
    }

    private void Reset()
    {
        canvas = GetComponentInChildren<Canvas>();
    }
}
