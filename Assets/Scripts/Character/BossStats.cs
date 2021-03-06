using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

public class BossStats : CharacterStats
{
    private GameObject damageText;
    private Transform player;
    private AIPath aiPath;
    private bool isChase = true;
    private float nextKnife;         //记录下一刀时间
    public GameObject bulletEffect;  //boss发一圈子弹时候特效效果
    public GameObject knife;        //boss的大砍刀
    public Slider bossBar;      //boss血量滑动条

    private EnemyStates enemyStates;

    protected override void Awake()
    {
        base.Awake();
        player = FindObjectOfType<PlayerController>().transform;
        aiPath = GetComponent<AIPath>();
    }

    private void Update()
    {
        Move();
        GenerateBullet();
        SwitchState();
        RefreshBossBar();
    }

    void RefreshBossBar()
    {
        bossBar.value = (float)CurrentHealth / MaxHealth;
    }

    public void TakeDamage(WeaponData_SO weaponData)
    {
        float chance = Random.Range(0, 1f);
        int damage = Random.Range(weaponData.minDamage, weaponData.maxDamage + 1);
        if (chance < weaponData.criticalChance) weaponData.isCritical = true;
        else weaponData.isCritical = false;

        if (weaponData.isCritical) damage *= 2;
        damage = Mathf.Max(damage - BaseDefence, 0);     //伤害最低是0，不能加血
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        //damageText = GameObject.Find("DamageInfoText");   //GameObject.Find只能找active的物体，不能找隐藏物体
        GameObject damageInfo = GameObject.Find("DamageInfo");   //可以把要用的文字挂在一个可见空物体下面，先找这个空物体
        damageText = damageInfo.transform.Find("DamageInfoText").gameObject;  //再用Transform.Find方法可以找子物体名字，隐藏的也可以找到
        if (weaponData.isCritical) damageText.GetComponent<Text>().color = Color.red;    //暴击让数字显示为红色
        else damageText.GetComponent<Text>().color = Color.yellow;            //不暴击让数字显示为黄色

        damageText.GetComponent<Text>().text = damage.ToString();    //更新文字内容为当前的伤害
        InvokeRepeating("SetDamageInfoTextPos", 0, 0.02f);            //实时更新文字的位置保证在怪物头顶而不是停留在原地
        damageText.SetActive(true);
        StartCoroutine(SetDamageInfoTextFalse());   //协程是与这个程序一起跑，协程后面的程序不影响，同时在跑

        if (CurrentHealth <= 0)                 //判断一下是否死亡，若死亡则播放死亡动画、停止运动并销毁
        {
            isDead = true;
            GameManager.Instance.isGameOver = true;    //通知GameManager游戏结束
            anim.SetBool("dead", isDead);
            //aiPath.maxSpeed = 0;   //要获取这个组件还得先引用一下命名空间(这句话我在状态机调用)
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
        if (isDead == false)
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


    void GenerateBullet()
    {
        if (isDead == false && isChase == true)
        {
            if (GetWeapon().weaponData.weaponType == WeaponType.GUN && (Time.time > nextFire))   //远程武器攻击
            {
                nextFire = Time.time + GetWeapon().weaponData.coolDown;
                int angle = 0;   //用于旋转的角度
                for (int i = 0; i < 12; i++)   //生成一圈子弹
                {
                    GameObject bullet = GetWeapon().weaponBulletPool.GetBullet();     //子弹池获取子弹
                    bullet.GetComponent<BulletController>().weaponData = Instantiate(weaponData);
                    bullet.SetActive(true);                                     //显示子弹
                    bullet.transform.eulerAngles = weaponPos.eulerAngles;   //生成后就将bullet角度与当时的weaponPos一致
                    bullet.GetComponent<BulletController>().isActive = true;
                    bullet.transform.position = weaponPos.position;
                    Vector3 bulletDir = weaponPos.transform.right;
                    Vector3 tempWeaponPos = new Vector3(weaponPos.rotation.x, weaponPos.rotation.y, weaponPos.rotation.z);
                    weaponPos.eulerAngles = new Vector3(tempWeaponPos.x, tempWeaponPos.y, tempWeaponPos.z + angle);  //weaponPos旋转30度
                    angle += 30;
                    bullet.GetComponent<BulletController>().rb.velocity = new Vector2(bulletDir.x, bulletDir.y) * 10;  //二维平面给一个速度
                }
            }
            if (Time.time > nextKnife)
            {
                nextKnife = Time.time + knife.GetComponent<BossKnife>().weaponData.coolDown;
                anim.Play("boss_chop");
            }
        }
    }

    void SwitchState()           //简单的状态机，实时改变状态，这个还要改，根据人物是否进入房间来改
    {
        if (isDead)
        {
            enemyStates = EnemyStates.DEAD;
        }
        else if (GameManager.Instance.playerDead == true || GameManager.Instance.isPlayerInBossRoom == false)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            if (Vector3.Distance(player.transform.position, transform.position) < AttackRange)
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
                knife.SetActive(false);
                anim.Play("boss_walk");
                break;
            case EnemyStates.CHASE:
                isChase = true;
                knife.SetActive(true);
                if (GetWeapon().weaponData.weaponType == WeaponType.GUN)
                    aiPath.maxSpeed = 6;
                anim.Play("boss_chop");
                break;
            case EnemyStates.DEAD:
                isDead = true;
                aiPath.maxSpeed = 0;
                transform.GetChild(0).GetChild(0).gameObject.SetActive(false);    //武器不显示
                knife.SetActive(false);
                coll.enabled = false;      //防止死亡了之后还能被攻击
                anim.Play("boss_dead");
                break;
            case EnemyStates.GUARD:
                isChase = false;
                knife.SetActive(false);
                aiPath.maxSpeed = 0;
                anim.Play("boss_idle");
                break;
        }
    }


    //private void OnDrawGizmos()        //在编辑器中可以将AttackRange范围可视化，便于调节
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(transform.position, 5);
    //}
}
