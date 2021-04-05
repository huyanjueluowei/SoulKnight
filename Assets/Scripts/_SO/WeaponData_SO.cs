using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { GUN,KNIFE}
public enum WeaponLevel { NORMAL,GOOD,RARE,EPIC,LEGEND}
[CreateAssetMenu(fileName ="New Weapon",menuName ="CharacterStats/Weapon Data")]
public class WeaponData_SO : ScriptableObject
{
    [Header("Attack Info")]
    public float attackRange;
    public float coolDown;
    public int minDamage;
    public int maxDamage;
    public int bulletAmount;          //一发子弹要消耗多少精力
    public float criticalChance;      //暴击率
    public string weaponName;
    public string bulletPoolName;     //子弹池名字
    public WeaponType weaponType;     //武器类型
    public WeaponLevel weaponLevel;   //武器级别
    public GameObject weaponPrefab;   //武器预制体
    public GameObject weaponOnGroundPrefab;   //在地上的武器预制体

    public bool isCritical = false;

    public GameObject bulletPrefab;
    bool onGround;   //判断是否在地面上
}
