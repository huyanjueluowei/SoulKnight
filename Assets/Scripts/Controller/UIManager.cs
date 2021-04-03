using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Text startTextButton;
    public Text loadingText;
    public GameObject loadingPanel;  //加载时的面板
    public Slider loadingSlider;
    private bool bigger;            //标记数值在变大
    private bool smaller;           //标记数值在变小

    private void Awake()
    {
        Time.timeScale = 1;                //防止出现卡死情况
    }
    private void Update()
    {
        ChangeStartTextAlpha();
    }

    public void OldGame()
    {
        StartCoroutine(LoadLevel());
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator LoadLevel()
    {
        loadingPanel.SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        operation.allowSceneActivation = false;
        while(!operation.isDone)
        {
            loadingSlider.value = operation.progress;
            loadingText.text = "正在加载中~" + operation.progress * 100 + "%";
            if(operation.progress>=0.9f)
            {
                loadingSlider.value = 1;
                loadingText.text= "正在加载中~" +  "100%";
                if(Input.anyKeyDown)
                {
                    operation.allowSceneActivation = true;
                }
            }
            yield return null;
        }
    }
    void ChangeStartTextAlpha()
    {
        float alpha = startTextButton.color.a;
        if(Mathf.Abs(alpha-1)<0.01f)
        {
            bigger = false;
            smaller = true;
        }
        if(Mathf.Abs(alpha-0.4f)<0.01f)       //如果直接判==0.4会有问题，毕竟这个Time.delta是个小数，可能不会判定为恰好想等，因此接近即可
        {
            bigger = true;
            smaller = false;
        }
        if(alpha>0.4&&smaller)
        {
            alpha-=Time.deltaTime;
            startTextButton.color=new Color(1,1,1,alpha);
        }
        else if(alpha<1&&bigger)
        {
            alpha+=Time.deltaTime;
            startTextButton.color = new Color(1, 1, 1, alpha);
        }
    }
}
