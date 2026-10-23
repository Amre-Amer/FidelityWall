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

    void Start()
    {
        renderTexture.width = g.resX;
        renderTexture.height = g.resY;
    }
}
