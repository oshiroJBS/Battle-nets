using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public string _characterName;
    public float HPMax = 100;
    [HideInInspector] public float HP = 100;
    public float ManaMax = 3f;
    [HideInInspector] public float ManaRecuperation = 0.6f;
    [HideInInspector] public int Shield = 0;
    [HideInInspector] public float WeaponModifier;
    [HideInInspector] public float Defence;


    [SerializeField] private TextMeshProUGUI HPText;

    [SerializeField] private TilesManager _TileManager = null;

    //for weapon
    [SerializeField] private DamageBehaviour _DamageSphere = null;
    [SerializeField] private ProjectileBehaviour _ProjectileSphere = null;

    //Spell related
    public SpellLibrary _Library = null;
    private TalentManager _Manager;

    private const float resetTimer = 1.5f;
    private bool ResetOnce = true;

    public ArrayList _StartingDeck = new ArrayList();
    private ArrayList p_Deck;

    //Player Position
    [SerializeField] private Transform m_Player = null;

    private int newX = 0;
    private int newY = 1;
    [HideInInspector] public Vector2 TilePosition;// the player's position in Tiles Unit
    [HideInInspector] public Vector2 lastTilePosition;// the player's position in Tiles Unit
    [HideInInspector] public Vector2 PosBeforeEnemySpace;// the player's position in Tiles Unit
    //

    //Player Status
    [HideInInspector] public bool _isStun = false;
    [HideInInspector] public float stunCooldown;
    private float stunTimer;

    [HideInInspector] public int _FireStack = 0;
    private const float _FireTick = 0.5f;
    private float _FireTimer = 0f;
    private const int FireDamage = 40;
    [SerializeField] private TextMeshProUGUI _FireText;


    [HideInInspector] public int _PoisonStack = 0;
    private int _LastPoisonStack = 0;
    private const float _PoisonTick = 0.8f;
    private float _PoisonTimer = 0f;
    [SerializeField] private TextMeshProUGUI _PoisonText;
    //

    ///UI ///

    //Spell text
    [SerializeField] private TextMeshProUGUI txt_SpellA = null;
    [SerializeField] private TextMeshProUGUI txt_SpellB = null;
    private string SpellA;
    private string A_Cost;
    private string SpellB;
    private string B_Cost;

    //Mana
    private float LastManaMax;
    [HideInInspector] public float CurrentMana;

    [SerializeField] private Image ShuffleIcon;
    [SerializeField] private Image ManaGauge;
    [SerializeField] private Image ManaSplitter;

    //Life 
    [SerializeField] private Image LifeGauge;

    private bool IsCasting;
    private bool IsOnEnnemyTile = false;

    void Start()
    {
        if (_Manager == null) _Manager = GameObject.FindObjectOfType<TalentManager>();

        this.ShuffleIcon.gameObject.SetActive(false);


        this.m_Player.position = Position(newX, newY);


        ResetOnce = true;
        //

        CurrentMana = 0f;
        this.lastTilePosition = new Vector2(newX, newY);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_Library._InGame) { return; }


        LifeGauge.fillAmount = HP / HPMax;
        HPText.text = HP + " / " + HPMax;

        ManaGestion();

        if (!IsCasting && !_isStun)
        {
            Mouvement();
            spellManager();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Iscasting(weapon(_characterName));
            }
        }
        else if (_isStun)
        {
            stunTimer += Time.deltaTime;

            if (stunTimer >= stunCooldown)
            {
                _isStun = false;
                stunTimer = 0;
            }
        }

        if (_PoisonStack != 0)
        {
            _PoisonText.transform.parent.gameObject.SetActive(true);

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
            _PoisonText.text = _PoisonStack.ToString();
        }
        else
        {
            _PoisonText.transform.parent.gameObject.SetActive(false);
        }

        if (_FireStack != 0)
        {
            _FireText.transform.parent.gameObject.SetActive(true);



            _FireTimer += Time.deltaTime;

            if (_FireTimer >= _FireTick)
            {
                if (_FireStack >= 10)
                {
                    GetDamaged((int)((FireDamage + _Manager.GetBurnModifier()) * _Manager.GetBurnMultiplier()));
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
                GetDamaged((int)((10 + _Manager.GetBurnModifier()) * _Manager.GetBurnMultiplier()));
            }

            if (_FireStack < 0)
                _FireStack = 0;

            _FireText.text = _FireStack.ToString();
        }
        else
        {
            _FireText.transform.parent.gameObject.SetActive(false);
        }

        this.TilePosition = new Vector2(newX, newY);
        this.lastTilePosition = TilePosition;
    }





    ////////////////////////// FONCTION ///////////////////////////////////

    public void InstantiatePlayer(string name)
    {
        switch (name)
        {
            default:
                break;

            case "kou":
                HPMax = 900;
                ManaMax = 3f;
                ManaRecuperation = 0.6f;
                Defence = 0;
                break;

            case "pina":
                HPMax = 700;
                ManaMax = 4f;
                ManaRecuperation = 0.8f;
                Defence = 0;
                break;

            case "cyon":
                HPMax = 1000;
                ManaMax = 3f;
                ManaRecuperation = 0.75f;
                Defence = 0;
                break;
        }

        HP = HPMax;
        this._characterName = name;
        _StartingDeck = new ArrayList(_Library.CreateStartingDeck(_characterName));
        p_Deck = new ArrayList(_StartingDeck);

        // first Spells
        SpellA = GetNewSpell();
        A_Cost = _Library.GetCost(SpellA);

        SpellB = GetNewSpell();
        B_Cost = _Library.GetCost(SpellB);
    }

    #region basic (mouvement + weapon)

    private void Mouvement()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ++newY;
            if (newY >= 4)
            {
                newY = 3;
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            --newY;
            if (newY < 0)
            {
                newY = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            --newX;
            if (newX < 0)
            {
                newX = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ++newX;
            if (newX >= 4)
            {
                newX = 3;
            }
        }
        if (newX != lastTilePosition.x || newY != lastTilePosition.y)
        {
            if (GetClosestObject(Position(newX, newY), "Tile").childCount != 0)
            {
                this.m_Player.position = Position((int)lastTilePosition.x, (int)lastTilePosition.y);
                newX = (int)lastTilePosition.x;
                newY = (int)lastTilePosition.y;

            }
            else
            {
                this.m_Player.position = Position(newX, newY);

                if (!IsOnEnnemyTile)
                    this.m_Player.parent = _TileManager.Tiles[newX][newY];
            }
        }
    }

    private float weapon(string name)
    {
        float weaponCD = 0f;

        switch (name)
        {
            default:
                ProjectileBehaviour Projectile = null;
                DamageBehaviour Instance = null;
                break;

            case "kou":
                if (this.CurrentMana < 1f) { break; }
                this.CurrentMana -= 1f;

                Projectile = Instantiate(_ProjectileSphere, Position((int)TilePosition.x + 1, (int)TilePosition.y), Quaternion.identity);
                Projectile._Damage = ModifyWeaponDamage(30);
                Projectile._Speed = 40f;
                Projectile._Direction = ProjectileBehaviour.Direction.Forward;
                Projectile._isPercing = true;

                weaponCD = 0.3f;
                break;

            case "pina":
                if (this.CurrentMana < 0.5f) { break; }
                this.CurrentMana -= 0.5f;

                for (int i = -1; i < 2; i++)
                {
                    Projectile = Instantiate(_ProjectileSphere, Position((int)TilePosition.x + 1, (int)TilePosition.y + i), Quaternion.identity);
                    Projectile._Damage = ModifyWeaponDamage(20);
                    Projectile._Speed = 30f;
                    Projectile._isPercing |= true;
                    Projectile._Direction = ProjectileBehaviour.Direction.Forward;
                    Projectile.Charm = true;
                }

                weaponCD = 0.3f;
                break;
            case "cyon":
                if (this.CurrentMana < 1f) { break; }

                this.CurrentMana -= 1f;
                Instance = Instantiate(_DamageSphere, Position((int)TilePosition.x + 4, (int)TilePosition.y), Quaternion.identity);
                Instance._Damage = ModifyWeaponDamage(30);
                Instance.StunTime = 0.5f;
                Instance._ActiveFrame = 0.2f;
                Instance.tag = "cyon";

                weaponCD = 0.2f;
                break;
        }
        return weaponCD;
    }

    private int ModifyWeaponDamage(int weaponDamage)
    {
        weaponDamage = weaponDamage + _Manager.GetWeponDamageModifier();
        return weaponDamage;
    }

    #endregion

    #region spell and cast

    public void clearStatus()
    {
        _FireStack = 0;
        _PoisonStack = 0;
    }

    private void spellManager()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (_Library.cast(SpellA, TilePosition))
            {
                SpellA = GetNewSpell();
                A_Cost = _Library.GetCost(SpellA);
            }

        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_Library.cast(SpellB, TilePosition))
            {
                SpellB = GetNewSpell();
                B_Cost = _Library.GetCost(SpellB);
            }
        }

        if (p_Deck.Count <= 0 && SpellA == null && SpellB == null && ResetOnce)
        {
            ResetOnce = false;
            Invoke("DeckReset", resetTimer);

            ShuffleIcon.gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.D) && ResetOnce)
        {
            SpellA = null; A_Cost = null;
            SpellB = null; B_Cost = null;

            ResetOnce = false;
            Invoke("DeckReset", resetTimer);

            ShuffleIcon.gameObject.SetActive(true);
        }

        txt_SpellA.text = SpellA + A_Cost;
        txt_SpellB.text = SpellB + B_Cost;
    }

    private string GetNewSpell()
    {
        string ToInitialize;

        if (p_Deck.Count <= 0)
        {
            ToInitialize = null;
        }
        else
        {
            int SpellID = Random.Range(0, p_Deck.Count - 1);

            // this._Library.
            ToInitialize = ((SpellScriptableObject)p_Deck[SpellID]).name;

            p_Deck.Remove(p_Deck[SpellID]);
        }

        return ToInitialize;
    }

    private void ManaGestion()
    {
        CurrentMana += Time.deltaTime * ManaRecuperation;
        CurrentMana = Mathf.Clamp(CurrentMana, 0, ManaMax);

        ManaGauge.fillAmount = CurrentMana / ManaMax;

        if (ManaMax != LastManaMax)
        {
            foreach (Transform child in ManaGauge.transform)
            {
                Destroy(child.gameObject);
            }

            int Iinstance = 1;
            if ((ManaMax - 1) % 2 != 0)
            {
                Iinstance = 0;
            }

            for (int i = Iinstance; i < ManaMax - 1; i += 2)
            {
                Image Split;

                Split = Instantiate(ManaSplitter, ManaGauge.transform);
                Split.transform.localPosition = new Vector3(50 * (i / ManaMax), 0, 0);

                if (i > 0)
                {
                    Split = Instantiate(ManaSplitter, ManaGauge.transform);
                    Split.transform.localPosition = new Vector3(50 * -(i / ManaMax), 0, 0);
                }
            }

            LastManaMax = ManaMax;
        }
    }

    public void Iscasting(float castingTime)
    {
        IsCasting = true;
        Invoke("StopCast", castingTime);
    }

    public void StopCast()
    {
        IsCasting = false;
    }

    #endregion

    #region special mouvement
    public void MouveToEnnemySpace(int x, int y, float Timer)
    {
        if (IsOnEnnemyTile) return;

        PosBeforeEnemySpace = new Vector2(newX, newY);

        newX = Mathf.Clamp(newX + x, 0, 7);
        newY = Mathf.Clamp(newY + y, 0, 3);

        this.m_Player.position = Position(newX, newY);
        this.TilePosition = new Vector2(newX, newY);
        this.lastTilePosition = TilePosition;

        IsOnEnnemyTile = true;
        Invoke("ReturnToPlayerGrid", Timer);
    }

    private void ReturnToPlayerGrid()
    {
        newX = (int)PosBeforeEnemySpace.x;
        newY = (int)PosBeforeEnemySpace.y;

        this.m_Player.position = Position(newX, newY);
        this.m_Player.parent = _TileManager.Tiles[newX][newY];

        this.TilePosition = new Vector2(newX, newY);
        this.lastTilePosition = TilePosition;

        IsOnEnnemyTile = false;
    }

    public void ForcedMovement(int x, int y)
    {
        if (IsOnEnnemyTile) return;

        newX = Mathf.Clamp(newX + x, 0, 3);
        newY = Mathf.Clamp(newY + y, 0, 3);

        this.m_Player.position = Position(newX, newY);
        this.m_Player.parent = _TileManager.Tiles[newX][newY];

        this.TilePosition = new Vector2(newX, newY);
        this.lastTilePosition = TilePosition;
    }
    public void Teleport(int x, int y)
    {

        if (IsOnEnnemyTile) return;

        newX = Mathf.Clamp(x, 0, 3);
        newY = Mathf.Clamp(y, 0, 3);

        this.m_Player.position = Position(newX, newY);
        this.m_Player.parent = _TileManager.Tiles[newX][newY];

        this.TilePosition = new Vector2(newX, newY);
        this.lastTilePosition = TilePosition;
    }    

    public Vector3 Position(int Xtile, int Ytile)
    {
        return new Vector3(this._TileManager.Tiles[Xtile][Ytile].position.x, this.m_Player.position.y, this._TileManager.Tiles[Xtile][Ytile].position.z);
    }

    #endregion 


    public void gainShield(int ShieldGained)
    {
        Shield += ShieldGained;
    }

    public void GetDamaged(int damage)
    {
        Shield -= damage;

        if (Shield < 0)
        {
            HP += Shield; // Lose difference beetween Shied and damage
            Shield = 0;
        }
    }

    public void DeckReset()
    {
        ShuffleIcon.gameObject.SetActive(false);
        ResetOnce = true;
        p_Deck = new ArrayList(_StartingDeck);

        SpellA = GetNewSpell();
        A_Cost = _Library.GetCost(SpellA);
        SpellB = GetNewSpell();
        B_Cost = _Library.GetCost(SpellB);
    }

    public static Transform GetClosestObject(Vector3 position, string Tag = "", float ScaleMin = 0.001f) // used to search for object
    {
        float closestDistance = Mathf.Infinity;
        Transform closestObject = null;
        GameObject[] Object;

        if (Tag == "")
        {
            Tag = "Untagged";
            Debug.Log("warning, No tag as been set for GetClosetObject");
        }

        Object = GameObject.FindGameObjectsWithTag(Tag);

        for (var i = 0; i < Object.Length; i++)
        {
            float dist = Vector3.Distance(position, Object[i].transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                if (Object[i].transform.localScale.x >= ScaleMin)
                {
                    closestObject = Object[i].transform;
                }
            }
        }
        return closestObject;
    }
}
