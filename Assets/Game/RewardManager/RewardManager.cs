using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using UnityEngine.Events;

[CreateAssetMenu(menuName ="Game/Reward Manager")]
public class RewardManager : ScriptableObject
{
    [SerializeReference] private SaveDataVariable saveData;
    [SerializeField] private UnityEvent onHintRewardGranted;
    public void HintReward()
    {
        saveData.Value.HintCount++;
        onHintRewardGranted.Invoke();
    }
}
