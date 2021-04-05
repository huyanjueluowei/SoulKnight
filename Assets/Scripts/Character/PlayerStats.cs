using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : CharacterStats
{
    private GameObject damageText;
    private Rigidbody2D rb;
    private float nextDefenceRestore;      //�ָ����Ƶ�ʱ��
    private Transform originalWeaponPos;   //����weaponPos��ʼ��ת�ǣ��������ǵ���ʱ��weaponPos�����

    [Header("SkillWeapon")]
    public Transform skillWeaponPos;
    public GameObject skillFireEffect;
    private int currentSkillPoint = 0;
    private const int maxSkillPoint = 200;       //ÿ������һ�㣬�ҿ�����һ�����400��(���ɳ���֡�ʽ��ͣ�����֣����ҿ���10����һ�μ���
    public GameObject mainWeapon;               //������
    public GameObject secondWeapon;              //������
    public bool isSecondWeapon;                  //����Ƿ�Ϊ������

    [Header("Skill")]
    public Image flashSlider;
    private bool isSkill=false;                  //���жϵ�ǰ�Ƿ���ʹ�ü���
    private void Update()                //Update���溯���е�࣬�о��ⲻ�Ǻ��£���ʱ˳����˲�����bug����֪����ô��
    {   
        if (isDead == false)             //ע�����¼�����������˳��ģ���Ȼ���ܳ������벻���Ĵ��󣬱���Skill�����RotateWeapon���棬����ǹ��û���ɣ��޷���ת
        {
            Skill();       
            RotateWeapon();
            ApplyWeapon();
            SwitchWeapon();
            GenerateBullet();
            KnifeAttack(); 
            RestoreDefence();
        }
        RefreshSkillUI();                //�������������Ϊ��������棬���˵�ʱ�������л𣬻𲻻���ʧ���Ե�ͦ��ֵ�
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
            skillWeaponPos.rotation = Quaternion.Euler(0, 0, z);
            if (Mathf.Abs(z) > 90)        //ǹ�ĽǶȵ���һ��,ע�ⲻ��ֱ�Ӹ�rotation�������Ǹĵ�������������ת�Ƕȣ�Ҫ��Local�ı���Ը����Ƕ�
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
        else   //��weaponPos�Ƕȵ���Ϊ��ʼ�Ƕȣ���õ����ɵ�������ǶȻ�����
            weaponPos.rotation = Quaternion.Euler(originalWeaponPos.rotation.x,originalWeaponPos.rotation.y,originalWeaponPos.rotation.z);
    }

    public void SwitchWeapon()
    {
        if (weaponPos.childCount == 0)
            Instantiate(mainWeapon, weaponPos);
        if(Input.GetAxis("Mouse ScrollWheel")!=0)         //�������л�����������
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

    void GenerateBullet()         //ʵ�������ӵ��ͷ����ӵ�Ч��
    {
        if(Input.GetMouseButtonDown(0)&&GetWeapon().weaponData.weaponType==WeaponType.GUN&&CurrentEnergy-GetWeapon().weaponData.bulletAmount>=0) 
        {
            if(Time.time>nextFire)
            {
                nextFire = Time.time + GetWeapon().weaponData.coolDown;
                GameObject bullet = GetWeapon().weaponBulletPool.GetBullet();     //�ӵ��ػ�ȡ�ӵ�
                CurrentEnergy-=GetWeapon().weaponData.bulletAmount;              //��һ���ӵ����Ķ�Ӧ����
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
    void KnifeAttack()           //��ս��������
    {
        if(Input.GetMouseButtonDown(0) &&GetWeapon().weaponData.weaponType==WeaponType.KNIFE&&CurrentEnergy - GetWeapon().weaponData.bulletAmount >= 0)
        {
            if (Time.time > nextFire)
            {
                nextFire = Time.time + GetWeapon().weaponData.coolDown;
                if (transform.childCount == 3)      //��������ֻ������λ�á���������λ�úͷ�ŭ��Ч���������ڼ�һ��������Ч���Ҳ����ظ���������
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
        //if (Input.GetMouseButtonDown(1))                                      //�Ҽ����ܣ����ӵ������ľ�������һ�¾Ϳ��������򣬾ޱ�̬����������RefreshUI�������ж�isSkill
        if (Time.time > nextFire && isSkill == true&&GetWeapon().weaponData.weaponType==WeaponType.GUN)
        {
            nextFire = Time.time + GetWeapon().weaponData.coolDown / 2;       //���ʱ������
            if (skillWeaponPos.childCount == 0)         //��ֻ֤����һ��ǹ��skillʱ����ÿ֡���ã���ÿ֡������ǹ
            {
                GameObject skillWeapon = Instantiate(weapon,skillWeaponPos);
                skillWeapon.GetComponent<SpriteRenderer>().sortingOrder = 2;    //ͼ��˳���һ�£���Player������ʾ
            }

            GameObject bullet = GetWeapon().weaponBulletPool.GetBullet();     //�ӵ��ػ�ȡ�ӵ�
            bullet.GetComponent<BulletController>().weaponData = Instantiate(weaponData);
            bullet.SetActive(true);

            bullet.transform.eulerAngles = skillWeaponPos.eulerAngles;   //���ɺ�ͽ�bullet�Ƕ��뵱ʱ��weaponPosһ��
            bullet.GetComponent<BulletController>().isActive = true;
            bullet.transform.position = skillWeaponPos.position;
            Vector3 dir = skillWeaponPos.transform.right;
            bullet.GetComponent<BulletController>().rb.velocity = new Vector2(dir.x, dir.y) * 20;  //��άƽ���һ���ٶ�

            //���������λ��ҲҪ�����ӵ�
            GameObject bullet2 = GetWeapon().weaponBulletPool.GetBullet();     //�ӵ��ػ�ȡ�ӵ�
            bullet2.GetComponent<BulletController>().weaponData = Instantiate(weaponData);
            bullet2.SetActive(true);                                     //��ʾ�ӵ�
            bullet2.transform.eulerAngles = weaponPos.eulerAngles;   //���ɺ�ͽ�bullet�Ƕ��뵱ʱ��weaponPosһ��
            bullet2.GetComponent<BulletController>().isActive = true;
            bullet2.transform.position = weaponPos.position;
            Vector3 dir2 = weaponPos.transform.right;
            bullet2.GetComponent<BulletController>().rb.velocity = new Vector2(dir2.x, dir2.y) * 20;  //��άƽ���һ���ٶ�
        }
        else if(Time.time > nextFire && isSkill == true)
        {
            nextFire = Time.time + GetWeapon().weaponData.coolDown / 2;       //���ʱ������
            if (transform.childCount == 3)      //��������ֻ������λ�á���������λ�úͷ�ŭ��Ч���������ڼ�һ��������Ч���Ҳ����ظ���������
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
                nextDefenceRestore = Time.time + 1.5f;     //ÿ��1.5s�ָ�һ������
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
        if (Time.timeScale == 1)   //�ҷ�����ͣʱflashSlider����仯���ͼ�����ж�
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
                skillFireEffect.SetActive(true);                      //����Ч
                currentSkillPoint -= 2;                               //���ܳ���5����                               
                if (currentSkillPoint <= 0)
                {
                    isSkill = false;
                    skillFireEffect.SetActive(false);
                    for (int i = 0; i < skillWeaponPos.childCount; i++)    //������ǹλ���µ���������
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
            nextDefenceRestore = Time.time + 4f;       //�ܵ�������4��ſ�ʼ�ָ�
        }
        else
        {
            nextDefenceRestore = Time.time + 4f;       //�ܵ�������4��ſ�ʼ�ָ�
            float chance = Random.Range(0, 1f);
            int damage = Random.Range(weaponData.minDamage, weaponData.maxDamage + 1);
            if (chance < weaponData.criticalChance) weaponData.isCritical = true;
            else weaponData.isCritical = false;

            if (weaponData.isCritical) damage = damage * 2;
            CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

            //damageText = GameObject.Find("DamageInfoText");   //GameObject.Findֻ����active�����壬��������������
            GameObject damageInfo = GameObject.Find("DamageInfo");   //���԰�Ҫ�õ����ֹ���һ���ɼ����������棬�������������
            damageText = damageInfo.transform.Find("DamageInfoText_Player").gameObject;  //����Transform.Find�������������������֣����ص�Ҳ�����ҵ�
            if (weaponData.isCritical) damageText.GetComponent<Text>().color = Color.red;    //������������ʾΪ��ɫ
            else damageText.GetComponent<Text>().color = Color.yellow;            //��������������ʾΪ��ɫ

            damageText.GetComponent<Text>().text = damage.ToString();    //������������Ϊ��ǰ���˺�
            InvokeRepeating("SetDamageInfoTextPos", 0, 0.02f);            //ʵʱ�������ֵ�λ�ñ�֤�ڹ���ͷ��������ͣ����ԭ��
            damageText.SetActive(true);
            StartCoroutine(SetDamageInfoTextFalse());   //Э�������������һ���ܣ�Э�̺���ĳ���Ӱ�죬ͬʱ����

            if (CurrentHealth <= 0)                 //�ж�һ���Ƿ��������������򲥷�����������ֹͣ�˶�������
            {
                isDead = true;
                coll.enabled = false;               //����֮���ײ����player��
                rb.velocity = Vector2.zero;         //����֮�����ƶ�
                transform.GetChild(0).GetChild(0).gameObject.SetActive(false);  //����������
                anim.SetBool("dead", isDead);
            }
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


}
