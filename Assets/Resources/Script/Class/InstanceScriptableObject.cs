using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellsScriptableObject", menuName = "ScriptableObject/Instance")]
public class InstanceScriptableObject : SpellScriptableObject
{
    public int ManaCost;
    public float castingTime = 0.2f;
    public int nbWave = 1;
    public float waveCooldown = 0.2f;
    public bool fstWaveCooldown = false;
    public bool isWaveStatic = true;

    public Vector2 _NbInstance;
    public Vector2 StartingPosition;

    public int _Damage;
    public float _LifeSpan;
    public float CastingInstance;
    public bool _isStatic;
    public bool _isFriendly;
}
