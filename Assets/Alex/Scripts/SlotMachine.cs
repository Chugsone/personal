using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
public class SlotMachine : MonoBehaviour
{
    private enum StatUpgrades
    {
        Health = 0,
        Damage = 1,
        Movement = 2,
        Defense = 3,

    }
    [SerializeField] private Animator anim;
    [SerializeField] private int commonStat = 15;
    [SerializeField] private int commonAbility = 15;
    [SerializeField] private int rareStat = 3;
    [SerializeField] private int rareAbility = 3;

    [SerializeField] private float healthBonus = 1.15f;
    [SerializeField] private float damageBonus = 1.15f;
    [SerializeField] private float movementBonus = 1.15f;
    [SerializeField] private float defenseBonus = 0.05f;

    [SerializeField] private GameObject pivot;


    private int spinIndex = 0;
    private bool busy;
    private bool spawned;
    private Queue<Sprite> upgradeList = new();
    private float animatorSpeed = 1f;
    private bool spinning;
    private Sprite rASprite; //I am now realizing I should have made this a list but im quite lazy
    private Sprite rSSprite;
    private Sprite cASprite;
    private Sprite cSSprite;
    private RuntimeAnimatorController rACon;
    private RuntimeAnimatorController rSCon;
    private RuntimeAnimatorController cACon;
    private RuntimeAnimatorController cSCon;
    private GameObject itemAnim;
    

    private GameObject item;
    private Stats playerStats;
    private GameObject player;
    private List<Sprite> upgradeOptions = new();
    private List<GameObject> upgradeGO = new();



    void Start()
    {
        animatorSpeed = anim.speed;
        rASprite = Resources.Load<Sprite>("Sprites/rareAbility");
        rSSprite = Resources.Load<Sprite>("Sprites/rareStat");
        cASprite = Resources.Load<Sprite>("Sprites/commonAbility");
        cSSprite = Resources.Load<Sprite>("Sprites/commonStat");
        rACon = Resources.Load<RuntimeAnimatorController>("Sprites/Animators/rareAbility");
        rSCon = Resources.Load<RuntimeAnimatorController>("Sprites/Animators/rareStat");
        cACon = Resources.Load<RuntimeAnimatorController>("Sprites/Animators/commonAbility");
        cSCon = Resources.Load<RuntimeAnimatorController>("Sprites/Animators/commonStat");
        itemAnim = Resources.Load<GameObject>("Prefabs/ItemAnim");

        item = Resources.Load<GameObject>("Prefabs/Item");
        player = GameObject.FindWithTag("Player");

        if (player.TryGetComponent<Stats>(out Stats ph))
        {
            playerStats = ph;
        }
        else
        {
            Debug.LogWarning("Player doesnt have a stats script, either update player or campfire script.");
        }

        anim.speed = 0f;
        upgradeList = CreateUpgradeList();
    }

    public void ToggleMenu()
    {
        
    }

    public void SpinSlots()
    {
        if (busy || playerStats.Spins == 0)
        {
            return;
        }
        playerStats.Spins -= 1;
        busy = true;
        anim.speed = 1f;
        anim.SetTrigger("Pull");
    }

    public void Spinning()
    {
        if (spinning == false)
        {
            spinning = true;
        }
        if (Random.Range(0f, 1f) > .5f)
        {
            spinIndex++;
            if (spinIndex >= 3)
            {
                anim.SetTrigger("Finish");
                Finish();
                
            }
        }
    }

