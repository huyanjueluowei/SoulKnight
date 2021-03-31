using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public Character_SO templateData;
    public Character_SO characterData;
    public WeaponData_SO weaponData;

    public BulletPool bulletPool;

    [Header("Weapon")]
    public Transform weaponPos;
    public float nextFire;

    [HideInInspector]
    public bool isCritical;

    public void Awake()
    {
        if (templateData != null)
            characterData = Instantiate(templateData);
    }

    private void Update()
    {
        
    }

    public void ApplyWeapon()
    {
        if (GetWeapon())
        {
            characterData.ApplyWeaponData(GetWeapon().weaponData);
        }
    }

    public WeaponController GetWeapon()   //����һ����ȡ����Controller�ĺ���������Ҫ����ʱ��ܶ��
    {
        if(weaponPos.GetChild(0)!=null)
        {
            weaponData = weaponPos.GetChild(0).GetComponent<WeaponController>().weaponData;  //�������weaponData��ֵ
            return weaponPos.GetChild(0).GetComponent<WeaponController>();
        }
        return null;
    }
    #region Read from Data_SO   
    public int MaxHealth    //������ô�����Ա����ô������ڵ���ʱ���һ�㣬ֱ��CharacterStats.MaxHealth������CharacterStats.characterData.maxHealth
    {  
        get { if (characterData != null) return characterData.maxHealth; else return 0; }
        set { characterData.maxHealth = value; }
    }
    public int CurrentHealth
    {
        get { if (characterData != null) return characterData.currentHealth; else return 0; }
        set { characterData.currentHealth = value; }
    }
    public int BaseDefence
    {
        get { if (characterData != null) return characterData.baseDefence; else return 0; }
        set { characterData.baseDefence = value; }
    }
    public int CurrentDefence
    {
        get { if (characterData != null) return characterData.currentDefence; else return 0; }
        set { characterData.currentDefence = value; }
    }
    public int MaxEnergy
    {
        get { if (characterData != null) return characterData.maxEnergy; else return 0; }
        set { characterData.maxEnergy = value; }
    }
    public int CurrentEnergy
    {
        get { if (characterData != null) return characterData.currentEnergy; else return 0; }
        set { characterData.currentEnergy = value; }
    }

    //������������
    public float AttackRange
    {
        get { if (characterData != null) return characterData.attackRange; else return 0; }
        set { characterData.attackRange = value; }
    }
    public float CoolDown
    {
        get { if (characterData != null) return characterData.coolDown; else return 0; }
        set { characterData.coolDown = value; }
    }
    public int MinDamage
    {
        get { if (characterData != null) return characterData.minDamage; else return 0; }
        set { characterData.minDamage = value; }
    }
    public int MaxDamage
    {
        get { if (characterData != null) return characterData.maxDamage; else return 0; }
        set { characterData.maxDamage = value; }
    }
    public float CriticalMutiplier
    {
        get { if (characterData != null) return characterData.criticalMutiplier; else return 0; }
        set { characterData.criticalMutiplier = value; }
    }
    public float CriticalChance
    {
        get { if (characterData != null) return characterData.criticalChance; else return 0; }
        set { characterData.criticalChance = value; }
    }
    #endregion


    
}
