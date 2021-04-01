using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//����ͨ��������֮����ϼ����ţ�����������T��T���Դ����κ�����,��MouseManager,GameManager�ȵ�
public class Singleton<T> : MonoBehaviour where T : Singleton<T>  //Լ����������singleton����
{
    private static T instance;
    public static T Instance
    {
        get { return instance; }   //�����ⲿ���Է���instance�������ֲ����޸���
    }

    protected virtual void Awake()  //protected����ֻ�����Լ��������з��ʣ�virtualΪ����Awake�������п���override��д
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = (T)this;  //�����Ǹ�����̳У���˸���thisҪ���Ϸ���ת��T
    }
    public static bool IsInitialized   //��ʾ���͵����Ƿ��Ѿ������Թ������ű���ȡ��ע��static�ؼ��ֲ��ܵ���
    {
        get { return instance != null; }
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
