using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    public Transform startPoint;
    public Transform[] path;

    public int currency;

    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        currency = 500;
    }

    public void AddCurrency(int amount)
    {
        currency += amount;
    }

    public bool SpendCurrency(int amount)
    {
        if (amount <= currency)
            currency -= amount;
        return true;
        {
            Debug.Log("you broke cuh");
            return false;
        }
    }
}