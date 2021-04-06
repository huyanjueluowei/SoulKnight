# Soul_knight说明文档

## 一、玩家角度实现的功能

（1）WASD控制人物移动

（2）左键开炮，闪电条满后按右键释放技能

（3）打完第一波小怪和第二波精英怪后生成武器宝箱，靠近宝箱打开，按空格键可以拾取武器

（4）靠近最右边房间饮水机，按空格可以恢复满血和精力，只能使用一次

（5）打完boss后左下角传送门开启，靠近按空格，通关！

（6）刀光、人物技能、人物光环等各种骚炫特效





## 二、程序角度实现的功能

### 1、人物移动    

主要方法：Input.GetAxisRaw(string axisName)，获取Horizontal和Vertical方向上的输入值，将向量值赋给人物刚体速度

```c#
float horizontalInput = Input.GetAxisRaw("Horizontal");
float verticalInput = Input.GetAxisRaw("Vertical");

rb.velocity = new Vector2(horizontalInput * speed, verticalInput * speed);
```



### 2、鼠标移动控制枪口旋转    

主要方法：Vector3.Angle(Vector3 from,Vector3 to)，从x轴正方向开始旋转，旋转角度为鼠标位置-人物武器位置

```c#
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
```





### 3、人物、武器数据设定，用到ScriptableObject

（1）人物基本数据

```c#
[CreateAssetMenu(fileName = "New Data", menuName = "CharacterStats/Data")]
public class Character_SO : ScriptableObject
{
    [Header("Base Info")]
    public int maxHealth;
    public int currentHealth;
    public int baseDefence;
    public int currentDefence;
    public int maxEnergy;
    public int currentEnergy;


    [Header("Attack Info")]
    public float attackRange;
    public float coolDown;
    public int minDamage;
    public int maxDamage;
    public float criticalChance;      //暴击率

    public void ApplyWeaponData(WeaponData_SO weapon)  //人物会调用，将武器数据赋给自己
    {
        attackRange = weapon.attackRange;
        coolDown = weapon.coolDown;
        minDamage = weapon.minDamage;
        maxDamage = weapon.maxDamage;
        criticalChance = weapon.criticalChance;
    }
}
```



（2）武器数据

```c#
public class WeaponData_SO : ScriptableObject
{
    [Header("Attack Info")]
    public float attackRange;
    public float coolDown;
    public int minDamage;
    public int maxDamage;
    public int bulletAmount;          //一发子弹要消耗多少精力
    public float criticalChance;      //暴击率
    public string weaponName;
    public string bulletPoolName;     //子弹池名字
    public WeaponType weaponType;     //武器类型
    public WeaponLevel weaponLevel;   //武器级别
    public GameObject weaponPrefab;   //武器预制体
    public GameObject weaponOnGroundPrefab;   //在地上的武器预制体

    public bool isCritical = false;

    public GameObject bulletPrefab;
    bool onGround;   //判断是否在地面上
}
```



（3）数据读取

​		由于人物会经常调用到ScriptableObject中的数据，调用到一个变量都要characterData.变量，很繁琐，因此在PlayerStats和enemyStats的共同父类上加上了读取数据的方法，定义若干属性变量，可以通过get和set对characterData中的数据进行读写，因此人物调用数据时直接调用属性变量即可，省去了很多代码量

```c#
#region Read from Data_SO   
public int MaxHealth    //设置这么多属性变量好处可以在调用时简洁一点，直接CharacterStats.MaxHealth而不用CharacterStats.characterData.maxHealth
{  
    get { if (characterData != null) return characterData.maxHealth; else return 0; }
    set { characterData.maxHealth = value; }
}
public int CurrentHealth
{
    get { if (characterData != null) return characterData.currentHealth; else return 0; }
    set { characterData.currentHealth = value; }
}
public int BaseDefence
{
    get { if (characterData != null) return characterData.baseDefence; else return 0; }
    set { characterData.baseDefence = value; }
}
public int CurrentDefence
{
    get { if (characterData != null) return characterData.currentDefence; else return 0; }
    set { characterData.currentDefence = value; }
}
public int MaxEnergy
{
    get { if (characterData != null) return characterData.maxEnergy; else return 0; }
    set { characterData.maxEnergy = value; }
}
public int CurrentEnergy
{
    get { if (characterData != null) return characterData.currentEnergy; else return 0; }
    set { characterData.currentEnergy = value; }
}

public float AttackRange
{
    get { if (characterData != null) return characterData.attackRange; else return 0; }
    set { characterData.attackRange = value; }
}
#endregion
```





