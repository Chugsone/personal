using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;




public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public float recoil = 0.5f; 
    [SerializeField] public Rigidbody2D rb; 
    [SerializeField] private GameObject gun;
    [SerializeField] private ParticleSystem dieParticles;
    [SerializeField] private Animator animator;
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private GameObject spawn;
    [SerializeField] private float ReloadTime;
    [SerializeField] private float dashCooldown = .25f;

    public TrailRenderer trailRenderer;
    public float trailTime;

    [HideInInspector] public bool isDead = false;
    
    private Vector2 movementInput;
    public Vector2 boxsize;
    private Vector2 input;
    public Vector3 offset;

    public float speed = 1f;
    public float topSpeed = 10f;   
    public float castDistance;
    
    public float KBForce;
    public float KBCounter;
    public float KBTotalTime;

    public GameObject projectilePrefab;
    public Camera mainCamera;
    public int health = 0;
    public float direction;
    public PlayerMovement playerMovement;
    public bool reloading;

    public AudioClip ShootFX;
    public AudioClip songing;
  
    public bool GodMode = false;
    public bool reloaded = true;
    public int mag;
    public int bullets = 8;

    public bool knockFromRight;

    public float dashPower = 3.67f;
    private float dashTimer = 0f;
    private Stats playerStats;
   
    private void Awake()
    {
        dashTimer = dashCooldown;
        playerMovement = GetComponent<PlayerMovement>();
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.emitting = false;
        if (gameObject.TryGetComponent<Stats>(out Stats ph))
        {
            playerStats = ph;
        }
        else
        {
            Debug.LogWarning("Player doesnt have a stats script, either update player or campfire script.");
        }
    }

    void Start()
    {
        AudioSource.PlayClipAtPoint(songing, transform.position);
        rb = GetComponent<Rigidbody2D>();
    }

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

        if (GodMode)
        {
            recoil = 0;
            mag = 100;
            dashCooldown = 0;
        }

      
    }

    void FixedUpdate()
    {
        //rb.linearVelocityX = Mathf.Lerp(rb.linearVelocityX, (movementInput.x * speed), weight);

        // Apply velocity in the FixedUpdate for consistent physics interactions (FixedUpdate is called at a fixed interval)

        if (KBCounter <= 0)
        {
            rb.AddForce(movementInput * speed);
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, topSpeed);
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

        //makes the gun flip upside down when aiming left
        if (gun.transform.right.x < 0)
        {
            gun.transform.localScale = new Vector3(1, -1, 1);
        }
        else
        {
            gun.transform.localScale = new Vector3(1, 1, 1);
        }
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
        playerStats.CurrentIFrames += playerStats.DashIFrames;
        dashTimer = dashCooldown;
        rb.linearVelocity = movementInput * dashPower;
        StartCoroutine(playTrail());
    }
    IEnumerator playTrail()
    {
        trailRenderer.emitting = true;
        yield return new WaitForSeconds(trailTime);
        trailRenderer.emitting = false;
    }

    public void Shoot(InputAction.CallbackContext context)
    {
       
        if (context.performed && reloaded && bullets > 0 && !reloading)
        {
            AudioSource.PlayClipAtPoint(ShootFX, transform.position);
            GameObject proj = Instantiate(projectilePrefab, spawn.transform.position, Quaternion.identity);
            
            Projectiles1 projScript = proj.GetComponent<Projectiles1>();
            projScript.pierceCount = playerStats.BulletHits;
            projScript.Damage = playerStats.Damage;
            proj.transform.right = gun.transform.right;
            reloaded = false;
            StartCoroutine(GunCooldown());
            bullets -= 1;
            StartCoroutine(Multishot());

            gunAnimator.SetTrigger("Shoot");
        }
    }

    IEnumerator Multishot()
    {
        for (int i = 0; i < playerStats.Multifire; i++)
        {
            if (bullets == 0)
            {
                yield break;
            }
            yield return new WaitForSeconds(0.1f);
            GameObject proj = Instantiate(projectilePrefab, spawn.transform.position, Quaternion.identity);
            Projectiles1 projScript = proj.GetComponent<Projectiles1>();
            projScript.pierceCount = playerStats.BulletHits;
            projScript.Damage = playerStats.Damage;
            proj.transform.right = gun.transform.right;
            bullets--;

        }
    }

    public void Reload(InputAction.CallbackContext context)
    {
        if (!reloading && bullets != 8)
        {
            StartCoroutine(ReloadTimer());
            reloading = true;
        }
    }

    IEnumerator ReloadTimer()
    {
        yield return new WaitForSeconds(ReloadTime);
        bullets = mag;
        reloading = false;
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

    public void GodModeEnable(InputAction.CallbackContext context)
    {
      GodMode = true;
    }
}