using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public GameObject bulletPrefab;
    public int poolAmount;          //Ҫ�ڱ༭�����趨
    public List<GameObject> bullets = new List<GameObject>();
    private int currentIndex=0;

    private void Start()
    {
        for(int i=0;i<poolAmount;i++)
        {
            GameObject obj = Instantiate(bulletPrefab);
            obj.transform.SetParent(transform);
            obj.SetActive(false);
            bullets.Add(obj);
        }
    }

    public GameObject GetBullet()
    {
        for(int i=0;i<bullets.Count;i++)
        {
            int tempIndex = (currentIndex + i) % bullets.Count;
            if(!bullets[tempIndex].activeInHierarchy)
            {
                currentIndex = (tempIndex+1) % bullets.Count;
                //Debug.Log("���Ի�ȡ���ӵ�");
                return bullets[tempIndex];
            }
        }
        //����б���ÿ���ӵ������ڻ״̬��������ӵ����б��в�����
        GameObject obj = Instantiate(bulletPrefab);
        obj.transform.SetParent(transform);
        bullets.Add(obj);
        return obj;
    }
}
