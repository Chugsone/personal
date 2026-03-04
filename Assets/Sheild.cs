using UnityEngine;
public class Sheild : MonoBehaviour
{  
    public Sprite peanuts, peanuts_1, peanuts_2, peanuts_3, peanuts_4, peanuts_5, peanuts_6, peanuts_7;
    SpriteRenderer peanut; 
    private PeanutState currentState;



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
