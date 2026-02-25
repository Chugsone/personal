using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

using static UnityEngine.RuleTile.TilingRuleOutput;
using System.Collections.Generic;
using JetBrains.Annotations;


public class PlayerMovement : MonoBehaviour

{
    [SerializeField] public Rigidbody2D rb;
    
    [SerializeField] private GameObject gun;
    [HideInInspector] public bool isDead = false;

    [SerializeField] private ParticleSystem dieParticles;

    [SerializeField] private Animator animator;

    public float speed = 1f;
    public float topSpeed = 10f;
    public Vector3 offset;
    public Vector2 boxsize;
    public float castDistance;
    private Vector2 movementInput;
   
    public GameObject projectilePrefab;
    public Camera mainCamera;
    public int health = 0;
    private Color playerColor;
    public float direction;
    public PlayerMovement playerMovement;

    private Vector2 input;

    public AudioSource source;
    public AudioClip ShootFX;
  
    [SerializeField] public float recoil = 0.5f;

    [SerializeField] private GameObject spawn;

    public bool reloaded = true;
   [SerializeField] private float ReloadTime;
    [SerializeField] private int mag;

    public float KBForce;
    public float KBCounter;
    public float KBTotalTime;

    public bool knockFromRight;

    public float dashPower = 3.67f;

    private float dashTimer = 0f;
    [SerializeField] private float dashCooldown = .25f;

    [SerializeField] private int bullets = 8;

    private void Awake()
    {
        dashTimer = dashCooldown;
        playerMovement = GetComponent<PlayerMovement>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerColor = GetComponent<SpriteRenderer>().color;
        rb = GetComponent<Rigidbody2D>();
    }

   
    // Update is called once per frame
  

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + offset + Vector3.down * castDistance, boxsize);
    }

    private void Update()
    {
        if (dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;
        }
        

    }
    void FixedUpdate()
    {
        //rb.linearVelocityX = Mathf.Lerp(rb.linearVelocityX, (movementInput.x * speed), weight);

        // Apply velocity in the FixedUpdate for consistent physics interactions (FixedUpdate is called at a fixed interval)

        if (KBCounter <= 0)
        {
            rb.AddForce(movementInput * speed);
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, 50f);
        }
        else
        {
            if (knockFromRight == true)
            {
                rb.linearVelocity = new Vector2(-KBForce, KBForce);
            }
            if (knockFromRight == false)
            {
                rb.linearVelocity = new Vector2(KBForce, KBForce);
            }
            KBCounter -= Time.deltaTime;
        }

            // what happens when you die
            if (isDead)
        {
            ParticleSystem newParticle = Instantiate(dieParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }



    }

    public void Aim(InputAction.CallbackContext context)
    {
        if (context.control.device is Mouse)
            AimMouse(context);
        else if (context.control.device is Gamepad)
            AimGamepad(context);
    }


    private void AimMouse(InputAction.CallbackContext context)
    {
        Vector2 mousepos = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
        gun.transform.right = mousepos - (Vector2)gun.transform.position;
    }

 

    private void AimGamepad(InputAction.CallbackContext context)
    {
        if (context.ReadValue<Vector2>() != Vector2.zero)
        {
            gun.transform.right = context.ReadValue<Vector2>();
        }

    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (!context.performed || dashTimer > 0)
        {
            return;
        }
        dashTimer = dashCooldown;
        rb.linearVelocity = movementInput * dashPower;
       


    }

   

    public void Shoot(InputAction.CallbackContext context)
    {
       
        if (context.performed && reloaded && bullets > 0)
        {
            AudioSource.PlayClipAtPoint(ShootFX, Vector2.zero);
            GameObject proj = Instantiate(projectilePrefab, spawn.transform.position, Quaternion.identity);
            proj.GetComponent<SpriteRenderer>().color = playerColor;

            proj.transform.right = gun.transform.right;
            reloaded = false;
            StartCoroutine(GunCooldown());
            bullets -= 1;
            
        }
    }

    public void Reload(InputAction.CallbackContext context)
    {
        StartCoroutine(ReloadTimer());
    }

    IEnumerator ReloadTimer()
    {
        yield return new WaitForSeconds(ReloadTime);
        bullets = mag;
    }

    public void Move(InputAction.CallbackContext context)
    {
            movementInput = context.ReadValue<Vector2>();
               animator.SetFloat("HorizontalSpeed", movementInput.x);
                animator.SetFloat("VerticalSpeed", movementInput.y);
    }
    IEnumerator GunCooldown()
    {
        yield return new WaitForSeconds(recoil);
        
        reloaded = true;
    }
    internal void TakeKnockback(int knockback)
    {
       //adds one knockback to the player in the inspector
       
    }



   
}

