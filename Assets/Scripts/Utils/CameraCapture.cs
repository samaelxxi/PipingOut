

using System.IO;
using UnityEngine;

public class CameraCapture : MonoBehaviour
{
    public KeyCode screenshotKey = KeyCode.F12;
    private Camera _camera;
 
    void Start () {
      _camera = GetComponent<Camera>();
    }
 
    private void LateUpdate()
    {
        if (Input.GetKeyDown(screenshotKey))
        {
            Capture();
        }
    }
 
    public void Capture()
    {
        RenderTexture activeRenderTexture = RenderTexture.active;
        Debug.Log(_camera);
        RenderTexture.active = _camera.targetTexture;
 
        _camera.Render();
 
        Texture2D image = new Texture2D(_camera.targetTexture.width, _camera.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, _camera.targetTexture.width, _camera.targetTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = activeRenderTexture;
 
        byte[] bytes = image.EncodeToPNG();
        Destroy(image);
 
        Debug.Log(bytes);
 
        File.WriteAllBytes(Path.Combine(Application.dataPath + "/Art/Skybox", $"{gameObject.name}.png"), bytes);
    }
}