### 4、左键开火

主要方法：对象池的使用。这次没有用普通的生成和销毁，直接用对象池，学了一下，也不复杂，就是先生成若干对象，要用的时候从对象池中获取，重新定位，要消失时不用销毁方法，而是隐藏并放入对象池。



（1）对象池：每个池子有自己的子弹预制体，根据游戏情况设定初始生成的子弹数，每次获取子弹就获取列表中currentIndex处的子弹，并且每次获取后currentIndex没有重新变为0，而是停在上一个最后获取到的子弹，这样在获取下一个时就一定可以获取（前提是子弹池中还有没用到的子弹）

```c#
public class BulletPool : MonoBehaviour
{
    public GameObject bulletPrefab;
    public int poolAmount;          //要在编辑器中设定
    public List<GameObject> bullets = new List<GameObject>();
    private int currentIndex=0;

    private void Start()
    {
        for(int i=0;i<poolAmount;i++)
        {
            GameObject obj = Instantiate(bulletPrefab);
            obj.transform.SetParent(transform);
            obj.SetActive(false);
            bullets.Add(obj);
        }
    }

    public GameObject GetBullet()
    {
        for(int i=0;i<bullets.Count;i++)
        {
            int tempIndex = (currentIndex + i) % bullets.Count;
            if(!bullets[tempIndex].activeInHierarchy)
            {
                currentIndex = (tempIndex+1) % bullets.Count;
                //Debug.Log("可以获取到子弹");
                return bullets[tempIndex];
            }
        }
        //如果列表中每个子弹都处于活动状态就再添加子弹到列表中并返回
        GameObject obj = Instantiate(bulletPrefab);
        obj.transform.SetParent(transform);
        bullets.Add(obj);
        return obj;
    }
}
```



（2）点击鼠标左键开火

主要思路：用到计时器，结合Time.time和nextFire计算是否可以发射子弹，如果Time.time>nextFire就立马让nextFire=Time.time+间隔时间，这样就能保证过一定时间之后才能发射子弹，子弹上面有刚体，生成之后给它一个速度矢量，子弹就会向矢量方向移动



```c#
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
```



### 5、右键技能

​		右键技能，如果拿枪就会生成两把一样的枪，并且生成双倍的子弹，冷却时间缩短，且不消耗能量（这个设定比较变态），并且用两个变量currentSkillPoint和maxSkillPoint来标记技能持续时间，总体代码在普通开火代码上稍作修改



```c#
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
```







### 6、可破坏的宝箱，奖励武器

​		宝箱是由几个部分拼装起来的，由于靠近宝箱盖子要平滑打开，因此主要对左半边盖子和右半边盖子进行操作，用到Vector3.MoveTowards(Vector3 current,Vector3 target,float maxDistanceDelta)方法，maxDistanceDelta是一个数（往往是自己定义的速度int speed)，在Update里面执行



​		说明：宝箱的结构为（前两个完全不会用到）

​		transform.GetChild(0)是宝箱主体

​		transform.GetChild(1)是宝箱盖子

​		transform.GetChild(2)是盖子左半边

​		transform.GetChild(3)是盖子右半边

​		transform.GetChild(4)是weaponPos，放置武器的位置

​		transform.GetChild(5)是一个Canvas，用于显示武器文本信息

```c#
private void Update()       //平滑打开宝箱
{
    if(isOpen==true&&Vector3.Distance(transform.GetChild(2).position, leftEnd) > 0.01f && Vector3.Distance(transform.GetChild(3).position, rightEnd) > 0.01f)
    {
        transform.GetChild(2).position = Vector3.MoveTowards(transform.GetChild(2).position, leftEnd, speed*Time.deltaTime);
        transform.GetChild(3).position = Vector3.MoveTowards(transform.GetChild(3).position, rightEnd, speed*Time.deltaTime);
        if (Vector3.Distance(transform.GetChild(2).position, leftEnd) < 0.012f && Vector3.Distance(transform.GetChild(3).position, rightEnd) < 0.012f)
            isOpenFinished = true;
    }
    RewardWeapon();            //isOpenFinished=true才执行，只会执行一次
    if (transform.GetChild(4).childCount!=0)  rewardWeapon= transform.GetChild(4).GetChild(0).gameObject;
}
```



