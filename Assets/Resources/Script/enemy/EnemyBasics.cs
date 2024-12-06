using TMPro;
using UnityEngine;

public class EnemyBasics : MonoBehaviour
{
    private Player _Player;

    public float HPMax = 100;
    [HideInInspector] public float HP;
    public TextMeshProUGUI HPDisplay;

    public string _SpellToCast = "Discharge";

    [SerializeField] private TilesManager _TileManager;
    private SpellLibrary _Library = null;

    [SerializeField] private GameObject projectile;

    [SerializeField] private bool _NeedPosition = true;
    [SerializeField] private float _CooldownAttack = 0.5f;
    private float _TimerAttack = 0f;
    private bool _CanAttack = false;
    [SerializeField] private float _AttackStagger = 0.2f;

    /*[SerializeField] */
    private const float _Yposition = 0.6f;
    private float _TimerMouvement = 0f;


    [SerializeField] private float _CooldownMouvement = 0.5f;
    [SerializeField] private Vector2 _TargetOfMouvement;

    //Tiles
    private int _TileX = 0;
    private int _TileY = 0;
    private Vector2 _TilePosition;

    private int _NormedTileX;
    //

    private Material _StartingMaterial;
    [SerializeField] private Material _HurtMaterial;


    private void Awake()
    {
        //initialization
        HP = HPMax;
        if (_Player == null) _Player = GameObject.FindObjectOfType<Player>();
        if (_TileManager == null) _TileManager = GameObject.FindObjectOfType<TilesManager>();
        if (_Library == null) _Library = GameObject.FindObjectOfType<SpellLibrary>();
    }

    private void Start()
    {
        _TimerMouvement = 0;
        _StartingMaterial = this.GetComponent<MeshRenderer>().material;

        this.transform.parent = _TileManager.Tiles[_NormedTileX][_TileY];
        //

        this._TileX = Random.Range(0, 3);
        this._TileY = Random.Range(0, 3);
        _NormedTileX = this._TileX + 4;
        //new starting point if occupied
        while (_TileManager.Tiles[_NormedTileX][_TileY].childCount > 1)
        {
            this._TileX = Random.Range(0, 3);
            this._TileY = Random.Range(0, 3);
            _NormedTileX = this._TileX + 4;
            this.transform.parent = _TileManager.Tiles[_NormedTileX][_TileY];
        }
        //
    }

    // Update is called once per frame
    private void Update()
    {
        HP = Mathf.Clamp(HP, 0, HPMax);

        HPDisplay.text = HP.ToString();

        ///scene reset
        if (this.HP <= 0)
        {
            if (!this._Player.retryScreen.activeSelf)
            {
                _Player.ActivateRetryScreen();
            }
            Destroy(this.gameObject);
        }
        ///

        _TimerMouvement += Time.deltaTime;
        _TimerAttack += Time.deltaTime;

        if (_TimerMouvement >= _CooldownMouvement)
        {
            this.Move();
            _TimerMouvement = 0;
        }

        _NormedTileX = this._TileX + 4;

        if (_NormedTileX >= this._TileManager.Tiles.Length)
        {
            this._NormedTileX = this._TileManager.Tiles.Length - 1;
        }

        if (_TileY >= this._TileManager.Tiles[_NormedTileX].Length)
        {
            this._TileY = this._TileManager.Tiles[_NormedTileX].Length - 1;
        }

        if (_TimerAttack >= _CooldownAttack)
        {
            if (!_NeedPosition)
            {
                _CanAttack = true;
            }

            if (_CanAttack)
            {
                _Library.cast(_SpellToCast, new Vector2(_NormedTileX, _TileY), true);
                _TimerAttack = 0;
                _TimerMouvement -= _AttackStagger;
            }
        }

        this.transform.position = this.Position(_NormedTileX, _TileY);
        this.transform.parent = _TileManager.Tiles[_NormedTileX][_TileY];

        _TilePosition = new Vector2(_TileX, _TileY);
        this.transform.parent = _TileManager.Tiles[_NormedTileX][_TileY];
    }

    private void Attack()
    {
        Instantiate(this.projectile, Position(_NormedTileX - 1, _TileY), Quaternion.identity);
    }

    private void OnTriggerEnter(Collider other)
    {
        GetComponent<MeshRenderer>().material = _HurtMaterial;
        Invoke("ResetMaterial", 0.3f);
    }

    private void ResetMaterial()
    {
        GetComponent<MeshRenderer>().material = _StartingMaterial;
    }

    private void Move()
    {
        Vector2 PositionByPlayer = _Player.TilePosition - this._TilePosition;
        PositionByPlayer.x = _NormedTileX - _Player.TilePosition.x;
        bool moved = false;

        if (PositionByPlayer != _TargetOfMouvement)
        {
            int newX;
            int newY;

            if (PositionByPlayer.y != _TargetOfMouvement.y)
            {
                newY = Mathf.Clamp(this._TileY + (int)Mathf.Clamp(PositionByPlayer.y - _TargetOfMouvement.y, -1, 1), 0, 3);

                if (_TileManager.Tiles[_NormedTileX][newY].childCount == 0)
                {
                    this._TileY = newY;
                    moved = true;
                }
            }

            if (PositionByPlayer.x != _TargetOfMouvement.x && !moved)
            {
                newX = Mathf.Clamp(this._TileX + (int)Mathf.Clamp(_TargetOfMouvement.x - PositionByPlayer.x, -1, 1), 0, 3);

                if (newX + 4 < _TileManager.Tiles.Length && newX + 4 >= 4)
                {
                    if (_TileManager.Tiles[newX + 4][_TileY].childCount == 0)// + 4 for going in the "Ennemy grid"
                    {
                        this._TileX = newX;
                    }
                }
            }

            _CanAttack = false;
        }
        else
        {
            _CanAttack = true;
        }
    }

    private Vector3 Position(int Xtile, int Ytile)
    {
        return new Vector3(this._TileManager.Tiles[Xtile][Ytile].position.x, _Yposition, this._TileManager.Tiles[Xtile][Ytile].position.z);
    }
}
