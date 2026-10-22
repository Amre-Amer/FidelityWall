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
    public float recordingDuration = 10; //30;
    public int resX;
    public int resY;
    public int pixelCount;

    void Awake()
    {
        resX = 19;
        resY = 10;
        pixelCount = resX * resY;
    }

}