```c#
void RewardWeapon()           //奖励武器
{
    if (isOpenFinished == true && isRewarded == false)
    {
        int weaponIndex = Random.Range(0, weapons.Length);
        rewardWeapon=Instantiate(weapons[weaponIndex], transform.GetChild(4));
        isRewarded = true;
    }
}
```







### 7、人物受到伤害

​		不管是人物还是敌人受到伤害，都要获取武器的伤害damage，而且伤害是在子弹碰到人物触发，因此想到在子弹上添加一个weaponData_SO，在子弹启动时就把主动方手上的weaponData赋给子弹的weaponData，碰到敌人触发时就调用敌人身上的TakeDamage函数，形参为WeaponData_SO weaponData



```c#
public void TakeDamage(WeaponData_SO weaponData)  //Player受到伤害
{
    if(CurrentDefence!=0)          //受到攻击先减少盾牌数量
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
```



```c#
public void TakeDamage(WeaponData_SO weaponData)    //敌人受到伤害
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
        if (isElite == false && GameManager.Instance.enemiesOne.Contains(gameObject))
            GameManager.Instance.enemiesOne.Remove(gameObject);          //用数量计算的话有时--抽风，多减了一次，我就换成列表的形式
        else if (isElite == true && GameManager.Instance.enemiesTwo.Contains(gameObject))
        {
            debutEffect.SetActive(false);
            GameManager.Instance.enemiesTwo.Remove(gameObject);
        }
        Destroy(gameObject, 2f);
    }
}
```

​		细节：受到伤害时人物头上会显示UI伤害文字，这个用到Camera.main.WorldToScreenPoint将世界坐标转化为屏幕坐标赋值给文字，并且在接下来几秒内用InvokeRepeating重复调用这个方法，保证人物移动时文字也能停留在人物头顶，在协程中yield return new WaitForSeconds方法等待几秒后将文字关闭，并利用CancelInvoke方法停止调用



```c#
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
```





### 8、敌人状态机

​		敌人有四个状态：GUARD,PATROL,CHASE,DEAD，用一个列举enum定义，每个状态都有对应的布尔变量，分别为isGuard,isPatrol,isChase,isDead，初始时isGuard为true,其他为false

​		敌人状态切换写在SwitchState方法中，该方法在Update中调用，实时更换状态，方法中首先根据四个bool变量的值切换敌人的状态，再用switch语句根据敌人的状态执行相应动作，播放相应动画（有些地方我没处理好，比如boss身上，当时没时间了，动画这方面没有好好利用动画器参数，而是直接用anim.Play来播放动画）

```c#
void SwitchState()           //简单的状态机，实时改变状态，这个还要改，根据人物是否进入房间来改
{
    if (isDead)
        enemyStates = EnemyStates.DEAD;
    else if(GameManager.Instance.playerDead == true||GameManager.Instance.isPlayerInMainRoom==false)
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
```





### 9、宝箱的生成

​		在杀死普通怪物和精英怪物后会生成宝箱，这个属于游戏管理范围，由GameManager来生成宝箱，GameManager写成一个单例，用到泛型，这是一个重要的知识点，我用的还不是特别熟练，单例脚本在整个游戏只存在一个实例，下面是单例代码，GameManager继承它：

```c#
//泛型通常在类名之后加上尖括号，里面是类型T，T可以代表任何类型,如MouseManager,GameManager等等
public class Singleton<T> : MonoBehaviour where T : Singleton<T>  //约束，必须是singleton类型
{
    private static T instance;
    public static T Instance
    {
        get { return instance; }   //想让外部可以访问instance，但是又不能修改它
    }

    protected virtual void Awake()  //protected表明只能在自己和子类中访问，virtual为了让Awake在子类中可以override重写
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = (T)this;  //由于是各种类继承，因此赋予this要加上泛型转换T
    }
    public static bool IsInitialized   //表示泛型单例是否已经生成以供其他脚本获取，注意static关键字不能掉了
    {
        get { return instance != null; }
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
```



GameManager继承单例的写法

```c#
public class GameManager : Singleton<GameManager>
```



在GameManager中定义了两个GameObject数组，分别代表第一波和第二怪物，在敌人生成时Start中（不放在Awake中是因为可能GameManager还没来得及生成，可能出现报错）就通知GameManager,将自己添加到数组，死亡后检查数组中有无自己，有则移除。直到两个数组都为空并且第一阶段怪物清除标记isFirstStageEnd==true时才生成宝箱



