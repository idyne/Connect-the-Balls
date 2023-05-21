using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;

public class NoAds : MonoBehaviour
{
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
    public void EnableNoAds()
    {
        saveData.Value.NoAds = true;
        GameManager.Instance.SaveToDevice();
        Hide();
    }
    public void Log(string message)
    {
        Debug.Log(message);
    }
}

public partial class SaveData
{
    public bool NoAds = false;
}