using UnityEngine;

public class SpellLibrary : MonoBehaviour
{
    // Start is called before the first frame update

    public SpellScriptableObject[] _Library;

    private Player _Player;

    [SerializeField] private GameObject DamageSphere = null;
    [SerializeField] private GameObject ProjectileSphere = null;

    private void Start()
    {
        if (_Player == null) _Player = GameObject.FindObjectOfType<Player>();
    }

    // Update is called once per frame
    public string GetCost(string SpellName)
    {
        int _cost = 0;
        for (int i = 0; i < _Library.Length; i++)
        {
            if (_Library[i].name == SpellName)
            {
                if (_Library[i] is ProjectileScriptableObject)
                {
                    _cost = ((ProjectileScriptableObject)_Library[i]).ManaCost;
                    return "(" + _cost + ")";
                }
                else if (_Library[i] is InstanceScriptableObject)
                {
                    _cost = ((InstanceScriptableObject)_Library[i]).ManaCost;
                    return "(" + _cost + ")";
                }
            }
        }
        return "";
    }

    public bool cast(string spellToCast, bool isEnnemi = false)
    {
        if (spellToCast == null) { return false; }

        for (int i = 0; i < _Library.Length; i++)
        {
            if (_Library[i].name == spellToCast)
            {
                if (_Library[i] is ProjectileScriptableObject)
                {
                    return CreateProjectileOnCast((ProjectileScriptableObject)_Library[i], isEnnemi);
                }
                else if (_Library[i] is InstanceScriptableObject)
                {
                    return CreateInstanceOncast((InstanceScriptableObject)_Library[i], isEnnemi);
                }
                return false;
            }
        }
        Debug.LogError("Spell is not in library");
        return false;
    }

    private bool CreateProjectileOnCast(ProjectileScriptableObject CastingProjectile, bool isEnnemi)
    {
        if (this._Player.CurrentMana < CastingProjectile.ManaCost) { return false; }

        this._Player.Iscasting(CastingProjectile.castingTime);

        GameObject Projectile = null;

        int xInstance = (int)(_Player.TilePosition.x + CastingProjectile.p_StartingPosition.x);
        int yInstance = (int)(_Player.TilePosition.y + CastingProjectile.p_StartingPosition.y);

        xInstance = Mathf.Clamp(xInstance, 0, 7);

        for (int i = 0; i < CastingProjectile.p_NbProjectile; i++)
        {
            if (yInstance >= 0 && yInstance <= 3)
            {
                Projectile = Instantiate(ProjectileSphere, _Player.Position(xInstance, yInstance), Quaternion.identity);
            }

            if (Projectile.TryGetComponent(out ProjectileBehaviour behaviour))
            {
                behaviour._Speed = CastingProjectile.p_speed;
                behaviour._Direction = (ProjectileBehaviour.Direction)CastingProjectile.SpellDirection;
                behaviour._Damage = CastingProjectile._Damage;
                behaviour._isPercing = CastingProjectile.p_IsLaser;
            }
        }

        this._Player.CurrentMana -= CastingProjectile.ManaCost;
        return true;
    }

    private bool CreateInstanceOncast(InstanceScriptableObject CastingInstance, bool isEnnemi)
    {
        if (this._Player.CurrentMana < CastingInstance.ManaCost) { return false; }
        this._Player.Iscasting(CastingInstance.castingTime);
        GameObject Instant = null;

        for (int y = 0; y < CastingInstance._NbInstance.y; y++)
        {
            for (int x = 0; x < CastingInstance._NbInstance.x; x++)
            {
                int xInstance = (int)(_Player.TilePosition.x + CastingInstance.StartingPosition.x + x);
                int yInstance = (int)(_Player.TilePosition.y + CastingInstance.StartingPosition.y + y);

                if (xInstance >= 0 && xInstance <= 7
                  && yInstance >= 0 && yInstance <= 3)
                {
                    Instant = GameObject.Instantiate(DamageSphere, _Player.Position((int)(_Player.TilePosition.x + CastingInstance.StartingPosition.x + x), (int)(_Player.TilePosition.y + CastingInstance.StartingPosition.y + y)), Quaternion.identity);

                    if (Instant.TryGetComponent(out DamageBehaviour behaviour))
                    {
                        behaviour._Damage = CastingInstance._Damage;
                        behaviour._ActiveFrame = CastingInstance._LifeSpan;
                    }
                }
            }
        }
        this._Player.CurrentMana -= CastingInstance.ManaCost;
        return true;
    }

    // For Later
    //private void CreateStructureOncast() 
    //{ 

    //}
}
