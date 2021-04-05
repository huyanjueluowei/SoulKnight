using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponContainer : MonoBehaviour
{
    private GameObject weaponInfoText;

    private void Awake()
    {
        weaponInfoText = transform.parent.GetChild(5).GetChild(0).gameObject;
    }
    private void Update()
    {
        DestroyWeapon();
        SetRewardTextPos();
    }
    void DestroyWeapon()
    {
        if (transform.childCount > 1)
        {
            transform.GetChild(1).SetSiblingIndex(0);   //本来是生成在第二个的，把它放在第一个来
            for (int i = 1; i < transform.childCount; i++)   //很鬼畜，它有时候会生成很多武器在地上，直接都销毁
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }

    void SetRewardTextPos()
    {
        if (transform.childCount != 0)
        {
            Vector3 rewardTextPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 2f, 0));
            weaponInfoText.transform.position = rewardTextPos;
            weaponInfoText.GetComponent<Text>().text = transform.GetChild(0).GetComponent<WeaponController>().weaponData.weaponName;
            switch(transform.GetChild(0).GetComponent<WeaponController>().weaponData.weaponLevel)
            {
                case WeaponLevel.NORMAL:     //辣鸡武器
                    weaponInfoText.GetComponent<Text>().color = Color.white;
                    break;
                case WeaponLevel.GOOD:      //还能用的武器
                    weaponInfoText.GetComponent<Text>().color = Color.green;
                    break;
                case WeaponLevel.RARE:      //稀有武器
                    weaponInfoText.GetComponent<Text>().color = Color.blue;
                    break;
                case WeaponLevel.EPIC:      //神经级武器
                    weaponInfoText.GetComponent<Text>().color = new Color32(255, 0, 255, 255);  //没有Color.purple我就找purple的32位值，用new Color32赋值
                    break;
                case WeaponLevel.LEGEND:    //一刀999武器
                    weaponInfoText.GetComponent<Text>().color = Color.yellow;
                    break;
            }
            weaponInfoText.SetActive(true);
        }
    }
}
