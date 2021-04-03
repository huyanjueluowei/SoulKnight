using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { GUN,KNIFE}
[CreateAssetMenu(fileName ="New Weapon",menuName ="CharacterStats/Weapon Data")]
public class WeaponData_SO : ScriptableObject
{
    [Header("Attack Info")]
    public float attackRange;
    public float coolDown;
    public int minDamage;
    public int maxDamage;
    public int bulletAmount;          //һ���ӵ�Ҫ���Ķ��پ���
    public float criticalChance;      //������
    public string weaponName;
    public string bulletPoolName;     //�ӵ�������
    public WeaponType weaponType;     //��������

    public bool isCritical = false;

    public GameObject bulletPrefab;
    bool onGround;   //�ж��Ƿ��ڵ�����
}
