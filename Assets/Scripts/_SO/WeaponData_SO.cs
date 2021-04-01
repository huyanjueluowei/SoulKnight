using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Weapon",menuName ="CharacterStats/Weapon Data")]
public class WeaponData_SO : ScriptableObject
{
    [Header("Attack Info")]
    public float attackRange;
    public float coolDown;
    public int minDamage;
    public int maxDamage;
    public float criticalChance;      //暴击率
    public string weaponName;

    public bool isCritical = false;

    public GameObject bulletPrefab;
    bool onGround;   //判断是否在地面上
}
