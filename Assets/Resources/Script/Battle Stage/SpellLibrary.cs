using UnityEngine;

public class SpellLibrary : MonoBehaviour
{
    // Start is called before the first frame update

    public SpellScriptableObject[] _Library;

    private Player _Player;

    [SerializeField] private GameObject DamageSphere = null;
    [SerializeField] private GameObject ProjectileSphere = null;

    public Vector2 VNull { get { return new Vector2(0, 0); } }

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

    public bool cast(string spellToCast, Vector2 Positon)
    {
        if (spellToCast == null) { return false; }

        for (int i = 0; i < _Library.Length; i++)
        {
            if (_Library[i].name == spellToCast)
            {
                if (_Library[i] is ProjectileScriptableObject)
                {
                    return CreateProjectileOnCast((ProjectileScriptableObject)_Library[i], Positon);
                }
                else if (_Library[i] is InstanceScriptableObject)
                {
                    return CreateInstanceOncast((InstanceScriptableObject)_Library[i], Positon);
                }
                return false;
            }
        }

        Debug.LogError("Spell is not in library");
        return false;
    }

    private bool CreateProjectileOnCast(ProjectileScriptableObject CastingProjectile, Vector2 Positon)
    {
        ///BAsic
        int SpellDirection = 1;
        if (Positon == _Player.TilePosition)
        {
            if (this._Player.CurrentMana < CastingProjectile.ManaCost) { return false; }
            this._Player.Iscasting(CastingProjectile.castingTime);
            this._Player.CurrentMana -= CastingProjectile.ManaCost;
        }
        else
        {
            SpellDirection = -1;
        }

        GameObject Projectile = null;
        ///

        for (int y = 0; y < CastingProjectile.p_NbProjectile.y; y++)
        {
            for (int x = 0; x < CastingProjectile.p_NbProjectile.x; x++)
            {
                int xInstance = 0;
                int yInstance = 0;

                if (!CastingProjectile._isStatic)
                {
                    xInstance = (int)(Positon.x + (CastingProjectile.p_StartingPosition.x + x) * SpellDirection);
                    yInstance = (int)(Positon.y + (CastingProjectile.p_StartingPosition.y + y) * SpellDirection);
                }
                else
                {
                    xInstance = (int)((CastingProjectile.p_StartingPosition.x + x) * SpellDirection);
                    yInstance = (int)((CastingProjectile.p_StartingPosition.y + y) * SpellDirection);
                }

                xInstance = Mathf.Clamp(xInstance, 0, 7);

                if (yInstance >= 0 && yInstance <= 3)
                {
                    Projectile = Instantiate(ProjectileSphere, _Player.Position(xInstance, yInstance), Quaternion.identity);
                }

                if (Projectile.TryGetComponent(out ProjectileBehaviour behaviour))
                {
                    behaviour._Speed = CastingProjectile.p_speed * SpellDirection;
                    behaviour._Direction = (ProjectileBehaviour.Direction)CastingProjectile.SpellDirection;
                    behaviour._Damage = CastingProjectile._Damage;
                    behaviour._isPercing = CastingProjectile.p_IsLaser;
                    behaviour._isComingBack = CastingProjectile._isComingBack;
                    behaviour._isFriendly = CastingProjectile._isFriendly;
                }
            }

        }
        /// 

        return true;
    }

    private bool CreateInstanceOncast(InstanceScriptableObject CastingInstance, Vector2 Positon)
    {
        ///Basic
        int SpellDirection = 1;
        if (Positon == _Player.TilePosition)
        {
            if (this._Player.CurrentMana < CastingInstance.ManaCost) { return false; }
            this._Player.Iscasting(CastingInstance.castingTime);
            this._Player.CurrentMana -= CastingInstance.ManaCost;
        }
        else
        {
            SpellDirection = -1;
        }

        GameObject Instant = null;
        ///

        for (int y = 0; y < CastingInstance._NbInstance.y; y++)
        {
            for (int x = 0; x < CastingInstance._NbInstance.x; x++)
            {
                int xInstance = 0;
                int yInstance = 0;

                ///CastingInstance._isStatic; if not static
                if (!CastingInstance._isStatic)
                {
                    xInstance = (int)(Positon.x + (CastingInstance.StartingPosition.x + x) * SpellDirection);
                    yInstance = (int)(Positon.y + (CastingInstance.StartingPosition.y + y) * SpellDirection);
                }
                else
                {
                    xInstance = (int)((CastingInstance.StartingPosition.x + x) * SpellDirection);
                    yInstance = (int)((CastingInstance.StartingPosition.y + y) * SpellDirection);
                }
                ///

                if (xInstance >= 0 && xInstance <= 7
                  && yInstance >= 0 && yInstance <= 3)
                {
                    Instant = GameObject.Instantiate(DamageSphere, _Player.Position(xInstance, yInstance), Quaternion.identity);

                    if (Instant.TryGetComponent(out DamageBehaviour behaviour))
                    {
                        behaviour._Damage = CastingInstance._Damage;
                        behaviour._ActiveFrame = CastingInstance._LifeSpan;
                    }
                }
            }
        }

        return true;
    }

    // For Later
    //private void CreateStructureOncast() 
    //{ 

    //}
}
