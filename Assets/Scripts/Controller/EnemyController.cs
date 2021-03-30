using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyStates { GUARD,CHASE,DEAD}    //ö�ٶ�����˵�״̬
[RequireComponent(typeof(CharacterStats))]
public class EnemyController : MonoBehaviour
{
    private EnemyStates enemyStates;
    private Animator anim;
    private GameObject player;

    bool isGuard;
    bool isChase;
    bool isDead;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        player = FindObjectOfType<PlayerController>().gameObject;
    }

    private void Update()
    {
        Move();
    }

    void Move()
    {
        if(transform.position.x<player.transform.position.x)   //������player���ʱ�������player����֮��ת180��
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }
}