```c#
void GenerateRewardBox()
{
    if(enemiesOne.Count==0&&enemiesTwo.Count==0&&isFirstStageEnd==false)           
    {
        isFirstStageEnd = true;
        for(int i=0;i<blueRewardBoxes.Length;i++)
        {
            blueRewardBoxes[i].SetActive(true);  //生成8个宝箱，我做了8个武器，4把枪4把刀
        }
    }
}
```







### 10、切换武器、拾取武器

（1）滚轮切换武器

滚轮涉及输入系统中的Mouse ScrollWheel,代码中就是Input.GetAxis("Mouse ScrollWheel")，返回一个float值，如果往上滑就大于0，往下滑就小于0，在不等于0时就切换武器

```c#
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
```



（2）拾取武器

​        这一部分比较难做，本来我打算直接在武器上加碰撞体，用直接交换位置的方法去换，但是发现这样写对位置操作会有很多繁琐的代码，不易读而且武器放在地上角度也不对，因此我将武器调整了角度又做了一套放在地上的武器预制体，当玩家在武器碰撞体范围内时若按下空格键可捡起武器，因此把拾取武器的逻辑挂到每一个放在地上的武器预制体上。

​		用到了OnTriggerStay2D，就是其他触发器接触该触发器时每帧都会调用的方法，在这里我有个地方没有处理好，就是我把输入的判定也放在这个里面，一般OnTriggerStay2D里面的方法和物理有关，检测输入应该放在Update里面判断，所以我放在OnTriggerStay2D里面检测输入可能出现不灵敏的情况，有时候会没有响应。



​		拾取武器逻辑：首先地上的武器要获取到player，通过GameManager来获取，因为在player也只有一个，在生成的时候就告诉GameManager自己是player，然后将player手上拿的武器的地上武器预制体生成到宝箱的位置，再将宝箱武器的手上预制体生成到玩家的手上（weaponPoint下面,此时生成到第二个位置），下一步立马把weaponPoint下面第一个武器销毁。

​		至于宝箱中原来武器的销毁交给武器容器来执行，每一帧检测容器中武器数量，如果大于1就把第二个武器（也就是待放到宝箱里的武器）放到第一个，用到SetSiblingIndex(int index)方法，再将后面的武器全部销毁，保证宝箱中一直只会有一把武器。综上就实现了拾取武器的功能，同时玩家多次按空格也可以多次放下、拾取。



拾取武器主要脚本（挂在每一个放在地上的武器预制体上）：

```c#
public class WeaponItem : MonoBehaviour   //这个脚本挂在放在地上的武器预制体上
{
    private GameObject weaponOnHand;  //拿到在手上用的预制体
    private PlayerStats playerStats;
    private void Awake()
    {
        weaponOnHand = GetComponent<WeaponController>().weaponData.weaponPrefab;
    }

    private void Start()    //我没放awake里面获取怕获取不到
    {
        playerStats = GameManager.Instance.playerStats;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (playerStats.isSecondWeapon == false)
            {
                Instantiate(playerStats.mainWeapon.GetComponent<WeaponController>().weaponData.weaponOnGroundPrefab, transform.parent); //把玩家手上武器生成到地上
                playerStats.mainWeapon = weaponOnHand;                  //更换玩家的主武器
                Destroy(playerStats.weaponPos.GetChild(0).gameObject);
            }         
            else
            {
                Instantiate(playerStats.secondWeapon.GetComponent<WeaponController>().weaponData.weaponOnGroundPrefab, transform.parent); //把玩家手上武器生成到地上
                playerStats.secondWeapon = weaponOnHand;                  //更换玩家的副武器
                Destroy(playerStats.weaponPos.GetChild(0).gameObject);
            }
        }
    }
}
```



实时检测宝箱中武器数量代码：

```c#
void DestroyWeapon()
{
    if (transform.childCount > 1)
    {
        transform.GetChild(1).SetSiblingIndex(0);   //本来是生成在第二个的，把它放在第一个来
        for (int i = 1; i < transform.childCount; i++)   //很鬼畜，它有时候会生成很多武器在地上，直接都销毁
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
```



另外每把武器有自己的等级，我设定了五种等级：普通、精良、稀有、史诗、传说武器，五种品质对应不同颜色，宝箱会根据武器的等级在武器上方显示武器名字并调节文字为对应的颜色

