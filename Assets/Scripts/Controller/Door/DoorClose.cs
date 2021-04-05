using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorClose : MonoBehaviour
{
    public GameObject doorOpen;

    private void Update()
    {
        if(GameManager.Instance.isFirstStageEnd==true)
        {
            gameObject.SetActive(false);
            doorOpen.SetActive(true);
        }
    }
}
