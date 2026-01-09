using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelSelect : MonoBehaviour
{
    public float unlocked = 1f;

    public void TutorialButton()
    {
        SceneManager.LoadScene("Tutorial");
        
    }

    public void GroundButton()
    {
        unlocked = 2;
        SceneManager.LoadScene("Ground");
        
    }

    public void BasementButton()
    {
        if (unlocked >= 2)
        {
         SceneManager.LoadScene("Basement");
            unlocked = 3;
        }
        else 
        {
            Debug.Log("Level Locked");
        }
    }

    public void LabButton()
    {
        SceneManager.LoadScene("Lab");
        unlocked = 4;
    }

    public void ServersButton()
    {
        SceneManager.LoadScene("Servers");
    }

    public void MenuButton()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Data Reset");

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
