using UnityEngine;
using Unity.InferenceEngine;

public class MobileNetSSDInference : MonoBehaviour
{
    public GlobalsMgr g;
    public ModelAsset modelAsset;
    // [SerializeField] private string modelFileName = "mobilenet_ssd.onnx";
    // [SerializeField] private Texture2D inputImage;

    private Model runtimeModel;
    private Worker worker;
    const int inputResolution = 300; // Standard MobileNet-SSD input size

    void Start()
    {
        // 1. Load the model from StreamingAssets
        // string modelPath = System.IO.Path.Combine(Application.streamingAssetsPath, modelFileName);
        runtimeModel = ModelLoader.Load(modelAsset);

        // 2. Create the worker (use GPUCompute for hardware acceleration)
        worker = new Worker(runtimeModel, BackendType.GPUCompute);

        // 3. Run inference test
        // RunInference(inputImage);
    }

    public void Inference(Texture2D tex)
    {
        RunInference(tex);
    }

    void RunInference(Texture2D image)
    {
        // Convert input texture to a Sentis Tensor (NCHW format: Batch, Channels, Height, Width)
        using Tensor<float> inputTensor = new Tensor<float>(new TensorShape(1, 1, g.resY, g.resX));

        // Execute the model
        worker.Schedule(inputTensor);

        // Retrieve output tensors (SSD outputs typically include boxes, classes, and scores)
        // Note: Exact output names depend on how the SSD model was exported to ONNX
        Tensor<float> outputBoxes = worker.PeekOutput("boxes") as Tensor<float>;
        Tensor<float> outputScores = worker.PeekOutput("scores") as Tensor<float>;

        // outputBoxes.MakeReadable();
        // outputScores.MakeReadable();

        // Process your bounding boxes and scores here
        Debug.Log("Inference complete. Processing detections...");

        outputBoxes.Dispose();
        outputScores.Dispose();
    }

    void OnDestroy()
    {
        worker?.Dispose();
    }
}
