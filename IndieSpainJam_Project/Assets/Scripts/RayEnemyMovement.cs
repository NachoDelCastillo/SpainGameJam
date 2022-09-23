using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayEnemyMovement : MonoBehaviour
{

    private enum State {GoingLocation, Loading, Shooting, Leaving}

    // Start is called before the first frame update
    [SerializeField] GameObject railsParent;
    [SerializeField] GameObject particlesLoading;
    [SerializeField] float velocity;
    [SerializeField] float rotationSpeed;

    State state;
    Rigidbody2D rb;
    GameObject player;
    
    // GoingLocation variables
    GameObject currentDestination;



    // LoadingState
    [SerializeField] float timeToLoad = 4f;
    [SerializeField] GameObject loadingLaser;
    [SerializeField] GameObject loadingSphere;
    float elapsedTimeToReload = 0;
    float startingRadius = 0.5f;


    // ShootingState
    [SerializeField] float timeFiring = 1.5f;
    [SerializeField] GameObject rayTrigger;
    float elapsedTimeToFire = 0;


    // LeavingState
    [SerializeField] int timesToShoot;
    [SerializeField] GameObject leavingPoint;
    int timesShot;


    void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();
        ChangeState(State.GoingLocation);
        player = GameObject.FindGameObjectWithTag("Player");
        loadingLaser.SetActive(false);
        loadingSphere.SetActive(false);
        rayTrigger.SetActive(false);
        particlesLoading.SetActive(false);
        timesShot = 0;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (state)
        {
            case State.GoingLocation:
                GoingLocationState();
                break;
            
            case State.Shooting:
                RayEnemyShootinState();
                break;

            case State.Loading:
                LoadingState();
                break;

            case State.Leaving:
                LeavingState();
                break;

        }
    }

    private void ChangeState(State newState)
    {
        switch (newState)
        {
            case State.GoingLocation:
                GameObject randomDestination;
                do
                {
                    int rnd = Random.Range(0, railsParent.transform.childCount);
                    randomDestination = railsParent.transform.GetChild(rnd).gameObject;
                } while (currentDestination == randomDestination);

                currentDestination = randomDestination;
                
                break;
            case State.Loading:
                loadingSphere.transform.localScale = Vector3.zero;
                particlesLoading.SetActive(true);
                loadingSphere.SetActive(true);
                elapsedTimeToReload = 0f;
                loadingSphere.transform.localScale = Vector3.one*startingRadius;
                break;
            case State.Shooting:
                loadingLaser.SetActive(false);
                rayTrigger.SetActive(true);
                elapsedTimeToFire = 0f;
                timesShot++;
                break;
        }

        state = newState;
    }


    private void GoingLocationState()
    {
        if (Vector2.Distance(transform.position, currentDestination.transform.position) > 0.5f)
        {
            Vector2 dir = currentDestination.transform.position - transform.transform.position; 
            rb.position = Vector2.Lerp(rb.position, rb.position + dir*velocity * Time.fixedDeltaTime, 0.3f);
            

            // Mirar al player
            //Vector2 direction = player.transform.position - transform.position;
            //direction.Normalize();
            //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            //Quaternion newRot = Quaternion.Euler(Vector3.forward * (angle)); 
            //transform.rotation = Quaternion.Lerp(transform.rotation, newRot, 0.6f);
        }
        else ChangeState(State.Loading);
    }


    private void LoadingState()
    {
        elapsedTimeToReload += Time.fixedDeltaTime;

        Vector2 direction = new Vector2(1,0);
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion newRot = Quaternion.Euler(Vector3.forward * (angle)); ;
        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, 0.1f);

        if (elapsedTimeToReload >= timeToLoad - 1)
        {
            loadingLaser.SetActive(true);
        }

        //Cambiar el srite de carga
        loadingSphere.transform.localScale = Vector2.one * startingRadius * (elapsedTimeToReload) / timeToLoad;

        if (elapsedTimeToReload >= timeToLoad)
        {
            loadingSphere.SetActive(false);
            particlesLoading.SetActive(false);
            ChangeState(State.Shooting);
        }

    }

    private void RayEnemyShootinState()
    {
        elapsedTimeToFire += Time.fixedDeltaTime;

        if(elapsedTimeToFire >= timeFiring)
        {
            rayTrigger.SetActive(false);
            if(timesShot >= timesToShoot)
            {
                ChangeState(State.Leaving);
            }
            else
            {
                ChangeState(State.GoingLocation);
            }
        }

    }


    private void LeavingState()
    {
        if (Vector2.Distance(transform.position, leavingPoint.transform.position) > 0.5)
        {
            Vector2 dir = leavingPoint.transform.position - transform.transform.position;
            rb.position = Vector2.Lerp(rb.position, rb.position + dir * velocity * Time.fixedDeltaTime, 0.3f);

            // Mirar al player
            Vector2 direction = leavingPoint.transform.position - transform.position;
            direction.Normalize();
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Quaternion newRot = Quaternion.Euler(Vector3.forward * (angle));
            transform.rotation = Quaternion.Lerp(transform.rotation, newRot, 0.6f);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
