using System.Collections;
using UnityEngine;

public class TalentManager : MonoBehaviour
{
    // Start is called before the first frame update
    private Player _Player;

    private float _DamageMultiplier = 1;
    private int _DamageModifier = 0;
    private int _DefenceUP = 0;
    private int _WeaponDamageUP = 0;
    private float _ManaRecupUp = 0;
    private float _HealOverTime = 0;
    private int _BurnModifier = 0;
    private float _BurnMultiplier = 0;
    private int _PoisonModifier = 0;
    private float _PoisonMultiplier = 0;

    [SerializeField] private TalentScriptableObject[] Library;
    private ArrayList _TalentList = new ArrayList();
    private TalentScriptableObject[] talentInUse = new TalentScriptableObject[0];
    private TalentScriptableObject[] lastTab = new TalentScriptableObject[0];


    private void Awake()
    {
        for (int i = 0; i < Library.Length; i++)
        {
            _TalentList.Add(Library[i]);
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

    public float GetBurnMultiplier()
    {
        _BurnMultiplier = 1;
        if (talentInUse.Length > 0)
        {
            foreach (TalentScriptableObject item in talentInUse)
            {
                if (item.isConditionMet())
                {
                    _BurnMultiplier += item.burnMultiplier;
                }
            }
        }
        return _BurnMultiplier;
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
