using UnityEngine;
using UnityEngine.UIElements;
public class Sheild : MonoBehaviour
{  
    public Sprite peanuts, peanuts_1, peanuts_2, peanuts_3, peanuts_4, peanuts_5, peanuts_6, peanuts_7;
    SpriteRenderer peanut; 
    private PeanutState currentState;
    private static Transform player;



    public enum PeanutState
    {
        Full,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven
    }

    private void Awake()
    {
        peanut = GetComponent<SpriteRenderer>();
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }
    }
    private void FixedUpdate()
    {
        transform.localPosition = (player.position - transform.parent.position).normalized + new Vector3(0f, 0.5f, 0f);
    
    }

    public void SetPeanutImage(PeanutState state)
    {
        switch (state)
        {
            case PeanutState.Full:
                peanut.sprite = peanuts;
                break;
            case PeanutState.One:
                peanut.sprite = peanuts_1;
                break;
            case PeanutState.Two:
                peanut.sprite = peanuts_2;
                break;
            case PeanutState.Three:
                peanut.sprite = peanuts_3;
                break;
            case PeanutState.Four:
                peanut.sprite = peanuts_4;
                break;
            case PeanutState.Five:
                peanut.sprite = peanuts_5;
                break;
            case PeanutState.Six:
                peanut.sprite = peanuts_6;
                break;
            case PeanutState.Seven:
                peanut.sprite = peanuts_7;
                break;

            default:
                transform.parent.GetComponent<PeanutAI>().HasShield = false;
                Destroy(gameObject);
                break;

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            currentState = (PeanutState)((int)currentState + 1);
            SetPeanutImage(currentState);
            Debug.Log($"State {(int)currentState}");
        }
    }

}
