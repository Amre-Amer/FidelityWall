using System.Collections.Generic;
using UnityEngine;

public class IndicatorsMgr : MonoBehaviour
{
    public GlobalsMgr g;
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
                g.toolsMgr.EnableDisableRenderer(indicator, true);
            } else
            {
                g.toolsMgr.EnableDisableRenderer(indicator, false);
            }
        }
    }

    void SetColors()
    {
        for(int n = 0; n < indicators.Count; n++)
        {
            GameObject indicator = indicators[n];
            g.toolsMgr.SetColor(indicator, colors.colors[n]);
        }
    }
}
