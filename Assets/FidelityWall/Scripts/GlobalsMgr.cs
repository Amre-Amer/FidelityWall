using Unity.Android.Gradle.Manifest;
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
    public float recordingDuration;
    public int resX;
    public int resY;
    public int pixelCount;
    public int cntFrames;

    void Awake()
    {
        recordingDuration = 30;
        resX = 38; // based on 19 * n
        resY = 20; // based on 10 * n
        pixelCount = resX * resY;
    }

    void Update()
    {
        cntFrames++;
    }

}
