using UnityEngine;
using UnityEngine.UI;

public class UiBars : MonoBehaviour
{
    private Stats playerStats;
    [SerializeField] private Image healthFill;
    [SerializeField] private Image levelFill;

    void Start()
    {
        playerStats = GameObject.FindWithTag("Player").GetComponent<Stats>();
    }

    void Update()
    {
        healthFill.fillAmount = (float) playerStats.Health / playerStats.MaxHealth;
        levelFill.fillAmount = (float) playerStats.Experience / playerStats.ExperienceRequirement;


    }
}
