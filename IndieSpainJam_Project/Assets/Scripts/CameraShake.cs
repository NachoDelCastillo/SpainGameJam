using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    Vector3 cameraInitialPosition;
    public float shakeMagnitude = 0.05f, shakeTime = 0.5f;
    public Camera mainCamera;
    public void ShakeIt()
    {
        cameraInitialPosition = mainCamera.transform.position;
        InvokeRepeating("StartCameraShaking", 0f, 0.005f);
        Invoke("StopCameraShaking", shakeTime);
    }
    void StartCameraShaking()
    {
        float cameraShakingoffsetx = Random.value * shakeMagnitude * 2 - shakeMagnitude;
        float cameraShakingoffsetY = Random.value * shakeMagnitude * 2 - shakeMagnitude;
        Vector3 cameraIntermadiatePosition = mainCamera.transform.position;
        cameraIntermadiatePosition.x += cameraShakingoffsetx;
        cameraIntermadiatePosition.y += cameraShakingoffsetY;
        mainCamera.transform.position = cameraIntermadiatePosition;
    }

    void StopCameraShaking()
    {
        CancelInvoke("StartCameraShaking");
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraInitialPosition, 0.5f * Time.deltaTime);
        if(mainCamera.transform.position != cameraInitialPosition) Invoke("StopCameraShaking", shakeTime);
    }
    
}
