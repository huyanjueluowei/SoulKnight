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
    private float nextKnife;         //��¼��һ��ʱ��
    public GameObject bulletEffect;  //boss��һȦ�ӵ�ʱ����ЧЧ��
    public GameObject knife;        //boss�Ĵ󿳵�
    public Slider bossBar;      //bossѪ��������

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
        damage = Mathf.Max(damage - BaseDefence, 0);     //�˺������0�����ܼ�Ѫ
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        //damageText = GameObject.Find("DamageInfoText");   //GameObject.Findֻ����active�����壬��������������
        GameObject damageInfo = GameObject.Find("DamageInfo");   //���԰�Ҫ�õ����ֹ���һ���ɼ����������棬�������������
        damageText = damageInfo.transform.Find("DamageInfoText").gameObject;  //����Transform.Find�������������������֣����ص�Ҳ�����ҵ�
        if (weaponData.isCritical) damageText.GetComponent<Text>().color = Color.red;    //������������ʾΪ��ɫ
        else damageText.GetComponent<Text>().color = Color.yellow;            //��������������ʾΪ��ɫ

        damageText.GetComponent<Text>().text = damage.ToString();    //������������Ϊ��ǰ���˺�
        InvokeRepeating("SetDamageInfoTextPos", 0, 0.02f);            //ʵʱ�������ֵ�λ�ñ�֤�ڹ���ͷ��������ͣ����ԭ��
        damageText.SetActive(true);
        StartCoroutine(SetDamageInfoTextFalse());   //Э�������������һ���ܣ�Э�̺���ĳ���Ӱ�죬ͬʱ����

        if (CurrentHealth <= 0)                 //�ж�һ���Ƿ��������������򲥷�����������ֹͣ�˶�������
        {
            isDead = true;
            GameManager.Instance.isGameOver = true;    //֪ͨGameManager��Ϸ����
            anim.SetBool("dead", isDead);
            //aiPath.maxSpeed = 0;   //Ҫ��ȡ����������������һ�������ռ�(��仰����״̬������)
            Destroy(gameObject, 2f);
        }
    }

    IEnumerator SetDamageInfoTextFalse()
    {
        yield return new WaitForSeconds(0.5f);   //ע��WaitForSecondsǰ����new,�������ʱ��֮�����������ִ��
        damageText.SetActive(false);
        CancelInvoke("SetDamageInfoTextPos");    //���ֹرպ�Ͱ�InvokeRepeating�رգ��������ֹرջ�һֱ���ã������ܴ�
    }

    void SetDamageInfoTextPos()
    {
        damageText.transform.position = mainCamera.WorldToScreenPoint(transform.position + new Vector3(0, 1, 0));  //����UI����Ļ���꣬�������������꣬��˽����������ת��Ϊ��Ļ�����ٸ�ֵ��UI���������
    }

    void Move()
    {
        if (isDead == false)
        {
            if (transform.position.x < player.position.x)   //������player���ʱ�������player����֮��ת180��
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
            if (GetWeapon().weaponData.weaponType == WeaponType.GUN && (Time.time > nextFire))   //Զ����������
            {
                nextFire = Time.time + GetWeapon().weaponData.coolDown;
                int angle = 0;   //������ת�ĽǶ�
                for (int i = 0; i < 12; i++)   //����һȦ�ӵ�
                {
                    GameObject bullet = GetWeapon().weaponBulletPool.GetBullet();     //�ӵ��ػ�ȡ�ӵ�
                    bullet.GetComponent<BulletController>().weaponData = Instantiate(weaponData);
                    bullet.SetActive(true);                                     //��ʾ�ӵ�
                    bullet.transform.eulerAngles = weaponPos.eulerAngles;   //���ɺ�ͽ�bullet�Ƕ��뵱ʱ��weaponPosһ��
                    bullet.GetComponent<BulletController>().isActive = true;
                    bullet.transform.position = weaponPos.position;
                    Vector3 bulletDir = weaponPos.transform.right;
                    Vector3 tempWeaponPos = new Vector3(weaponPos.rotation.x, weaponPos.rotation.y, weaponPos.rotation.z);
                    weaponPos.eulerAngles = new Vector3(tempWeaponPos.x, tempWeaponPos.y, tempWeaponPos.z + angle);  //weaponPos��ת30��
                    angle += 30;
                    bullet.GetComponent<BulletController>().rb.velocity = new Vector2(bulletDir.x, bulletDir.y) * 10;  //��άƽ���һ���ٶ�
                }
            }
            if (Time.time > nextKnife)
            {
                nextKnife = Time.time + knife.GetComponent<BossKnife>().weaponData.coolDown;
                anim.Play("boss_chop");
            }
        }
    }

    void SwitchState()           //�򵥵�״̬����ʵʱ�ı�״̬�������Ҫ�ģ����������Ƿ���뷿������
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
                transform.GetChild(0).GetChild(0).gameObject.SetActive(false);    //��������ʾ
                knife.SetActive(false);
                coll.enabled = false;      //��ֹ������֮���ܱ�����
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


    //private void OnDrawGizmos()        //�ڱ༭���п��Խ�AttackRange��Χ���ӻ������ڵ���
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(transform.position, 5);
    //}
}
