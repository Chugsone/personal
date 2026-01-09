using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void StartButton()
    {
        SceneManager.LoadScene("Level Select");
        Debug.Log("Player pressed button 'Start', loading scene 'Tutorial'");
    }

    public void CreditsButton()
    {
        SceneManager.LoadScene("Credits");
        Debug.Log("Player pressed button 'Credits', loading scene 'Credits'");
    }

    public void QuitButton()
    {
        Application.Quit();
        Debug.Log("Player pressed button 'Quit', ending the application");
    }
}