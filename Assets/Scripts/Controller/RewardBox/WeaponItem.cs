using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : MonoBehaviour   //这个脚本挂在放在地上的武器预制体上
{
    private GameObject weaponOnHand;  //拿到在手上用的预制体
    private PlayerStats playerStats;
    private void Awake()
    {
        weaponOnHand = GetComponent<WeaponController>().weaponData.weaponPrefab;
    }

    private void Start()    //我没放awake里面获取怕获取不到
    {
        playerStats = GameManager.Instance.playerStats;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (playerStats.isSecondWeapon == false)
            {
                Instantiate(playerStats.mainWeapon.GetComponent<WeaponController>().weaponData.weaponOnGroundPrefab, transform.parent); //把玩家手上武器生成到地上
                playerStats.mainWeapon = weaponOnHand;                  //更换玩家的主武器
                Destroy(playerStats.weaponPos.GetChild(0).gameObject);
            }         
            else
            {
                Instantiate(playerStats.secondWeapon.GetComponent<WeaponController>().weaponData.weaponOnGroundPrefab, transform.parent); //把玩家手上武器生成到地上
                playerStats.secondWeapon = weaponOnHand;                  //更换玩家的副武器
                Destroy(playerStats.weaponPos.GetChild(0).gameObject);
            }
        }
    }
}
