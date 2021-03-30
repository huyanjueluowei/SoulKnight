using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    public Camera mainCamera;
    private void Update()
    {
        RotateWeapon();
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
}
