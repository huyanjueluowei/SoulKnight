using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : MonoBehaviour
{
    public WeaponData_SO weaponData;
    public string knifeName;      //刀光的名字，用来每次切换时候做判断，如果不一样就要切换到对应武器的刀光
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(CompareTag("PlayerBullet")&&collision.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyStats>().TakeDamage(weaponData);
        }
        else if(CompareTag("PlayerBullet")&&collision.CompareTag("Boss"))
        {
            collision.gameObject.GetComponent<BossStats>().TakeDamage(weaponData);
        }
    }

    public void SetFalse()   //动画隐藏
    {
        gameObject.SetActive(false);      
    }
}
