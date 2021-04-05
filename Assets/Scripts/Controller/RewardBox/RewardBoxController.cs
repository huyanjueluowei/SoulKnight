using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardBoxController : MonoBehaviour
{
    private bool isOpen = false;         //判断宝箱打开状态，以免Player多次靠近宝箱，盖子一直移动，应该只移动一次
    private bool isOpenFinished = false;   //判断宝箱是否完全打开
    private bool isRewarded = false;       //判断是否已经给出武器
    private const float speed = 2;
    private GameObject rewardWeapon;
    Vector3 leftEnd;
    Vector3 rightEnd;

    public GameObject[] weapons;
    private void Awake()
    {
        leftEnd = transform.GetChild(2).position - new Vector3(1, 0, 0);   //左半边终点
        rightEnd = transform.GetChild(3).position + new Vector3(1, 0, 0);  //右半边终点
    }
    private void Update()       //平滑打开宝箱
    {
        if(isOpen==true&&Vector3.Distance(transform.GetChild(2).position, leftEnd) > 0.01f && Vector3.Distance(transform.GetChild(3).position, rightEnd) > 0.01f)
        {
            transform.GetChild(2).position = Vector3.MoveTowards(transform.GetChild(2).position, leftEnd, speed*Time.deltaTime);
            transform.GetChild(3).position = Vector3.MoveTowards(transform.GetChild(3).position, rightEnd, speed*Time.deltaTime);
            if (Vector3.Distance(transform.GetChild(2).position, leftEnd) < 0.012f && Vector3.Distance(transform.GetChild(3).position, rightEnd) < 0.012f)
                isOpenFinished = true;
        }
        RewardWeapon();
        if (transform.GetChild(4).childCount!=0)  rewardWeapon= transform.GetChild(4).GetChild(0).gameObject;
    }
    private void OnTriggerEnter2D(Collider2D collision)               //切换宝箱状态
    {
        if(collision.CompareTag("Player")&&isOpen==false)
        {
            isOpen = true;
        }
    }

    void RewardWeapon()           //奖励武器
    {
        if (isOpenFinished == true && isRewarded == false)
        {
            int weaponIndex = Random.Range(0, weapons.Length);
            rewardWeapon=Instantiate(weapons[weaponIndex], transform.GetChild(4));
            isRewarded = true;
        }
    }

}
