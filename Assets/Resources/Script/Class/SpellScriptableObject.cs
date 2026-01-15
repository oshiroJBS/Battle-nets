using UnityEngine;

public class SpellScriptableObject : ScriptableObject
{
    [Header("Health Settings")]
    public string CharacterDeckStarter;

    public int ManaCost;
    public float castingTime = 0.2f;
    [Header("IF negative THEN heal")]
    public int _Damage;
    [Header("IF negative THEN remove shield")]
    public int _Shield;

    public int nbWave = 1;
    [Header("time between Each Wave")]
    public float waveCooldown = 0.2f;
    public bool fstWaveCooldown = false;
    public bool isWaveStatic = true;

    [Header("based on charactet player IFNOT static")]
    public Vector2 _StartingPosition;

    public Vector2 _NbObject;
    [Header("Move target of vector")]
    public Vector2 _ForcedMouvement = new Vector2(0, 0);
    [Header("Move target to random tile")]
    public bool RandomTPonHit = false;

    public float MoveToEnnemyGridForTime;
    [Header("TP¨player on the opposite tile before cast")]
    public bool TpOnOpposite = false;
    [Header("TP¨player on random tile before cast")]
    public bool RandomTpOnCast = false;
    public Vector2 _TpOnCast = new Vector2(0, 0);
    public float StunTime = 0f;

    [Header("true = can't harm player")]
    public bool _isFriendly = true;
    [Header("set position to player | ennemi only")]
    public bool _aimed = false;
    [Header("reCalculate the starting point at each wave")]
    public bool _resetStartPoint = false;
    [Header("start on the opposite tile")]
    public bool _OppositeTile = false;
    [Header("spawn on a define set of tile")]
    public bool _isStatic;

    public bool _Charm = false;
    public int _FireStack = 0;
    public int _PoisonStack = 0;

    public Sprite Icon;
}
