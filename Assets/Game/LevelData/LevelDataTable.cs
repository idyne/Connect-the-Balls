using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
[CreateAssetMenu(menuName = "Game/Level Data Table")]
public class LevelDataTable : ScriptableObject
{

    [SerializeReference] private List<LevelData> _levelDatas = new List<LevelData>();

    public List<LevelData> levelDatas { get => _levelDatas; }

    public void Sort()
    {
        _levelDatas = levelDatas.OrderBy((levelData) => levelData.width * levelData.length).ThenBy((levelData) => levelData.headPairs.Length).ToList();
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(LevelDataTable))]
public class LevelDataTableEditor : Editor
{
    private LevelDataTable levelDataTable;
    private void OnEnable()
    {
        levelDataTable = target as LevelDataTable;
    }
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Sort"))
            levelDataTable.Sort();
        DrawDefaultInspector();
    }
}
#endif