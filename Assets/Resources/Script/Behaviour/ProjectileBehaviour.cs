using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    // Start is called before the first frame update

    //public string _Direction = "Forward";
    //public AnimatorController spellAnimation;
    


    public int _Damage = 0;
    public float _Speed = 1f;
    public bool _isPercing = false;
    private float m_XLimit = 14;
    private float m_YLimit = 12;

    public enum Direction
    {
        Forward, Backward, Up, Down
    }

    public Direction _Direction;

    // Update is called once per frame

    //private void Start()
    //{
    //    AnimatorController  A = GetComponentInChildren<AnimatorController>();
    //    A = spellAnimation;
    //}
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

        if(Mathf.Abs(this.transform.position.z) >= m_XLimit || Mathf.Abs(this.transform.position.x) >= m_YLimit)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Player>(out Player player))
        {
            player.HP -= _Damage;
            if (player.HP < 0)
                player.HP = 0;

            Debug.Log(player.HP);
        }
        else if (other.TryGetComponent<EnemyBasics>(out EnemyBasics enemy))
        {
            enemy.HP -= _Damage;
            if (enemy.HP < 0)
                enemy.HP = 0;

            Debug.Log(enemy.HP);
        }

        Debug.Log(_isPercing);
        
        if (!this._isPercing) 
        {
            Destroy(this.gameObject);
        }
    }
}
