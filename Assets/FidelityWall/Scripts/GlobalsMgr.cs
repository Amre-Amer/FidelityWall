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
    public float recordingDuration;
    public int resX;
    public int resY;
    public int pixelCount;

    void Awake()
    {
        recordingDuration = 30;
        resX = 38;
        resY = 20;
        pixelCount = resX * resY;
    }

}
