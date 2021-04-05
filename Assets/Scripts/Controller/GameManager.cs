using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private GameObject player;
    public bool isFirstStageEnd=false;            //作为第一阶段游戏的标志（普通怪物和精英怪）
    public bool isGameOver = false;            //作为游戏结束的标志（打死boss游戏结束）
    private bool isEliteGenerated = false;    //是否生成精英怪
    public bool isPlayerInMainRoom = false;           //判断player是否走进了主房间
    public bool isPlayerInBossRoom = false;           //判断player是否走进了boss房间
    public PlayerStats playerStats;
    public GameObject[] blueRewardBoxes;          //蓝色宝箱
    public GameObject transitionDoor;             //传送门

    public List<GameObject> enemiesOne = new List<GameObject>();    //第一波怪物
    public List<GameObject> enemiesTwo = new List<GameObject>();    //第二波怪物
    public GameObject[] elites;   //精英怪
    public bool playerDead     //获取player是否死亡
    {
        get { return player.GetComponent<PlayerStats>().isDead; }
    }
    protected override void Awake()
    {
        base.Awake();
        Time.timeScale = 1;                               //不加这句退出重新进入会卡死，因为Time.timeScale在打开菜单时变为0，要变回来
        player = FindObjectOfType<PlayerController>().gameObject;
        playerStats = player.GetComponent<PlayerStats>();
    }
    private void Update()
    {
        GenerateNextRoundEnemy();
        GenerateRewardBox();
        if (isGameOver) transitionDoor.SetActive(true);
    }
    void GenerateRewardBox()
    {
        if(enemiesOne.Count==0&&enemiesTwo.Count==0&&isFirstStageEnd==false)           
        {
            isFirstStageEnd = true;
            for(int i=0;i<blueRewardBoxes.Length;i++)
            {
                blueRewardBoxes[i].SetActive(true);
            }
        }
    }

    void GenerateNextRoundEnemy()
    {
        if(enemiesOne.Count==0&&isEliteGenerated==false)
        {
            for(int i=0;i<elites.Length;i++)
            {
                elites[i].SetActive(true);
            }
            isEliteGenerated = true;
        }
    }
}
