using UnityEngine;

public class GlobalsMgr : MonoBehaviour
{
    public ToolsMgr toolsMgr;
    public TrainingMgr trainingMgr;
    public AnswersMgr answersMgr;
    public InferenceMgr inferenceMgr;
    public IndicatorsMgr indicatorsMgr;
    public FidelityWallMgr fidelityWallMgr;
    public YoloV8Inference yoloV8Inference;
    public MobileNetSSDInference mobileNetSSDInference;
    public float recordingDuration;
    public int resX;
    public int resY;
    public int pixelCount;
    public int cntFrames;
    public bool ynPause;

    void Awake()
    {
        recordingDuration = 30;
        resX = 64; // 38; // based on 19 * n
        resY = 16; // 20; // based on 10 * n
        pixelCount = resX * resY;
    }

    void Update()
    {
        cntFrames++;
    }

}
