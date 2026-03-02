using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SpellsScriptableObject", menuName = "ScriptableObject/Talent")]
public class TalentScriptableObject : ScriptableObject
{
    [Header("Condition")]
    [Header("//")]
    public int HpMin;
    public int HpPercentMin;
    public int HpMax;
    public int HpPercentMax;
    public int shieldMax;
    public int shieldMin;

    public enum StatusEffect
    {
        none, Poison, burn, either, both
    }
    public StatusEffect status = StatusEffect.none;

    [Header("trigger every X Step")]
    public int stepsNecessary = 0;
    [Header("trigger when immobile for X sec")]
    public float imobile = 0;

    [Header("trigger every X spell used")]
    public float nbAtkNecessary;
    [Header("trigger if didn't atk for X sec")]
    public float atkCooldown;
    [Header("trigger if didn't got hit for X sec")]
    public float hitCooldown;

    [Header("always active")]
    public bool always = false;
    [Header("sec between each activation")]
    public float cooldown;
    [Header("///")]
    [Header(" ")]

    ////////////////////////////////////////////////

    [Header("Effect")]
    [Header("//")]

    public int damageModifier = 0;
    public int damageMultiplier = 0;

    public int defenceUP = 0;
    public int WeaponDamageUP = 0;

    public float manaRecupUp = 0;
    public float HealOverTime = 0;

    public int burnModifier = 0;
    public float combustModifier = 0;

    public int PoisonModifier = 0;
    public float PoisonMultiplier = 0;

    public SpellScriptableObject SpellToCast = null;

    ////////////values//////////

    [HideInInspector] public float timer_Immobile = 0;
    [HideInInspector] public float timer_Atk = 0;
    [HideInInspector] public float timer_Hit = 0;
    [HideInInspector] public float timer_cooldown = 0;
    [HideInInspector] public float counter_Atk = 0;
    [HideInInspector] public int counter_Step = 0;

    public class Condition
    {
        public string conditionName;
        public float conditionValue;
    }

    public List<Condition> ls_Conditions = new List<Condition>();

    ///////////////////////fonctions/////////////////////////////////


    public void CreateConditions()
    {
        ls_Conditions = new List<Condition>();

        if (always)
        {
            ls_Conditions.Add(new Condition { conditionName = nameof(always), conditionValue = 1f });
            return;
        }

        if (HpMin > 0)      /////// HP MIN /////////
        {
            ls_Conditions.Add(new Condition { conditionName = nameof(HpMin), conditionValue = HpMin });
        }

        if (HpPercentMin > 0)      /////// HpPercentMin /////////
        {
            ls_Conditions.Add(new Condition { conditionName = nameof(HpPercentMin), conditionValue = HpPercentMin });
        }
        if (HpMax > 0)      /////// HpMax /////////
        {
            ls_Conditions.Add(new Condition { conditionName = nameof(HpMax), conditionValue = HpMax });
        }
        if (HpPercentMax > 0)      /////// HpPercentMax /////////
        {
            ls_Conditions.Add(new Condition { conditionName = nameof(HpPercentMax), conditionValue = HpPercentMax });
        }
        if (shieldMax > 0)      /////// shieldMax /////////
        {
            ls_Conditions.Add(new Condition { conditionName = nameof(shieldMax), conditionValue = shieldMax });
        }
        if (shieldMin > 0)      /////// shieldMin /////////
        {
            ls_Conditions.Add(new Condition { conditionName = nameof(shieldMin), conditionValue = shieldMin });
        }

        if (stepsNecessary > 0)      /////// stepsNecessary /////////
        {
            ls_Conditions.Add(new Condition { conditionName = nameof(stepsNecessary), conditionValue = stepsNecessary });
        }
        if (imobile > 0)      /////// imobile /////////
        {
            ls_Conditions.Add(new Condition { conditionName = nameof(imobile), conditionValue = imobile });
        }
        if (nbAtkNecessary > 0)      /////// nbAtkNecessary /////////
        {
            ls_Conditions.Add(new Condition { conditionName = nameof(nbAtkNecessary), conditionValue = nbAtkNecessary });
        }
        if (atkCooldown > 0)      /////// atkCooldown /////////
        {
            ls_Conditions.Add(new Condition { conditionName = nameof(atkCooldown), conditionValue = atkCooldown });
        }
        if (hitCooldown > 0)      /////// hitCooldown /////////
        {
            ls_Conditions.Add(new Condition { conditionName = nameof(hitCooldown), conditionValue = hitCooldown });
        }
        if (cooldown > 0)      /////// coolDown /////////
        {
            ls_Conditions.Add(new Condition { conditionName = nameof(cooldown), conditionValue = cooldown });
        }
    }


