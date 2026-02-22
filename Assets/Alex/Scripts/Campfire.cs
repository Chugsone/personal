using UnityEngine;

public class Campfire : MonoBehaviour
{
    private SpriteRenderer fireRenderer;
    private Animator animator;
    private Stats playerStats;
    private bool interacted;
    [SerializeField] private Sprite outSprite;
    [SerializeField] private int healAmount = 30;

    void Start()
    {
        if (GameObject.FindWithTag("Player").TryGetComponent<Stats>(out Stats ph))
        {
            playerStats = ph;
        }
        else
        {
            Debug.LogWarning("Player doesnt have a stats script, either update player or campfire script.");
        }
        animator = GetComponent<Animator>();
        fireRenderer = GetComponent<SpriteRenderer>();

    }

    public void UseCampfire()
    {
        Debug.Log("used");
        if (interacted)
        {
            return;
        }

        Debug.Log("test");
        animator.enabled = false;
        fireRenderer.sprite = outSprite;
        interacted = true;
        playerStats.Health += healAmount;
        Destroy(gameObject, 5);

    }
}

