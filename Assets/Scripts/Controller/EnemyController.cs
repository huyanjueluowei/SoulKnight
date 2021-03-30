using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyStates { GUARD,CHASE,DEAD}    //枚举定义敌人的状态
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
        if(transform.position.x<player.transform.position.x)   //怪物在player左边时怪物对着player，反之则转180度
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }
}
