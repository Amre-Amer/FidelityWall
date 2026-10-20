using System.Collections.Generic;
using UnityEngine;

public class IndicatorsMgr : MonoBehaviour
{
    public Colors colors;
    public List<GameObject>indicators;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetColors();    
        SetColor(3);    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetColor(int nOn)
    {
        for(int n = 0; n < indicators.Count; n++)
        {
            GameObject indicator = indicators[n];
            if (n == nOn)
            {
                indicator.GetComponentInChildren<Renderer>().enabled = true;
            } else
            {
                indicator.GetComponentInChildren<Renderer>().enabled = false;
            }
        }
    }

    void SetColors()
    {
        for(int n = 0; n < indicators.Count; n++)
        {
            GameObject indicator = indicators[n];
            Renderer rend = indicator.GetComponentInChildren<Renderer>();
            rend.material.color = colors.colors[n];
        }
    }
}
