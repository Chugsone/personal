using UnityEngine;
using UnityEngine.InputSystem;

using System.Collections;


public class PlayerMovement : MonoBehaviour

{
    [SerializeField] public Rigidbody2D rb;
    
    [SerializeField] private GameObject gun;
    [HideInInspector] public bool isDead = false;

    [SerializeField] private ParticleSystem dieParticles;

    public float weight = 1;

    public float speed = 5f;
    public Vector3 offset;
    public Vector2 boxsize;
    public float castDistance;
    private Vector2 movementInput;
    public bool HasDoubleJump = true;
    public GameObject projectilePrefab;
    public Camera mainCamera;
    public int health = 0;
    private Color playerColor;
    public float direction;

    public AudioSource source;
    public AudioClip ShootFX, JumpFX;
  
    [SerializeField] public float reloadTimer = 0.5f;

    [SerializeField] private GameObject spawn;

    public bool reloaded = true;

    [SerializeField] public float _jumpForce = 15f;
    private bool jumped = false;
    [SerializeField] private LayerMask _groundLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerColor = GetComponent<SpriteRenderer>().color;
        rb = GetComponent<Rigidbody2D>();
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>(); 

        if (movementInput.x != 0)
        {
            direction = Mathf.Sign(movementInput.x);
        }
    }
    public bool IsGrounded()
    {
        if (Physics2D.BoxCast(transform.position + offset, boxsize, 0, -transform.up, castDistance, _groundLayer))
        {
            HasDoubleJump = true;
            return true;

        }
        else
        {
            return false;
        }
    }
    // Update is called once per frame
  

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + offset + Vector3.down * castDistance, boxsize);
    }

    
    void FixedUpdate()
    {
        rb.linearVelocityX = Mathf.Lerp(rb.linearVelocityX, (movementInput.x * speed), weight);

        // Apply velocity in the FixedUpdate for consistent physics interactions (FixedUpdate is called at a fixed interval)
        if (jumped)
        {
            
            rb.linearVelocityY = _jumpForce;
            jumped = false;
        }

        // what happens when you die
        if (isDead)
        {
            ParticleSystem newParticle = Instantiate(dieParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public void AimGampePad(InputAction.CallbackContext context)
    {
        if (context.ReadValue<Vector2>() != Vector2.zero)
        {
            gun.transform.right = context.ReadValue<Vector2>();
        }

    }

    public void AimMouse(InputAction.CallbackContext context)
    {
        Vector2 mousepos = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
        gun.transform.right = mousepos - (Vector2)gun.transform.position;
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
    IEnumerator GunCooldown()
    {
        yield return new WaitForSeconds(reloadTimer);
        
        reloaded = true;
    }
    internal void TakeKnockback(int knockback)
    {
       //adds one knockback to the player in the inspector
       
    }



    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() == 1 && weight>=1)
        {
            if (IsGrounded())
            {
                rb.linearVelocityY = _jumpForce;
                AudioSource.PlayClipAtPoint(JumpFX, Vector2.zero);
            }
            if (!IsGrounded())
            {
                if (HasDoubleJump == true)
                {
                    rb.linearVelocityY = _jumpForce;
                    AudioSource.PlayClipAtPoint(JumpFX, Vector2.zero);
                    HasDoubleJump = false;
                }
            }
        }
    }
}

