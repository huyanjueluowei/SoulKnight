using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoMain : MonoBehaviour
{
    public void TransitionToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
