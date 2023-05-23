using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundImage : MonoBehaviour
{
    private static BackgroundImage instance = null;
    public static BackgroundImage Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<BackgroundImage>();
            return instance;
        }
    }
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image image;
    [SerializeField] private Sprite[] backgrounds;
    int currentIndex = 0;
    private void Awake()
    {
        canvas.worldCamera = Camera.main;
        canvas.planeDistance = 100;
    }

    private void Reset()
    {
        canvas = GetComponentInChildren<Canvas>();
    }

    public void NextImage()
    {
        image.sprite = backgrounds[(++currentIndex) % backgrounds.Length];
    }
}
