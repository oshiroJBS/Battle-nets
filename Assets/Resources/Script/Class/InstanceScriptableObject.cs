using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellsScriptableObject", menuName = "ScriptableObject/Instance")]
public class InstanceScriptableObject : SpellScriptableObject
{
    public int ManaCost;
    public float castingTime = 0.2f;
    public Vector2 _NbInstance;
    public Vector2 StartingPosition;
    public int _Damage;
    public float _LifeSpan;
}
