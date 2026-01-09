using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float damage;
    [SerializeField] private float meleeSpeed;
 
    public float moveSpeed = 5f;

    private Vector2 movement;

    public GameObject projectilePrefab;
    public Camera mainCamera;

    float meleeCooldown = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        rb.linearVelocity = movement * moveSpeed;
        if (timeUntilMelee > 0)
        {
            if (Input.GetKeyDown(0))
            {
                Animation.SetTrigger("Attack");
                timeUntilMelee = meleeCooldown;
            }
        }
        else  {
            timeUntilMelee -= Time.deltaTime;
        }

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
