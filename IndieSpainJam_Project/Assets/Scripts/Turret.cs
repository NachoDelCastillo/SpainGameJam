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


    bool shooting;
    [SerializeField] float fireRate = 5;
    float timeBetweenShots = 0;
    float timeElaspedSinceLastShot = 0;
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
            Debug.Log("Disparando");
            timeElaspedSinceLastShot += Time.deltaTime;
            if(timeElaspedSinceLastShot >= timeBetweenShots)
            {
                //disparar
                Instantiate(bulletPrefab, bulletsSpawnPoint.transform.position, bulletsSpawnPoint.transform.rotation);
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
        cannonPivot.transform.Rotate(new Vector3(0, 0, -rotationInput * rotationSpeed * Time.deltaTime));


        //Version 2
        //Debug.Log("Rotation input " + rotationInput);
        //Vector3 targetRotation = new Vector3(0, 0, cannonPivot.transform.rotation.eulerAngles.z + (-rotationInput) * rotationSpeed);

        //Debug.Log("TargetRotation " + targetRotation.z);

        //cannonPivot.transform.rotation = Quaternion.Lerp(cannonPivot.transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime);


    }


}
