// https://www.google.com/search?q=unity+6.3+c%23+inference+with+onnx+model+&sca_esv=4edad2ce5cdf1c22&rlz=1C1HKFL_enUS1231US1231&biw=1536&bih=695&sxsrf=APpeQnuPpEuHCuie18w9AgEP0mlmHtGAwA%3A1792612901475&ei=JRrZaq26HIOcw8cPhK-MsAw&uact=5&oq=unity+6.3+c%23+inference+with+onnx+model+&gs_lp=Egxnd3Mtd2l6LXNlcnAiJ3VuaXR5IDYuMyBjIyBpbmZlcmVuY2Ugd2l0aCBvbm54IG1vZGVsIDIFEAAY7wUyBRAAGO8FMggQABiABBiiBDIFEAAY7wVIsIUBUKoHWJ53cAF4AZABAJgBe6ABnwyqAQQzLjEysAEAuAEDyAEA-AEBmAIHoAK8BsICChAAGEcY1gQYsAPCAgUQIRigAcICBRAhGJ8FwgIFECEYqwKYAwDiAwQYACBd4gMEGAAgXuIDBBgAIF_iAwQYACBg4gMEGAAgYeIDBBgAIGKIBgGQBgiSBwMxLjagB7RNsgcDMC42uAekBsIHBTItNC4zyAc3gAgB&sclient=gws-wiz-serp
using UnityEngine;
using Unity.InferenceEngine;

public class InferenceMgr : MonoBehaviour
{
    public GlobalsMgr g;
    [Header("Model Configuration")]
    [SerializeField] private ModelAsset modelAsset; // Assign your .onnx file here
    [SerializeField] private BackendType backend = BackendType.GPUCompute; // Use GPUCompute or CPU
    private Model runtimeModel;
    private Worker worker;

    void Start()
    {
        runtimeModel = ModelLoader.Load(modelAsset);
        worker = new Worker(runtimeModel, backend);
        if(g.fidelityWallMgr.inferenceTraining == FidelityWallMgr.InferenceTraining.train)
        {
            g.indicatorsMgr.ShowHideAccuracy(false);
        }
    }

    public void Infer()
    {
        g.trainingMgr.GrabLiveData();
    }

    public float[] Inference(float[] inputData)
    {
        TensorShape inputShape = new TensorShape(1, g.pixelCount);  
        using Tensor<float> inputTensor = new Tensor<float>(inputShape, inputData);
        worker.Schedule(inputTensor);
        Tensor<float> outputTensor = worker.PeekOutput(0) as Tensor<float>;
        float[] results = outputTensor.DownloadToArray();
        float[] resultsNormalized = g.toolsMgr.NormalizeFloatArray(results);
        g.indicatorsMgr.SetScores(resultsNormalized);
        int nClassification = g.toolsMgr.GetIndexOfMax(resultsNormalized);      
        g.indicatorsMgr.SetAccuracy(resultsNormalized, nClassification);
        g.indicatorsMgr.SetColor(nClassification);
        g.answersMgr.SetAnswer(nClassification.ToString());
        return resultsNormalized;
    }

    void OnDestroy()
    {
        worker?.Dispose();
    }
}
