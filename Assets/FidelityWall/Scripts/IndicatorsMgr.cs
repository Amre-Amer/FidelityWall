using System.Collections.Generic;
using UnityEngine;

public class IndicatorsMgr : MonoBehaviour
{
    public GlobalsMgr g;
    public Colors colors;
    public List<GameObject>indicators;
    public List<GameObject>scores;

    void Start()
    {
        SetColors();    
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
            Vector3 sca = score.transform.localScale;
            sca.y = values[n];
            score.transform.localScale = sca;
        }
    }

    public void ShowHideColorChoice(List<int> nOns)
    {        
        for(int n = 0; n < indicators.Count; n++)
        {
            GameObject indicator = indicators[n];
            if (nOns.Contains(n))
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
