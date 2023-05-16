using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FateGames.Core;
using DG.Tweening;

public class CameraController : FateMonoBehaviour
{
    private Board board => Board.Instance;


    private void Start()
    {
        
        //AdjustPosition();
    }

    public void AdjustPosition()
    {
        float aspectRatio = (Screen.width / (float)Screen.height);
        Vector3 pos = new Vector3(board.LevelData.width / 2f, board.LevelData.width * 2f / aspectRatio, board.LevelData.length / 2f );
        transform.DOMove(pos, 1).SetEase(Ease.OutBack);
    }
}
#if UNITY_EDITOR

[ CustomEditor(typeof(CameraController))]
public class CameraControllerEditor : Editor
{
    private CameraController cameraController;
    private void OnEnable()
    {
        cameraController = target as CameraController;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if(GUILayout.Button("Adjust Position"))
        {
            cameraController.AdjustPosition();
        }
    }
}
#endif