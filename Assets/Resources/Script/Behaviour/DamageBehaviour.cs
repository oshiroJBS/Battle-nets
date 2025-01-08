using UnityEngine;

public class DamageBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public float _ActiveFrame = 0.2f;
    public int _Damage;
    public bool _isStatic = false;
    public bool _isFriendly = false;
    public Vector2 _ForcedMouvement = new Vector2(0, 0);
    public float StunTime = 0f;
    public bool Charm = false;
    public bool RandomTPonHit = false;
    public int _FireStack = 0;
    public int _PoisonStack = 0;

    void Start()
    {
        Invoke("End", _ActiveFrame);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player) && !_isFriendly)
        {
            player.GetDamaged(_Damage);
            player._FireStack += _FireStack;
            player._PoisonStack += _PoisonStack;

            if (player.HP < 0)
                player.HP = 0;

            if (RandomTPonHit)
            {
                _ForcedMouvement.x = UnityEngine.Random.Range(-3, 3);
                _ForcedMouvement.y = UnityEngine.Random.Range(-3, 3);
            }
            player.ForcedMovement((int)_ForcedMouvement.x, (int)_ForcedMouvement.y);


            if (StunTime > 0)
            {
                player.stunCooldown = StunTime;
                player._isStun = true;
            }
        }
        else if (other.TryGetComponent<EnemyBasics>(out EnemyBasics enemy))
        {
            enemy.GetDamaged(_Damage);
            enemy._FireStack += _FireStack;
            enemy._PoisonStack += _PoisonStack;

            if (enemy.HP < 0)
                enemy.HP = 0;

            if (Charm)
            {
                enemy.GetCharmed();
            }

            if (RandomTPonHit)
            {
                _ForcedMouvement.x = UnityEngine.Random.Range(-3, 3);
                _ForcedMouvement.y = UnityEngine.Random.Range(-3, 3);
            }

            enemy.ForcedMovement((int)_ForcedMouvement.x, (int)_ForcedMouvement.y);


            if (StunTime > 0)
            {
                enemy.stunCooldown += StunTime;
                enemy._isStun = true;
            }
        }
        else if (other.TryGetComponent<obstacleBehaviour>(out obstacleBehaviour obstacle))
        {
            obstacle.HP -= _Damage;
        }
    }

    private void End()
    {
        Destroy(gameObject);
    }
}
