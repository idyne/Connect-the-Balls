using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using UnityEngine.Purchasing;

public class NoAdsButton : MonoBehaviour
{
    private static NoAdsButton instance = null;
    public static NoAdsButton Instance
    {

        get
        {
            if (instance == null) instance = FindObjectOfType<NoAdsButton>();
            return instance;
        }
    }
    [SerializeField] private Canvas canvas;
    [SerializeField] private SaveDataVariable saveData;

    private void Start()
    {
        if (saveData.Value.NoAds) Hide();
    }
    public void Hide()
    {
        canvas.enabled = false;
    }


}
