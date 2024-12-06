using System.Collections;
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

    public bool cast(string spellToCast, Vector2 Positon, bool Enemy = false)
    {
        if (spellToCast == null) { return false; }

        for (int i = 0; i < _Library.Length; i++)
        {
            if (_Library[i].name == spellToCast)
            {
                if (_Library[i] is ProjectileScriptableObject)
                {
                    ProjectileScriptableObject pso = (ProjectileScriptableObject)_Library[i];
                    StartCoroutine(CastProjectile(pso, Positon, Enemy));
                    return true;
                }
                else if (_Library[i] is InstanceScriptableObject)
                {
                    InstanceScriptableObject iso = (InstanceScriptableObject)_Library[i];
                    StartCoroutine(CastInstance(iso, Positon, Enemy));
                    return true;
                }
                return false;
            }
        }

        Debug.LogError("Spell is not in library");
        return false;
    }

    IEnumerator CastProjectile(ProjectileScriptableObject pso, Vector2 CastingPositon, bool enemy)
    {
        bool first = true;
        Vector2 Positon = CastingPositon;

        if (!enemy)
        {
            if (this._Player.CurrentMana < pso.ManaCost) { yield break; }
            this._Player.Iscasting(pso.castingTime);
            this._Player.CurrentMana -= pso.ManaCost;
        }

        for (int i = 0; i < pso.nbWave; i++)
        {
            if (!pso.isWaveStatic && !enemy)
            {
                Positon = _Player.TilePosition;
            }

            if (first && pso.fstWaveCooldown)
            {
                first = false;
                yield return new WaitForSeconds(pso.waveCooldown);
            }
            CreateProjectileOnCast(pso, Positon, enemy);
 
            yield return new WaitForSeconds(pso.waveCooldown);
        }
        StopCoroutine("CastProjectile");

        //return returnValue;
    }

    IEnumerator CastInstance(InstanceScriptableObject iso, Vector2 CastingPositon, bool enemy)
    {
        bool first = true;
        Vector2 Positon = CastingPositon;

        if (!enemy)
        {
            if (this._Player.CurrentMana < iso.ManaCost) { yield break; }
            this._Player.Iscasting(iso.castingTime);
            this._Player.CurrentMana -= iso.ManaCost;
        }

        for (int i = 0; i < iso.nbWave; i++)
        {

            if (!iso.isWaveStatic && !enemy)
            {
                Positon = _Player.TilePosition;
            }

            if (first && iso.fstWaveCooldown)
            {
                first = false;
                yield return new WaitForSeconds(iso.waveCooldown);
            }

            CreateInstanceOncast(iso, Positon, enemy);

            yield return new WaitForSeconds(iso.waveCooldown);
        }
        StopCoroutine("CastInstance");

        //return returnValue;
    }

    private bool CreateProjectileOnCast(ProjectileScriptableObject CastingProjectile, Vector2 Positon, bool enemy)
    {
        ///BAsic
        int SpellDirection = 1;

        if (enemy)
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

    private bool CreateInstanceOncast(InstanceScriptableObject CastingInstance, Vector2 Positon, bool enemy)
    {
        ///Basic
        int SpellDirection = 1;

        if (enemy)
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
                        behaviour._isFriendly = CastingInstance._isFriendly;
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
