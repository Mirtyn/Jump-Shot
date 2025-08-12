using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProjectBehaviour : MonoBehaviour
{
    public static GameManager GameManager;

    public void ReloadLevel()
    {
        GameManager.ShowLeaderboard();
    }

    public void ReLoad()
    {
        SceneManager.LoadScene(0);
    }
}
