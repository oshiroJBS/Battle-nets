using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GUIcanvas : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject charaScreen;
    public GameObject retryScreen;
    public GameObject pauseScreen;
    public GameObject countDownScreen;
    public GameObject optionScreen;

    public GameObject SpellScreen;
    public GameObject LVScreen;
    public GameObject BasicSkillScreen;

    public TextMeshProUGUI[] SkillButtonTXT;
    public Image[] SkillButtonIMG;
    private string[] NewSpellNames = new string[3];

    public TextMeshProUGUI txtMeshCoolDown;

    private Player _player;
    private SpellLibrary _SpellLibrary;

    [HideInInspector] public int tempExp;
    private float playerExp;
    private int playerLV = 1;
    private float ExpBarMax = 5;
    private float LastExpBarMax = 0;
    public Image lvBar;
    public TextMeshProUGUI LVtxt;
    [SerializeField] private Image LevelSplitter;

    private GameObject lastScreen;

    private int _levelNb = 0;
    private int _levelTreshHold = 5;
    private int _difficulty = 1;
    [SerializeField] private GameObject[] EnemyLv1;
    [SerializeField] private GameObject[] EnemyLv2;
    [SerializeField] private GameObject[] EnemyLv3;
    [SerializeField] private GameObject[] EnemyLv4;
    [SerializeField] private GameObject[] EnemyLv5;
    // Start is called before the first frame update

    private bool LevelEnded = true;

    void Start()
    {
        ActiveScreen(titleScreen);

        if (_SpellLibrary == null) _SpellLibrary = GameObject.FindObjectOfType<SpellLibrary>();
        if (_player == null) _player = GameObject.FindObjectOfType<Player>();

        _SpellLibrary._InGame = false;

    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR  //debug

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("/kill");
            foreach (var item in FindObjectsOfType<EnemyBasics>())
            {
                item.HP = 0;
            }
        }
