using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    private int _screenSizeX;
    private int _screenSizeY;
    private Camera _camera;
    private const float TargetAspect = 1.3f;

    private void Start()
    {
        _camera = Camera.main;
        RescaleCamera();
    }

    private void Update()
    {
        RescaleCamera();
    }
    
    private void OnPreCull()
    {
        var rect = _camera.rect;
            
        _camera.rect = new Rect(0, 0, 1, 1);;
        GL.Clear(true, true, Color.black);
        _camera.rect = rect;
    }
    
    private void RescaleCamera()
    {
        if (Screen.width == _screenSizeX) return;
        
        var windowAspect = (float)Screen.width / (float)Screen.height;
        var scaleHeight = windowAspect / TargetAspect;

        var scaleWidth = 1.0f / scaleHeight;

        var rect = _camera.rect;

        rect.width = scaleWidth;
        rect.height = 1.0f;
        rect.x = (1.0f - scaleWidth) / 2.0f;
        rect.y = 0;

        _camera.rect = rect;
 
        _screenSizeX = Screen.width;
    }
}