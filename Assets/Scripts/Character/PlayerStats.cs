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
        mousePos.z += 10;      //�ҷ���mousePos  z��-10��2d�����ⶼҪ���0�������������Ļ�ǹ����ת��ܲ���������Ϊz��Ӱ��Ƕ���ƫ��
        if (mousePos.y > weaponPos.position.y)
        {
            z = Vector3.Angle(Vector3.right, mousePos - weaponPos.position);
        }
        else
        {
            z = -Vector3.Angle(Vector3.right, mousePos - weaponPos.position);
        }
        weaponPos.rotation = Quaternion.Euler(0, 0, z);
        if (Mathf.Abs(z) > 90)        //ǹ�ĽǶȵ���һ��,ע�ⲻ��ֱ�Ӹ�rotation�������Ǹĵ�������������ת�Ƕȣ�Ҫ��Local�ı���Ը����Ƕ�
        {
            weaponPos.GetChild(0).transform.localEulerAngles = new Vector3(180, 0, 0);
        }
        else
        {
            weaponPos.GetChild(0).transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }


    void GenerateBullet()         //ʵ�������ӵ��ͷ����ӵ�Ч��
    {
        if(Input.GetMouseButtonDown(0)) 
        {
            if(Time.time>nextFire)
            {
                nextFire = Time.time + GetWeapon().weaponData.coolDown;
                GameObject bullet = GetWeapon().weaponBulletPool.GetBullet();     //�ӵ��ػ�ȡ�ӵ�
                bullet.GetComponent<BulletController>().weaponData = Instantiate(weaponData);
                bullet.SetActive(true);                                     //��ʾ�ӵ�
                bullet.transform.eulerAngles = weaponPos.eulerAngles;   //���ɺ�ͽ�bullet�Ƕ��뵱ʱ��weaponPosһ��
                bullet.GetComponent<BulletController>().isActive = true;
                bullet.transform.position = weaponPos.position;
                Vector3 dir = weaponPos.transform.right;
                bullet.GetComponent<BulletController>().rb.velocity = new Vector2(dir.x,dir.y)*20;  //��άƽ���һ���ٶ�
            }
        }
    }
}
