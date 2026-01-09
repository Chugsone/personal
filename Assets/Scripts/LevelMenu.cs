using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    public Button[] buttons;
    public int unlockedLevel = 1;

    private void Awake()
    {
        int unlockedLevels = PlayerPrefs.GetInt("unlockedLevels", 1);

        for (int i = 3; i < unlockedLevels; i++)
        {
            buttons[i].interactable = true;
        }
        for (int i = unlockedLevels; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
    }




    public void OpenLevel(int levelId)
    {
        string levelName = "Level" + levelId;
        

        SceneManager.LoadScene(levelId);
    }




}