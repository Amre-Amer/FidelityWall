using UnityEngine;
using TMPro;

public class ToolsMgr : MonoBehaviour
{
    public TMP_Text textFps;
    int cntFps;

    void Start()
    {
        InvokeRepeating(nameof(Fps), 1, 1);
    }

    void Update()
    {
        cntFps++;    
    }

    void Fps()
    {
        textFps.text = "<size=24>fps:</size> " + cntFps.ToString();
        cntFps = 0;
    }

    public void SetColor(GameObject go, Color color)
    {
        Renderer rend = go.GetComponentInChildren<Renderer>();
        rend.material.color = color;
    }    

    public void EnableDisableRenderer(GameObject go, bool ynOn)
    {
        go.GetComponentInChildren<Renderer>().enabled = ynOn;
    }

    public float[] NormalizeFloatArray(float[] inputs)
    {
        float[]outputs = new float[inputs.Length];
        float min = -1;
        float max = -1;
        float range;
        for(int n = 0; n < inputs.Length; n++)
        {
            float input = inputs[n];
            if (input < min || n == 0) min = input;
            if (input > max || n == 0) max = input;
        }
        range = max - min;
        for(int n = 0; n < outputs.Length; n++)
        {
            float value = inputs[n];
            outputs[n] = (value - min) / range;
        }
        return outputs;
    }

    public int GetIndexOfMax(float[] outputs)
    {
        int nMax = -1;
        float max = -1;
        for(int n = 0; n < outputs.Length; n++)
        {
            float val = outputs[n];
            if (val > max)
            {
                nMax = n;
                max = val;
            }
        }
        return nMax;
    }
}
