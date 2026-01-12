using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyHealth : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private int hitPoints = 3;
    [SerializeField] private int currencyOnDestroy = 10;

    private bool isDestroyed = false;
    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        if (hitPoints <= 0 && !isDestroyed)
        {
            EnemySpawner.onEnemyDestroy.Invoke();
            LevelManager.main.AddCurrency(currencyOnDestroy);
            isDestroyed = true;
            Destroy(gameObject);
        }
    }
}