using NUnit.Framework;
using System.Collections;
using UnityEngine;

public class FPS : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _fps;
    private float _fpsValue;
    [SerializeField] private float _fpsUpdateRate;
    private float _fpsTime;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        FPSChecker();
    }

    private void FPSChecker()
    {
        if (_fpsTime >= _fpsUpdateRate)
        {
            _fpsTime = 0;
            _fps.text = "FPS: " + (_fpsValue/_fpsUpdateRate).ToString();
            _fpsValue = 0;
        }
        else
        {
            _fpsTime += Time.deltaTime;
        }

        _fpsValue++;
        
        
    }

  

}