using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public string _characterName;
    [SerializeField] private float HPMax = 100;
    [HideInInspector] public float HP;
    [SerializeField] private TextMeshProUGUI HPText;

    [SerializeField] private TilesManager _TileManager = null;

    //for weapon
    [SerializeField] private GameObject DamageSphere = null;

    //Spell related
    public SpellLibrary _Library = null;

    private const float resetTimer = 1.5f;
    private bool ResetOnce = true;

    public ArrayList _StartingDeck = new ArrayList();
    private ArrayList p_Deck;

    //Player
    [SerializeField] private Transform m_Player = null;

    private int newX = 0;
    private int newY = 1;
    public Vector2 TilePosition;// the player's position in Tiles Unit
    public Vector2 lastTilePosition;// the player's position in Tiles Unit
    public Vector2 PosBeforeEnemySpace;// the player's position in Tiles Unit

    //Player Status
    public bool _isStun = false;
    public float stunCooldown;
    private float stunTimer;

    public int Shield = 0;

    public int _FireStack = 0;
    private const float _FireTick = 0.5f;
    private float _FireTimer = 0.7f;

    public int _PoisonStack = 0;
    private const float _PoisonTick = 0.7f;
    private float _PoisonTimer = 0.7f;
    //

    ///UI ///
    public GameObject retryScreen;

    //Spell text
    [SerializeField] private TextMeshProUGUI txt_SpellA = null;
    [SerializeField] private TextMeshProUGUI txt_SpellB = null;
    private string SpellA;
    private string A_Cost;
    private string SpellB;
    private string B_Cost;

    //Mana
    public float ManaMax = 3f;
    private float LastManaMax;
    public float CurrentMana;
    public float ManaRecuperation = 0.5f;

    [SerializeField] private Image ShuffleIcon;
    [SerializeField] private Image ManaGauge;
    [SerializeField] private Image ManaSplitter;

    //Life 
    [SerializeField] private Image LifeGauge;

    private bool IsCasting;
    private bool IsOnEnnemyTile = false;



    void Start()
    {
        this.ShuffleIcon.gameObject.SetActive(false);
        this.retryScreen.SetActive(false);
        HP = HPMax;

        this.m_Player.position = Position(newX, newY);


        _StartingDeck = new ArrayList(_Library.CreateStartingDeck(_characterName));

        p_Deck = new ArrayList(_StartingDeck);

        // first Spells
        SpellA = GetNewSpell();
        A_Cost = _Library.GetCost(SpellA);

        SpellB = GetNewSpell();
        B_Cost = _Library.GetCost(SpellB);
        //
        CurrentMana = 0f;
        this.lastTilePosition = new Vector2(newX, newY);
        ResetOnce = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_Library._InGame) { return; }
        if (this.retryScreen.activeSelf) { return; }


        LifeGauge.fillAmount = HP / HPMax;
        HPText.text = HP + " / " + HPMax;

        ManaGestion();

        if (!IsCasting && !_isStun)
        {
            Mouvement();
            spellManager();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                weapon();
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
            _PoisonTimer += Time.deltaTime;

            if (_PoisonTimer >= _PoisonTick)
            {
                HP -= _PoisonStack;
                _PoisonTimer = 0;
                _PoisonStack -= 5;
            }
            if (_PoisonStack < 0) 
                _PoisonStack = 0;  
        }

        if (_FireStack != 0)
        {
            if (_FireStack >= 10)
            {
                GetDamaged(40);
                _FireStack -= 10;
            }

            _FireTimer += Time.deltaTime;

            if (_FireTimer >= _FireTick)
            {
                _FireTimer = 0;
                _FireStack--;
            }

            if (_FireStack == 0)
            {
                GetDamaged(10);
            }

            if (_FireStack < 0)
                _FireStack = 0;
        }

        this.TilePosition = new Vector2(newX, newY);
        this.lastTilePosition = TilePosition;
    }

    ////////////////////////// FONCTION ///////////////////////////////////

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

                if(!IsOnEnnemyTile)
                this.m_Player.parent = _TileManager.Tiles[newX][newY];
            }
        }
    }

    private void weapon()
    {
        if (this.CurrentMana < 0.5f) { return; }
        this.CurrentMana -= 0.5f;
        Instantiate(DamageSphere, Position(newX + 1, newY), Quaternion.identity);
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
        CurrentMana += Time.fixedDeltaTime * ManaRecuperation;
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

    public void MouveToEnnemySpace(int x, int y,float Timer)
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

    public Vector3 Position(int Xtile, int Ytile)
    {
        return new Vector3(this._TileManager.Tiles[Xtile][Ytile].position.x, this.m_Player.position.y, this._TileManager.Tiles[Xtile][Ytile].position.z);
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

    private void DeckReset()
    {
        ShuffleIcon.gameObject.SetActive(false);
        ResetOnce = true;
        p_Deck = new ArrayList(_StartingDeck);

        SpellA = GetNewSpell();
        A_Cost = _Library.GetCost(SpellA);
        SpellB = GetNewSpell();
        B_Cost = _Library.GetCost(SpellB);
    }

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
