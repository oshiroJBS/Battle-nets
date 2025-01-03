using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIcanvas : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject retryScreen;
    public GameObject pauseScreen;
    public GameObject countDownScreen;
    public GameObject optionScreen;

    public GameObject SkillScreen;

    public TextMeshProUGUI[] SkillButtonTXT;
    public Image[] SkillButtonIMG;
    private string[] NewSpellNames = new string[3];

    public TextMeshProUGUI txtMeshCoolDown;

    private Player _player;
    private SpellLibrary _SpellLibrary;

    private bool inCooldown = false;
    private float cooldown = 3;
    private string cooldowntxt;
    private float timer = 0;
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


    void Start()
    {
        ActiveScreen(titleScreen);

        if (_SpellLibrary == null) _SpellLibrary = GameObject.FindObjectOfType<SpellLibrary>();
        if (_player == null) _player = GameObject.FindObjectOfType<Player>();

        _SpellLibrary._InGame = false;
        timer = 0;
        cooldown = 3;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("quit");
            Application.Quit();
        }

        //Pause
        if (Input.GetKeyDown(KeyCode.P) && _SpellLibrary._InGame)
        {
            PauseGame();
        }

        if (inCooldown)
        {
            timer += Time.deltaTime;
            cooldowntxt = ((int)(cooldown - timer) + 1).ToString();
            txtMeshCoolDown.text = cooldowntxt;
            if (timer >= cooldown)
            {
                UnPauseGame();
            }
        }
        // End Pause

        //level
        if (GameObject.FindObjectOfType<EnemyBasics>() == null && _SpellLibrary._InGame)
        {
            _SpellLibrary._InGame = false;
            CreateSkillPanels();
            SkillScreen.SetActive(true);
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
        _difficulty = 1;
        NewLevel();

        _SpellLibrary._InGame = true;
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
                NbEnnemy = Random.Range(1, 3);

                for (int i = 0; i < NbEnnemy; i++)
                {
                    int randEnnemy = Random.Range(0, EnemyLv1.Length);
                    Instantiate(EnemyLv1[randEnnemy]);
                }

                break;

            case 2:
                NbEnnemy = Random.Range(2, 4);

                for (int i = 0; i < NbEnnemy; i++)
                {
                    int randEnnemy = Random.Range(0, EnemyLv2.Length);
                    Instantiate(EnemyLv1[randEnnemy]);
                }

                break;

            case 3:
                NbEnnemy = Random.Range(2, 5);

                for (int i = 0; i < NbEnnemy; i++)
                {
                    int randEnnemy = Random.Range(0, EnemyLv3.Length);
                    Instantiate(EnemyLv1[randEnnemy]);
                }
                break;

            case 4:
                NbEnnemy = Random.Range(3, 5);

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
    }

    private void CreateSkillPanels()
    {
        int LastSpell = 300;
        int RandSpell = Random.Range(0, _SpellLibrary._SpellAvailable.Count);

        for (int i = 0; i < SkillButtonTXT.Length; i++)
        {
            while (RandSpell == LastSpell)
            {
                RandSpell = Random.Range(0, _SpellLibrary._SpellAvailable.Count - 1);
            }

            string SpellName = ((SpellScriptableObject)_SpellLibrary._SpellAvailable[RandSpell]).name;
            Debug.Log(SpellName);
            SkillButtonTXT[i].text = SpellName;
            NewSpellNames[i] = SpellName;
            SkillButtonIMG[i].sprite = _SpellLibrary.GetIcone(SpellName);

            LastSpell = RandSpell;
        }
    }

    public void GainSkill(int ButtonNB)
    {
        _SpellLibrary.AddSpellToPlayerDeck(NewSpellNames[ButtonNB]);
        NewLevel();
        _SpellLibrary._InGame = true;
    }

    #region Screen


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
    }

    public void StartCooldown()
    {
        ActiveScreen(countDownScreen);
        inCooldown = true;
    }

    private void UnPauseGame()
    {
        OffAllScreen();
        _SpellLibrary._InGame = true;

        inCooldown = false;
        timer = 0;
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
        retryScreen.SetActive(false);
        pauseScreen.SetActive(false);
        countDownScreen.SetActive(false);
        optionScreen.SetActive(false);
        SkillScreen.SetActive(false);
    }

    #endregion
}
