using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellScriptableObject : ScriptableObject
{
    public string CharacterDeckStarter;

    public int ManaCost;
    public float castingTime = 0.2f;
    public int _Damage;

    public int nbWave = 1;
    public float waveCooldown = 0.2f;
    public bool fstWaveCooldown = false;
    public bool isWaveStatic = true;

    public Vector2 _StartingPosition;
    public Vector2 _NbObject;
    public Vector2 _ForcedMouvement = new Vector2(0,0);
    public bool RandomTPonHit = false;

    public float MoveToEnnemyGridForTime;
    public bool RandomTPonCAst = false;
    public Vector2 _TpOnCast = new Vector2(0,0);
    public float StunTime = 0f;

    public bool _isFriendly;
    public bool _isStatic;

    public bool _Charm = false;
    public int _FireStack = 0;
    public int _PoisonStack = 0;

    public Sprite Icon;
}
