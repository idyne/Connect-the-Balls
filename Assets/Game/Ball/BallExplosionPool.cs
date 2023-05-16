using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallExplosionPool : MonoBehaviour
{
    [SerializeField] private EffectPool effectPool;
    private static BallExplosionPool instance = null;
    public static BallExplosionPool Instance
    {

        get
        {
            if (instance == null) instance = FindObjectOfType<BallExplosionPool>();
            return instance;
        }
    }

    public EffectPool EffectPool { get => effectPool; }
}
