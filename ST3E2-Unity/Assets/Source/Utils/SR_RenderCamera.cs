using System.IO;
using UnityEngine;

public class SR_RenderCamera : MonoBehaviour
{
    public int FileCounter = 0;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    public void CamCapture()
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;

        //GL.ClearWithSkybox(true, cam);
        GL.Clear(true, true, Color.clear);
        cam.Render();

        Texture2D Image = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
        Image.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        Image.Apply();
        RenderTexture.active = currentRT;

        var Bytes = Image.EncodeToPNG();
        Destroy(Image);

        string outputPath =
            Application.dataPath + "/Textures/Billboards/Output/Bb-" + FileCounter + ".png";
        File.WriteAllBytes(outputPath, Bytes);
        FileCounter++;
        Debug.Log("output " + FileCounter);
    }

}
