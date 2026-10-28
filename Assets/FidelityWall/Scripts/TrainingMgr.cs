using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;
using System.Text;

public class TrainingMgr : MonoBehaviour
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
    float timeStartRecording;
    bool ynDone;
    public TMP_Text textStats;
    int nRecordsSaved;
    string txtElapsedTime;
    public GameObject backdrop;
    public GameObject scores;
    List<string> pixelsAllTxt = new();
    int bytesTot;

    void Start()
    {
        if (g.fidelityWallMgr.trainingInference == FidelityWallMgr.TrainingInference.infer)
        {
            textStats.gameObject.SetActive(false);
        }
        if (g.fidelityWallMgr.trainingInference == FidelityWallMgr.TrainingInference.train)
        {
            scores.SetActive(false);
            g.inferenceMgr.textRating.gameObject.SetActive(false);
        }
        filespecPixels = Path.Combine(path, filenamePixels);        
        filespecClassifications = Path.Combine(path, filenameClassifications);     
        if (g.fidelityWallMgr.trainingInference == FidelityWallMgr.TrainingInference.train)
        {
            Invoke(nameof(StartRecording), 1);
        }   
    }

    void StartRecording()
    {
        Invoke(nameof(Done), g.recordingDuration);
        timeStartRecording = Time.realtimeSinceStartup;
        InvokeRepeating(nameof(ElapsedTime), 0, 1);
    }

    void ElapsedTime()
    {
        if (ynDone) return;
        float t = Time.realtimeSinceStartup - timeStartRecording;
        txtElapsedTime = "recording: " + t.ToString("F0") + " / " + g.recordingDuration + " sec";
        Debug.Log(txtElapsedTime + " records saved: " + nRecordsSaved + "\n");
    }

    void Done()
    {
        ynDone = true;
        g.toolsMgr.SetColor(backdrop, Color.green);
        Debug.Log("training done");
        SaveData();
    }

    void SaveData()
    {
        File.WriteAllText(filespecClassifications, txtClassifications);
        txtPixels = PixelsAllToString();
        File.WriteAllText(filespecPixels, txtPixels);
    }

    public void GrabLiveData()
    {
        AsyncGPUReadback.Request(renderTexture, 0, OnCompleteReadbackLive);
    }

    void InferencePersons(Unity.Collections.NativeArray<Color32> pixels)
    {
        // Texture2D tex = new Texture2D(g.resX, g.resY, GraphicsFormat.R8G8B8A8_UNorm, TextureCreationFlags.None);
        // Texture2D tex = new Texture2D(640, 640, GraphicsFormat.R8G8B8A8_UNorm, TextureCreationFlags.None);
        Texture2D tex = RenderTexture2Texture2D(renderTexture);
        g.yoloV8Inference.Inference(tex);
        // g.mobileNetSSDInference.Inference(tex);
    }

    Texture2D Color32Array2Texture2D(Unity.Collections.NativeArray<Color32> pixels)
    {
        Texture2D tex = new Texture2D(g.resX, g.resY, TextureFormat.RGBA32, false);
        Color32[] colors = pixels.ToArray();
        tex.SetPixels32(colors);
        tex.Apply();
        return tex;
    }

    Texture2D RenderTexture2Texture2D(RenderTexture rend)
    {
        Texture2D tex = new Texture2D(rend.width, rend.height, TextureFormat.RGBA32, false);
        RenderTexture previousActive = RenderTexture.active;
        RenderTexture.active = rend;
        tex.ReadPixels(new Rect(0, 0, rend.width, rend.height), 0, 0);
        tex.Apply();
        RenderTexture.active = previousActive;
        return tex;
    }

    void OnCompleteReadbackLive(AsyncGPUReadbackRequest request)
    {
        if (request.hasError)
        {
            // Debug.LogError("GPU readback error occurred.");
            return;
        }
        Unity.Collections.NativeArray<Color32> pixels = request.GetData<Color32>();
        if (g != null)
        {
            float[] greys = ColorArrayToGrey(pixels.ToArray());
            g.inferenceMgr.Inference(greys); // using 640x640
            // InferencePersons(pixels);
        }
    }

    public void RecordTrainingData()
    {
        if(ynDone) return;
        AsyncGPUReadback.Request(renderTexture, 0, OnCompleteReadback);
    }

    void OnCompleteReadback(AsyncGPUReadbackRequest request)
    {
        if (request.hasError)
        {
            // Debug.LogError("GPU readback error occurred.");
            return;
        }
        Color32[] pixels = request.GetData<Color32>().ToArray();
        // string pixelsTxt = PixelsToString(pixels);
        string pixelsTxt = ColorArrayToString(pixels);
        pixelsAllTxt.Add(pixelsTxt);

        bytesTot += pixels.Length * 4;

        string s = "\n";
        if (txtClassifications == "") s = "";
        nIndicator = g.answersMgr.FindIndicator();
        txtClassifications += s + nIndicator;

        nRecordsSaved++;

        ShowStats();
    }

    void ShowStats()
    {
        string txt = "<size=24>Records Saved:</font>";
        txt += "\n" + nRecordsSaved;
        txt += "\n" + txtElapsedTime;
        txt += "\n" + (bytesTot / 1000000) + " mb";
        textStats.text = txt;
    }

    string PixelsAllToString()
    {
        string txtAll = "";
        for(int n = 0; n < pixelsAllTxt.Count; n++)
        {
            string txt = pixelsAllTxt[n];
            string s = "\n";
            if (txtAll == "") s = "";
            txtAll += s + txt;
            Debug.Log("pixels --> string: " + n + " / " + pixelsAllTxt.Count + "\n");
        }
        return txtAll;       
    }

    string PixelsToString(Color32[] pixels)
    {
        string txt = "";        
        for(int n = 0; n < pixels.Count(); n++) 
        {
            Color c = pixels[n];
            string s = ",";
            if (txt == "") s = "";
            txt += s + c.grayscale;
        }
        return txt;
    }

    float[] ColorArrayToGrey(Color32[] colors)
    {
        float[] greys = new float[colors.Length];
        for(int n = 0; n < colors.Length; n++)
        {
            Color32 c = colors[n];
            greys[n] = 0.299f * c.r + 0.587f * c.g + 0.114f * c.b;
        }
        return greys;
    }

    public string ColorArrayToString(Color32[] colors)
    {
        StringBuilder sb = new StringBuilder(colors.Length * 4);
        for (int i = 0; i < colors.Length; i++)
        {
            Color32 c = colors[i];
            string grey = (0.299f * c.r + 0.587f * c.g + 0.114f * c.b).ToString();
            sb.Append(grey);
            if (i < colors.Length - 1)
            {
                sb.Append(",");
            }
        }
        return sb.ToString();
    }
}
