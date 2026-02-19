using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

using static UnityEngine.RuleTile.TilingRuleOutput;
using System.Collections.Generic;


public class PlayerMovement : MonoBehaviour

{
    [SerializeField] public Rigidbody2D rb;
    
    [SerializeField] private GameObject gun;
    [HideInInspector] public bool isDead = false;

    [SerializeField] private ParticleSystem dieParticles;

  

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
  
    [SerializeField] public float reloadTimer = 0.5f;

    [SerializeField] private GameObject spawn;

    public bool reloaded = true;

    public float KBForce;
    public float KBCounter;
    public float KBTotalTime;

    public bool knockFromRight;

    public float dashPower;



    private void Awake()
    {
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

    public void Dash()
    {
        rb.linearVelocity = new Vector2(playerMovement.direction * dashPower, 0f);
    }
    public void Shoot(InputAction.CallbackContext context)
    {
       
        if (context.performed && reloaded)
        {
            AudioSource.PlayClipAtPoint(ShootFX, Vector2.zero);
            GameObject proj = Instantiate(projectilePrefab, spawn.transform.position, Quaternion.identity);
            proj.GetComponent<SpriteRenderer>().color = playerColor;

            proj.transform.right = gun.transform.right;
            reloaded = false;
            StartCoroutine(GunCooldown());
            
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }
    IEnumerator GunCooldown()
    {
        yield return new WaitForSeconds(reloadTimer);
        
        reloaded = true;
    }
    internal void TakeKnockback(int knockback)
    {
       //adds one knockback to the player in the inspector
       
    }



   
}

