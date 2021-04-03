using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardBoxController : MonoBehaviour
{
    private bool isOpen = false;         //判断宝箱打开状态，以免Player多次靠近宝箱，盖子一直移动，应该只移动一次
    private float speed = 2;
    Vector3 leftEnd;
    Vector3 rightEnd;
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
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)               //切换宝箱状态
    {
        if(collision.CompareTag("Player")&&isOpen==false)
        {
            isOpen = true;
        }
    }
}
