using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Game/Level Data Table")]
public class LevelDataTable : ScriptableObject
{

    [SerializeReference] private List<LevelData> _levelDatas = new List<LevelData>();

    public List<LevelData> levelDatas { get => _levelDatas; }
}
