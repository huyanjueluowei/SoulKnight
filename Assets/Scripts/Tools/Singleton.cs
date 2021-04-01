using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//泛型通常在类名之后加上尖括号，里面是类型T，T可以代表任何类型,如MouseManager,GameManager等等
public class Singleton<T> : MonoBehaviour where T : Singleton<T>  //约束，必须是singleton类型
{
    private static T instance;
    public static T Instance
    {
        get { return instance; }   //想让外部可以访问instance，但是又不能修改它
    }

    protected virtual void Awake()  //protected表明只能在自己和子类中访问，virtual为了让Awake在子类中可以override重写
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = (T)this;  //由于是各种类继承，因此赋予this要加上泛型转换T
    }
    public static bool IsInitialized   //表示泛型单例是否已经生成以供其他脚本获取，注意static关键字不能掉了
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
