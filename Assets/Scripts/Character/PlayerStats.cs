using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    public Camera mainCamera;
    private void Update()
    {
        RotateWeapon();
        ApplyWeapon();
        GenerateBullet();
    }
    public void RotateWeapon()
    {
        float z;
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z += 10;      //我发现mousePos  z是-10，2d里面这都要变成0，如果不改这个的话枪的旋转会很不流畅，因为z的影响角度有偏差
        if (mousePos.y > weaponPos.position.y)
        {
            z = Vector3.Angle(Vector3.right, mousePos - weaponPos.position);
        }
        else
        {
            z = -Vector3.Angle(Vector3.right, mousePos - weaponPos.position);
        }
        weaponPos.rotation = Quaternion.Euler(0, 0, z);
        if (Mathf.Abs(z) > 90)        //枪的角度调整一下,注意不能直接改rotation，否则那改的是相对世界的旋转角度，要用Local改变相对父级角度
        {
            weaponPos.GetChild(0).transform.localEulerAngles = new Vector3(180, 0, 0);
        }
        else
        {
            weaponPos.GetChild(0).transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }


    void GenerateBullet()         //实现生成子弹和发射子弹效果
    {
        if(Input.GetMouseButtonDown(0)) 
        {
            if(Time.time>nextFire)
            {
                nextFire = Time.time + GetWeapon().weaponData.coolDown;
                GameObject bullet = GetWeapon().weaponBulletPool.GetBullet();     //子弹池获取子弹
                bullet.GetComponent<BulletController>().weaponData = Instantiate(weaponData);
                bullet.SetActive(true);                                     //显示子弹
                bullet.transform.eulerAngles = weaponPos.eulerAngles;   //生成后就将bullet角度与当时的weaponPos一致
                bullet.GetComponent<BulletController>().isActive = true;
                bullet.transform.position = weaponPos.position;
                Vector3 dir = weaponPos.transform.right;
                bullet.GetComponent<BulletController>().rb.velocity = new Vector2(dir.x,dir.y)*20;  //二维平面给一个速度
            }
        }
    }
}
