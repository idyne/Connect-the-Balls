using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName ="Game/Level Data Table")]
public class LevelDataTable : ScriptableObject
{
    [SerializeField] public List<LevelData> levelDatas = new();
}
