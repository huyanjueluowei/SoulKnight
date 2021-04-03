using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private GameObject player;
    private bool isGameOver=false;            //��Ϊ��Ϸ�����ı�־ 
    public GameObject blueRewardBox;          //��ɫ����
    public Transform boxPos;   //��ʱ������һ������λ��

    public List<GameObject> enemies = new List<GameObject>();
    public bool playerDead     //��ȡplayer�Ƿ�����
    {
        get { return player.GetComponent<PlayerStats>().isDead; }
    }
    protected override void Awake()
    {
        base.Awake();
        Time.timeScale = 1;                               //��������˳����½���Ῠ������ΪTime.timeScale�ڴ򿪲˵�ʱ��Ϊ0��Ҫ�����
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
