using TMPro;
using UnityEngine;

public class EnemyBasics : MonoBehaviour
{
    private Player _Player;
    private TalentManager _Manager;

    public float HPMax = 100;
    [HideInInspector] public float HP;
    public TextMeshProUGUI HPDisplay;
    [SerializeField] private int ExpValue = 0;

    public SpellScriptableObject _SpellToCast;

    private TilesManager _TileManager;
    private SpellLibrary _Library = null;
    private GUIcanvas _GUI = null;

    [SerializeField] private bool _NeedPosition = true;
    [SerializeField] private float _CooldownAttack = 0.5f;
    private float _TimerAttack = 0f;
    private bool _CanAttack = false;
    [SerializeField] private float _AttackStagger = 0.2f;

    /*[SerializeField] */
    private const float _Yposition = 0.6f;
    private float _TimerMouvement = 0f;


    [SerializeField] private float _CooldownMouvement = 0.5f;
    [SerializeField] private bool _RandomMouvement = false;
    [SerializeField] private bool _isScared = false;
    [SerializeField] private Vector2 _TargetOfMouvement;
    private Vector2 _TargetTile;

    //Tiles
    private int _TileX = 0;
    private int _TileY = 0;
    private Vector2 _TilePosition;

    private int _NormedTileX;
    //

    //Status
    public int _Shield = 0;

    [HideInInspector] public int _FireStack = 0;
    private const float _FireTick = 0.5f;
    private float _FireTimer = 0f;

    [HideInInspector] public int _PoisonStack = 0;
    private int _LastPoisonStack = 0;
    private const float _PoisonTick = 0.8f;
    private float _PoisonTimer = 0f;

    [HideInInspector] public bool _isStun = false;
    [HideInInspector] public float stunCooldown;
    private float stunTimer;

    [HideInInspector] public int charmCount = 0;
    [HideInInspector] public bool isCharmed = false;
    private int _Stuck = 0;
    //

    private Material _StartingMaterial;
    [SerializeField] private Material _HurtMaterial;

    // image

    [SerializeField] private UnityEngine.UI.Image CharmIcon;
    [SerializeField] private UnityEngine.UI.Image BurnIcon;
    [SerializeField] private TextMeshProUGUI BurnText;
    [SerializeField] private UnityEngine.UI.Image PoisonIcon;
    [SerializeField] private TextMeshProUGUI PoisonText;


    private void Awake()
    {
        //initialization
        HP = HPMax;
        if (_Player == null) _Player = GameObject.FindObjectOfType<Player>();
        if (_TileManager == null) _TileManager = GameObject.FindObjectOfType<TilesManager>();
        if (_Library == null) _Library = GameObject.FindObjectOfType<SpellLibrary>();
        if (_GUI == null) _GUI = GameObject.FindObjectOfType<GUIcanvas>();
        if (_Manager == null) _Manager = GameObject.FindObjectOfType<TalentManager>();
    }

    private void Start()
    {
        BurnIcon.gameObject.SetActive(false);
        PoisonIcon.gameObject.SetActive(false);
        CharmIcon.fillAmount = 0;
        _TimerMouvement = 0;
        _StartingMaterial = this.GetComponent<MeshRenderer>().material;

        this._TileX = Random.Range(0, 3);
        this._TileY = Random.Range(0, 3);
        _NormedTileX = this._TileX + 4;

        this.transform.parent = _TileManager.Tiles[_NormedTileX][_TileY];
        
        
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
        if (!_Library._InGame) { return; }
        HP = Mathf.Clamp(HP, 0, HPMax);

        HPDisplay.text = HP.ToString();


        ///scene reset
        if (this.HP <= 0)
        {
            Debug.Log("isDying");
            Die();
        }
        ///

        if (_isStun)
        {
            stunTimer += Time.deltaTime;

            if (stunTimer >= stunCooldown)
            {
                _isStun = false;
                stunTimer = 0;
                stunCooldown = 0;
            }
        }
        else
        {
            if (_CooldownMouvement != 0)
            {
                _TimerMouvement += Time.deltaTime;

                if (_TimerMouvement >= _CooldownMouvement)
                {
                    this.Move();
                    _TimerMouvement = 0;
                }
            }

            //Attack
            _TimerAttack += Time.deltaTime;
            if (_TimerAttack >= _CooldownAttack)
            {
                if (!_NeedPosition)
                {
                    _CanAttack = true;
                }

                if (_CanAttack)
                {
                    _Library.cast(_SpellToCast.name, new Vector2(_NormedTileX, _TileY), true);
                    _TimerAttack = 0;
                    _TimerMouvement -= _AttackStagger;
                }
            }
        }

        if (_PoisonStack != 0)
        {
            PoisonIcon.gameObject.SetActive(true);

            _PoisonTimer += Time.deltaTime;

            if (_LastPoisonStack < _PoisonStack)
            {
                _PoisonTimer = 0;
            }

            if (_PoisonTimer >= _PoisonTick)
            {
                HP -= _PoisonStack;
                _PoisonTimer = 0;
                _PoisonStack -= 5;
            }

            if (_PoisonStack < 0)
                _PoisonStack = 0;

            _LastPoisonStack = _PoisonStack;
            PoisonText.text = _PoisonStack.ToString();
        }
        else
        {
            PoisonIcon.gameObject.SetActive(false);
        }

        if (_FireStack != 0)
        {
            BurnIcon.gameObject.SetActive(true);

            _FireTimer += Time.deltaTime;

            if (_FireTimer >= _FireTick)
            {
                if (_FireStack >= 10)
                {
                    GetDamaged(40);
                    _FireStack -= 10;
                }
                else
                {
                    _FireStack--;
                }
                _FireTimer = 0;
            }

            if (_FireStack == 0)
            {
                GetDamaged(10);
            }

            if (_FireStack < 0)
                _FireStack = 0;

            BurnText.text = _FireStack.ToString();
        }
        else
        {
            BurnIcon.gameObject.SetActive(false);
        }

        if (charmCount != 0)
        {
            CharmIcon.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            CharmIcon.transform.parent.gameObject.SetActive(false);
        }

        this.transform.position = this.Position(_NormedTileX, _TileY);
        this.transform.parent = _TileManager.Tiles[_NormedTileX][_TileY];

        _TilePosition = new Vector2(_TileX, _TileY);
        this.transform.parent = _TileManager.Tiles[_NormedTileX][_TileY];
    }


