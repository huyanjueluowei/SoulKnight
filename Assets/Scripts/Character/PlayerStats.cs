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
    }


    void GenerateBullet()         //ʵ�������ӵ��ͷ����ӵ�Ч��
    {
        if(Input.GetMouseButtonDown(0)&&isDead==false) 
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

    public void TakeDamage(WeaponData_SO weaponData)
    {
        float chance = Random.Range(0, 1f);
        int damage = Random.Range(weaponData.minDamage, weaponData.maxDamage + 1);
        if (chance < weaponData.criticalChance) weaponData.isCritical = true;
        else weaponData.isCritical = false;

        if (weaponData.isCritical) damage=damage*2;
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
