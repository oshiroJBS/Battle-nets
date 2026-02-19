using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TalentManager;
using static UnityEditor.Experimental.GraphView.GraphView;

public class TalentManager : MonoBehaviour
{
    // Start is called before the first frame update
    private Player _Player;
    public enum TalentStatus
    {
        Neutral, GotHit, Moved, DidAttacked
    }

    private float _DamageMultiplier = 1;
    private int _DamageModifier = 0;
    private int _DefenceUP = 0;
    private int _WeaponDamageUP = 0;
    private float _ManaRecupUp = 0;
    private float _HealOverTime = 0;
    private int _BurnModifier = 0;
    private float _CombustModifier = 0;
    private int _PoisonModifier = 0;
    private float _PoisonMultiplier = 0;


    private Dictionary<string, TalentScriptableObject> dic_TalentList = new Dictionary<string, TalentScriptableObject>();
    [SerializeField] private TalentScriptableObject[] _TalentLibrary;
    private ArrayList _TalentList = new ArrayList();
    private TalentScriptableObject[] talentInUse = new TalentScriptableObject[0];
    private TalentScriptableObject[] lastTab = new TalentScriptableObject[0];


    private void Awake()
    {
        foreach (TalentScriptableObject TalentBF in _TalentLibrary)
        {
            dic_TalentList[TalentBF.name] = TalentBF;
            _TalentList.Add(TalentBF);
        }
    }

    void Start()
    {
        if (_Player == null) _Player = GameObject.FindObjectOfType<Player>();
    }

    public void GetInateTalent(string charName)
    {
        switch (charName)
        {
            case "kou":
                break;
            case "pina":
                break;
            case "cyon":
                break;
        }
    }




    #region GET TargetModifier

    public int GetDamageModifier()
    {
        _DamageModifier = 0;

        if (talentInUse.Length > 0)
        {
            foreach (TalentScriptableObject item in talentInUse)
            {
                if (item.isConditionMet())
                {
                    _DamageModifier += item.damageModifier;
                }
            }
        }
        return _DamageModifier;
    }

    public float GetDamageMultiplier()
    {
        _DamageMultiplier = 1;
        if (talentInUse.Length > 0)
        {
            foreach (TalentScriptableObject item in talentInUse)
            {
                if (item.isConditionMet())
                {
                    _DamageMultiplier += item.damageMultiplier;
                }
            }
        }
        return _DamageMultiplier;
    }

    public int GetDefenseModifier()
    {
        _DefenceUP = 0;
        if (talentInUse.Length > 0)
        {
            foreach (TalentScriptableObject item in talentInUse)
            {
                if (item.isConditionMet())
                {
                    _DefenceUP += item.defenceUP;
                }
            }
        }
        return _DefenceUP;
    }

    public int GetWeponDamageModifier()
    {
        _WeaponDamageUP = 0;
        if (talentInUse.Length > 0)
        {
            foreach (TalentScriptableObject item in talentInUse)
            {
                if (item.isConditionMet())
                {
                    _WeaponDamageUP += item.WeaponDamageUP;
                }
            }
        }
        return _WeaponDamageUP;
    }

    public float GetManaRecupModifier()
    {
        _ManaRecupUp = 0;
        if (talentInUse.Length > 0)
        {
            foreach (TalentScriptableObject item in talentInUse)
            {
                if (item.isConditionMet())
                {
                    _ManaRecupUp += item.manaRecupUp;
                }
            }
        }
        return _ManaRecupUp;
    }

    public float GetHealOverTime()
    {
        _HealOverTime = 0;
        if (talentInUse.Length > 0)
        {
            foreach (TalentScriptableObject item in talentInUse)
            {
                if (item.isConditionMet())
                {
                    _HealOverTime += item.HealOverTime;
                }
            }
        }
        return _HealOverTime;
    }

    public float GetBurnModifier()
    {
        _BurnModifier = 0;
        if (talentInUse.Length > 0)
        {
            foreach (TalentScriptableObject item in talentInUse)
            {
                if (item.isConditionMet())
                {
                    _BurnModifier += item.burnModifier;
                }
            }
        }
        return _BurnModifier;
    }

