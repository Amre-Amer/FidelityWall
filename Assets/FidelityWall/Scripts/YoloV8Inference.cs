using Unity.InferenceEngine;
using UnityEngine;

public class YoloV8Inference : MonoBehaviour
{
    public GlobalsMgr g;
     [SerializeField] private ModelAsset modelAsset; // Assign your .onnx file here
//    [SerializeField] private string modelFileName = "yolov8n.sentis";
    private Model runtimeModel;
    private Worker engine;

    void Start()
    {
        // 1. Load the model from StreamingAssets
        // string modelPath = System.IO.Path.Combine(Application.streamingAssetsPath, modelFileName);
        runtimeModel = ModelLoader.Load(modelAsset);

        // 2. Create the worker engine (GPUCompute or CPU)
        engine = new Worker(runtimeModel, BackendType.GPUCompute);

        // engine = WorkerFactory.CreateWorker(BackendType.GPUCompute, runtimeModel);

        // 3. Run inference with a texture/image (resized to 640x640 for YOLOv8n)
        // RunInference(inputTexture);
    }

    public void Inference(Texture tex)
    {
        // Allocate once on startup
        Tensor<float> inputTensor = new Tensor<float>(new TensorShape(1, 3, tex.height, tex.width));

        TextureTransform transformSettings = new TextureTransform();

            // .SetDimensions(256, 256, 3)          // Resize texture data to 256x256 with 3 channels (RGB)
            // .SetTensorLayout(TensorLayout.NCHW);

        TextureConverter.ToTensor(tex, inputTensor, transformSettings);

        // Convert texture to a Sentis Tensor (1 x 3 x 640 x 640)
        // Tensor<float> inputTensor = TextureConverter.ToTensor(tex, width: 640, height: 640, channels: 3);
        // Tensor<float> inputTensor = TextureConverter; // .ToTensor(tex, width: 640, height: 640, channels: 3);

        // Execute the neural network
        engine.Schedule(inputTensor);

        // Retrieve output tensor (raw shape depends on YOLOv8n, e.g., 1 x 84 x 8400)
        Tensor<float> outputTensor = engine.PeekOutput() as Tensor<float>;
        // outputTensor.MakeReadable(); // Download from GPU to CPU memory if needed for parsing

        float[] results = outputTensor.DownloadToArray();
        float[] resultsNormalized = g.toolsMgr.NormalizeFloatArray(results);

        string txt = "";
            string s = "\n";
        foreach(float f in resultsNormalized)
        {
            txt += s + f;
        }
        Debug.Log("Yolo: " + txt + "\n");
        // Clean up input tensor
        inputTensor.Dispose();
    }

    void OnDestroy()
    {
        // Dispose of engine and model to free native memory
        engine?.Dispose();
        // runtimeModel?.();
    }
}
