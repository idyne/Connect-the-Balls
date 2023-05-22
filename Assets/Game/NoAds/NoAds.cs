using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using UnityEngine.Purchasing;

public class NoAds : MonoBehaviour
{
    private static NoAds instance = null;
    public static NoAds Instance
    {

        get
        {
            if (instance == null) instance = FindObjectOfType<NoAds>();
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
    public void Show()
    {
        canvas.enabled = true;
    }
    public void EnableNoAds()
    {
        saveData.Value.NoAds = true;
        GameManager.Instance.SaveToDevice();
        Hide();
        NoAdsButton.Instance.Hide();
    }
    public void Log(string message)
    {
        Debug.Log(message);
    }
    public void OnPurchaseClicked()
    {
        DemoStorePage.Instance.HandlePurchase("com.voyager.connectballs.removeadvertisement");
    }
}

public partial class SaveData
{
    public bool NoAds = false;
}