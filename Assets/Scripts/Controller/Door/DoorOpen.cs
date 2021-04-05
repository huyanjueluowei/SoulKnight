using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    public GameObject doorClosed;   //关闭的门，进入打开的门之后就启动

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.isPlayerInMainRoom = true;
            gameObject.SetActive(false);
            doorClosed.SetActive(true);
        }
    }
}
