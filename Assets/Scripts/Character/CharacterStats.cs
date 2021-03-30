using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public Character_SO characterData;
    public WeaponData_SO weaponData;

    [Header("Weapon")]
    public Transform weaponPos;    //只针对敌人用

    [HideInInspector]
    public bool isCritical;
}