    public bool isConditionMet(Player TargetPlayer)
    {
        bool conditionMet = true;

        foreach (Condition c in ls_Conditions)
        {
            switch (c.conditionName)
            {
                case "always":
                    return conditionMet;

                case "HpMin":
                    if (HpMin > TargetPlayer.HP)
                    {
                        conditionMet = false;
                        return conditionMet;
                    }
                    break;
                case "HpPercentMin":
                    if (HpPercentMin > (TargetPlayer.HP / TargetPlayer.HPMax) * 100)
                    {
                        conditionMet = false;
                        return conditionMet;
                    }
                    break;
                case "HpMax":
                    if (HpMax < TargetPlayer.HP)
                    {
                        conditionMet = false;
                        return conditionMet;
                    }
                    break;
                case "HpPercentMax":
                    if (HpPercentMax < (TargetPlayer.HP / TargetPlayer.HPMax) * 100)
                    {
                        conditionMet = false;
                        return conditionMet;
                    }
                    break;
                case "shieldMax":
                    if (shieldMax < TargetPlayer.Shield)
                    {
                        conditionMet = false;
                        return conditionMet;
                    }
                    break;
                case "shieldMin":
                    if (shieldMin > TargetPlayer.Shield)
                    {
                        conditionMet = false;
                        return conditionMet;
                    }
                    break;
                case "stepsNecessary":
                    if (stepsNecessary > counter_Step)
                    {
                        conditionMet = false;
                        return conditionMet;
                    }
                    break;
                case "imobile":
                    if (imobile > timer_Immobile)
                    {
                        conditionMet = false;
                        return conditionMet;
                    }
                    break;
                case "nbAtkNecessary":
                    if (nbAtkNecessary > counter_Atk)
                    {
                        conditionMet = false;
                        return conditionMet;
                    }
                    break;
                case "atkCooldown":
                    if (atkCooldown > timer_Atk)
                    {
                        conditionMet = false;
                        return conditionMet;
                    }
                    break;
                case "hitCooldown":
                    if (hitCooldown > timer_Hit)
                    {
                        conditionMet = false;
                        return conditionMet;
                    }
                    break;
                case "coolDown":
                    if (cooldown > timer_cooldown)
                    {
                        conditionMet = false;
                        return conditionMet;
                    }
                    break;

                default:
                    break;
            }
        }

        switch (status)
        {
            case StatusEffect.either:
                if (TargetPlayer.CheckStatut() == StatusEffect.none)
                {
                    conditionMet = false;
                }
                break;

            case StatusEffect.none:
                if (TargetPlayer.CheckStatut() != StatusEffect.none)
                {
                    conditionMet = false;
                }
                break;
            case StatusEffect.Poison:
                if (TargetPlayer.CheckStatut() != StatusEffect.Poison
                    && TargetPlayer.CheckStatut() != StatusEffect.both)
                {
                    conditionMet = false;
                }
                break;
            case StatusEffect.burn:
                if (TargetPlayer.CheckStatut() != StatusEffect.burn
                    && TargetPlayer.CheckStatut() != StatusEffect.both)
                {
                    conditionMet = false;
                }
                break;
            case StatusEffect.both:
                if (TargetPlayer.CheckStatut() != StatusEffect.both)
                {
                    conditionMet = false;
                }
                break;
            default:
                break;
        }


        return conditionMet;
    }

    public void Activate()
    {

    }
}
