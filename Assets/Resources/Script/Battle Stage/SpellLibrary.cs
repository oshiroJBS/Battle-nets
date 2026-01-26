using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpellLibrary : MonoBehaviour
{
    // Start is called before the first frame update

    private Dictionary<string, SpellScriptableObject> dic_SpellTable = new Dictionary<string, SpellScriptableObject>();

    public SpellScriptableObject[] _Library;
    public ArrayList _SpellAvailable;

    private Player _Player;
    private TalentManager _Manager;

    [SerializeField] private GameObject DamageSphere = null;
    [SerializeField] private GameObject ProjectileSphere = null;

    public bool _InGame = false;
    //public Vector2 VNull { get { return new Vector2(0, 0); } }


    private void Awake()
    {
        _SpellAvailable = new ArrayList();

        foreach (SpellScriptableObject SpellBF in _Library)
        {
            dic_SpellTable[SpellBF.name] = SpellBF;
            _SpellAvailable.Add(SpellBF);
        }
        _Library = new SpellScriptableObject[0];
        Debug.Log(dic_SpellTable.Count);
    }

    private void Start()
    {
        if (_Player == null) _Player = GameObject.FindObjectOfType<Player>();
        if (_Manager == null) _Manager = GameObject.FindObjectOfType<TalentManager>();

        Debug.Log("Spell Count : " + dic_SpellTable.Count);
        Debug.Log(_Manager.GetDamageMultiplier());
    }


    #region Casting Stuff
    public string GetCost(string SpellName)
    {
        int _cost = 0;
        if (dic_SpellTable[SpellName] != null)
        {
            if (dic_SpellTable[SpellName] is ProjectileScriptableObject)
            {
                _cost = ((ProjectileScriptableObject)dic_SpellTable[SpellName]).ManaCost;
                return "(" + _cost + ")";
            }
            else if (dic_SpellTable[SpellName] is InstanceScriptableObject)
            {
                _cost = ((InstanceScriptableObject)dic_SpellTable[SpellName]).ManaCost;
                return "(" + _cost + ")";
            }
        }
        return "";
    }

    public bool cast(string spellToCast, Vector2 Positon, bool Enemy = false)
    {
        if (spellToCast == null) { return false; }
        SpellScriptableObject BFspell = dic_SpellTable[spellToCast];

        if (BFspell.ManaCost > _Player.CurrentMana)
            return false;

        if (BFspell is ProjectileScriptableObject)
        {
            ProjectileScriptableObject pso = (ProjectileScriptableObject)BFspell;

            if (!Enemy && pso.MoveToEnnemyGridForTime != 0) _Player.MouveToEnnemySpace((int)pso._TpOnCast.x, (int)pso._TpOnCast.y, pso.MoveToEnnemyGridForTime);
            else if (!Enemy && pso._TpOnCast != Vector2.zero) _Player.ForcedMovement((int)pso._TpOnCast.x, (int)pso._TpOnCast.y);

            StartCoroutine(CastProjectile(pso, Positon, Enemy));

            if (pso.RandomTpOnCast)
            {
                int randx = Random.Range(0, 3);
                int randy = Random.Range(0, 3);
                _Player.Teleport(randx, randy);
            }
            else if (pso.TpOnOpposite && !Enemy)
            {
                _Player.Teleport(Mathf.Abs((int)_Player.TilePosition.x - 3), Mathf.Abs((int)_Player.TilePosition.y - 3));
            }

            return true;
        }
        else if (BFspell is InstanceScriptableObject)
        {
            InstanceScriptableObject iso = (InstanceScriptableObject)BFspell;

            if (!Enemy && iso.MoveToEnnemyGridForTime != 0) _Player.MouveToEnnemySpace((int)iso._TpOnCast.x, (int)iso._TpOnCast.y, iso.MoveToEnnemyGridForTime);
            else if (!Enemy && iso._TpOnCast != Vector2.zero) _Player.ForcedMovement((int)iso._TpOnCast.x, (int)iso._TpOnCast.y);

            StartCoroutine(CastInstance(iso, Positon, Enemy));

            if (iso.RandomTpOnCast)
            {
                int randx = Random.Range(0, 3);
                int randy = Random.Range(0, 3);
                _Player.ForcedMovement(randx, randy);
            }

            return true;
        }

        Debug.LogError("Spell is not in library");
        return false;
    }

    IEnumerator CastProjectile(ProjectileScriptableObject pso, Vector2 CastingPositon, bool enemy)
    {
        bool first = true;
        Vector2 _Position = CastingPositon;

        if (!enemy)
        {
            this._Player.Iscasting(pso.castingTime);
            this._Player.CurrentMana -= pso.ManaCost;
        }


        if (pso._aimed && enemy)
        {
            _Position.y = _Player.TilePosition.y;
        }
        else if (pso._OppositeTile)
        {
            if (CastingPositon.x > 3 || enemy)
                _Position.x = 0;
            else
            {
                _Position.x = 7;
            }

            _Position.y = 3 - CastingPositon.y;
        }

        for (int i = 0; i < pso.nbWave; i++)
        {
            if (!pso.isWaveStatic && !enemy)
            {
                _Position = _Player.TilePosition;

                if (pso._OppositeTile)
                {
                    if (CastingPositon.x > 3)
                        _Position.x = 0;
                    else
                    {
                        _Position.x = 7;
                        enemy = true;
                    }

                    _Position.y = 3 - _Player.TilePosition.y;
                }
            }

            if (!pso.isWaveStatic && enemy)
            {
                if (pso._aimed)
                {
                    _Position.y = _Player.TilePosition.y;
                }
            }

            if (first && pso.fstWaveCooldown)
            {
                first = false;
                yield return new WaitForSeconds(pso.waveCooldown);
            }

            CreateProjectileOnCast(pso, _Position, enemy);

            yield return new WaitForSeconds(pso.waveCooldown);
        }
        StopCoroutine("CastProjectile");

        //return returnValue;
    }

    IEnumerator CastInstance(InstanceScriptableObject iso, Vector2 CastingPositon, bool enemy)
    {
        bool first = true;

        Vector2 _Position = CastingPositon;

        if (!enemy)
        {
            this._Player.Iscasting(iso.castingTime);
            this._Player.CurrentMana -= iso.ManaCost;
        }

        else if (iso._aimed && enemy)
        {
            _Position.x = _Player.TilePosition.x;
            _Position.y = _Player.TilePosition.y;
        }
        else if (iso._OppositeTile)
        {
            _Position.x = 7 - CastingPositon.x;
            _Position.y = 3 - CastingPositon.y;
        }


        for (int i = 0; i < iso.nbWave; i++)
        {
            if (!iso.isWaveStatic)
            {
                if (!enemy)
                {
                    _Position = _Player.TilePosition;

                    if (iso._OppositeTile)
                    {
                        _Position.x = 7 - _Player.TilePosition.x;
                        _Position.y = 3 - _Player.TilePosition.y;
                    }
                }
                else if (iso._aimed && enemy)
                {
                    _Position.x = _Player.TilePosition.x;
                    _Position.y = _Player.TilePosition.y;
                }
            }

            if (first && iso.fstWaveCooldown)
            {
                first = false;
                yield return new WaitForSeconds(iso.waveCooldown);
            }

            CreateInstanceOncast(iso, _Position, enemy);

            yield return new WaitForSeconds(iso.waveCooldown);
        }
        StopCoroutine("CastInstance");

        //return returnValue;
    }

    private bool CreateProjectileOnCast(ProjectileScriptableObject CastingProjectile, Vector2 Positon, bool enemy)
    {
        ///Basic
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
                //if ((CastingProjectile._StartingPosition.x < 0 || CastingProjectile._StartingPosition.x > 7
                // || CastingProjectile._StartingPosition.y < 0 || CastingProjectile._StartingPosition.y > 3)
                // && CastingProjectile._isStatic)
                //{

                //}

                int xInstance = 0;
                int yInstance = 0;

                if (!CastingProjectile._isStatic)
                {
                    if (CastingProjectile._resetStartPoint)
                    {
                        xInstance = (int)(Positon.x + x * SpellDirection);
                        yInstance = (int)(Positon.y + y * SpellDirection);
                    }
                    else
                    {
                        xInstance = (int)(Positon.x + (CastingProjectile._StartingPosition.x + x) * SpellDirection);
                        yInstance = (int)(Positon.y + (CastingProjectile._StartingPosition.y + y) * SpellDirection);
                    }
                }
                else
                {
                    xInstance = (int)((CastingProjectile._StartingPosition.x + x) * SpellDirection);
                    yInstance = (int)((CastingProjectile._StartingPosition.y + y) * SpellDirection);
                }


                if (xInstance >= 0 && xInstance <= 7
                    && yInstance >= 0 && yInstance <= 3)
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
                    behaviour._Shield = CastingProjectile._Shield;

                    if (enemy)
                    {
                        behaviour._isFriendly = false;
                        behaviour._Damage = CastingProjectile._Damage;
                    }
                    else
                    {
                        behaviour._isFriendly = CastingProjectile._isFriendly;
                        behaviour._Damage = (int)((CastingProjectile._Damage + _Manager.GetDamageModifier()) * _Manager.GetDamageMultiplier());
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
                ///
                if (CastingInstance.isRandom)
                {
                    if (enemy)
                        xInstance = Random.Range(0, 3);
                    else
                        xInstance = Random.Range(4, 7);
                    yInstance = Random.Range(0, 4);
                }
                else
                {
                    if (!CastingInstance._isStatic)
                    {
                        if (CastingInstance._resetStartPoint)
                        {
                            xInstance = (int)(Positon.x + x * SpellDirection);
                            yInstance = (int)(Positon.y + y * SpellDirection);
                        }
                        else
                        {
                            xInstance = (int)(Positon.x + (CastingInstance._StartingPosition.x + x) * SpellDirection);
                            yInstance = (int)(Positon.y + (CastingInstance._StartingPosition.y + y) * SpellDirection);
                        }
                    }
                    else
                    {
                        xInstance = (int)((CastingInstance._StartingPosition.x + x) * SpellDirection);
                        yInstance = (int)((CastingInstance._StartingPosition.y + y) * SpellDirection);
                    }
                }
                ///

                if (xInstance >= 0 && xInstance <= 7
                  && yInstance >= 0 && yInstance <= 3)
                {
                    Instant = Instantiate(DamageSphere, _Player.Position(xInstance, yInstance), Quaternion.identity);

                    if (Instant.TryGetComponent(out DamageBehaviour behaviour))
                    {
                        behaviour._ActiveFrame = CastingInstance._LifeSpan;
                        behaviour._ForcedMouvement = CastingInstance._ForcedMouvement;
                        behaviour.StunTime = CastingInstance.StunTime;
                        behaviour.Charm = CastingInstance._Charm;
                        behaviour.RandomTPonHit = CastingInstance.RandomTPonHit;
                        behaviour._FireStack = CastingInstance._FireStack;
                        behaviour._PoisonStack = CastingInstance._PoisonStack;
                        behaviour._Shield = CastingInstance._Shield;

                        if (enemy)
                        {
                            behaviour._isFriendly = false;
                            behaviour._Damage = CastingInstance._Damage;
                        }
                        else
                        {
                            behaviour._isFriendly = CastingInstance._isFriendly;
                            behaviour._Damage = (int)((CastingInstance._Damage + _Manager.GetDamageModifier()) * _Manager.GetDamageMultiplier());
                        }
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

        foreach (var spellBF in dic_SpellTable)
        {
            if(spellBF.Value.CharacterDeckStarter == charName)
            {
                NameList.Add(spellBF.Value);

                _SpellAvailable.Remove(spellBF.Value);
            }
        }

        return NameList;
    }

    public void AddSpellToPlayerDeck(string spellName)
    {
        _Player._StartingDeck.Add(dic_SpellTable[spellName]);
        _SpellAvailable.Remove(dic_SpellTable[spellName]);
    }
    /////////////////////////////// UI/////////////////////////////
    public Sprite GetIcone(string SpellName)
    {
        return dic_SpellTable[SpellName].Icon;
    }

    // For Later
    //private void CreateStructureOncast() 
    //{ 

    //}
}
