using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossDoorOpen : MonoBehaviour
{
    public Slider bossBar;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            GameManager.Instance.isPlayerInBossRoom = true;
            bossBar.gameObject.SetActive(true);
        }
    }
}
