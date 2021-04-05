using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private GameObject player;
    public bool isFirstStageEnd=false;            //��Ϊ��һ�׶���Ϸ�ı�־����ͨ����;�Ӣ�֣�
    public bool isGameOver = false;            //��Ϊ��Ϸ�����ı�־������boss��Ϸ������
    private bool isEliteGenerated = false;    //�Ƿ����ɾ�Ӣ��
    public bool isPlayerInMainRoom = false;           //�ж�player�Ƿ��߽���������
    public bool isPlayerInBossRoom = false;           //�ж�player�Ƿ��߽���boss����
    public PlayerStats playerStats;
    public GameObject[] blueRewardBoxes;          //��ɫ����
    public GameObject transitionDoor;             //������

    public List<GameObject> enemiesOne = new List<GameObject>();    //��һ������
    public List<GameObject> enemiesTwo = new List<GameObject>();    //�ڶ�������
    public GameObject[] elites;   //��Ӣ��
    public bool playerDead     //��ȡplayer�Ƿ�����
    {
        get { return player.GetComponent<PlayerStats>().isDead; }
    }
    protected override void Awake()
    {
        base.Awake();
        Time.timeScale = 1;                               //��������˳����½���Ῠ������ΪTime.timeScale�ڴ򿪲˵�ʱ��Ϊ0��Ҫ�����
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
