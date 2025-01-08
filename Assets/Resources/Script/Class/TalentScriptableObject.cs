using UnityEngine;
using static UnityEditor.Progress;


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
    public StatusEffect Status = StatusEffect.none;

    [Header("trigger every X Step")]
    public int Moved = 0;
    [Header("trigger when immobile for X sec")]
    public int imobile = 0;

    [Header("trigger if did'nt atk for X sec")]
    public float atkCooldown;
    [Header("trigger if did'nt got hit for X sec")]
    public float hitCooldown;

    [Header("always active")]
    public bool always = false;

    [Header("Effect")]
    [Header("//")]

    public int damageModifier = 0;
    public int damageMultiplier = 0;

    public int defenceUP = 0;
    public int WeaponDamageUP = 0;

    public float manaRecupUp = 0;
    public float HealOverTime = 0;

    public int burnModifier = 0;
    public float burnMultiplier = 0;

    public int PoisonModifier = 0;
    public float PoisonMultiplier = 0;

    public bool isConditionMet()
    {
        //if (item.Moved != 0)
        //{
        //}
        //if (item.imobile != 0)
        //{
        //}
        //if (item.atkCooldown != 0)
        //{
        //}
        //if (item.hitCooldown != 0)
        //{
        //}

        if (always) return true;
       bool conditionMet = false;
        Player _Player = GameObject.FindObjectOfType<Player>();


        if (HpMin != 0)
        {
            if (_Player.HP > HpMin)
                conditionMet = true;
            else
                return false;
        }
        if (HpPercentMin != 0)
        {
            if (_Player.HP / _Player.HPMax > HpPercentMin / 100)
                conditionMet = true;
            else
                return false;
        }
        if (HpMax != 0)
        {
            if (_Player.HP < HpMax)
                conditionMet = true;
            else
                return false;
        }
        if (HpPercentMax != 0)
        {
            if (_Player.HP / _Player.HPMax < HpPercentMax)
                conditionMet = true;
            else return false;
        }
        if (shieldMin != 0)
        {
            if (_Player.Shield > shieldMin)
                conditionMet = true;
            else return false;
        }
        if (shieldMax != 0)
        {
            if (_Player.Shield < shieldMax)
                conditionMet = true;
            else return false;
        }

        if (Status != StatusEffect.none)
        {
            switch (Status)
            {
                case StatusEffect.Poison:
                    if (_Player._PoisonStack != 0)
                        conditionMet = true;
                    else
                        return false;
                    break;

                case StatusEffect.burn:
                    if (_Player._FireStack != 0)
                        conditionMet = true;
                    else
                        return false;
                    break;

                case StatusEffect.either:
                    if (_Player._FireStack != 0 || _Player._PoisonStack != 0)
                        conditionMet = true;
                    else
                        return false;
                    break;

                case StatusEffect.both:
                    if (_Player._FireStack != 0 && _Player._PoisonStack != 0)
                        conditionMet = true;
                    else
                        return false;
                    break;

                default:
                    break;
            }
        }

        return conditionMet;
    }
}
