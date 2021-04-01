using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "CharacterStats/Data")]
public class Character_SO : ScriptableObject
{
    [Header("Base Info")]
    public int maxHealth;
    public int currentHealth;
    public int baseDefence;
    public int currentDefence;
    public int maxEnergy;
    public int currentEnergy;


    [Header("Attack Info")]
    public float attackRange;
    public float coolDown;
    public int minDamage;
    public int maxDamage;
    public float criticalChance;      //±©»÷ÂÊ

    public void ApplyWeaponData(WeaponData_SO weapon)
    {
        attackRange = weapon.attackRange;
        coolDown = weapon.coolDown;
        minDamage = weapon.minDamage;
        maxDamage = weapon.maxDamage;
        criticalChance = weapon.criticalChance;
    }
}