    public void Upgrade(int index)
    {
        if (!spawned)
        {
            return;
        }
        spawned = false;
        if (upgradeOptions[index] == cSSprite || upgradeOptions[index] == rSSprite) ///Common & Rare Stat
        {
            System.Array enumValues = System.Enum.GetValues(typeof(StatUpgrades));
            switch (Random.Range(0, (int) enumValues.GetValue(enumValues.Length - 1)))
            {
                case (int) StatUpgrades.Health:
                    playerStats.MaxHealth = (int) ((float)playerStats.MaxHealth * (upgradeOptions[index] == cSSprite ? healthBonus : (healthBonus - 1f) * 3f + 1f));
                    break;

                case (int) StatUpgrades.Damage:
                    playerStats.Damage = (int) ((float)playerStats.Damage * (upgradeOptions[index] == cSSprite ? damageBonus : (damageBonus - 1f) * 3f + 1f));
                    break;

                case (int) StatUpgrades.Movement:
                    playerStats.MoveSpeed *= upgradeOptions[index] == cSSprite ? movementBonus : (movementBonus - 1f) * 3f + 1f;
                    break;

                case (int) StatUpgrades.Defense:
                    playerStats.Defense *= upgradeOptions[index] == cSSprite ? defenseBonus : defenseBonus * 3f;
                    break;
            }
        }
        else if (upgradeOptions[index] == cASprite) //Common Ability
        {
            
        }
        else if (upgradeOptions[index] == rASprite) // Rare Ability
        {
            
        }
        
        else
        {
            Debug.LogError("SLOT MACHINE BROKE !?!??!?!?!??!>? Triojkdfngkn sdg do[jknhokmes pkl gio[dfio[gjksdo[jkngijkmo]]]]");
        }
        StartCoroutine(UpgradeAnim(upgradeGO[index], index));

        foreach (GameObject itemGO in upgradeGO)
        {
            if (itemGO == upgradeGO[index])
            {
                continue;
            }
            Destroy(itemGO);
        }
        upgradeGO.Clear();
    }

    IEnumerator UpgradeAnim(GameObject upgrade, int index)
    {

        RectTransform pivotTransform = pivot.GetComponent<RectTransform>();
        RectTransform upgradeTransform = upgrade.GetComponent<RectTransform>();
        upgradeTransform.anchoredPosition = pivotTransform.anchoredPosition;
        Vector3 targetLoc = new Vector3(pivotTransform.anchoredPosition.x, 50.5f, 0f);
        Vector3 targetScale = new Vector3(10f, 10f, 0f);
        Vector3 startScale = upgradeTransform.localScale;
        float time = 0f;
        float duration = .5f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            upgradeTransform.anchoredPosition = Vector2.Lerp(upgradeTransform.anchoredPosition, targetLoc, t);
            upgradeTransform.localScale = Vector3.Lerp(startScale, targetScale, t);


            yield return null;
        }

        Destroy(upgrade);
        GameObject itemAnimInstance = Instantiate(itemAnim, gameObject.transform);
        if (upgradeOptions[index] == cSSprite) ///Common Stat
        {
            itemAnimInstance.GetComponent<Animator>().runtimeAnimatorController = cSCon;
        }
        else if (upgradeOptions[index] == rSSprite) //Rare Stat
        {
            itemAnimInstance.GetComponent<Animator>().runtimeAnimatorController = rSCon;
        }
        else if (upgradeOptions[index] == cASprite) //Common Ability
        {
            itemAnimInstance.GetComponent<Animator>().runtimeAnimatorController = cACon;
        }
        else if (upgradeOptions[index] == rASprite) // Rare Ability
        {
            itemAnimInstance.GetComponent<Animator>().runtimeAnimatorController = rACon;
        }
        upgradeOptions.Clear();
        busy = false;

    }

    private void Finish()
    {
        if (spawned || !busy) //This may break it idk
        {
            return;
        }
        animatorSpeed = anim.speed;
        anim.speed = 0f;
        for (int i = 0; i < 3; i++)
        {
            if (upgradeList.Count <= 0)
            {
                upgradeList = CreateUpgradeList();
            }
            GameObject itemInstance = Instantiate(item, gameObject.transform); 
            upgradeGO.Add(itemInstance);
            Sprite itemSprite = upgradeList.Dequeue();
            Debug.Log(itemSprite.name);
            upgradeOptions.Add(itemSprite);
            itemInstance.GetComponent<Image>().sprite = itemSprite;
            if (i == 1)
            {
                itemInstance.GetComponent<RectTransform>().anchoredPosition += new Vector2(105f, 0f);
            }
            else if (i == 2)
            {
                itemInstance.GetComponent<RectTransform>().anchoredPosition += new Vector2(210f, 0f);
            }
        }
        spawned = true;
    }

    

    private Queue<Sprite> CreateUpgradeList()
    {
        List<Sprite> upgrade = new();
        for (int i = 0; i < commonAbility; i++)
        {
            upgrade.Add(cASprite);
        }
        for (int i = 0; i < commonStat; i++)
        {
            upgrade.Add(cSSprite);
        }
        for (int i = 0; i < rareAbility; i++)
        {
            upgrade.Add(rASprite);
        }
        for (int i = 0; i < rareStat; i++)
        {
            upgrade.Add(rSSprite);
        }
        upgrade.Shuffle();


        return new Queue<Sprite>(upgrade);
    }
}
