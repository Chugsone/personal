using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using UnityEngine.Rendering.Universal;
using System.Collections;
using UnityEngine.SceneManagement;

public class BossScript : MonoBehaviour
{ 
    public UnityEvent SliceAttack; 
    [SerializeField] private Light2D bossLight;
    [SerializeField] private Image bossBar;
    [SerializeField] private GameObject deathWarning;
    [SerializeField] private float attackRange = 40f;
    [SerializeField] private int explodeDamage = 50;


    private GameObject projectilePrefab;
    private GameObject circleWarningPrefab;

    private GameObject player;
    private Stats playerStats;
    [HideInInspector] public int Health;
    private int _maxHealth;
    private int lastHealth;
    private bool toggle;
    private bool shootPlayer = true;


    private bool fighting = false;


    void Start()
    {
        Health = GetComponent<Stats>().MaxHealth;
        _maxHealth = Health;
        lastHealth = Health;
        player = GameObject.FindWithTag("Player");
        if (player.TryGetComponent<Stats>(out Stats ph))
        {
            playerStats = ph;
        }
        else
        {
            Debug.LogWarning("Player doesnt have a stats script, either update player or campfire script.");
        }
        projectilePrefab = Resources.Load<GameObject>("Prefabs/BossProj");
        circleWarningPrefab = Resources.Load<GameObject>("Prefabs/CircleWarning");

    }

    void Update()
    {
        if (!fighting)
        {    
            if (Vector2.Distance(transform.position, player.transform.position) <= 4.67f)
            {
                bossLight.enabled = true;
                fighting = true;
                StartCoroutine(HandleAttacks());
                bossBar.gameObject.transform.parent.gameObject.SetActive(true);
            }
        }

        if (lastHealth != Health)
        {
            bossBar.fillAmount = (float) Health / (float) _maxHealth;
            lastHealth = Health;
            if (Health == 0)
            {
                StopAllCoroutines();
                StartCoroutine(Death());
            }
        }

    }


    IEnumerator Death()
    {
        deathWarning.SetActive(true);
        yield return new WaitForSeconds(3f);
        Collider2D nearby = Physics2D.OverlapCircle(transform.position, attackRange);

        if (nearby.gameObject.CompareTag("Player"))
        {
            if (playerStats.Health - explodeDamage <= 0f)
            {
                Destroy(nearby.gameObject);
                SceneManager.LoadScene("Lose");
                yield break; //return;
            }
            
            
        }
        Destroy(player);
        SceneManager.LoadScene("Win");
    }

    IEnumerator HandleAttacks()
    {
        //
        yield return new WaitForSeconds(2f);
        StartCoroutine(ShootCircle(3));
        if (shootPlayer)
        {
            shootPlayer = false;
        StartCoroutine(ShootPlayer());
        }
        yield return new WaitForSeconds(10f);
        toggle = true;
        //Spawn Enemies here damage phase

        yield return new WaitForSeconds(5f);

        SliceAttack.Invoke();

        yield return new WaitForSeconds(4.5f);
        StartCoroutine(HandleAttacks());
        toggle = false;

        
    }

    IEnumerator ShootCircle(int bulletsPerShot)
    {
        bool shooting = true;
        float angle = 0f;
        float angleIncrease = 5f;
        while (shooting)
        {
            if (toggle)
            {
                yield break;
            }
            for (int i = 0; i < bulletsPerShot; i++)
            {
                GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0f, 0f, angle + i * (360/bulletsPerShot)));
                BossProj projScript = proj.GetComponent<BossProj>();
                projScript.Damage = 10;
            }
            angle += 5f;

            if (Mathf.Abs(angle) > 120)
            {
                angleIncrease *= -1f;
            }
            yield return new WaitForSeconds(0.3f);
        }
            
    }

    IEnumerator ShootPlayer()
    {
        float cooldown = 1.5f;

        yield return new WaitForSeconds(cooldown);
        bool shooting = true;
        Vector2 direction = new ();
        float angle = 0f;
        while (shooting)
        {
            direction = player.transform.position - transform.position;
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            GameObject warning = Instantiate(circleWarningPrefab, player.transform.position, Quaternion.identity);
            Destroy(warning, 0.5f);
            yield return new WaitForSeconds(0.5f);
            GameObject proj = Instantiate(projectilePrefab, transform.position + new Vector3(direction.normalized.x * 2f, direction.normalized.y * 2f, 0f), Quaternion.Euler(0f, 0f, angle));
            BossProj projScript = proj.GetComponent<BossProj>();
            projScript.ProjectileSpeed *= 4f;
            projScript.Damage = 20;


            yield return new WaitForSeconds(cooldown);


            
        }
    }
}
