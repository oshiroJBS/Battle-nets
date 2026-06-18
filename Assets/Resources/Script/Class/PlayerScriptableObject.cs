using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "PlayerScriptableObject", menuName = "NewCharacter")]

public class PlayerScriptableObject : ScriptableObject
{
    [Header("Player Settings")]
    public string _CharacterName = "Name";

    public float _HpMax = 300;
    public int _Defense;
    public float _ManaMax = 3;
    public float ManaRecuperation = 0.3f;
    public float WeaponModifier;

    public SpellScriptableObject[] StartingDeck = new SpellScriptableObject[4];
    public SpellScriptableObject Weapon;

    public TalentScriptableObject InateTalent;

    [SerializeField] private Image Icone;
    public string _description;

    [SerializeField] private Image Sprit;
    [SerializeField] private Image[] SpritCycle;
    [SerializeField] private Image MovingSprit;
    [SerializeField] private Image AttackingSprit;
    [SerializeField] private Image DamagedSprit;
}
