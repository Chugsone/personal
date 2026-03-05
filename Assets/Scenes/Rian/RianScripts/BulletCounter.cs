using UnityEngine;
public class BulletCounter : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private TMPro.TMP_Text ammo;
    private Canvas canvas;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        CreateBC();
    }

    // Update is called once per frame
    void Update()
    {
        if (canvas == null)
        {
            
            return;
        }
        ammo.SetText($"{playerMovement.bullets}|{playerMovement.mag}");
        if (playerMovement.reloading)
        {
            ammo.SetText("Reloading");
        }
    }

    private void CreateBC()
    {
       canvas = GameObject.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.Log("no canvas found");
            return;
        }

        GameObject empty = new ("BulletCounter");
        RectTransform textTransform = empty.AddComponent<RectTransform>();
        textTransform.SetParent(canvas.transform, false);
        textTransform.sizeDelta = new (200, 50);
        textTransform.anchorMin = Vector2.one;
        textTransform.anchorMax = Vector2.one;
        textTransform.anchoredPosition = new Vector3(-100f, -50f, 0f);
        ammo = empty.AddComponent<TMPro.TextMeshProUGUI>();
        ammo.fontSize = 20;
        ammo.font = Resources.Load<TMPro.TMP_FontAsset>("Fonts/Pixel");
        ammo.alignment = TMPro.TextAlignmentOptions.Center; 
    }
}
