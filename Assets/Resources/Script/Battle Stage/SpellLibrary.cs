using System.Collections;
using UnityEngine;

public class SpellLibrary : MonoBehaviour
{
    // Start is called before the first frame update

    public SpellScriptableObject[] _Library;
    public ArrayList _SpellAvailable;

    private Player _Player;

    [SerializeField] private GameObject DamageSphere = null;
    [SerializeField] private GameObject ProjectileSphere = null;

    public bool _InGame = false;
    //public Vector2 VNull { get { return new Vector2(0, 0); } }


    private void Awake()
    {
        _SpellAvailable = new ArrayList();
        for (int i = 0; i < _Library.Length; i++)
        {
            _SpellAvailable.Add(_Library[i]);
        }
    }

    private void Start()
    {
        if (_Player == null) _Player = GameObject.FindObjectOfType<Player>();
    }


    #region Casting Stuff
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

                    if (!Enemy && pso.MoveToEnnemyGridForTime != 0) _Player.MouveToEnnemySpace((int)pso._TpOnCast.x, (int)pso._TpOnCast.y, pso.MoveToEnnemyGridForTime);
                    if (!Enemy && pso._TpOnCast != Vector2.zero) _Player.ForcedMovement((int)pso._TpOnCast.x, (int)pso._TpOnCast.y);

                    StartCoroutine(CastProjectile(pso, Positon, Enemy));

                    if (pso.RandomTPonCAst)
                    {
                        int randx = UnityEngine.Random.Range(0, 3);
                        int randy = UnityEngine.Random.Range(0, 3);
                        _Player.ForcedMovement(randx, randy);
                    }

                    return true;
                }
                else if (_Library[i] is InstanceScriptableObject)
                {
                    InstanceScriptableObject iso = (InstanceScriptableObject)_Library[i];

                    if (!Enemy && iso.MoveToEnnemyGridForTime != 0) _Player.MouveToEnnemySpace((int)iso._TpOnCast.x, (int)iso._TpOnCast.y, iso.MoveToEnnemyGridForTime);
                    else if (!Enemy && iso._TpOnCast != Vector2.zero) _Player.ForcedMovement((int)iso._TpOnCast.x, (int)iso._TpOnCast.y);

                    StartCoroutine(CastInstance(iso, Positon, Enemy));

                    if (iso.RandomTPonCAst)
                    {
                        int randx = UnityEngine.Random.Range(0, 3);
                        int randy = UnityEngine.Random.Range(0, 3);
                        _Player.ForcedMovement(randx, randy);
                    }

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

        Debug.Log("casting");
        ///BAsic
        int SpellDirection = 1;

        if (enemy)
        {
            SpellDirection = -1;
        }

        GameObject Projectile = null;
        ///

        for (int y = 0; y < CastingProjectile._NbObject.y; y++)
        {
            for (int x = 0; x < CastingProjectile._NbObject.x; x++)
            {
                int xInstance = 0;
                int yInstance = 0;

                if (!CastingProjectile._isStatic)
                {
                    xInstance = (int)(Positon.x + (CastingProjectile._StartingPosition.x + x) * SpellDirection);
                    yInstance = (int)(Positon.y + (CastingProjectile._StartingPosition.y + y) * SpellDirection);
                }
                else
                {
                    xInstance = (int)((CastingProjectile._StartingPosition.x + x) * SpellDirection);
                    yInstance = (int)((CastingProjectile._StartingPosition.y + y) * SpellDirection);
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
                    behaviour._isPercing = CastingProjectile.p_IsLaser;
                    behaviour._isComingBack = CastingProjectile._isComingBack;
                    behaviour._ForcedMouvement = CastingProjectile._ForcedMouvement;
                    behaviour.StunTime = CastingProjectile.StunTime;
                    behaviour.Charm = CastingProjectile._Charm;
                    behaviour.RandomTPonHit = CastingProjectile.RandomTPonHit;
                    behaviour._FireStack = CastingProjectile._FireStack;
                    behaviour._PoisonStack = CastingProjectile._PoisonStack;

                    if (enemy)
                    {
                        behaviour._isFriendly = false;
                        behaviour._Damage = CastingProjectile._Damage;
                    }
                    else
                    {
                        behaviour._isFriendly = CastingProjectile._isFriendly;
                        behaviour._Damage = CastingProjectile._Damage;
                    }
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

        for (int y = 0; y < CastingInstance._NbObject.y; y++)
        {
            for (int x = 0; x < CastingInstance._NbObject.x; x++)
            {
                int xInstance = 0;
                int yInstance = 0;

                ///CastingInstance._isStatic; if not static
                if (!CastingInstance._isStatic)
                {
                    xInstance = (int)(Positon.x + (CastingInstance._StartingPosition.x + x) * SpellDirection);
                    yInstance = (int)(Positon.y + (CastingInstance._StartingPosition.y + y) * SpellDirection);
                }
                else
                {
                    xInstance = (int)((CastingInstance._StartingPosition.x + x) * SpellDirection);
                    yInstance = (int)((CastingInstance._StartingPosition.y + y) * SpellDirection);
                }
                ///

                if (xInstance >= 0 && xInstance <= 7
                  && yInstance >= 0 && yInstance <= 3)
                {
                    Instant = Instantiate(DamageSphere, _Player.Position(xInstance, yInstance), Quaternion.identity);

                    if (Instant.TryGetComponent(out DamageBehaviour behaviour))
                    {
                        behaviour._Damage = CastingInstance._Damage;
                        behaviour._ActiveFrame = CastingInstance._LifeSpan;
                        behaviour._ForcedMouvement = CastingInstance._ForcedMouvement;
                        behaviour.StunTime = CastingInstance.StunTime;
                        behaviour.Charm = CastingInstance._Charm;
                        behaviour.RandomTPonHit = CastingInstance.RandomTPonHit;
                        behaviour._FireStack = CastingInstance._FireStack;
                        behaviour._PoisonStack = CastingInstance._PoisonStack;

                        if (enemy)
                            behaviour._isFriendly = false;
                        else
                            behaviour._isFriendly = CastingInstance._isFriendly;

                    }
                }
            }
        }

        return true;
    }
    #endregion


    public ArrayList CreateStartingDeck(string charName)
    {
        ArrayList NameList = new ArrayList();

        for (int i = 0; i < _Library.Length; i++)
        {
            if (_Library[i].CharacterDeckStarter == charName)
            {
                NameList.Add(_Library[i]);

                _SpellAvailable.Remove(_Library[i]);
            }
        }
        Debug.Log(_SpellAvailable.Count);

        return NameList;
    }

    public void AddSpellToPlayerDeck(string spellName)
    {
        for (int i = 0; i < _Library.Length; i++)
        {
            if (_Library[i].name == spellName)
            {
                _Player._StartingDeck.Add(_Library[i]);
            }
        }
    }
    /////////////////////////////// UI/////////////////////////////
    public Sprite GetIcone(string SpellName)
    {
        for (int i = 0; i < _Library.Length; i++)
        {
            if (_Library[i].name == SpellName)
                return _Library[i].Icon;
        }
        return null;
    }

    // For Later
    //private void CreateStructureOncast() 
    //{ 

    //}
}
