using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;

    public float moveSpeed = 5f;

    private Vector2 movement;

    public GameObject projectilePrefab;
    public Camera mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = movement * moveSpeed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!context.canceled)
        {
            movement = context.ReadValue<Vector2>();
        }
        else 
        {
            movement = Vector2.zero;
        }
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 mouseWorldPosition = (Vector2) mainCamera.ScreenToWorldPoint(Input.mousePosition);
            float angle = Mathf.Atan2(mouseWorldPosition.y - transform.position.y, mouseWorldPosition.x - transform.position.x) * Mathf.Rad2Deg;
            GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(new Vector3(0, 0, angle)));
        }
    }
}
