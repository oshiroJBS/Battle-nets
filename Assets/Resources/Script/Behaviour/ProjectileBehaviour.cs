using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    // Start is called before the first frame update

    //public string _Direction = "Forward";
    //public AnimatorController spellAnimation;

    private SpellLibrary _Library;


    public int _Damage = 0;
    public float _Speed = 1f;
    public bool _isComingBack = false;
    public bool _isPercing = false;
    public bool _isFriendly = true;

    private float m_StartingSpeed;
    private float m_Limit = 15;
    private float _cooldown = 0.5f;
    public Vector2 _ForcedMouvement = new Vector2(0, 0);
    public float StunTime = 0f;
    public bool Charm = false;
    public bool RandomTPonHit = false;
    public int _FireStack = 0;
    public int _PoisonStack = 0;

    public enum Direction
    {
        Forward, Backward, Up, Down
    }

    public Direction _Direction;

    // Update is called once per frame

    private void Awake()
    {
        if (_Library == null) _Library = GameObject.FindObjectOfType<SpellLibrary>();
    }

    private void Start()
    {
        //AnimatorController A = GetComponentInChildren<AnimatorController>();
        //A = spellAnimation;
        m_StartingSpeed = _Speed;
    }
    void Update()
    {
        if (!_Library._InGame) { return; }

        switch (_Direction)
        {
            case Direction.Forward:
                this.transform.Translate(Vector3.forward * _Speed * Time.deltaTime);
                break;

            case Direction.Backward:
                this.transform.Translate(Vector3.back * _Speed * Time.deltaTime);
                break;

            case Direction.Down:
                this.transform.Translate(Vector3.right * _Speed * Time.deltaTime);
                break;

            case Direction.Up:
                this.transform.Translate(Vector3.left * _Speed * Time.deltaTime);
                break;
        }

        if (Mathf.Abs(this.transform.position.z) >= m_Limit || Mathf.Abs(this.transform.position.x) >= m_Limit)
        {
            if (_isComingBack)
            {
                _Speed = 0;
                _isComingBack = false;
                Invoke("comeBack", _cooldown);
                m_Limit += 1;
            }
            else if (_Speed != 0f)
            {
                Destroy(gameObject);
            }
        }
    }
    private void comeBack()
    {
        _Speed = -1 * m_StartingSpeed;
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

            if (!this._isPercing)
            {
                Destroy(this.gameObject);
            }

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

            if (!this._isPercing)
            {
                Destroy(this.gameObject);
            }

            if (StunTime > 0)
            {
                enemy.stunCooldown += StunTime;
                enemy._isStun = true;
            }
        }
        else if (other.TryGetComponent<obstacleBehaviour>(out obstacleBehaviour obstacle))
        {
            obstacle.HP -= _Damage;

            if (!this._isPercing)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
