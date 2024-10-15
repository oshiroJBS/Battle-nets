using UnityEngine;

[CreateAssetMenu(fileName = "SpellsScriptableObject",menuName ="ScriptableObject/Projectile")]
public class ProjectileScriptableObject : SpellScriptableObject
{
    public int ManaCost;
    public float castingTime = 0.2f;

    public int _Damage;

    public float p_speed;
    public Vector2 p_StartingPosition;

    public int p_NbProjectile;
    public bool p_IsLaser;

    public enum Direction
    {
        Forward, Backward, Up, Down
    }

    public Direction SpellDirection;
}
