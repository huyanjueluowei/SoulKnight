using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

public enum EnemyStates { GUARD,PATROL,CHASE,DEAD }
public class EnemyStats : CharacterStats
{
    private GameObject damageText;
    private Transform player;
    private AIPath aiPath;
    private GameObject bullet;
    private bool isChase=true;

    private EnemyStates enemyStates;

    protected override void Awake()
    {
        base.Awake();
        player = FindObjectOfType<PlayerController>().transform;
        aiPath = GetComponent<AIPath>();
        if (GetWeapon().weaponData.weaponType == WeaponType.KNIFE)
            bullet = transform.GetChild(1).gameObject;
    }
    private void Start()
    {
        GameManager.Instance.enemies.Add(gameObject);                              //自己是敌人，生成时候添加到当前敌人列表
    }
    private void Update()
    {
        Move();
        RotateWeapon();
        GenerateBullet();
        SwitchState();
    }
    public void TakeDamage(WeaponData_SO weaponData)
    {
        float chance = Random.Range(0, 1f);
        int damage = Random.Range(weaponData.minDamage, weaponData.maxDamage + 1);
        if (chance < weaponData.criticalChance) weaponData.isCritical = true;
        else weaponData.isCritical = false;

        if (weaponData.isCritical) damage *= 2;
        damage = Mathf.Max(damage - BaseDefence, 0);     //伤害最低是0，不能加血
        CurrentHealth = Mathf.Max(CurrentHealth - damage,0);

        //damageText = GameObject.Find("DamageInfoText");   //GameObject.Find只能找active的物体，不能找隐藏物体
        GameObject damageInfo = GameObject.Find("DamageInfo");   //可以把要用的文字挂在一个可见空物体下面，先找这个空物体
        damageText=damageInfo.transform.Find("DamageInfoText").gameObject;  //再用Transform.Find方法可以找子物体名字，隐藏的也可以找到
        if (weaponData.isCritical) damageText.GetComponent<Text>().color = Color.red;    //暴击让数字显示为红色
        else damageText.GetComponent<Text>().color = Color.yellow;            //不暴击让数字显示为黄色

        damageText.GetComponent<Text>().text = damage.ToString();    //更新文字内容为当前的伤害
        InvokeRepeating("SetDamageInfoTextPos", 0, 0.02f);            //实时更新文字的位置保证在怪物头顶而不是停留在原地
        damageText.SetActive(true);
        StartCoroutine(SetDamageInfoTextFalse());   //协程是与这个程序一起跑，协程后面的程序不影响，同时在跑

        if(CurrentHealth<=0)                 //判断一下是否死亡，若死亡则播放死亡动画、停止运动并销毁
        {
            isDead = true;
            anim.SetBool("dead", isDead);
            //aiPath.maxSpeed = 0;   //要获取这个组件还得先引用一下命名空间(这句话我在状态机调用)
            if(GameManager.Instance.enemies.Contains(gameObject))
                GameManager.Instance.enemies.Remove(gameObject);          //用数量计算的话有时--抽风，多减了一次，我就换成列表的形式
            Destroy(gameObject, 2f);
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

    void Move()
    {
        if (GetComponent<EnemyStats>().isDead == false)
        {
            if (transform.position.x < player.position.x)   //怪物在player左边时怪物对着player，反之则转180度
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
        }
    }

    void RotateWeapon()
    {
        if (isDead == false)
        {
            float z;
            if (player.transform.position.y > weaponPos.position.y)
            {
                z = Vector3.Angle(Vector3.right, player.transform.position - weaponPos.position);
            }
            else
            {
                z = -Vector3.Angle(Vector3.right, player.transform.position - weaponPos.position);
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

    void GenerateBullet()
    {
        if (isDead == false&&isChase==true)
        {
            if (GetWeapon().weaponData.weaponType == WeaponType.GUN&&(Time.time > nextFire))   //远程武器攻击
            {
                nextFire = Time.time + GetWeapon().weaponData.coolDown;
                GameObject bullet = GetWeapon().weaponBulletPool.GetBullet();     //子弹池获取子弹
                bullet.GetComponent<BulletController>().weaponData = Instantiate(weaponData);
                bullet.SetActive(true);                                     //显示子弹
                bullet.transform.eulerAngles = weaponPos.eulerAngles;   //生成后就将bullet角度与当时的weaponPos一致
                bullet.GetComponent<BulletController>().isActive = true;
                bullet.transform.position = weaponPos.position;
                Vector3 bulletDir = weaponPos.transform.right;
                bullet.GetComponent<BulletController>().rb.velocity = new Vector2(bulletDir.x, bulletDir.y) * 20;  //二维平面给一个速度
            }
            else if(Time.time>nextFire)    //近战武器
            {
                nextFire = Time.time + GetWeapon().weaponData.coolDown;
                bullet.GetComponent<BulletController>().weaponData = Instantiate(weaponData);
                bullet.SetActive(true);                                     //显示子弹
                aiPath.maxSpeed = 8;                                       //以很快速度冲Player
            }
        }
    }

    void SwitchState()           //简单的状态机，实时改变状态，这个还要改，根据人物是否进入房间来改
    {
        if (isDead)
            enemyStates = EnemyStates.DEAD;
        else if(GameManager.Instance.playerDead == true)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            if(Vector3.Distance(player.transform.position,transform.position)<AttackRange)
            {
                //Debug.Log("Found Player!");
                enemyStates = EnemyStates.CHASE;
            }
            else
                enemyStates = EnemyStates.PATROL;
        }
        switch (enemyStates)
        {
            case EnemyStates.PATROL:
                isChase = false;
                aiPath.maxSpeed = 3;
                if (bullet) bullet.SetActive(false);      //猪的子弹不显示
                break;
            case EnemyStates.CHASE:
                isChase = true;
                if(GetWeapon().weaponData.weaponType==WeaponType.GUN)
                    aiPath.maxSpeed = 6;
                break;
            case EnemyStates.DEAD:
                isDead = true;
                aiPath.maxSpeed = 0;
                transform.GetChild(0).GetChild(0).gameObject.SetActive(false);    //武器不显示
                if (bullet) bullet.SetActive(false);      //猪的子弹不显示
                coll.enabled = false;      //防止死亡了之后还能被攻击
                break;
            case EnemyStates.GUARD:
                isChase = false;
                aiPath.maxSpeed = 0;
                break;
        }
    }



    //private void OnDrawGizmos()        //在编辑器中可以将AttackRange范围可视化，便于调节
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(transform.position, 5);
    //}
}
