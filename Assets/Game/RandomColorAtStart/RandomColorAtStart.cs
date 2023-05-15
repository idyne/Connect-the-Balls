using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColorAtStart : FateMonoBehaviour
{
    [SerializeField] private Renderer m_renderer;
    [SerializeField] private int materialIndex = 0;
    [SerializeField] private Gradient gradient;

    private void Start()
    {
        SetRandomColor();
    }
    private void Reset()
    {
        m_renderer = GetComponentInChildren<Renderer>();
    }
    public void SetRandomColor()
    {
        m_renderer.material.color = gradient.Evaluate(Random.value);
    }
}
