using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   

public class HealthBarEnemy : MonoBehaviour
{

    private Slider slider;


    public void UpdateHealth( float currentValue, float maxValue)
    {
        slider.value  = currentValue / maxValue;
    }


    void Start()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
