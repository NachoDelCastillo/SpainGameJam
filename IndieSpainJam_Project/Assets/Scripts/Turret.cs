using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject cannonPivot;
    [SerializeField] GameObject bulletsSpawnPoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float rotationSpeed;
    [SerializeField] float coneAngle;



    bool shooting, startedMoving;
    [SerializeField] float fireRate = 5;
    float timeBetweenShots = 0;
    float timeElaspedSinceLastShot = 0;
    float rotMultiplier = 1, rotIncrease = 2;
    float lastInput;
    void Start()
    {
        shooting = false;
        timeBetweenShots = 1f / fireRate;      
    }

    // Update is called once per frame
    void Update()
    {
        if (shooting)
        {
            //Debug.Log("Disparando");
            timeElaspedSinceLastShot += Time.deltaTime;
            if (timeElaspedSinceLastShot >= timeBetweenShots)
            {
                //disparar
                float angleToAdd = Random.Range(-coneAngle / 2f, coneAngle / 2);
                Vector3 desiredAngle = bulletsSpawnPoint.transform.rotation.eulerAngles + new Vector3(0, 0, angleToAdd);
                Instantiate(bulletPrefab, bulletsSpawnPoint.transform.position, Quaternion.Euler(desiredAngle));
                timeElaspedSinceLastShot = 0;
            }
        }


    }

    public void changeShooting(bool newValue)
    {
        shooting = newValue;

        //para poder disparar ell mismo frame que se pulsa la tecla
        timeElaspedSinceLastShot = timeBetweenShots;
    }

    public void RotateCannon(float rotationInput)
    {
        // version 1
        startedMoving = true;

        if (rotationInput == 0)
        {
            rotMultiplier = 1;
            return;
        }


        rotMultiplier += Time.deltaTime * rotIncrease;
        cannonPivot.transform.Rotate(new Vector3(0, 0, -rotationInput * rotationSpeed * Mathf.Pow(rotMultiplier, 2) * Time.deltaTime));
        float z = cannonPivot.transform.localEulerAngles.z;
        z = Mathf.Clamp(z, 90, 270);
        cannonPivot.transform.localEulerAngles = new Vector3(cannonPivot.transform.eulerAngles.x, cannonPivot.transform.eulerAngles.y, z);
        //cannonPivot.transform.eulerAngles = new Vector3(cannonPivot.transform.eulerAngles.x, cannonPivot.transform.eulerAngles.y, z);


        //Version 2
        //Debug.Log("Rotation input " + rotationInput);
        //Vector3 targetRotation = new Vector3(0, 0, cannonPivot.transform.rotation.eulerAngles.z + (-rotationInput) * rotationSpeed);

        //Debug.Log("TargetRotation " + targetRotation.z);

        //cannonPivot.transform.rotation = Quaternion.Lerp(cannonPivot.transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime);


    }


}