#endif

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("quit");
            Application.Quit();
        }

        //Pause
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (_SpellLibrary._InGame)
            {
            PauseGame();
            }
            else if(pauseScreen.activeInHierarchy)
            {
                UnPauseGame();
            }
        }


        // End Pause

        //End of the level
        if (GameObject.FindObjectOfType<EnemyBasics>() == null && !LevelEnded)
        {
            LevelEnded = true;
            Invoke("GainLevel", 1.5f);
        }
        //

        //EndGame
        if (_player.HP <= 0)
        {
            EndGame();
        }
    }


    public void StartGame()
    {
        _SpellLibrary.RemoveDeckFromLibrary(_player._StartingDeck);
        _difficulty = 1;
        NewLevel();

        _SpellLibrary._InGame = true;
    }

    public void SelectCharacter()
    {
        ActiveScreen(charaScreen);
    }

    private void GainLevel()
    {
        _player.clearStatus();

        _SpellLibrary._InGame = false;
        bool canGainSkill = false;

        for (int i = 0; i < tempExp; i++)
        {
            playerExp++;
            if (playerExp >= ExpBarMax)
            {
                playerLV++;
                playerExp = 0;
                ExpBarMax += 3;
                canGainSkill = true;
            }
        }

        //
        if (LastExpBarMax != ExpBarMax)
        {
            foreach (Transform child in lvBar.transform)
            {
                Destroy(child.gameObject);
            }

            int Iinstance = 1;
            if ((ExpBarMax - 1) % 2 != 0)
            {
                Iinstance = 0;
            }

            for (int i = Iinstance; i < ExpBarMax - 1; i += 2)
            {
                Image Split;

                Split = Instantiate(LevelSplitter, lvBar.transform);
                Split.transform.localPosition = new Vector3(50 * (i / ExpBarMax), 0, 0);

                if (i > 0)
                {
                    Split = Instantiate(LevelSplitter, lvBar.transform);
                    Split.transform.localPosition = new Vector3(50 * -(i / ExpBarMax), 0, 0);
                }
            }
        }
        LastExpBarMax = ExpBarMax;
        //

        tempExp = 0;
        lvBar.fillAmount = playerExp / ExpBarMax;
        LVtxt.text = "Level " + playerLV.ToString();
        ActiveScreen(LVScreen);

        if (canGainSkill)
        {
            BasicSkillScreen.SetActive(true);
        }
        else
        {
            Invoke("CreateSpellPanels", 3f);
        }
    }

    public void GainBasicSkill(int skill)
    {
        switch (skill)
        {
            default:
                break;

            case 0:
                _player.HPMax += 100;
                break;
            case 1:
                _player.HP += 200;
                break;
            case 2:
                _player.WeaponModifier += 5;
                break;
            case 3:
                _player.Defence += 3;
                break;
            case 4:
                _player.ManaRecuperation += 0.1f;
                break;
            case 5:
                _player.ManaMax += 1;
                break;
        }
        BasicSkillScreen.SetActive(false);

        CreateSpellPanels();
    }


    private void NewLevel()
    {
        OffAllScreen();

        _levelNb++;

        if (_difficulty < 5)
        {
            if (_levelNb >= _levelTreshHold)
            {
                _difficulty += 1;
                _levelTreshHold += 5;
            }
        }

        int NbEnnemy;
        switch (_difficulty)
        {
            case 1:
                NbEnnemy = Random.Range(2, 3);

                for (int i = 0; i < NbEnnemy; i++)
                {
                    int randEnnemy = Random.Range(0, EnemyLv1.Length);
                    Instantiate(EnemyLv1[randEnnemy]);
                }

                break;

            case 2:
                NbEnnemy = Random.Range(3, 5);

                for (int i = 0; i < NbEnnemy; i++)
                {
                    int randEnnemy = Random.Range(0, EnemyLv2.Length);
                    Instantiate(EnemyLv1[randEnnemy]);
                }

                break;

            case 3:
                NbEnnemy = Random.Range(4, 5);

                for (int i = 0; i < NbEnnemy; i++)
                {
                    int randEnnemy = Random.Range(0, EnemyLv3.Length);
                    Instantiate(EnemyLv1[randEnnemy]);
                }
                break;

            case 4:
                NbEnnemy = 5;

                for (int i = 0; i < NbEnnemy; i++)
                {
                    int randEnnemy = Random.Range(0, EnemyLv4.Length);
                    Instantiate(EnemyLv1[randEnnemy]);
                }
                break;

            case 5:
                NbEnnemy = Random.Range(4, 5);

                for (int i = 0; i < NbEnnemy; i++)
                {
                    int randEnnemy = Random.Range(0, EnemyLv5.Length);
                    Instantiate(EnemyLv1[randEnnemy]);
                }
                break;
        }
        LevelEnded = false;
    }

    private void CreateSpellPanels()
    {
        int FirstSpell = Random.Range(0, _SpellLibrary._SpellAvailable.Count - 1);

        string SpellName = ((SpellScriptableObject)_SpellLibrary._SpellAvailable[FirstSpell]).name;
        SkillButtonTXT[0].text = SpellName;
        NewSpellNames[0] = SpellName;
        SkillButtonIMG[0].sprite = _SpellLibrary.GetIcone(SpellName);


    GetSecond: int SecondSpell = Random.Range(0, _SpellLibrary._SpellAvailable.Count);

        if (SecondSpell == FirstSpell)
            goto GetSecond;

        SpellName = ((SpellScriptableObject)_SpellLibrary._SpellAvailable[SecondSpell]).name;
        SkillButtonTXT[1].text = SpellName;
        NewSpellNames[1] = SpellName;
        SkillButtonIMG[1].sprite = _SpellLibrary.GetIcone(SpellName);

    GetThird: int thirdSpell = Random.Range(0, _SpellLibrary._SpellAvailable.Count);

        if (thirdSpell == SecondSpell || thirdSpell == FirstSpell)
            goto GetThird;

        SpellName = ((SpellScriptableObject)_SpellLibrary._SpellAvailable[thirdSpell]).name;
        SkillButtonTXT[2].text = SpellName;
        NewSpellNames[2] = SpellName;
        SkillButtonIMG[2].sprite = _SpellLibrary.GetIcone(SpellName);

        ActiveScreen(SpellScreen);
    }

    public void GainNewSpell(int ButtonNB)
    {
        _SpellLibrary.AddSpellToPlayerDeck(NewSpellNames[ButtonNB]);
        NewLevel();
        _SpellLibrary._InGame = true;
        _player.DeckReset();
    }

    #region Screens


    public void EndGame()
    {
        ActiveScreen(retryScreen);
        _SpellLibrary._InGame = false;
    }

    public void Option()
    {
        OffAllScreen();
        optionScreen.SetActive(true);
    }

    public void PauseGame()
    {
        ActiveScreen(pauseScreen);
        _SpellLibrary._InGame = false;

        Time.timeScale = 0;
    }

    public void UnPauseGame()
    {
        OffAllScreen();
        Time.timeScale = 1;
        _SpellLibrary._InGame = true;

    }

    public void Return()
    {
        ActiveScreen(lastScreen);
    }

    private void ActiveScreen(GameObject Screen)
    {
        OffAllScreen();
        Screen.SetActive(true);
        lastScreen = Screen;
    }

    private void OffAllScreen()
    {
        titleScreen.SetActive(false);
        charaScreen.SetActive(false);
        retryScreen.SetActive(false);
        pauseScreen.SetActive(false);
        countDownScreen.SetActive(false);
        optionScreen.SetActive(false);
        SpellScreen.SetActive(false);
        LVScreen.SetActive(false);
        BasicSkillScreen.SetActive(false);
    }

    #endregion
}
