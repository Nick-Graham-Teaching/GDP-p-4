using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void PlayTut()
    {
        SceneManager.LoadScene("Tutorial");
    }


    public void BackTut()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void BackMain()
    {
        SceneManager.LoadScene("SamplScene");
    }

    public void QuitGmae()
    {
        Application.Quit();
    }

}
