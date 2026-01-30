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


    public enum OutOfBoundStart
    {
        None,Up,Down,Right,Left
    }
    public OutOfBoundStart OOB_start = OutOfBoundStart.None;
}