    public float GetCombustModifier()
    {
        _CombustModifier = 1;
        if (talentInUse.Length > 0)
        {
            foreach (TalentScriptableObject item in talentInUse)
            {
                if (item.isConditionMet())
                {
                    _CombustModifier += item.combustModifier;
                }
            }
        }
        return _CombustModifier;
    }

    public float GetPoisonModifier()
    {
        _PoisonModifier = 0;
        if (talentInUse.Length > 0)
        {
            foreach (TalentScriptableObject item in talentInUse)
            {
                if (item.isConditionMet())
                {
                    _PoisonModifier += item.PoisonModifier;
                }
            }
        }
        return _PoisonModifier;
    }

    public float GetPoisonMultiplier()
    {
        _PoisonMultiplier = 1;
        if (talentInUse.Length > 0)
        {
            foreach (TalentScriptableObject item in talentInUse)
            {
                if (item.isConditionMet())
                {
                    _PoisonMultiplier += item.PoisonMultiplier;
                }
            }
        }
        return _PoisonMultiplier;
    }

    #endregion


    #region ConditionManager

    public void UpdateTalentsCondition(TalentStatus newStatus)
    {
    }

    #endregion


    public void AddTalent(TalentScriptableObject talent)
    {
        talentInUse = new TalentScriptableObject[lastTab.Length + 1];
        talentInUse[0] = talent;

        for (int i = 0; i < lastTab.Length; i++)
        {
            talentInUse[i + 1] = lastTab[i];
        }

        lastTab = new TalentScriptableObject[talentInUse.Length];
        lastTab = talentInUse;
    }
}








//if (talentInUse.Length > 0)
//{
//    foreach (TalentScriptableObject item in talentInUse)
//    {

//        switch (newStatus)
//        {
//            case TalentStatus.Neutral:
//                item.immobileTimer += Time.deltaTime;
//                item.AtkTimer += Time.deltaTime;
//                item.hitTimer += Time.deltaTime;

//                break;

//            case TalentStatus.GotHit:
//                item.immobileTimer += Time.deltaTime;
//                item.AtkTimer += Time.deltaTime;
//                break;

//            case TalentStatus.Moved:

//                item.AtkTimer += Time.deltaTime;
//                item.hitTimer += Time.deltaTime;

//                item.step++;
//                if (item.step >= item.stepsNecessary && item.stepsNecessary != 0)
//                {

//                }
//                item.step = 0;

//                break;

//            case TalentStatus.DidAttacked:
//                item.immobileTimer += Time.deltaTime;
//                item.hitTimer += Time.deltaTime;

//                item.nbAtk++;
//                if (item.nbAtk >= item.nbAtkNecessary && item.nbAtkNecessary != 0)
//                {

//                }
//                item.nbAtk = 0;
//                break;

//            default:
//                break;
//        }


//        //if (_Player.Sta == item.Status) { }
//        if (item.always == true) { }


//        if (item.HpMin > 0)
//        {
//            if (_Player.HP >= item.HpMin)
//            {

//            }
//        }

//        if (item.HpPercentMin > 0)
//        {
//            if ((_Player.HP / _Player.HPMax) * 100 >= item.HpPercentMin)
//            {

//            }
//        }

//        if (item.HpMax > 0)
//        {
//            if (_Player.HP >= item.HpMax)
//            {

//            }
//        }

//        if (item.HpPercentMax > 0)
//        {
//            if ((_Player.HP / _Player.HPMax) * 100 <= item.HpPercentMax)
//            {

//            }
//        }

//        if (item.shieldMax > 0)
//        {
//            if (_Player.Shield <= item.shieldMax)
//            {

//            }
//        }

//        if (item.shieldMin > 0)
//        {
//            if (_Player.Shield >= item.shieldMin)
//            {

//            }
//        }

//        if (item.Status != TalentScriptableObject.StatusEffect.none) { }
//        if (item.stepsNecessary > 0) { }
//        if (item.imobile > 0) { }
//        if (item.nbAtkNecessary > 0) { }
//        if (item.atkCooldown > 0) { }
//        if (item.hitCooldown > 0) { }
//        if (item.coolDown > 0) { }
//    }
//}