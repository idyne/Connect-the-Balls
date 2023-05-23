using DG.Tweening;
using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DataEditController : MonoBehaviour
{
    [SerializeField] private SaveDataVariable saveData;
    [SerializeField] private TextMeshProUGUI levelText = null;
    [SerializeField] private GameObject UIParent = null;

 
    public void SetLevel()
    {
        if (int.TryParse(levelText.text.Substring(0, levelText.text.Length - 1), out int result))
        {
            saveData.Value.Level = Mathf.Clamp(result, 1, int.MaxValue);
            GameManager.Instance.LoadCurrentLevel();
        }
    }

    public void ToggleUI()
    {
        UIParent.SetActive(!UIParent.activeSelf);
    }

    public void NextBackground()
    {
        BackgroundImage.Instance.NextImage();
    }

    public void NextHand()
    {
        HandCursor.Instance.NextImage();
    }

    public void NextHandSize()
    {
        HandCursor.Instance.NextSize();
    }
    public int resulutionIndex = 0;


}
