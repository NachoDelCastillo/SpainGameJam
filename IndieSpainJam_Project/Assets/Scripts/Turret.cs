using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject cannonPivot;
    [SerializeField] GameObject bulletsSpawnPoint;
    [SerializeField] float rotationSpeed;



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RotateCannon(float rotationInput)
    {
        // version 1
        //cannonPivot.transform.Rotate(new Vector3(0,0,-rotationInput * rotationSpeed * Time.deltaTime));


        //Version 2
        //Quaternion targetRotation = cannonPivot.transform.rotation + rotationInput * rotationSpeed * Time.deltaTime;
        //Quaternion targetRotation = cannonPivot.transform.rotation + rotationInput * rotationSpeed;

        Vector3 targetRotation = new Vector3(0,0,cannonPivot.transform.rotation.z + (-rotationInput) * rotationSpeed);
        cannonPivot.transform.rotation = Quaternion.Slerp(cannonPivot.transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime);
        //cannonPivot.transform.Rotate(new Vector3(0, 0, Mathf.Lerp(cannonPivot.transform.rotation.z, cannonPivot.transform.rotation.z + (-rotationInput) * rotationSpeed * Time.deltaTime)));


        //var targetRotation = Quaternion.Euler(0, 0, Quaternion.LookRotation(m_TurretHead.position m_Target.position).eulerAngles.y);

        //m_TurretHead.rotation = Quaternion.Slerp(m_TurretHead.rotation, targetRotation, m_RotationSpeed * Time.deltaTime);

    }


}
