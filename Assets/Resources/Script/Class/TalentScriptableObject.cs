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
    public StatusEffect Status = StatusEffect.none;

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
    public float coolDown;
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

    [HideInInspector] public int step = 0;
    [HideInInspector] public float immobileTimer = 0;
    [HideInInspector] public float AtkTimer = 0;
    [HideInInspector] public float nbAtk = 0;
    [HideInInspector] public float hitTimer = 0;


    private Dictionary<string, float> dic_Talents = new Dictionary<string, float>();

    ///////////////////////fonctions/////////////////////////////////
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
            if (_Player.HP / _Player.HPMax < HpPercentMax /100)
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

    public void Activate()
    {

    }
}
