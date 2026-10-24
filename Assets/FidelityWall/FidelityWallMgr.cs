using UnityEngine;

public class FidelityWallMgr : MonoBehaviour
{
    public GlobalsMgr g;
    public enum InferenceTraining
    {
        infer,
        train
    }
    public InferenceTraining inferenceTraining;
    public RenderTexture renderTexture;

    void Awake()
    {
        // inferenceTraining = InferenceTraining.infer;    
    }

    void Start()
    {
        renderTexture.width = g.resX;
        renderTexture.height = g.resY;
        if (inferenceTraining == InferenceTraining.infer)
        {
            // test visual diff between load and not loaded
            // InvokeRepeating(nameof(ToggleYnPause), 5, 5);
        }
    }

    void ToggleYnPause()
    {
        g.ynPause = !g.ynPause;
        Debug.Log("toggling g.ynPause\n");
    }
}
