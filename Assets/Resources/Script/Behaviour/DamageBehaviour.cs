using Unity.VisualScripting;
using UnityEngine;

public class DamageBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public float _ActiveFrame = 0.2f;
    public int _Damage;
    public int _Shield = 0;
    public bool _isStatic = false;
    public bool _isFriendly = false;
    public Vector2 _ForcedMouvement = new Vector2(0, 0);
    public float StunTime = 0f;
    public bool Charm = false;
    public bool RandomTPonHit = false;
    public int _FireStack = 0;
    public int _PoisonStack = 0;

    private float buffer = 0f;
    private Material _Material;
    [SerializeField] private Material _bufferMat;
    [SerializeField] private Canvas _warning;

    void Start()
    {
        if (!_isFriendly)
        {
            _warning.enabled = true;
            buffer = 0.7f;
            this.GetComponent<Collider>().enabled = false;
            _Material = this.GetComponent<MeshRenderer>().material;
            this.GetComponent<MeshRenderer>().material = _bufferMat;
            Invoke("Activate", buffer);
        }
        else
        {
            _warning.enabled = false;
            buffer = 0f;
        }

        Invoke("End", _ActiveFrame + buffer);
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player) && !_isFriendly)
        {
            player.Shield = _Shield;
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
            enemy._Shield += _Shield;
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

    private void Activate()
    {

        _warning.enabled = false;
        this.GetComponent<Collider>().enabled = true;
        this.GetComponent<MeshRenderer>().material = _Material;
    }

    private void End()
    {
        Destroy(gameObject);
    }
}
