using System.Collections.Generic;
using UnityEngine;

public class IndicatorsMgr : MonoBehaviour
{
    public GlobalsMgr g;
    public Colors colors;
    public List<GameObject>indicators;
    public List<GameObject>scores;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetColors();    
        // SetColor(3);    
        if (g.fidelityWallMgr.inferenceTraining == FidelityWallMgr.InferenceTraining.train)
        {
            ShowHideScores(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowHideScores(bool ynOn)
    {
        foreach(GameObject score in scores)
        {
            score.SetActive(ynOn);
        }
    }

    public void SetScores(float[] values)
    {
        for(int n = 0; n < scores.Count; n++)
        {
            GameObject score = scores[n];
            score.transform.localScale = Vector3.one * values[n];
        }
    }

    public void SetColorHeights(float[] values)
    {
    }

    void SetColorHeight(GameObject go, float fraction)
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
