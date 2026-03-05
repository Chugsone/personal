using UnityEngine;
using System.Collections.Generic;


public class Tutorial : MonoBehaviour
{

    [SerializeField] private List<GameObject> tutorialGO;
    private int currentTutorial = 0;


   public void Back(bool on)
    {
        gameObject.SetActive(on);
    }

    public void ChangeTutorial(int direction)
    {
        if (currentTutorial + direction < 0 || currentTutorial + direction >= tutorialGO.Count)
        {
            return;
        }

        tutorialGO[currentTutorial].SetActive(false);
        currentTutorial += direction;
        tutorialGO[currentTutorial].SetActive(true);

    }

    
}
