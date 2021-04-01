using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

public enum EnemyStates { GUARD,CHASE,DEAD }
public class EnemyStats : CharacterStats
{
    private GameObject damageText;
    private GameObject player;
    private AIPath aiPath;
    private bool isChase=true;
    private bool isGuard = false;
    private EnemyStates enemyStates;

    protected override void Awake()
    {
        base.Awake();
        player = FindObjectOfType<PlayerController>().gameObject;
        aiPath = GetComponent<AIPath>();
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
        damage = Mathf.Max(damage - BaseDefence, 0);     //�˺������0�����ܼ�Ѫ
        CurrentHealth = Mathf.Max(CurrentHealth - damage,0);

        //damageText = GameObject.Find("DamageInfoText");   //GameObject.Findֻ����active�����壬��������������
        GameObject damageInfo = GameObject.Find("DamageInfo");   //���԰�Ҫ�õ����ֹ���һ���ɼ����������棬�������������
        damageText=damageInfo.transform.Find("DamageInfoText").gameObject;  //����Transform.Find�������������������֣����ص�Ҳ�����ҵ�
        if (weaponData.isCritical) damageText.GetComponent<Text>().color = Color.red;    //������������ʾΪ��ɫ
        else damageText.GetComponent<Text>().color = Color.yellow;            //��������������ʾΪ��ɫ

        damageText.GetComponent<Text>().text = damage.ToString();    //������������Ϊ��ǰ���˺�
        InvokeRepeating("SetDamageInfoTextPos", 0, 0.02f);            //ʵʱ�������ֵ�λ�ñ�֤�ڹ���ͷ��������ͣ����ԭ��
        damageText.SetActive(true);
        StartCoroutine(SetDamageInfoTextFalse());   //Э�������������һ���ܣ�Э�̺���ĳ���Ӱ�죬ͬʱ����

        if(CurrentHealth<=0)                 //�ж�һ���Ƿ��������������򲥷�����������ֹͣ�˶�������
        {
            isDead = true;
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
        if (GetComponent<EnemyStats>().isDead == false)
        {
            if (transform.position.x < player.transform.position.x)   //������player���ʱ�������player����֮��ת180��
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
            if (Mathf.Abs(z) > 90)        //ǹ�ĽǶȵ���һ��,ע�ⲻ��ֱ�Ӹ�rotation�������Ǹĵ�������������ת�Ƕȣ�Ҫ��Local�ı���Ը����Ƕ�
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
            if (Time.time > nextFire)
            {
                nextFire = Time.time + GetWeapon().weaponData.coolDown;
                GameObject bullet = GetWeapon().weaponBulletPool.GetBullet();     //�ӵ��ػ�ȡ�ӵ�
                bullet.GetComponent<BulletController>().weaponData = Instantiate(weaponData);
                bullet.SetActive(true);                                     //��ʾ�ӵ�
                bullet.transform.eulerAngles = weaponPos.eulerAngles;   //���ɺ�ͽ�bullet�Ƕ��뵱ʱ��weaponPosһ��
                bullet.GetComponent<BulletController>().isActive = true;
                bullet.transform.position = weaponPos.position;
                Vector3 bulletDir = weaponPos.transform.right;
                bullet.GetComponent<BulletController>().rb.velocity = new Vector2(bulletDir.x, bulletDir.y) * 20;  //��άƽ���һ���ٶ�
            }
        }
    }

    void SwitchState()           //�򵥵�״̬����ʵʱ�ı�״̬�������Ҫ�ģ����������Ƿ���뷿������
    {
        if (isDead)
            enemyStates = EnemyStates.DEAD;
        else if(GameManager.Instance.playerDead == true)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.CHASE;
        }
        switch (enemyStates)
        {
            case EnemyStates.CHASE:
                isChase = true;
                aiPath.maxSpeed = 5;
                break;
            case EnemyStates.DEAD:
                isDead = true;
                aiPath.maxSpeed = 0;
                transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                coll.enabled = false;      //��ֹ������֮���ܱ�����
                break;
            case EnemyStates.GUARD:
                isGuard = true;
                isChase = false;
                aiPath.maxSpeed = 0;
                break;
        }
    }
}
