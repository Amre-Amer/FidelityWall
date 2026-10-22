using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class DataMgr : MonoBehaviour
{
    public GlobalsMgr g;
    public RenderTexture renderTexture;
    public List<Texture2D> images = new();
    public List<int>classifications = new();
    int nIndicator;
    string txtPixels = "";
    string txtClassifications = "";
    string filenamePixels = "pixels.txt";
    string filenameClassifications = "labels.txt";
    string path = "C:\\Users\\Amre\\FidelityWall\\Assets\\FidelityWall\\Python";
    string filespecPixels;
    string filespecClassifications;
    float recordingDuration = 30;

    void Start()
    {
        filespecPixels = Path.Combine(path, filenamePixels);        
        filespecClassifications = Path.Combine(path, filenameClassifications);     
        if (g.fidelityWallMgr.inferenceTraining == FidelityWallMgr.InferenceTraining.train)
        {
            Invoke(nameof(SaveData), recordingDuration);
        }   
    }

    void SaveData()
    {
        File.WriteAllText(filespecPixels, txtPixels);
        File.WriteAllText(filespecClassifications, txtClassifications);
        Debug.Log("done");
    }

    public void GrabLiveData()
    {
        AsyncGPUReadback.Request(renderTexture, 0, OnCompleteReadbackLive);
    }

    void OnCompleteReadbackLive(AsyncGPUReadbackRequest request)
    {
        if (request.hasError)
        {
            // Debug.LogError("GPU readback error occurred.");
            return;
        }
        Unity.Collections.NativeArray<Color32> pixels = request.GetData<Color32>();
        float[] greys = Colors2Greys(pixels);
        if (g != null)
        {
            g.inferenceMgr.Inference(greys);
        }
    }

    public void RecordTrainingData()
    {
        nIndicator = g.answersMgr.FindIndicator();
        AsyncGPUReadback.Request(renderTexture, 0, OnCompleteReadback);
    }

    void OnCompleteReadback(AsyncGPUReadbackRequest request)
    {
        if (request.hasError)
        {
            Debug.LogError("GPU readback error occurred.");
            return;
        }
        Unity.Collections.NativeArray<Color32> pixels = request.GetData<Color32>();
        string txtColors = Colors2Data(pixels);
        string s = "\n";
        if (txtPixels == "") s = "";
        txtPixels += s + txtColors;
        txtClassifications += s + nIndicator;
    }

    float[] Colors2Greys(Unity.Collections.NativeArray<Color32> pixels)
    {
        float[] greys = new float[pixels.Count()];
        for(int n = 0; n < pixels.Count(); n++) 
        {
            Color color = pixels[n];
            float grayIntensity = color.grayscale;
            greys[n] = grayIntensity;
        }
        return greys;
    }

    string Greys2String(float[] greys)
    {
        string txtData = "";        
        for(int n = 0; n < greys.Length; n++)
        {
            float grayIntensity = greys[n];
            string s = ",";
            if (txtData == "") s = "";
            txtData += s + grayIntensity;
        }
        return txtData;
    }

    string Colors2Data(Unity.Collections.NativeArray<Color32> pixels)
    {
        float[] greys = Colors2Greys(pixels);
        string txtData = Greys2String(greys);
        return txtData;
    }
}
