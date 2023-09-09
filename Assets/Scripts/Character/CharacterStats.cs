using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "Turn-Based RPG Project/CharacterStats", order = 0)]
public class CharacterStats : ScriptableObject
{
    [Header("Main Stats")]
    [SerializeField] private int strength;
    [SerializeField] private int vitality;
    [SerializeField] private int dexterity;
    [SerializeField] private int agility;
    [SerializeField] private int intelligence;

    [Header("Secondary Stats")]
    private float hp = 5;
    private float mp = 10;
    private float dodge = 5;
    private float hitRatio = 20;


    public float GetMaxHealth()
    {
        return hp + (vitality * 2);
    }

    public float GetMaxMana()
    {
        return mp + (intelligence * 3);
    }

    public float GetDodgeChance()
    {
        return dodge + (agility * 2);
    }

    public float GetHitRatio()
    {
        return hitRatio + (dexterity * 4);
    }

    public int GetInteligence() => intelligence;
    public int GetVitality() => vitality;
    public int GetDesterity() => dexterity;
    public int GetStrength() => strength;
    public int GetAgility() => agility;
}
