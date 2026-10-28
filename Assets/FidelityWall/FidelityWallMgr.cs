using Unity.VisualScripting;
using UnityEngine;

public class FidelityWallMgr : MonoBehaviour
{
    public GlobalsMgr g;
    public enum TrainingInference
    {
        train,
        infer
    }
    public TrainingInference trainingInference;
    public RenderTexture renderTexture;
    public bool ynShowSecond;
    public bool ynPersonSecond;
    bool ynShowSecondLast;
    bool ynPersonSecondLast;
    public GameObject personSecond;


    void Awake()
    {
        // inferenceTraining = InferenceTraining.infer;    
    }

    void Start()
    {
        renderTexture.width = g.resX;
        renderTexture.height = g.resY;
        if (trainingInference == TrainingInference.infer)
        {
            // test visual diff between load and not loaded
            // InvokeRepeating(nameof(ToggleYnPause), 5, 5);
        }
        personSecond.SetActive(g.fidelityWallMgr.ynPersonSecond);
    }

    void Update()
    {
        if (g.ynPause) return;
        if (ynPersonSecondLast != ynPersonSecond)
        {
            personSecond.SetActive(ynPersonSecond);
        }
        if (ynShowSecondLast != ynShowSecond)
        {
            
        }
        ynPersonSecondLast = ynPersonSecond;
        ynShowSecondLast = ynShowSecond;
    }

    void ToggleYnPause()
    {
        g.ynPause = !g.ynPause;
        Debug.Log("toggling g.ynPause\n");
    }
}
