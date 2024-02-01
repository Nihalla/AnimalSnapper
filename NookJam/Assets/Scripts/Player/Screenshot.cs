using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Screenshot : MonoBehaviour
{
    private bool takeScreenshot = false;
    private Camera s_camera;
    private static Screenshot instance;
    private string file_name = "default";
    private UploadScreen j_m_script;
    [SerializeField] private AnimalLog log_script;


    private void Awake()
    {
        instance = this;
        s_camera = gameObject.GetComponent<Camera>();
        j_m_script = GameObject.FindGameObjectWithTag("JournalManager").GetComponent<UploadScreen>();
    }

    private void OnPostRender()
    {
        if(takeScreenshot)
        {
            takeScreenshot = false;
            RenderTexture renderTexture = s_camera.targetTexture;

            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);

            byte[] byteArray = renderResult.EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + "/Screenshots/" + file_name + ".png", byteArray);
            RenderTexture.ReleaseTemporary(renderTexture);
            s_camera.targetTexture = null;
            j_m_script.SetTexture(file_name);
        }    
    }

    private void TakeScreenshot(int width, int height)
    {
        s_camera.targetTexture = RenderTexture.GetTemporary(width, height);
        takeScreenshot = true;
        UpdatePhoto();
    }

    public void CallScreenShot(int widht, int height, string animal)
    {
        file_name = animal;
        instance.TakeScreenshot(widht, height);
    }

    public void UpdatePhoto()
    {
        log_script.TakenPhoto();
    }
}
