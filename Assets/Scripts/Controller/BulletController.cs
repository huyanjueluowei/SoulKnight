using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public bool isActive=false;
    public Rigidbody2D rb;
    public WeaponData_SO weaponData;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {

    }

    

    private void OnTriggerEnter2D(Collider2D collision)   //ע����Ӧ��ײ��ĳ�trigger
    {
        if(collision.tag=="Border"||collision.tag=="Enemy")
        {
            gameObject.SetActive(false);
            isActive = false;
        }
    }
}
