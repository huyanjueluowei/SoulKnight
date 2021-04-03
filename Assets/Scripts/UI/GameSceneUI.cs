using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneUI : MonoBehaviour
{
    public GameObject settingPanel;
    private bool isSettingPanelOpen = false;

    private void Update()
    {
        OpenSettingPanel();
    }

    void OpenSettingPanel()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            settingPanel.SetActive(!isSettingPanelOpen);
            isSettingPanelOpen = !isSettingPanelOpen;
            if(isSettingPanelOpen==true)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }

    public void LoadToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
