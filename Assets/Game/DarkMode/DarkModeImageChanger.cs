using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using UnityEngine.UI;

[RequireComponent(typeof(GameEventListener))]
public class DarkModeImageChanger : MonoBehaviour
{
    [SerializeField] private BoolVariable darkModeOn;
    [SerializeField] private Image image;
    [SerializeField] private Sprite lightModeSprite, darkModeSprite;


    void Start()
    {
        SetMaterial();
    }

    public void SetMaterial()
    {
        image.sprite = darkModeOn.Value ? darkModeSprite : lightModeSprite;
    }
}
