using UnityEngine;

public class OpenSlots : MonoBehaviour
{

    private GameObject player;
    private Stats playerStats;


    public void Toggle()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");

            if (player.TryGetComponent<Stats>(out Stats ph))
            {
                playerStats = ph;
            }
            else
            {
                Debug.LogWarning("Player doesnt have a stats script, either update player or campfire script.");
            }
        }
        if (playerStats.Spins == 0)
        {
            return;
        }
        if (SlotMachine.Instance == null)
        {
            gameObject.SetActive(true);
            return;
        }
        if (!SlotMachine.Instance.MenuToggled)
        {
            gameObject.SetActive(true);
            return;
        }
    }
}
