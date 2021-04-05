using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossKnife : MonoBehaviour
{
    public WeaponData_SO weaponData;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CompareTag("EnemyBullet") && collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerStats>().TakeDamage(weaponData);
        }
    }
}
