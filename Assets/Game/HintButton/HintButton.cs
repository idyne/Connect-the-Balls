using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
public class HintButton : FateMonoBehaviour
{
    [SerializeField] private SaveDataVariable saveData;
    [SerializeField] private TMPro.TextMeshProUGUI hintCountText;
    [SerializeField] private GameObject adIcon;

    private void Start()
    {
        SetHintCount();
    }
    public void SetHintCount()
    {
        if(saveData.Value.HintCount <= 0)
        {
            adIcon.SetActive(true);
            hintCountText.gameObject.SetActive(false);
        }
        else
        {
            adIcon.SetActive(false);
            hintCountText.gameObject.SetActive(true);
            hintCountText.text = saveData.Value.HintCount.ToString();
        }
        
    }
}