```c#
void SetRewardTextPos()
{
    if (transform.childCount != 0)  //习惯性判空以免报错
    {
        Vector3 rewardTextPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 2f, 0));
        weaponInfoText.transform.position = rewardTextPos;
        weaponInfoText.GetComponent<Text>().text = transform.GetChild(0).GetComponent<WeaponController>().weaponData.weaponName;
        switch(transform.GetChild(0).GetComponent<WeaponController>().weaponData.weaponLevel)
        {
            case WeaponLevel.NORMAL:     //辣鸡武器
                weaponInfoText.GetComponent<Text>().color = Color.white;
                break;
            case WeaponLevel.GOOD:      //还能用的武器
                weaponInfoText.GetComponent<Text>().color = Color.green;
                break;
            case WeaponLevel.RARE:      //稀有武器
                weaponInfoText.GetComponent<Text>().color = Color.blue;
                break;
            case WeaponLevel.EPIC:      //神经级武器
                weaponInfoText.GetComponent<Text>().color = new Color32(255, 0, 255, 255);  //没有Color.purple我就找purple的32位值，用new Color32赋值
                break;
            case WeaponLevel.LEGEND:    //一刀999武器
                weaponInfoText.GetComponent<Text>().color = Color.yellow;
                break;
        }
        weaponInfoText.SetActive(true);
    }
}
```





### 11、boss环形子弹

Player靠近boss时，若冷却时间结束，会生成一圈环形子弹

逻辑：在原有生成子弹方法基础上利用一个for循环，定义一个局部变量angle，生成一个子弹angle加30度，并根据angle调整weaponPos的eulerAngles，这样for循环12次即可生成一圈子弹

```c#
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
```



## 三、引用的插件和包

这个项目只用到了一个插件和一个包

（1）插件：Cinemachine。这个插件可以让摄像机镜头实时跟随一个物体。

本来还装了一个插件2D extras，但是不太会用，本来想用这个画出游戏中色彩斑斓的地面，素材不好找也不太会用，就没有用到这个插件，所有瓦片都是一个颜色。

（2）包：用到了Pathfinding包，这个包功能很强大，2d和3d中都可以用，可以自动寻路、绕过障碍物寻找指定目标，如果单纯写怪物追人还是很好写，但如果有很多障碍物的话就不太容易，所以在怪物AI上采用了这个包。







## 四、代码架构

项目总共写了26个脚本，一千多行代码，不过大部分脚本内容简单也不多，围绕人物的状态来扩展即可，核心脚本为GameManager,characterStats,playerStats和enemyStats，GameManager用来控制游戏总进程，characterStats用于存储人物的数据，playerStats和enemyStats均继承characterStats，写入各种与数据有关的方法。



26个脚本及功能如下：

（1）PlayerController，这个脚本中的内容不涉及数据，只写了一个方法，WASD控制人物移动

（2）CharacterData_SO，定义人物数据

（3）WeaponData_SO，定义武器数据

（4）CharacterStats，存储、读写人物数据，是playerStats、enemyStats、bossStats的父类

（5）PlayerStats，执行获取子弹、攻击、受到伤害等与数据有关的功能

（6）EnemyStats，执行获取子弹、受到伤害、切换状态等与数据有关的功能

（7）BossStats，boss独有的脚本，由于攻击方式及一些判定与普通怪物有些不同，专门写一个boss脚本

（8）WeaponController，挂在手中拿的武器上，存储武器数据和子弹池对象

（9）BulletPool，挂在子弹池根物体上，存储子弹池数量，子弹预制体，实现管理子弹池子弹功能（获取、添加子弹）

（10）BulletController，挂在远程子弹预制体上，存储武器数据，主要实现碰撞到人物时让被碰撞者受到伤害功能

（11）KnifeController，挂在刀光（近程）子弹上，存储武器数据和刀光名称（用于检测场景中的武器刀光与手上刀的数据里的刀光是否一致，若不一致则换成对应的刀光），主要实现让被碰撞者受到伤害功能

（12）BossKnife，挂在boss上，只针对player，只判断player标签进行伤害

（13）GameManager，挂在空物体GameManager上，控制游戏进程，定义许多检测游戏进程的bool变量，如isFirstStageEnd（第一阶段是否结束），isGameOver（boss是否被消灭）,isPlayerInMainRoom（player是否走进了主房间，切换敌人状态）,isPlayerInBossRoom,isEliteGenerated（精英怪是否生成）等变量。  

​			获取游戏中的敌人和player，存储随游戏进程生成的宝箱、传送门等，并根据进程bool变量对人物状态和物品生成进行控制

（14）Singleton，不挂在物体上，这是一个单例工具，可被任何脚本继承，如果想写成单例模式就让脚本继承它

