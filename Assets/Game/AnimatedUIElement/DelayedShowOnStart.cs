using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AnimatedUIElement))]
public class DelayedShowOnStart : MonoBehaviour
{
    [SerializeField] private float delay = 0;
    private AnimatedUIElement animatedUIElement;

    private void Awake()
    {
        animatedUIElement = GetComponent<AnimatedUIElement>();
    }

    private void Start()
    {
        animatedUIElement.HideWithoutAnimation();
        Invoke(nameof(animatedUIElement.Show), delay);
    }
}