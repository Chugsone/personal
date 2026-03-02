using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static Sheild;

public class Stats : MonoBehaviour
{
    private static Stats _playerStats;

    Sheild sheild;

    [Header("Put name here :wilted_rose:")]
    [Tooltip("How much health the object has")][SerializeField] private int _maxHealth = 100;
    public void Awake()
    {
         sheild = GetComponent<Sheild>();
        
    }
    public int MaxHealth
    {
        get { return _maxHealth; }
        set 
        { 
            int oldMaxHealth = _maxHealth;
            _maxHealth = value; 
            _health += value - oldMaxHealth;
        }
    }

    private void Update()
    {
        if (gameObject.CompareTag("Shield"))
        {
            sheild.SetPeanutImage((Sheild.PeanutState) (Health));
        }
    }

    private int _health; 
    //[HideInInspector]
    public int Health
    {
        get { return _health; } 
        set
        {
            

            if (value < _health) //Removing Health ... .GetComponent<Stats>().Health -= damage;
            {
                if (_currentIFrames > 0)
                {
                    return;    
                }

                if (damageParticles != null)
                {
                    damageParticles.Play();
                }
                if (gameObject.CompareTag("Enemy") && _playerStats.Vampirism > 0f)
                {
                    _playerStats.Health += Mathf.RoundToInt(_playerStats.Vampirism * (_health - value));

                }
                if (gameObject.name == "Boss")
                {
                    float distance = Vector2.Distance(transform.position, _playerStats.gameObject.transform.position);
                    if (distance > 10f)
                    {
                        _defense = Mathf.Clamp((distance - 10f) / 10f, 0f, 0.5f);
                    }
                    Debug.Log($"Distance: {distance} --- DMG Mit {Mathf.RoundToInt(_defense * 100f)}%");
                    _health = Mathf.Clamp(Mathf.RoundToInt((float)_health - (((float) _health - (float) value) * (1 - _defense))), 0, _maxHealth);
                    gameObject.GetComponent<BossScript>().Health = _health;
                    _defense = 0f;
                }
                else
                {
                    _health = Mathf.Clamp(Mathf.RoundToInt((float)_health - (((float) _health - (float) value) * (1 - _defense))), 0, _maxHealth);
                }
            }
            else //Adding Health
            {
                if (healParticles != null)
                {
                    healParticles.Play();
                }
                _health = Mathf.Clamp(value, 0, _maxHealth);
            }

            if (_health <= 0)
            {
                if (gameObject.CompareTag("Enemy"))
                {
                    if (_playerStats == null)
                    {
                        Debug.LogWarning("Player Stats script not found.");
                    }
                    _playerStats.Experience += _exp;

                }
                else if (gameObject.CompareTag("Player"))
                {
                    
                }
            }
        }
    }

    [Tooltip("The speed an object can move")][SerializeField] private float _moveSpeed = 25f;
    public float MoveSpeed
    {
        get { return _moveSpeed; }
        set 
        { 
            if (gameObject.CompareTag("Player"))
            {
                gameObject.GetComponent<PlayerMovement>().speed = value;
            }
            _moveSpeed = value; 
        }
    }
    
    [Tooltip("The percentage of damage mitigation 0 being none 1 being 100% mitigation")] [Range(0f, 1f)] [SerializeField] private float _defense;
    [HideInInspector] public float Defense
    {
        get { return _defense; }
        set { _defense = Mathf.Clamp(value, 0f, 1f); }
    }



    [Tooltip("How much health the object has")][SerializeField] private int _damage = 100; 
    public int Damage
    {
        get { return _damage; }
        set { _damage = value; }
    }


    [Header("Exp stuff")]
    private int _exp;
    [HideInInspector] public int Experience
    {
        get { return _exp; }
        set
        {
            _exp = value; //Update EXP Bar
            if (_exp > _expReq)
            {
                LevelUp();
                
            }
        }
    }

    [Tooltip("How much exp is required to level up")] [SerializeField] private int _expReq = 100;
    [HideInInspector] public int ExperienceRequirement 
    {
        get { return _expReq; }
        set
        {
            _expReq = value;
        }
    }

    [Tooltip("The amount increase of exp required to level up (2 being double the exp)")] [SerializeField] private float _expMultiplier = 1.5f;

    private int _level;
    [HideInInspector] public int Level
    {
        get { return _level; }
        set
        {
            Level = value; //Update EXP Bar

        }
    }

    private int _spins;
    [HideInInspector] public int Spins
    {
        get { return _spins; }
        set
        {
            _spins = value; 
        }
    }

    private int _bulletHits = 1; //How many hits a bullet can do before it breaks
    [HideInInspector] public int BulletHits
    {
        get { return _bulletHits; }
        set
        {
            _bulletHits = value; 

        }
    }

    private int _multifire = 0; //How many shots you fire at once
    [HideInInspector] public int Multifire
    {
        get { return _multifire; }
        set
        {
            _multifire = value; 

        }
    }

    [SerializeField] private int _magazine = 8; 
    [HideInInspector] public int Magazine
    {
        get { return _magazine; }
        set
        {
            _magazine = value; 
            if (gameObject.CompareTag("Player"))
            {
                gameObject.GetComponent<PlayerMovement>().mag = value;
            }

        }
    }

    private float _dashIFrames = 0; 
    [HideInInspector] public float DashIFrames
    {
        get { return _dashIFrames; }
        set
        {
            _dashIFrames = value; 

        }
    }

    private float _currentIFrames = 0; 
    [HideInInspector] public float CurrentIFrames
    {
        get {return _currentIFrames;}
        set
        {
            _currentIFrames = value;
            StartCoroutine(UpdateIFrames());
        }
    }

    private float _vampirism = 0; 
    [HideInInspector] public float Vampirism
    {
        get {return _vampirism;}
        set
        {
            _vampirism = value;
        }
    }


    [Header("Unity Events")]
    public UnityEvent levelUp;

        
    [Header("Particles")]
    [SerializeField] ParticleSystem healParticles;
    [SerializeField] ParticleSystem damageParticles;

    private void LevelUp()
    {
        _spins++;
        _exp -= _expReq;
        _level++;                
        _expReq = Mathf.RoundToInt(_expReq * _expMultiplier);
        levelUp.Invoke();
        MaxHealth = Mathf.RoundToInt(_maxHealth*1.1f);
    }

    
    IEnumerator UpdateIFrames()
    {
        while (_currentIFrames > 0)
        {
            _currentIFrames -= Time.deltaTime;
            yield return null;
        }
    }
    

    void Start()
    {
        _health = _maxHealth;
        if (gameObject.CompareTag("Player"))
        {
            _playerStats = this;
        }
    }



}
