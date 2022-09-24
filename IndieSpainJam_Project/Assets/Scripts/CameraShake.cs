using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    Vector3 cameraInitialPosition;
    public float shakeMagnitude = 0.05f, shakeTime = 0.5f;
    public Camera mainCamera;
    bool cameraReturningOriginalPos = false;
    private void Start()
    {
        cameraInitialPosition = mainCamera.transform.position;
    }
    private void Update()
    {
        if (Vector3.Distance(mainCamera.transform.localPosition, cameraInitialPosition) <= 0.1f)
        {
            cameraReturningOriginalPos = false;
            mainCamera.transform.position = cameraInitialPosition;
        }
        if (cameraReturningOriginalPos)
        {
            mainCamera.transform.localPosition = Vector3.Lerp(mainCamera.transform.localPosition, cameraInitialPosition, 10 * Time.deltaTime);
            Debug.Log(mainCamera.transform.localPosition);
            Debug.Log(mainCamera.transform.position);
            //Debug.Log(mainCamera.transform.localPosition == cameraInitialPosition);
        }
    }
    public void ShakeIt()
    {
        cameraReturningOriginalPos = false;
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
        mainCamera.transform.localPosition = cameraIntermadiatePosition;
    }

    void StopCameraShaking()
    {
        CancelInvoke("StartCameraShaking");
        cameraReturningOriginalPos = true;
    }
    
}
