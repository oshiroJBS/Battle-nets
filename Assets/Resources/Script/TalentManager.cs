using UnityEngine;

public class TalentManager : MonoBehaviour
{
    // Start is called before the first frame update
    private Player _Player;

    private float _DamageMultiplier = 1;
    public float _DamageModifier = 0;

    private bool _kouPassif = false;
    private bool _ImOnFire = false;

    public float _BurnModifier = 1;
    public float _PoisonModifier = 1;


    void Start()
    {
        if (_Player == null) _Player = GameObject.FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public float GetModifier()
    {


        return _DamageModifier;
    }

    public float GetMultiplier()
    {
        _DamageMultiplier = 1;

        if (_ImOnFire)
        {
            _DamageMultiplier += 0.2f;
        }

        return _DamageMultiplier;
    }


    ////////////////////////////////////// Activate Passif /////////////////////////////////

    public void ActivateKouPAssif()
    {
        _kouPassif = true;
    }
    public void ActivateImOnFire()
    {
        _ImOnFire = true;
    }
}
