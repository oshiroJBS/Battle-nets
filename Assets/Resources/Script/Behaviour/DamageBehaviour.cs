using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public float _ActiveFrame = 0.2f;
    public int _Damage;
    public bool _isStatic = false;
    public bool _isFriendly = false;

    void Start()
    {
        Invoke("End", _ActiveFrame);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player) && !_isFriendly)
        {
            player.HP -= _Damage;
            if (player.HP < 0)
                player.HP = 0;
        }
        else if (other.TryGetComponent<EnemyBasics>(out EnemyBasics enemy))
        {
            enemy.HP -= _Damage;
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
