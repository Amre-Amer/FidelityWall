// https://www.google.com/search?q=unity+6.3+c%23+inference+with+onnx+model+&sca_esv=4edad2ce5cdf1c22&rlz=1C1HKFL_enUS1231US1231&biw=1536&bih=695&sxsrf=APpeQnuPpEuHCuie18w9AgEP0mlmHtGAwA%3A1792612901475&ei=JRrZaq26HIOcw8cPhK-MsAw&uact=5&oq=unity+6.3+c%23+inference+with+onnx+model+&gs_lp=Egxnd3Mtd2l6LXNlcnAiJ3VuaXR5IDYuMyBjIyBpbmZlcmVuY2Ugd2l0aCBvbm54IG1vZGVsIDIFEAAY7wUyBRAAGO8FMggQABiABBiiBDIFEAAY7wVIsIUBUKoHWJ53cAF4AZABAJgBe6ABnwyqAQQzLjEysAEAuAEDyAEA-AEBmAIHoAK8BsICChAAGEcY1gQYsAPCAgUQIRigAcICBRAhGJ8FwgIFECEYqwKYAwDiAwQYACBd4gMEGAAgXuIDBBgAIF_iAwQYACBg4gMEGAAgYeIDBBgAIGKIBgGQBgiSBwMxLjagB7RNsgcDMC42uAekBsIHBTItNC4zyAc3gAgB&sclient=gws-wiz-serp
using UnityEngine;
using Unity.InferenceEngine; // Required namespace for Unity 6

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
    }

    public void Infer()
    {
        g.dataMgr.GrabLiveData();
    }

    public float[] Inference(float[] inputData)
    {
        TensorShape inputShape = new TensorShape(1, 190);         
        using Tensor<float> inputTensor = new Tensor<float>(inputShape, inputData);
        worker.Schedule(inputTensor);
        Tensor<float> outputTensor = worker.PeekOutput(0) as Tensor<float>;
        float[] results = outputTensor.DownloadToArray();
        int nClassification = GetIndexOfMax(results);      
        g.indicatorsMgr.SetColor(nClassification);
        g.answersMgr.SetAnswer(nClassification.ToString());
        Debug.Log("Inference:" + nClassification);
        return results;
    }

    int GetIndexOfMax(float[] outputs)
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

    void OnDestroy()
    {
        // 7. Always dispose of the worker to free native/GPU memory allocations
        worker?.Dispose();
    }
}
