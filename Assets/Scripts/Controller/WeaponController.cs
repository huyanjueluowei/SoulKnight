using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public WeaponData_SO weaponData;
    public BulletPool weaponBulletPool;

    private void Start()
    {
        if(weaponData.bulletPoolName!=null)
        {
            if(GameObject.Find(weaponData.bulletPoolName)!=null)   //这里不判空会报错，它连着调用几次，懵逼
                weaponBulletPool = GameObject.Find(weaponData.bulletPoolName).GetComponent<BulletPool>();
        }
    }
}
