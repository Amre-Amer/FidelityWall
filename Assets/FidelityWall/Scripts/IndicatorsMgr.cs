using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class IndicatorsMgr : MonoBehaviour
{
    public GlobalsMgr g;
    public Colors colors;
    public List<GameObject>indicators;
    public List<GameObject>scores;
    // public GameObject accuracy;
    // public GameObject accuracyMin;
    // public GameObject accuracyMax;
    float min;
    float max;
    int nCount;

    void Start()
    {
        SetColors();    
        // if (g.fidelityWallMgr.inferenceTraining == FidelityWallMgr.InferenceTraining.train)
        // {
        //     ShowHideScores(false);
        // }
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

    // public void ShowHideAccuracy(bool ynOn)
    // {
    //     accuracy.SetActive(ynOn);
    // }

    float GetSumDiff(float[] values, int nMax)
    {
        float val = values[nMax];
        float sumDiff = 0;
        foreach(float v in values)
        {
            sumDiff += Mathf.Abs(v - val);
        }
        return sumDiff;
    }

    void SetScaleX(GameObject go, float sx)
    {
        Vector3 sca = go.transform.localScale;
        sca.x = sx;
        go.transform.localScale = sca;
    }

    void CopyScale(GameObject goTarget, GameObject goSource)
    {
        goTarget.transform.localScale = goSource.transform.localScale;
    }

    // public void SetAccuracy(float[] values, int nMax)
    // {
    //     float sumDiff = GetSumDiff(values, nMax);
    //     SetScaleX(accuracy, sumDiff);
    //     if (sumDiff < min || nCount == 0) {
    //         min = sumDiff;
    //         CopyScale(accuracyMin, accuracy);
    //     }
    //     if (sumDiff > max || nCount == 0) {
    //         max = sumDiff;
    //         CopyScale(accuracyMax, accuracy);
    //     }
    //     nCount++;
    //     // Debug.Log(min.ToString("F2") + " < " + sumDiff.ToString("F2") + " < " + max.ToString("F2") + "\n");
    // }

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
