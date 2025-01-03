using UnityEngine;

[CreateAssetMenu(fileName = "SpellsScriptableObject",menuName ="ScriptableObject/Projectile")]
public class ProjectileScriptableObject : SpellScriptableObject
{
    public float p_speed;

    public bool p_IsLaser;
    public bool _isComingBack;


    public enum Direction
    {
        Forward, Backward, Up, Down
    }
    public Direction SpellDirection;
}
