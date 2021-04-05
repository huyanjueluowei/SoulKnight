using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : CharacterStats
{
    private GameObject damageText;
    private Rigidbody2D rb;
    private float nextDefenceRestore;      //恢复盾牌的时间
    private Transform originalWeaponPos;   //储存weaponPos初始旋转角，当武器是刀的时候weaponPos变回来

    [Header("SkillWeapon")]
    public Transform skillWeaponPos;
    public GameObject skillFireEffect;
    private int currentSkillPoint = 0;
    private const int maxSkillPoint = 200;       //每秒增加一点，我看它是一秒调用400次(生成出来帧率降低，很奇怪），我控制10秒来一次技能
    public GameObject mainWeapon;               //主武器
    public GameObject secondWeapon;              //副武器
    public bool isSecondWeapon;                  //标记是否为副武器

    [Header("Skill")]
    public Image flashSlider;
    private bool isSkill=false;                  //来判断当前是否在使用技能
    private void Update()                //Update里面函数有点多，感觉这不是好事，有时顺序错了产生的bug都不知道怎么搞
    {   
        if (isDead == false)             //注意以下几个方法是有顺序的，不然可能出现意想不到的错误，比如Skill如果放RotateWeapon后面，技能枪还没生成，无法旋转
        {
            Skill();       
            RotateWeapon();
            ApplyWeapon();
            SwitchWeapon();
            GenerateBullet();
            KnifeAttack(); 
            RestoreDefence();
        }
        RefreshSkillUI();                //这个放外面是因为如果放里面，死了的时候身上有火，火不会消失，显得挺奇怪的
    }
    protected override void Awake()
    {
        base.Awake();
        Instantiate(mainWeapon, weaponPos);
        rb = GetComponent<Rigidbody2D>();
        originalWeaponPos = weaponPos.transform;
    }
    public void RotateWeapon()
    {
        if (GetWeapon() != null && GetWeapon().weaponData.weaponType == WeaponType.GUN)
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
            skillWeaponPos.rotation = Quaternion.Euler(0, 0, z);
            if (Mathf.Abs(z) > 90)        //枪的角度调整一下,注意不能直接改rotation，否则那改的是相对世界的旋转角度，要用Local改变相对父级角度
            {
                weaponPos.GetChild(0).transform.localEulerAngles = new Vector3(180, 0, 0);
                if (isSkill && skillWeaponPos.childCount != 0)
                    skillWeaponPos.GetChild(0).transform.localEulerAngles = new Vector3(180, 0, 0);
            }
            else
            {
                weaponPos.GetChild(0).transform.localEulerAngles = new Vector3(0, 0, 0);
                if (isSkill && skillWeaponPos.childCount != 0)
                    skillWeaponPos.GetChild(0).transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }
        else   //将weaponPos角度调整为初始角度，免得刀生成到他下面角度会很奇怪
            weaponPos.rotation = Quaternion.Euler(originalWeaponPos.rotation.x,originalWeaponPos.rotation.y,originalWeaponPos.rotation.z);
    }

    public void SwitchWeapon()
    {
        if (weaponPos.childCount == 0)
            Instantiate(mainWeapon, weaponPos);
        if(Input.GetAxis("Mouse ScrollWheel")!=0)         //鼠标滚轮切换主副手武器
        {
            if(weaponPos.childCount!=0)
                Destroy(weaponPos.GetChild(0).gameObject);
            isSecondWeapon = !isSecondWeapon;
            if (isSecondWeapon)
                Instantiate(secondWeapon, weaponPos);
            else
                Instantiate(mainWeapon, weaponPos);
        }
    }

    void GenerateBullet()         //实现生成子弹和发射子弹效果
    {
        if(Input.GetMouseButtonDown(0)&&GetWeapon().weaponData.weaponType==WeaponType.GUN&&CurrentEnergy-GetWeapon().weaponData.bulletAmount>=0) 
        {
            if(Time.time>nextFire)
            {
                nextFire = Time.time + GetWeapon().weaponData.coolDown;
                GameObject bullet = GetWeapon().weaponBulletPool.GetBullet();     //子弹池获取子弹
                CurrentEnergy-=GetWeapon().weaponData.bulletAmount;              //打一发子弹消耗对应精力
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
    void KnifeAttack()           //近战武器攻击
    {
        if(Input.GetMouseButtonDown(0) &&GetWeapon().weaponData.weaponType==WeaponType.KNIFE&&CurrentEnergy - GetWeapon().weaponData.bulletAmount >= 0)
        {
            if (Time.time > nextFire)
            {
                nextFire = Time.time + GetWeapon().weaponData.coolDown;
                if (transform.childCount == 3)      //本来底下只有武器位置、技能武器位置和愤怒特效三个，现在加一个刀光特效，且不用重复生成销毁
                    Instantiate(GetWeapon().weaponData.bulletPrefab, transform); 
                else if(GetWeapon().weaponData.bulletPrefab.name!=transform.GetChild(3).GetComponent<KnifeController>().knifeName)
                {
                    Destroy(transform.GetChild(3).gameObject);
                    Instantiate(GetWeapon().weaponData.bulletPrefab, transform);
                }
                GameObject knife = transform.GetChild(3).gameObject;
                knife.GetComponent<KnifeController>().weaponData = Instantiate(weaponData);
                knife.SetActive(true);
            }
        }
    }
    void Skill()
    {
        //if (Input.GetMouseButtonDown(1))                                      //右键技能，打子弹不消耗精力（点一下就可以连续打，巨变态哈哈），在RefreshUI里面已判断isSkill
        if (Time.time > nextFire && isSkill == true&&GetWeapon().weaponData.weaponType==WeaponType.GUN)
        {
            nextFire = Time.time + GetWeapon().weaponData.coolDown / 2;       //间隔时间缩短
            if (skillWeaponPos.childCount == 0)         //保证只生成一把枪，skill时间内每帧调用，不每帧都生成枪
            {
                GameObject skillWeapon = Instantiate(weapon,skillWeaponPos);
                skillWeapon.GetComponent<SpriteRenderer>().sortingOrder = 2;    //图层顺序改一下，在Player后面显示
            }

            GameObject bullet = GetWeapon().weaponBulletPool.GetBullet();     //子弹池获取子弹
            bullet.GetComponent<BulletController>().weaponData = Instantiate(weaponData);
            bullet.SetActive(true);

            bullet.transform.eulerAngles = skillWeaponPos.eulerAngles;   //生成后就将bullet角度与当时的weaponPos一致
            bullet.GetComponent<BulletController>().isActive = true;
            bullet.transform.position = skillWeaponPos.position;
            Vector3 dir = skillWeaponPos.transform.right;
            bullet.GetComponent<BulletController>().rb.velocity = new Vector2(dir.x, dir.y) * 20;  //二维平面给一个速度

            //正常发射的位置也要生成子弹
            GameObject bullet2 = GetWeapon().weaponBulletPool.GetBullet();     //子弹池获取子弹
            bullet2.GetComponent<BulletController>().weaponData = Instantiate(weaponData);
            bullet2.SetActive(true);                                     //显示子弹
            bullet2.transform.eulerAngles = weaponPos.eulerAngles;   //生成后就将bullet角度与当时的weaponPos一致
            bullet2.GetComponent<BulletController>().isActive = true;
            bullet2.transform.position = weaponPos.position;
            Vector3 dir2 = weaponPos.transform.right;
            bullet2.GetComponent<BulletController>().rb.velocity = new Vector2(dir2.x, dir2.y) * 20;  //二维平面给一个速度
        }
        else if(Time.time > nextFire && isSkill == true)
        {
            nextFire = Time.time + GetWeapon().weaponData.coolDown / 2;       //间隔时间缩短
            if (transform.childCount == 3)      //本来底下只有武器位置、技能武器位置和愤怒特效三个，现在加一个刀光特效，且不用重复生成销毁
                Instantiate(GetWeapon().weaponData.bulletPrefab, transform);
            else if (GetWeapon().weaponData.bulletPrefab.name != transform.GetChild(3).GetComponent<KnifeController>().knifeName)
            {
                Destroy(transform.GetChild(3).gameObject);
                Instantiate(GetWeapon().weaponData.bulletPrefab, transform);
            }
            GameObject knife = transform.GetChild(3).gameObject;
            knife.GetComponent<KnifeController>().weaponData = Instantiate(weaponData);
            knife.SetActive(true);
        }
    }



    public void RestoreDefence()
    {
        if(CurrentDefence<BaseDefence)
        {
            if (Time.time > nextDefenceRestore)
            {
                nextDefenceRestore = Time.time + 1.5f;     //每过1.5s恢复一个盾牌
                CurrentDefence++;
            }
        }
    }
    public void RefreshSkillUI()
    {
        if(isDead)
        {
            skillFireEffect.SetActive(false);
            return;
        }
        if (Time.timeScale == 1)   //我发现暂停时flashSlider还会变化，就加这个判断
        {
            if (Input.GetMouseButtonDown(1) && flashSlider.fillAmount == 1)
            {
                isSkill = true;
            }

            if (isSkill == false && currentSkillPoint < maxSkillPoint)
            {
                currentSkillPoint++;
            }
            else if (isSkill == true)
            {
                skillFireEffect.SetActive(true);                      //打开特效
                currentSkillPoint -= 2;                               //技能持续5秒钟                               
                if (currentSkillPoint <= 0)
                {
                    isSkill = false;
                    skillFireEffect.SetActive(false);
                    for (int i = 0; i < skillWeaponPos.childCount; i++)    //将技能枪位置下的武器销毁
                    {
                        Destroy(skillWeaponPos.GetChild(i).gameObject);
                    }
                }
            }
            flashSlider.fillAmount = (float)currentSkillPoint / maxSkillPoint;
        }
    }
    public void TakeDamage(WeaponData_SO weaponData)
    {
        if(CurrentDefence!=0)
        {
            CurrentDefence--;
            nextDefenceRestore = Time.time + 4f;       //受到攻击过4秒才开始恢复
        }
        else
        {
            nextDefenceRestore = Time.time + 4f;       //受到攻击过4秒才开始恢复
            float chance = Random.Range(0, 1f);
            int damage = Random.Range(weaponData.minDamage, weaponData.maxDamage + 1);
            if (chance < weaponData.criticalChance) weaponData.isCritical = true;
            else weaponData.isCritical = false;

            if (weaponData.isCritical) damage = damage * 2;
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
