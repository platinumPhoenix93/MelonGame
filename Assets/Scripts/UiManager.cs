using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{

    //This class handles loading of scenes


    //Reloads current scene
    public void RestartLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }


    public void QuitApplication()
    {
        Application.Quit();
    }

    //Loads next scene if one exists
    public void GoToNextLevel()
    {
        Scene scene = SceneManager.GetActiveScene();

        if(scene.buildIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(scene.buildIndex + 1);
        }
    }

    public void GoToLevelOne()
    {
        SceneManager.LoadScene("LevelOne");
    }

    public void GoToLevelTwo()
    {
        SceneManager.LoadScene("LevelTwo");
    }

    public void GoToLevelThree()
    {
        SceneManager.LoadScene("LevelThree");
    }

}
