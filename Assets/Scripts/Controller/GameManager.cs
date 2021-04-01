using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private GameObject player;
    public bool playerDead     //ªÒ»°player «∑ÒÀ¿Õˆ
    {
        get { return player.GetComponent<PlayerStats>().isDead; }
    }
    protected override void Awake()
    {
        base.Awake();
        player = FindObjectOfType<PlayerController>().gameObject;
    }

}
