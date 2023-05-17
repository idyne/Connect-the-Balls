using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level Creator")]
public class LevelCreator : ScriptableObject
{
    [SerializeField] private LevelDataTable levelDataTable;
    [SerializeField] private BallType[] ballTypes;
    [SerializeField] private List<string> level_ids = new();
    [SerializeField] private List<string> solution_ids = new();
    public void CreateFromText()
    {
        string rootPath = "Assets/Game/LevelGenerator/Data/";
        DirectoryInfo info = new DirectoryInfo(rootPath);
        FileInfo[] fileInfo = info.GetFiles("*.txt");
        Dictionary<string, string> levelDatas = new();
        Dictionary<string, string> solutionDatas = new();
        foreach (FileInfo file in fileInfo)
        {
            string fileNameWithoutExtension = file.Name[0..^4];
            string[] splitFileName = fileNameWithoutExtension.Split("_");
            string id = splitFileName[0];

            StreamReader inp_stm = new(file.FullName);
            string text = "";
            while (!inp_stm.EndOfStream)
            {
                string inp_ln = inp_stm.ReadLine();
                text += inp_ln + "\n";
            }
            inp_stm.Close();
            if (!level_ids.Contains(id) && splitFileName[^1] == "level")
            {

                levelDatas.Add(id, text);
                level_ids.Add(id);
            }
            else if (!solution_ids.Contains(id) && splitFileName[^1] == "solution")
            {
                solutionDatas.Add(id, text);
                solution_ids.Add(id);
            }
        }
        foreach (string id in levelDatas.Keys)
        {
            CreateLevel(id, levelDatas[id], solutionDatas[id]);
        }
        levelDataTable.Sort();
#if UNITY_EDITOR
        EditorUtility.SetDirty(levelDataTable);
#endif
        Debug.Log(levelDatas.Count + " levels created.");
    }
    public void CreateLevel(string id, string txt, string solutionTxt)
    {
#if UNITY_EDITOR
        string[] lines = txt.Trim().Split("\n", System.StringSplitOptions.RemoveEmptyEntries);
        string firstLine = lines[0].Trim();
        string[] splitFirstLine = firstLine.Split();
        if (splitFirstLine.Length != 3) { Debug.LogError("Invalid input", this); return; }
        int numberOfHeadPairs = int.Parse(splitFirstLine[0].Trim());
        int width = int.Parse(splitFirstLine[1].Trim());
        int length = int.Parse(splitFirstLine[2].Trim());
        LevelData levelData = CreateInstance<LevelData>();
        levelData.id = id;
        levelData.solution = new(solutionTxt);
        levelData.width = width;
        levelData.length = length;
        levelData.headPairs = new LevelData.HeadPair[numberOfHeadPairs];
        for (int i = 1; i < lines.Length; i++)
        {
            LevelData.HeadPair headPair = new();
            headPair.type = ballTypes[i - 1];
            levelData.headPairs[i - 1] = headPair;
            string input = lines[i];
            Regex pattern = new Regex(@"\((?<x>\d+), (?<y>\d+)\)");
            MatchCollection matches = pattern.Matches(input);
            int j = 0;
            foreach (Match match in matches)
            {
                int x = int.Parse(match.Groups["x"].Value);
                int y = int.Parse(match.Groups["y"].Value);
                LevelData.Head head = new(y, x);
                if (j++ == 0)
                    levelData.headPairs[i - 1].a = head;
                else
                    levelData.headPairs[i - 1].b = head;
            }
        }
        string folderPath = "Assets/Game/LevelData/Levels/" + numberOfHeadPairs + "x" + width + "x" + length; // Set the path for the new folder
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath); // Create the new folder
        }
        string[] fileNames = Directory.GetFiles(folderPath); // Get an array of file names

        int fileCount = fileNames.Length;

        string path = folderPath + "/LevelData_" + numberOfHeadPairs + "x" + width + "x" + length + "_" + fileCount / 2 + ".asset";
        AssetDatabase.CreateAsset(levelData, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = levelData;
        levelDataTable.levelDatas.Add(levelData);
#endif
    }



}
#if UNITY_EDITOR

[CustomEditor(typeof(LevelCreator))]
public class LevelCreatorEditor : Editor
{
    private LevelCreator levelCreator;
    private void OnEnable()
    {
        levelCreator = target as LevelCreator;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Create"))
        {
            levelCreator.CreateFromText();
        }
    }
}
#endif