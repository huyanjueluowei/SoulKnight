using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    public float speed;         //�����ٶ�
    public Transform weaponPos;
    public Camera mainCamera;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        anim.SetFloat("run",Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.y));
        RotateWeapon();
    }
    private void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        rb.velocity = new Vector2(horizontalInput * speed, verticalInput * speed);
        if (horizontalInput > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (horizontalInput < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    public void RotateWeapon()
    {
        float z;
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z += 10;      //�ҷ���mousePos  z��-10��2d�����ⶼҪ���0�������������Ļ�ǹ����ת��ܲ���������Ϊz��Ӱ��Ƕ���ƫ��
        if(mousePos.y>weaponPos.position.y)
        {
            z = Vector3.Angle(Vector3.right, mousePos - weaponPos.position);
        }
        else
        {
            z = -Vector3.Angle(Vector3.right,mousePos - weaponPos.position);
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
