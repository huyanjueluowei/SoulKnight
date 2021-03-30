using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public Character_SO characterData;
    public WeaponData_SO weaponData;

    [Header("Weapon")]
    public Transform weaponPos;    //ֻ��Ե�����

    [HideInInspector]
    public bool isCritical;
}
