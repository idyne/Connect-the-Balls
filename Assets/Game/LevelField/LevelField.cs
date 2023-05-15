using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;

public class LevelField : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI levelText;
    [SerializeField] private SaveDataVariable saveData;
    private void Start()
    {
        SetLevelText();
    }
    public void SetLevelText()
    {
        levelText.text = "Level " + saveData.Value.Level;
    }
}
