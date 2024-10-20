using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    // Start is called before the first frame update

    //public string _Direction = "Forward";
    //public AnimatorController spellAnimation;



    public int _Damage = 0;
    public float _Speed = 1f;
    public bool _isComingBack = false;
    public bool _isPercing = false;
    public bool _isFriendly;

    private float m_StartingSpeed;
    private float m_Limit = 15;
    private float _cooldown = 0.5f;


    public enum Direction
    {
        Forward, Backward, Up, Down
    }

    public Direction _Direction;

    // Update is called once per frame

    private void Start()
    {
        //AnimatorController A = GetComponentInChildren<AnimatorController>();
        //A = spellAnimation;
        m_StartingSpeed = _Speed;
    }
    void Update()
    {
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
            player.HP -= _Damage;
            if (player.HP < 0)
                player.HP = 0;

            if (!this._isPercing)
            {
                Destroy(this.gameObject);
            }
        }
        else if (other.TryGetComponent<EnemyBasics>(out EnemyBasics enemy))
        {
            enemy.HP -= _Damage;
            if (enemy.HP < 0)
                enemy.HP = 0;

            if (!this._isPercing)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
