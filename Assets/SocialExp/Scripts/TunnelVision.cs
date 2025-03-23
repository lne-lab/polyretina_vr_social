using LNE.ProstheticVision;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

using System.Runtime.InteropServices;


public class TunnelVision : MonoBehaviour
{

    private Camera eyeCamera;  // Single camera for either left or right eye
    public float fov = 20f; // Field of View for the tunnel vision
    private bool debugMode = false; // Toggle for debug mode
    private bool isLeftEye = true; // Toggle to switch between left and right eye

    private Material tunnelVisionMaterial;
     Vector2 gazePos = new Vector2(0.5f, 0.5f); // Default gaze position in viewport coordinates
     Vector2 offset = new Vector2(0.5f, 0.5f); //offset for the gaze
    private HeadsetModel headset = HeadsetModel.VivePro;
    private StereoTargetEyeMask targetEye = StereoTargetEyeMask.Right;
    [Header("Eye Tracking")]
    private EyeGaze.Source eyeGazeSource = EyeGaze.Source.EyeTracking;


    void Start()
    {
        // Create a simple shader material for the tunnel vision
        tunnelVisionMaterial = new Material(Shader.Find("Hidden/TunnelVision"));
        eyeCamera = gameObject.GetComponent<Camera>();
       // SRanipal_Eye_API.RegisterEyeDataCallback_v2(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
    }

    void Update()
    {
        if (debugMode)
        {
            HandleDebugInput();
        }
        else
        {
            UpdateGazePosition();
        }

        // Update the shader with the gaze position and FOV
        tunnelVisionMaterial.SetVector("_GazePosition", gazePos);
        tunnelVisionMaterial.SetFloat("_FOV", fov);
    }

    void HandleDebugInput()
    {
        // Move the gaze position with the arrow keys
        if (Input.GetKey(KeyCode.UpArrow))
            gazePos.y += 0.01f;
        if (Input.GetKey(KeyCode.DownArrow))
            gazePos.y -= 0.01f;
        if (Input.GetKey(KeyCode.LeftArrow))
            gazePos.x -= 0.01f;
        if (Input.GetKey(KeyCode.RightArrow))
            gazePos.x += 0.01f;

        // Clamp the values to ensure they stay within the viewport
        gazePos.x = Mathf.Clamp(gazePos.x, 0f, 1f);
        gazePos.y = Mathf.Clamp(gazePos.y, 0f, 1f);
    }

    void UpdateGazePosition()
    {
        // Use the eye tracking API to get the current gaze position for a specific eye

        var eyeGaze = EyeGaze.Get(eyeGazeSource, headset);
        gazePos = eyeGaze + offset;
       
    }
    public void SetTargetEye(StereoTargetEyeMask target)
    {
        targetEye = target;
    }
   


    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // Apply the tunnel vision effect when using the camera
        if (isLeftEye || !isLeftEye)
        {
            Graphics.Blit(source, destination, tunnelVisionMaterial);
        }
    }
}