（15）RewardBoxController，挂在宝箱根物体上，存储武器列表，实现宝箱开关和生成奖励武器的功能

（16）WeaponContainer，挂在宝箱根物体的第5个子物体--WeaponPos上，宝箱的武器会生成在它之下，因此它相当于武器的Container，用它来管理Container中的武器，在拾取武器时负责调整武器顺序并销毁多余武器，并负责武器文字的显示

（17）WeaponItem，挂在地上放置的物体上，用于实现检测Player是否靠近以及武器的拾取功能

（18）DoorOpen，挂在主房间的OpenDoor（TileMap）上（是触发器，无刚体），当Player走近时将门关闭（让虚门关闭，四周实体门启动）

（19）DoorClose，挂在主房间的CloseDoor（TileMap）上（有刚体），实时检测GameManager中的进程bool变量isFirstStageEnd是否为true，若为true则关闭CloseDoor，启动OpenDoor

（20）BossDoorOpen，挂在boss房间门（TileMap）上（是触发器，无刚体），当Player走近时将GameManager中的进程bool变量isPlayerInBossRoom设定为true（其实好像没用到这个变量），并显示boss血条

（21）TransitionDoor，挂在左下角房间的传送门上，当GameManager中的进程bool变量isGameOver=true时启动传送门，player走近并按下空格时加载通关场景

（22）GotoMain，挂在通关场景的背景图片上，通关场景整个屏幕都是一个按钮，点击任意一个位置都会调用GotoMain中的回到主菜单函数

（23）GameSceneUI，挂在游戏场景的空物体GameSceneUI上，用于控制esc菜单显示，菜单打开时Time.timeScale=0暂停，关闭时Time.timeScale=1

（24）HealthBarUI，挂在人物血量条根物体下，用于实时更新三个滑动条的value以及数据显示

（25）UIManager，挂在主菜单的空物体UIManager上，用于控制异步加载面板的显示，开始按钮事件，退出游戏事件等

（26）WaterController，挂在游戏场景最右边房间的饮水机上，人物靠近按空格可以恢复所有血量和能量，只能使用一次，再次按空格无效果







## 五、感想与反思

​		这次实习是我第一次独立做游戏（所有代码都靠自己所学或上网查找博客来写，没有依赖其他视频或其他同学的代码），感觉很有成就感，7天也算做了一个有模有样的游戏，每天平均做5个小时，实现几个功能，总的来说我的项目没有设计特别复杂的算法或功能，都是一些比较简单的功能，不过我学unity也没有很久，通过这次实习复习一下基础常用的api也蛮不错的。

​		项目优点：在以前看的教程影响下，我的项目里面每个文件夹分类放好各种物品，方便查找，代码，函数名，脚本名命名尽量规范、完整，整个项目制作完成后跑起来没有任何红色或黄色提示信息（Unity抽风除外）。

​		项目缺点及需要改进的地方：

​        (1)最复杂的功能------怪物AI以及随机地下城功能没有实现，这个感觉涉及一些复杂的算法知识，我算法水平比较有限，就没能独立做出来。

​		(2)打击感方面可以加强，打击感我认为主要来源于三个方面，1、刀光     2、刀光或子弹打在敌人身上产生的特效   3、敌人被打击时的动画或击退效果。  		这个项目里面我只实现了第一点，选了一堆特别骚炫的刀光特效，子弹还好，但是刀光就没啥打击感，第二点我觉得用些粒子特效会比较好，但是粒子特效我用的不熟也不太会做。第三点，在素材中没有被击打的动画素材，击退效果我觉得还好做，可以在刚体上加一个脉冲力。

​		还有一个重要细节我没做出来，就是刀攻击时刀要砍下去，这个细节我有尝试过，但感觉不太好做，如果有帧动画肯定简单，没有帧动画就要旋转，这个我不知道是代码控制还是可以做成一个动画，感觉有点复杂，在我做的游戏中刀位置是一直没变的。

​		(3)boss状态机可以进一步完善，boss我是最后一天晚上做的，动画方面就是一个状态一个动画，都没有怎么用到动画器，如果好好利用动画器，多设一些变量来控制动画可能会让boss动画看起来更流畅。

​		(4)代码风格和架构有待优化，有些函数为了实现一个小功能堆砌了很多行代码，也许可以用更精炼的代码来解决。代码中有些数据和功能实现上可能也有重复，如武器的attackRange其实并没怎么用到，一些函数的功能也有大片重复，但是被反复写了很多遍，这也是可以优化的点。