using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class IndicatorsMgr : MonoBehaviour
{
    public GlobalsMgr g;
    public Colors colors;
    public List<GameObject>indicators;
    public List<GameObject>scores;
    public TMP_Text textAccuracy;
    float accuracyAve;
    float smooth = .001f;

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

    public void ShowHideAccuracy(bool ynOn)
    {
        textAccuracy.gameObject.SetActive(ynOn);
    }

    public void SetAccuracy(float[] values, int nMax)
    {
        float max = values[nMax];
        float ave = values.Average();
        float accuracy = max - ave;
        accuracyAve = smooth * accuracy + (1 - smooth) * accuracyAve;
        textAccuracy.text = accuracyAve.ToString("F2");
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
