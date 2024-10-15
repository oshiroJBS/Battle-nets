using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public float _ActiveFrame = 0.2f;
    public int _Damage;

    void Start()
    {
        Invoke("End", _ActiveFrame);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            player.HP -= _Damage;
        }
        else if (other.TryGetComponent<EnemyBasics>(out EnemyBasics enemy))
        {
            enemy.HP -= _Damage;
        }
    }

    private void End()
    {
        Destroy(gameObject);
    } 
}
