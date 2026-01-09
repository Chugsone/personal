using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class NewMittensAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    Rigidbody2D rb;
    Transform target;
    Vector2 moveDirection;
    public Vector3 offset = new(1, 0);

    public bool playerDetector;
    public float detectionRange = 10f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Start()
    {
        target = GameObject.Find("Player").transform;

    }

    private void Update()
    {

        {
            Vector3 direction = (target.position - (Vector3)transform.position + offset).normalized;
            moveDirection = direction;


        }
        if (Vector2.Distance(transform.position, target.position) < detectionRange)
        {
            playerDetector = true;

        }
        else
        {
            playerDetector = false;
            rb.linearVelocity = Vector2.zero;
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Mittens has detected a collision.");
        if (collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Enemy"))
        {
            SceneManager.LoadScene("BattleTest");
            if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
            {
                Debug.Log("scene on " + SceneManager.GetActiveScene().buildIndex);
                Debug.Log("scene unlocked " + PlayerPrefs.GetInt("ReachedIndex"));



                PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex);
                PlayerPrefs.SetInt("unlockedLevels", SceneManager.GetActiveScene().buildIndex - 1);
                PlayerPrefs.Save();



                Debug.Log("scene on " + SceneManager.GetActiveScene().buildIndex);
                Debug.Log("scene unlocked " + PlayerPrefs.GetInt("ReachedIndex"));
            }
        }
    }

    private void FixedUpdate()
    {

        if (target && Vector2.Distance(target.position, transform.position) < detectionRange)

        {
            rb.linearVelocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

}



