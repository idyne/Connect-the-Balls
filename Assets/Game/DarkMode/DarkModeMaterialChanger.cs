using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;

[RequireComponent(typeof(GameEventListener))]
public class DarkModeMaterialChanger : MonoBehaviour
{
    [SerializeField] private BoolVariable darkModeOn;
    [SerializeField] private Renderer m_renderer;
    [SerializeField] private int materialIndex;
    [SerializeField] private Material lightModeMaterial, darkModeMaterial;


    void Start()
    {
        SetMaterial();
    }

    public void SetMaterial()
    {
        Material[] materials = m_renderer.materials;
        materials[materialIndex] = darkModeOn.Value ? darkModeMaterial : lightModeMaterial;
        m_renderer.materials = materials;
    }
}
