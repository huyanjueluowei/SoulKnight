using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public Character_SO templateData;
    public Character_SO characterData;
    public WeaponData_SO weaponData;

    public BulletPool bulletPool;

    public Camera mainCamera;
    public GameObject weapon;               //��ȡ����Ԥ����
    protected Animator anim;
    protected BoxCollider2D coll;


    [Header("Weapon")]
    public Transform weaponPos;
    public float nextFire;

    [HideInInspector]
    public bool isCritical;
    public bool isDead=false;
    protected virtual void Awake()    //protected��ʾ�Լ���������Է��ʣ�virtual��ʾ�����������ڴ˻��������
    {
        characterData = Instantiate(templateData);
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        
    }

    public void ApplyWeapon()
    {
        if (GetWeapon())
        {
            weapon = weaponPos.GetChild(0).gameObject;
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

    public float AttackRange
    {
        get { if (characterData != null) return characterData.attackRange; else return 0; }
        set { characterData.attackRange = value; }
    }
    #endregion


    
}
