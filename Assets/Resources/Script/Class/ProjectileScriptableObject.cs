using UnityEngine;

[CreateAssetMenu(fileName = "SpellsScriptableObject",menuName ="ScriptableObject/Projectile")]
public class ProjectileScriptableObject : SpellScriptableObject
{
    public int ManaCost;
    public float castingTime = 0.2f;
    public int nbWave = 1;
    public float waveCooldown = 0.2f;
    public bool fstWaveCooldown = false;
    public bool isWaveStatic = true;

    public int _Damage;

    public float p_speed;
    public Vector2 p_StartingPosition;

    public Vector2 p_NbProjectile;
    public bool p_IsLaser;
    public bool _isComingBack;
    public bool _isStatic;
    public bool _isFriendly;


    public enum Direction
    {
        Forward, Backward, Up, Down
    }
    public Direction SpellDirection;
}
