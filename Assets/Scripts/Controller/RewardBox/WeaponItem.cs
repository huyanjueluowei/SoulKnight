using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : MonoBehaviour   //����ű����ڷ��ڵ��ϵ�����Ԥ������
{
    private GameObject weaponOnHand;  //�õ��������õ�Ԥ����
    private PlayerStats playerStats;
    private void Awake()
    {
        weaponOnHand = GetComponent<WeaponController>().weaponData.weaponPrefab;
    }

    private void Start()    //��û��awake�����ȡ�»�ȡ����
    {
        playerStats = GameManager.Instance.playerStats;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (playerStats.isSecondWeapon == false)
            {
                Instantiate(playerStats.mainWeapon.GetComponent<WeaponController>().weaponData.weaponOnGroundPrefab, transform.parent); //����������������ɵ�����
                playerStats.mainWeapon = weaponOnHand;                  //������ҵ�������
                Destroy(playerStats.weaponPos.GetChild(0).gameObject);
            }         
            else
            {
                Instantiate(playerStats.secondWeapon.GetComponent<WeaponController>().weaponData.weaponOnGroundPrefab, transform.parent); //����������������ɵ�����
                playerStats.secondWeapon = weaponOnHand;                  //������ҵĸ�����
                Destroy(playerStats.weaponPos.GetChild(0).gameObject);
            }
        }
    }
}
