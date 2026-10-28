// https://www.google.com/search?q=unity+6.3+c%23+inference+with+onnx+model+&sca_esv=4edad2ce5cdf1c22&rlz=1C1HKFL_enUS1231US1231&biw=1536&bih=695&sxsrf=APpeQnuPpEuHCuie18w9AgEP0mlmHtGAwA%3A1792612901475&ei=JRrZaq26HIOcw8cPhK-MsAw&uact=5&oq=unity+6.3+c%23+inference+with+onnx+model+&gs_lp=Egxnd3Mtd2l6LXNlcnAiJ3VuaXR5IDYuMyBjIyBpbmZlcmVuY2Ugd2l0aCBvbm54IG1vZGVsIDIFEAAY7wUyBRAAGO8FMggQABiABBiiBDIFEAAY7wVIsIUBUKoHWJ53cAF4AZABAJgBe6ABnwyqAQQzLjEysAEAuAEDyAEA-AEBmAIHoAK8BsICChAAGEcY1gQYsAPCAgUQIRigAcICBRAhGJ8FwgIFECEYqwKYAwDiAwQYACBd4gMEGAAgXuIDBBgAIF_iAwQYACBg4gMEGAAgYeIDBBgAIGKIBgGQBgiSBwMxLjagB7RNsgcDMC42uAekBsIHBTItNC4zyAc3gAgB&sclient=gws-wiz-serp
using UnityEngine;
using Unity.InferenceEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class InferenceMgr : MonoBehaviour
{
    public GlobalsMgr g;
    [Header("Model Configuration")]
    [SerializeField] private ModelAsset modelAsset; // Assign your .onnx file here
    [SerializeField] private BackendType backend = BackendType.GPUCompute; // Use GPUCompute or CPU
    private Model runtimeModel;
    private Worker worker;
    int tn;
    float ratingAve;
    float ratingSum;
    int ratingNum;
    public TMP_Text textRating;

    void Start()
    {
        runtimeModel = ModelLoader.Load(modelAsset);
        worker = new Worker(runtimeModel, backend);
        // float[] nums = new float[10]{0.1f, .2f, .25f, .3f, .35f, 1, .35f, .3f, .2f, .1f};
        // RateConfidenceDistribution(nums);
        // Debug.Log("-----------------------------------------------");
        // nums = new float[10]{0.1f, .2f, .25f, .3f, .35f, 4, .35f, 1f, .2f, .1f};
        // RateConfidenceDistribution(nums);
        // Debug.Log("-----------------------------------------------");
        // nums = new float[10]{0.1f, .2f, .25f, 3, .35f, 4, .35f, 1f, .2f, .1f};
        // RateConfidenceDistribution(nums);
        // Debug.Log("-----------------------------------------------");
    }

    public void Infer()
    {
        g.trainingMgr.GrabLiveData();
    }

    public float[] Inference(float[] inputData)
    {
        float t = Time.realtimeSinceStartup;

        TensorShape inputShape = new TensorShape(1, g.pixelCount);  
        using Tensor<float> inputTensor = new Tensor<float>(inputShape, inputData);
        worker.Schedule(inputTensor);
        Tensor<float> outputTensor = worker.PeekOutput(0) as Tensor<float>;
        
        float[] results = outputTensor.DownloadToArray();
        float[] resultsNormalized = g.toolsMgr.NormalizeFloatArray(results);
        
        g.indicatorsMgr.SetScores(resultsNormalized);
        int nWinner = ShowWinners(resultsNormalized);
        RateConfidenceDistribution(resultsNormalized);
        
        ShowElapsedTime(t);
        return resultsNormalized;
    }

    float RateConfidenceDistribution(float[] resultsNormalized)
    {
        int nWinner = System.Array.IndexOf(resultsNormalized, resultsNormalized.Max());
        string txt = "";
        int rating = 0;
        int dir = 1;
        for(int n = 0; n < resultsNormalized.Length - 1; n++)
        {

            float nValue = resultsNormalized[n];
            float nValueNext = resultsNormalized[n + 1];
            float slope = Mathf.Sign(nValueNext - nValue);
            if (n == nWinner) dir = -1;
            string r = "-";
            if (slope == dir)
            {
                rating++;
                r = " ";
            } 
            int g = (int)(nValue * 30) + 1;
            txt += "\n\t" + nValue.ToString("F2") + r + "\t" + new string('|', g);
        }
        ratingSum += rating;
        ratingNum++;
        ratingAve = ratingSum / ratingNum;
        string msg = "rating: " + rating.ToString("F1") + " / " + ratingAve.ToString("F1") + txt + "\n";
        Debug.Log(msg);
        textRating.text = rating.ToString("F1") + "/" + ratingAve.ToString("F1");
        return rating;
    }
    void ShowElapsedTime(float t)
    {
        t = Time.realtimeSinceStartup - t;
        tn++;
        if (tn % UnityEngine.Random.Range(30, 50) == 0) {
            Debug.Log(g.cntFrames + " inference: " + (t * 1000).ToString("F1") + " ms\n");
        }
    }

    int ShowWinners(float[] resultsNormalized)
    {
        int nClassification = g.toolsMgr.GetIndexOfMax(resultsNormalized);      
        List<int>nOns = new();
        nOns.Add(nClassification);
        if (g.fidelityWallMgr.ynShowSecond)
        {
            int nClassificationSecond = g.toolsMgr.GetIndexOfMaxSecond(resultsNormalized);      
            nOns.Add(nClassificationSecond);
        }
        g.indicatorsMgr.ShowHideColorChoice(nOns);
        g.answersMgr.SetAnswer(nClassification.ToString());
        return nClassification;
    }

    void OnDestroy()
    {
        worker?.Dispose();
    }
}
