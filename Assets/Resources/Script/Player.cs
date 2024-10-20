using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
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
    public float LastManaMax;
    public float CurrentMana;
    public float ManaRecuperation = 0.5f;
    [SerializeField] private Image ShuffleIcon;
    [SerializeField] private Image ManaGauge;
    [SerializeField] private Image ManaSplitter;

    //Life 
    [SerializeField] private Image LifeGauge;

    private bool IsCasting;

    void Start()
    {
        this.ShuffleIcon.gameObject.SetActive(false);
        this.retryScreen.SetActive(false);
        HP = HPMax;

        this.m_Player.position = Position(newX, newY);

        _StartingDeck.Add("Discharge");
        _StartingDeck.Add("Alo");
        _StartingDeck.Add("Cut");
        _StartingDeck.Add("Lightning");

        p_Deck = new ArrayList(_StartingDeck);

        // firts Spells
        SpellA = GetNewSpell();
        A_Cost = _Library.GetCost(SpellA);

        SpellB = GetNewSpell();
        B_Cost = _Library.GetCost(SpellB);
        //
        CurrentMana = 0f;

        ResetOnce = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Reload Scene

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("GameScene");
        }

        LifeGauge.fillAmount = HP / HPMax;
        HPText.text = HP + " / " + HPMax;

        if (this.HP <= 0)
        {
            if (!this.retryScreen.activeSelf)
            {
                ActivateRetryScreen();
            }
            return; // is Player Dead ?
        }
        if (this.retryScreen.activeSelf) { return; }

        ManaGestion();

        if (!IsCasting)
        {
            Mouvement();
            spellManager();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                weapon();
            }
        }

        this.TilePosition = new Vector2(newX, newY);
        this.m_Player.parent = _TileManager.Tiles[newX][newY];
    }

    ////////////////////// FONCTION ///////////////////////////////////

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
        this.m_Player.position = Position(newX, newY);
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
            if (_Library.cast(SpellA))
            {
                SpellA = GetNewSpell();
                A_Cost = _Library.GetCost(SpellA);
            }

            if (SpellA != null)
            {
                Debug.Log("Spell A :" + SpellA.ToString());
                Debug.Log(A_Cost);
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_Library.cast(SpellB))
            {
                SpellB = GetNewSpell();
                B_Cost = _Library.GetCost(SpellB);
            }

            if (SpellB != null)
            {
                Debug.Log("Spell B :" + SpellB.ToString());
                Debug.Log(B_Cost);
            }
        }

        if (p_Deck.Count <= 0 && SpellA == null && SpellB == null && ResetOnce)
        {
            ResetOnce = false;
            Invoke("DeckReset", resetTimer);

            ShuffleIcon.gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.V) && ResetOnce)
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

            ToInitialize = (string)p_Deck[SpellID];

            Debug.Log("New spell slot : " + ToInitialize);
            p_Deck.Remove(p_Deck[SpellID]);
        }

        return ToInitialize;
    }

    public void ActivateRetryScreen()
    {
        this.retryScreen.SetActive(true);
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
}
