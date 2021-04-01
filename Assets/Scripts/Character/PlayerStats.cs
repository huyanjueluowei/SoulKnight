using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : CharacterStats
{
    private GameObject damageText;
    private void Update()
    {
        RotateWeapon();
        ApplyWeapon();
        GenerateBullet();
    }
    public void RotateWeapon()
    {
        if (isDead == false)
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


    void GenerateBullet()         //实现生成子弹和发射子弹效果
    {
        if(Input.GetMouseButtonDown(0)&&isDead==false) 
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

    public void TakeDamage(WeaponData_SO weaponData)
    {
        float chance = Random.Range(0, 1f);
        int damage = Random.Range(weaponData.minDamage, weaponData.maxDamage + 1);
        if (chance < weaponData.criticalChance) weaponData.isCritical = true;
        else weaponData.isCritical = false;

        if (weaponData.isCritical) damage=damage*2;
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        //damageText = GameObject.Find("DamageInfoText");   //GameObject.Find只能找active的物体，不能找隐藏物体
        GameObject damageInfo = GameObject.Find("DamageInfo");   //可以把要用的文字挂在一个可见空物体下面，先找这个空物体
        damageText = damageInfo.transform.Find("DamageInfoText_Player").gameObject;  //再用Transform.Find方法可以找子物体名字，隐藏的也可以找到
        if (weaponData.isCritical) damageText.GetComponent<Text>().color = Color.red;    //暴击让数字显示为红色
        else damageText.GetComponent<Text>().color = Color.yellow;            //不暴击让数字显示为黄色

        damageText.GetComponent<Text>().text = damage.ToString();    //更新文字内容为当前的伤害
        InvokeRepeating("SetDamageInfoTextPos", 0, 0.02f);            //实时更新文字的位置保证在怪物头顶而不是停留在原地
        damageText.SetActive(true);
        StartCoroutine(SetDamageInfoTextFalse());   //协程是与这个程序一起跑，协程后面的程序不影响，同时在跑

        if (CurrentHealth <= 0)                 //判断一下是否死亡，若死亡则播放死亡动画、停止运动并销毁
        {
            isDead = true;
            coll.enabled = false;               //死了之后箭撞不到player了
            rb.velocity = Vector2.zero;         //死了之后不能移动
            transform.GetChild(0).GetChild(0).gameObject.SetActive(false);  //把武器隐藏
            anim.SetBool("dead", isDead);
        }
    }
    IEnumerator SetDamageInfoTextFalse()
    {
        yield return new WaitForSeconds(0.5f);   //注意WaitForSeconds前面有new,等完这个时间之后下面的语句才执行
        damageText.SetActive(false);
        CancelInvoke("SetDamageInfoTextPos");    //文字关闭后就把InvokeRepeating关闭，以免文字关闭还一直调用，开销很大
    }

    void SetDamageInfoTextPos()
    {
        damageText.transform.position = mainCamera.WorldToScreenPoint(transform.position + new Vector3(0, 1, 0));  //由于UI是屏幕坐标，物体是世界坐标，因此将物体的坐标转换为屏幕坐标再赋值给UI的坐标就行
    }
}
