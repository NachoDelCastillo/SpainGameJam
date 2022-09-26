using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayEnemyMovement : MonoBehaviour
{

    private enum State {WaitingToEnter, GoingLocation, Loading, Shooting, Leaving}

    //Semáfoross
    static bool[] railDisponible = { true, true, true };
    static int rayEnemiesInside = 0;
    int indexRailEscogido;

    bool odioMiVida = true;

    // Start is called before the first frame update
    public GameObject railsParent;
    [SerializeField] Sprite defaultSprite;
    [SerializeField] Sprite defaultEyesSprite;
    [SerializeField] Sprite loadAndShootSprite;
    [SerializeField] Sprite loadAndShootingEyesSprite;

    [SerializeField] GameObject particlesLoading;
    public float velocity;
    [SerializeField] float rotationSpeed;

    CameraShake cameraShake;
    State state;
    Rigidbody2D rb;
    GameObject player;
    GameObject turret;
    SpriteRenderer spriteRenderer;
    [SerializeField]SpriteRenderer eyesSpriteRenderer;



    //Shield 
    [SerializeField] GameObject shieldGO;
    [SerializeField] float timeGlowHit = 0;
    [SerializeField] AnimationCurve curveGlowingShield;

    bool glowing;
    float elapsedTimeShieldGlowing = 0;


    // GoingLocation variables
    GameObject currentDestination;



    // LoadingState
    public float timeToLoad = 4f;
    [SerializeField] GameObject loadingLaser;
    [SerializeField] GameObject loadingSphere;
    [SerializeField] AnimationCurve loadingSpehereScaleoverTime;
    float elapsedTimeToReload = 0;
    float startingRadius = 0.5f;


    // ShootingState
    public float timeFiring = 1.5f;
    [SerializeField] GameObject rayTrigger;
    float elapsedTimeToFire = 0;


    // LeavingState
    public int timesToShoot;
    Vector3 leavingPoint;
    int timesShot;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = defaultSprite;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ChangeState(State.WaitingToEnter);
        player = GameObject.FindGameObjectWithTag("Player");
        turret = GameObject.FindGameObjectWithTag("Turret");
        loadingLaser.SetActive(false);
        startingRadius = loadingSphere.transform.localScale.x;
        loadingSphere.SetActive(false);
        rayTrigger.SetActive(false);
        particlesLoading.SetActive(false);
        timesShot = 0;
        cameraShake = GetComponent<CameraShake>();

        spriteRenderer.sprite = defaultSprite;
        eyesSpriteRenderer.sprite = defaultEyesSprite;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if(!player) player = GameObject.FindGameObjectWithTag("Player");

        for (int x = 0; x < railsParent.transform.childCount; x++)
            railsParent.transform.GetChild(x).GetComponent<SpriteRenderer>().color = (railDisponible[x]) ? Color.green : Color.red; ;
        //Si no hay un rail en el que esté -> está esperando a la derecha a que se le asigne uno
        if (indexRailEscogido == -1)
            return;


        updateShield();
        //Debug.Log(state);
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
           default:
                break;

        }
    }

    IEnumerator WaitUntillIsRoom()
    {
        while(rayEnemiesInside >= 3)
        {
            yield return 0;
        }
        rayEnemiesInside++;
        Debug.Log("Sale state esperando: " + rayEnemiesInside + " enemigos dentro");

        ChangeState(State.GoingLocation);
    }
    IEnumerator WaitUntilRailAvaible()
    {
        Debug.Log("Intenta seleccionar camino");
        indexRailEscogido = -1;
        GameObject randomDestination;
        int rnd;
        while (!railDisponible[0] && !railDisponible[1] && !railDisponible[2])
        {
            yield return 0;
        }

        do
        {
            rnd = Random.Range(0, railsParent.transform.childCount);
            randomDestination = railsParent.transform.GetChild(rnd).gameObject;
        } while (!railDisponible[rnd]);

        railDisponible[rnd] = false;
        indexRailEscogido = rnd;
        currentDestination = randomDestination;

        Debug.Log("Sale selecciona camino ");
    }

    private void ChangeState(State newState)
    {
        switch (newState)
        {
            case State.WaitingToEnter:
                state = newState;
                StartCoroutine(WaitUntillIsRoom());
                break;
            case State.GoingLocation:
                state = newState;
                StartCoroutine(WaitUntilRailAvaible());
                
                break;
            case State.Loading:
                state = newState;
                particlesLoading.SetActive(true);
                loadingSphere.SetActive(true);
                loadingSphere.transform.localScale = Vector3.zero;
                spriteRenderer.sprite = loadAndShootSprite;
                eyesSpriteRenderer.sprite = loadAndShootingEyesSprite;
                elapsedTimeToReload = 0f;
                break;
            case State.Shooting:
                state = newState;
                loadingLaser.SetActive(false);
                rayTrigger.SetActive(true);
                loadingSphere.SetActive(true);
                elapsedTimeToFire = 0f;
                timesShot++;
                break;
            case State.Leaving:
                state = newState;
                rayEnemiesInside--;
                railDisponible[indexRailEscogido] = true;
                loadingLaser.SetActive(false);
                loadingSphere.SetActive(false);spriteRenderer.sprite = defaultSprite;


                //decidir por dnde sale

                leavingPoint = new Vector3(-Camera.main.orthographicSize * Camera.main.aspect - 10, Random.Range(Camera.main.orthographicSize, -Camera.main.orthographicSize), 0);
                break;
        }

        //Debug.Log("Se acmbia state: " + state);


    }

    void updateShield()
    {

        if (!player) return;

        Vector2 direction = turret.transform.position - shieldGO.transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion newRot = Quaternion.Euler(Vector3.forward * (angle));
        shieldGO.transform.rotation = newRot;
        if (glowing)
        {

            elapsedTimeShieldGlowing += Time.deltaTime;
            SpriteRenderer pSysShield = shieldGO.GetComponentInChildren<SpriteRenderer>();
            Color colorShield = new Color(pSysShield.color.r, pSysShield.color.g, pSysShield.color.b, curveGlowingShield.Evaluate(elapsedTimeShieldGlowing/timeGlowHit)/255f);
            pSysShield.color = colorShield;
            Debug.Log(pSysShield.color.a/255f);

            if (elapsedTimeShieldGlowing >= timeGlowHit)
            {
                glowing = false;
                elapsedTimeShieldGlowing = 0;
                pSysShield.color = new Color(pSysShield.color.r, pSysShield.color.g, pSysShield.color.b, 0);

            }
        }
        //SpriteRenderer pSysShield = shieldGO.GetComponentInChildren<SpriteRenderer>();
        //pSysShield.material.SetFloat("_Intensity", pSysShield.material.GetFloat("_Intensity") + 1);
        
    }

    private void GoingLocationState()
    {
        if (Vector2.Distance(transform.position, currentDestination.transform.position) > 0.5f)
        {
            Vector2 dir = currentDestination.transform.position - transform.transform.position;
            rb.position = Vector2.Lerp(rb.position, rb.position + dir * velocity * Time.fixedDeltaTime, 0.3f);


            // Mirar al player
            //Vector2 direction = player.transform.position - transform.position;
            //direction.Normalize();
            //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            //Quaternion newRot = Quaternion.Euler(Vector3.forward * (angle));
            //transform.rotation = Quaternion.Lerp(transform.rotation, newRot, 0.6f);
        }
        else {
           ChangeState(State.Loading);
        }
    }


    private void LoadingState()
    {
        elapsedTimeToReload += Time.fixedDeltaTime;

        Vector2 direction = new Vector2(1,0);
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion newRot = Quaternion.Euler(Vector3.forward * (angle)); ;
        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, 0.1f);

        


        if (elapsedTimeToReload <= timeToLoad)
        {
            loadingSphere.transform.localScale = Vector2.one * (startingRadius) * loadingSpehereScaleoverTime.Evaluate((elapsedTimeToReload) / (timeToLoad));
            
            if (!AudioManager_PK.instance.sounds[17].source.isPlaying && odioMiVida)
                AudioManager_PK.instance.Play("Charging", Random.Range(0.9f, 1f));

            if (odioMiVida && loadingSpehereScaleoverTime.Evaluate((elapsedTimeToReload) / (timeToLoad)) < loadingSpehereScaleoverTime.Evaluate(((elapsedTimeToReload) / (timeToLoad)) - 0.01f))
            {
                AudioManager_PK.instance.Stop("Charging");
                odioMiVida = false;
            }

        }
        else if (elapsedTimeToReload >= timeToLoad)
        {
            odioMiVida = true;

            particlesLoading.SetActive(false);
            //Dispare
            cameraShake.ShakeIt();

            ChangeState(State.Shooting);
        }


        if (elapsedTimeToReload >= timeToLoad - 0.1f)
        {
            if (!AudioManager_PK.instance.sounds[18].source.isPlaying)
            {
                AudioManager_PK.instance.Play("BigLaser", Random.Range(0.9f, 1f));
            }
        }

    }


    

    private void RayEnemyShootinState()
    {

        elapsedTimeToFire += Time.fixedDeltaTime;

        if(elapsedTimeToFire >= timeFiring)
        {
            rayTrigger.SetActive(false);
            loadingSphere.SetActive(false);
            railDisponible[indexRailEscogido] = true;

            if (timesShot >= timesToShoot)
            {
                ChangeState(State.Leaving);
            }
            else
            {
                ChangeState(State.GoingLocation);
            }
            spriteRenderer.sprite = defaultSprite;
            eyesSpriteRenderer.sprite = defaultEyesSprite;

            AudioManager_PK.instance.Stop("BigLaser");
        }

    }


    private void LeavingState()
    {
        if (Vector2.Distance(transform.position, leavingPoint) > 2)
        {
            Vector2 dir = leavingPoint - transform.transform.position;
            rb.position = Vector2.Lerp(rb.position, rb.position + dir * velocity * Time.fixedDeltaTime, 0.3f);

            // Mirar al player
            Vector2 direction = Vector2.right;
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



    private void OnTriggerEnter2D(Collider2D collision)
    {
        Bullet b = collision.GetComponent<Bullet>();
        if (b)
        {
            //Vector2 direction = collision.transform.position - shieldGO.transform.position;
            //direction.Normalize();
            //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            //Quaternion newRot = Quaternion.Euler(Vector3.forward * (angle));
            //shieldGO.transform.rotation = newRot;
            elapsedTimeShieldGlowing = 0;
            glowing = true;
            b.ShieldImpacted();
            

        }
    }
}
