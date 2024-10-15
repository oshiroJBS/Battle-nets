using UnityEngine;

public class TilesManager : MonoBehaviour
{

    //Player's Rows
    [SerializeField] private Transform[] Row1 = null;
    [SerializeField] private Transform[] Row2 = null;
    [SerializeField] private Transform[] Row3 = null;
    [SerializeField] private Transform[] Row4 = null;

    //ennemy's Rows
    [SerializeField] private Transform[] Row5 = null;
    [SerializeField] private Transform[] Row6 = null;
    [SerializeField] private Transform[] Row7 = null;
    [SerializeField] private Transform[] Row8 = null;


    public Transform[][] Tiles = new Transform[8][];

    // Start is called before the first frame update
    void Awake()
    {
        Tiles[0] = Row1;
        Tiles[1] = Row2;
        Tiles[2] = Row3;
        Tiles[3] = Row4;

        Tiles[4] = Row5;
        Tiles[5] = Row6;
        Tiles[6] = Row7;
        Tiles[7] = Row8;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
