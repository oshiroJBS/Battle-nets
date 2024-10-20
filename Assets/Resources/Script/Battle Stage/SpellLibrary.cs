using Unity.VisualScripting;
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



        for (int y = 0; y < CastingProjectile.p_NbProjectile.y; y++)
        {
            for (int x = 0; x < CastingProjectile.p_NbProjectile.x; x++)
            {
                int xInstance = 0;
                int yInstance = 0;

                if (!CastingProjectile._isStatic)
                {
                   xInstance = (int)(_Player.TilePosition.x + CastingProjectile.p_StartingPosition.x + x);
                   yInstance = (int)(_Player.TilePosition.y + CastingProjectile.p_StartingPosition.y + y);
                }
                else
                {
                  xInstance = (int)(CastingProjectile.p_StartingPosition.x + x);
                  yInstance = (int)(CastingProjectile.p_StartingPosition.y + y);
                }

                xInstance = Mathf.Clamp(xInstance, 0, 7);

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
                    behaviour._isComingBack = CastingProjectile._isComingBack;
                    behaviour._isFriendly = CastingProjectile._isFriendly;
                }
            }

        }
        /// 

        this._Player.CurrentMana -= CastingProjectile.ManaCost;
        return true;
    }

    private bool CreateInstanceOncast(InstanceScriptableObject CastingInstance, bool isEnnemi)
    {
        if (this._Player.CurrentMana < CastingInstance.ManaCost) { return false; }
        this._Player.Iscasting(CastingInstance.castingTime);
        GameObject Instant = null;


        /////CastingInstance._isStatic; if not static
        for (int y = 0; y < CastingInstance._NbInstance.y; y++)
        {
            for (int x = 0; x < CastingInstance._NbInstance.x; x++)
            {
                int xInstance = 0;
                int yInstance = 0;

                if (!CastingInstance._isStatic)
                {
                    xInstance = (int)(_Player.TilePosition.x + CastingInstance.StartingPosition.x + x);
                    yInstance = (int)(_Player.TilePosition.y + CastingInstance.StartingPosition.y + y);
                }
                else
                {
                   xInstance = (int)(CastingInstance.StartingPosition.x + x);
                   yInstance = (int)(CastingInstance.StartingPosition.y + y);
                }

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
                ///
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
