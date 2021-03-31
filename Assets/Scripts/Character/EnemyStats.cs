using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    public void TakeDamage(WeaponData_SO weaponData)
    {
        float chance = Random.Range(0, 1f);
        int damage = Random.Range(MinDamage, MaxDamage + 1);
        if (chance < CriticalChance) isCritical = true;
        else isCritical = false;

        if (isCritical) damage *= 2;
        damage = Mathf.Max(damage - BaseDefence, 0);     //伤害最低是0，不能加血
        CurrentHealth = Mathf.Max(CurrentHealth - damage,0);
    }
}
