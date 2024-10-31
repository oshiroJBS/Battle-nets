using TMPro;
using UnityEngine;

public class obstacleBehaviour : MonoBehaviour
{
    private Player _Player;
    [SerializeField] private TilesManager _TileManager;

    public float HPMax = 100;
    [HideInInspector] public float HP;
    public TextMeshProUGUI HPDisplay;

    private int _TileX = 0;
    private int _TileY = 0;

    private void Awake()
    {
        //initialization
        HP = HPMax;
        if (_Player == null) _Player = GameObject.FindObjectOfType<Player>();
        if (_TileManager == null) _TileManager = GameObject.FindObjectOfType<TilesManager>();
    }
    private void Start()
    {

        this.transform.parent = _TileManager.Tiles[_TileX][_TileY];
        //

        this._TileX = Random.Range(0, 7);
        this._TileY = Random.Range(0, 3);

        //new starting point if occupied
        while (_TileManager.Tiles[_TileX][_TileY].childCount > 1)
        {
            this._TileX = Random.Range(0, 7);
            this._TileY = Random.Range(0, 3);
        }
        //

        this.transform.parent = _TileManager.Tiles[_TileX][_TileY];
        this.transform.position = Position(_TileX, _TileY);
    }

    private void Update()
    {
        HP = Mathf.Clamp(HP, 0, HPMax);

        HPDisplay.text = HP.ToString();

        if (this.HP <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private Vector3 Position(int Xtile, int Ytile)
    {
        return new Vector3(this._TileManager.Tiles[Xtile][Ytile].position.x, 0, this._TileManager.Tiles[Xtile][Ytile].position.z);
    }

}