    public void GetDamaged(int damage)
    {
        GetComponent<MeshRenderer>().material = _HurtMaterial;

        _Shield -= damage;

        if (_Shield < 0)
        {
            HP += _Shield; // Lose difference beetween Shied and damage
            _Shield = 0;
        }


        if (charmCount >= 3 && !isCharmed)
        {
            isCharmed = false;
            charmCount = 0;
            CharmIcon.fillAmount = 0;
        }

        _TimerMouvement -= 0.2f;
        _TimerAttack -= 0.2f;

        Invoke("ResetMaterial", 0.3f);
    }

    public void GetCharmed()
    {
        charmCount++;

        if (charmCount >= 3)
        {
            isCharmed = true;
            charmCount = 3;
        }
        CharmIcon.fillAmount = charmCount / 3f;
    }

    public void gainShield(int ShieldGained)
    {
        _Shield += ShieldGained;
    }

    public void ForcedMovement(int x, int y)
    {
        if (x == 0 && y == 0) return;

        if (_TileManager.Tiles[Mathf.Clamp(_NormedTileX + x, 4, 7)][Mathf.Clamp(_TileY + y, 0, 3)].childCount == 0)// + 4 for going in the "Ennemy grid"
        {
            _TileX = Mathf.Clamp(_TileX + x, 0, 3);
            _TileY = Mathf.Clamp(_TileY + y, 0, 3);

            _TilePosition = new Vector2(_TileX, _TileY);
            _NormedTileX = this._TileX + 4;
            this.transform.position = this.Position(_NormedTileX, _TileY);
            this.transform.parent = _TileManager.Tiles[_NormedTileX][_TileY];
        }
        else
        {
            ForcedMovement(x - Mathf.Clamp(x, -1, 1), y - Mathf.Clamp(y, -1, 1));
        }
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

        if (_TileX == 0 && isCharmed)
        {
            isCharmed = false;
            charmCount = 0;
            CharmIcon.fillAmount = 0;
            stunCooldown += 0.3f;
            _isStun = true;
            _Stuck = 0;
            return;
        }
        else if (_Stuck >= 2)
        {
            isCharmed = false;
            charmCount = 0;
            CharmIcon.fillAmount = 0;
            stunCooldown += 0.3f;
            _isStun = true;
            _Stuck = 0;
            return;
        }

        if (isCharmed)
        {
            _TargetTile = new Vector2(4, _TileY);
        }
        else
        {
            if (_RandomMouvement)
            {
                _TargetTile = new Vector2(4 + Random.Range(0, 3), Random.Range(0, 3));
            }
            else if (_isScared)
            {

                if (_Player.TilePosition.x > 1)
                    _TargetTile.x = 7;
                else
                    _TargetTile.x = 4;

                if (_Player.TilePosition.y > 1)
                    _TargetTile.y = 0;
                else
                    _TargetTile.y = 3;

            }
            else
                _TargetTile = _Player.TilePosition + _TargetOfMouvement;
        }

        if (_NormedTileX != _TargetTile.x || _TilePosition.y != _TargetTile.y)
        {
            int newX;
            int newY;

            if (_TilePosition.y != _TargetTile.y)
            {
                newY = Mathf.Clamp(this._TileY + (int)Mathf.Clamp(_TargetTile.y - _TileY, -1, 1), 0, 3);

                if (_TileManager.Tiles[_NormedTileX][newY].childCount == 0)
                {
                    this._TileY = newY;
                    moved = true;
                }
            }

            if (_TilePosition.x != _TargetTile.x && !moved)
            {
                newX = Mathf.Clamp(this._TileX + (int)Mathf.Clamp(_TargetTile.x - _NormedTileX, -1, 1), 0, 3);

                if (_TileManager.Tiles[newX + 4][_TileY].childCount == 0)// + 4 for going in the "Ennemy grid"
                {
                    this._TileX = newX;
                    moved = true;
                }
            }

            if (!moved && isCharmed)
            {
                _Stuck += 1;
            }

            _CanAttack = false;
        }
        else
        {
            _CanAttack = true;
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
    }

    private Vector3 Position(int Xtile, int Ytile)
    {
        return new Vector3(this._TileManager.Tiles[Xtile][Ytile].position.x, _Yposition, this._TileManager.Tiles[Xtile][Ytile].position.z);
    }

    private void Die()
    {
        _GUI.tempExp += ExpValue;
        Debug.Log(_GUI.tempExp);
        Destroy(this.gameObject);
    }
}
