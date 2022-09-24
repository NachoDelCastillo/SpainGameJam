using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Turret : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject cannonPivot;
    [SerializeField] GameObject bulletsSpawnPoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] ParticleSystem shootPart;
    [SerializeField] float rotationSpeed;
    [SerializeField] float coneAngle;



    bool shooting, drawBack;
    [SerializeField] float fireRate = 5, capMultiplier;
    float timeBetweenShots = 0;
    float timeElaspedSinceLastShot = 0;
    float rotMultiplier = 1, rotIncrease = 2;
    float lastInput, anglesDrawBack = 10;

    //Ammo
    [SerializeField] GameObject ammoIndicator;
    [SerializeField] int maxAmmo;
    CameraShake cameraShake;
    int currentAmmo;
    Image imageToFill;

    void Start()
    {
        shooting = false;
        timeBetweenShots = 1f / fireRate;
        imageToFill = ammoIndicator.GetComponent<Image>();
        currentAmmo = (int)(maxAmmo * 1);
        cameraShake = GetComponent<CameraShake>();
    }

    // Update is called once per frame

    void Update()
    {
        if (shooting && currentAmmo > 0)
        {
            //Debug.Log("Disparando");
            cameraShake.ShakeIt();
            timeElaspedSinceLastShot += Time.deltaTime;
            if (timeElaspedSinceLastShot >= timeBetweenShots)
            {
                //disparar
                float angleToAdd = Random.Range(-coneAngle / 2f, coneAngle / 2);
                Vector3 desiredAngle = bulletsSpawnPoint.transform.rotation.eulerAngles + new Vector3(0, 0, angleToAdd);
                Instantiate(bulletPrefab, bulletsSpawnPoint.transform.position, Quaternion.Euler(desiredAngle));
                shootPart.Play();
                currentAmmo--;
                timeElaspedSinceLastShot = 0;

                imageToFill.fillAmount = (float)currentAmmo / (float)maxAmmo;
                imageToFill.color = (imageToFill.fillAmount > 0.5) ? Color.Lerp(Color.yellow, Color.green, (imageToFill.fillAmount - 0.5f) * 2) : Color.Lerp(Color.yellow, Color.red, Mathf.Abs(imageToFill.fillAmount - 0.5f) * 2);
            }
        }


    }

    public void changeShooting(bool newValue)
    {
        shooting = newValue;

    }

    public void RotateCannon(float rotationInput)
    {
        // version 1

        if (lastInput * rotationInput < 0) rotMultiplier = 1;

        if (rotationInput == 0 && lastInput != 0)
        {
            drawBack = true;
            rotMultiplier = 1;

            if (lastInput >= 0) StartCoroutine(DrawBack(1));
            else StartCoroutine(DrawBack(-1));

            lastInput = rotationInput;
            return;
        }

        if (rotationInput != 0)
        {
            drawBack = false;
            StopCoroutine(DrawBack(1));

            rotMultiplier += Time.deltaTime * rotIncrease;
            rotMultiplier = Mathf.Clamp(rotMultiplier, 1, capMultiplier);
            cannonPivot.transform.Rotate(new Vector3(0, 0, -rotationInput * rotationSpeed * Mathf.Pow(rotMultiplier, 2) * Time.deltaTime));

            //no dejar rotar
            //float z = cannonPivot.transform.localEulerAngles.z;
            //z = Mathf.Clamp(z, 90, 270);
            //cannonPivot.transform.localEulerAngles = new Vector3(cannonPivot.transform.eulerAngles.x, cannonPivot.transform.eulerAngles.y, z);

            lastInput = rotationInput;
        }


        //Version 2
        //Debug.Log("Rotation input " + rotationInput);
        //Vector3 targetRotation = new Vector3(0, 0, cannonPivot.transform.rotation.eulerAngles.z + (-rotationInput) * rotationSpeed);

        //Debug.Log("TargetRotation " + targetRotation.z);

        //cannonPivot.transform.rotation = Quaternion.Lerp(cannonPivot.transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime);
    }



    IEnumerator DrawBack(float lastDir)
    {
        float initialZ = cannonPivot.transform.localEulerAngles.z;
        float lastZ = cannonPivot.transform.localEulerAngles.z;
        while (drawBack && Mathf.Abs(initialZ - lastZ) < anglesDrawBack)
        {
            cannonPivot.transform.Rotate(new Vector3(0, 0, -lastDir * rotationSpeed * 5 * Time.deltaTime));
            lastZ = cannonPivot.transform.localEulerAngles.z;
            //Debug.Log("initial z" + initialZ);
            //Debug.Log("last z" + lastZ);
            yield return null;
        }


        initialZ = cannonPivot.transform.localEulerAngles.z;
        lastZ = cannonPivot.transform.localEulerAngles.z;
        while (drawBack && Mathf.Abs(initialZ - lastZ) < anglesDrawBack / 2)
        {
            cannonPivot.transform.Rotate(new Vector3(0, 0, lastDir * rotationSpeed * Time.deltaTime));
            lastZ = cannonPivot.transform.localEulerAngles.z;
            //Debug.Log("initial z" + initialZ);
            //Debug.Log("last z" + lastZ);
            yield return null;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.gameObject.CompareTag("Coal"))
        {
            TutorialManager.GetInstance().TryToChangePhase(TutorialManager.tutPhases.meterCarbonEnTorreta);

            collision.transform.GetChild(0).DORotate(new Vector3(0, 0, -720), 1, RotateMode.FastBeyond360);
            collision.transform.GetChild(0).DOScale(0, 1);
            collision.transform.DOMove(cannonPivot.transform.position, 1);

            currentAmmo += maxAmmo / 3;
            if (currentAmmo > maxAmmo)
            {
                currentAmmo = maxAmmo;
            }
            imageToFill.fillAmount = (float)currentAmmo / (float)maxAmmo;
            imageToFill.color = (imageToFill.fillAmount > 0.5) ? Color.Lerp(Color.yellow, Color.green, (imageToFill.fillAmount - 0.5f) * 2) : Color.Lerp(Color.yellow, Color.red, Mathf.Abs(imageToFill.fillAmount - 0.5f) * 2);
            StartCoroutine(DestroyCoal(collision.transform));
        }
    }


    IEnumerator DestroyCoal(Transform coal)
    {
        yield return new WaitForSeconds(1);
        Destroy(coal.gameObject);
    }
}