using UnityEngine;
public class BulletCounter : MonoBehaviour
{
    private PlayerMovement playerMovement;
    public TMPro.TMP_Text Ammo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        Ammo.SetText($"{playerMovement.bullets}|{playerMovement.mag}");
        if (playerMovement.reloading)
        {
            Ammo.SetText("Reloading...");
        }
    }
}
