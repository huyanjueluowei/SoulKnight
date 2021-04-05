using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : MonoBehaviour
{
    public WeaponData_SO weaponData;
    public string knifeName;      //��������֣�����ÿ���л�ʱ�����жϣ������һ����Ҫ�л�����Ӧ�����ĵ���
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(CompareTag("PlayerBullet")&&collision.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyStats>().TakeDamage(weaponData);
        }
    }

    public void SetFalse()   //��������
    {
        gameObject.SetActive(false);      
    }
}
