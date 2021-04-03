using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private GameObject player;
    private bool isGameOver=false;            //作为游戏结束的标志 
    public GameObject blueRewardBox;          //蓝色宝箱
    public Transform boxPos;   //临时创建的一个宝箱位置

    public List<GameObject> enemies = new List<GameObject>();
    public bool playerDead     //获取player是否死亡
    {
        get { return player.GetComponent<PlayerStats>().isDead; }
    }
    protected override void Awake()
    {
        base.Awake();
        Time.timeScale = 1;                               //不加这句退出重新进入会卡死，因为Time.timeScale在打开菜单时变为0，要变回来
        player = FindObjectOfType<PlayerController>().gameObject;
    }
    private void Update()
    {
        GenerateRewardBox();
    }
    void GenerateRewardBox()
    {
        if(enemies.Count==0&&isGameOver==false)           
        {
            isGameOver = true;
            Instantiate(blueRewardBox, boxPos);
        }
    }
}
