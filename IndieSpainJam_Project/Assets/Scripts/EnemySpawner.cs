using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public delegate void MyDelegate();
    public static MyDelegate myDelegate;


    [SerializeField] GameObject[] enemyPrefabs;
    [SerializeField] GameObject rayEnemyDestination;
    [SerializeField] GameObject rayEnemyLeavingPoint;

    [SerializeField] AnimationCurve animationCurve;

    Transform[] spawnPoints;

    [SerializeField] float timeBetweenEnemys, minTimeForNextWave, maxTimeForNextWave;

    [SerializeField] int minEnemys, maxEnemys;

    [SerializeField] Transform[] targetWagons;

    [SerializeField] float multiplier;

    [HideInInspector] public List<GameObject> enemysAlive = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        spawnPoints = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            spawnPoints[i] = transform.GetChild(i);
        }

        myDelegate = CheckEnemiesOnSpeedChange;

        //Invoke("CreateEnemys", 1);
    }

    // Update is called once per frame
    void Update()
    {
        multiplier = animationCurve.Evaluate(TrainManager.Instance.GetmainVelocity() / 100);
    }

    public void CreateEnemys()
    {
        //Debug.Log("CreateEnemys()");
        int nPlayers = LocalMultiplayerManager.GetInstance().GetNumPlayers();

        int numEnmy = Random.Range(Mathf.RoundToInt(minEnemys * multiplier), Mathf.RoundToInt(maxEnemys * multiplier * nPlayers)); ;

        for (int i = 0; i < numEnmy; i++)
            AddEnemy(-1);

        Invoke("CreateEnemys", Random.Range(minTimeForNextWave * (1.25f / (multiplier * nPlayers)), maxTimeForNextWave * (1 / multiplier)));
        //Invoke("CreateEnemys", 5);
    }


    void CheckEnemiesOnSpeedChange()
    {
        switch (TrainManager.Instance.GetmainVelocity())
        {
            case 10:
                break;
            case 20:
                CreateRayEnemy(2, 2.5f, 0.8f, 1f);
                break;
            case 30:
                
                break;
            case 40:
                CreateRayEnemy(1, 2.5f, 0.8f, 1.5f);
                break;
            case 50:
                if(LocalMultiplayerManager.GetInstance().GetNumPlayers() > 1)
                    CreateRayEnemy(1, 2.5f, 0.8f, 2f);
                break;
            case 60:
                if (LocalMultiplayerManager.GetInstance().GetNumPlayers() > 1)
                    CreateRayEnemy(1, 2.5f, 0.8f,2.25f);
                CreateRayEnemy(3, 3, 1f, 1.5f);
                break;
            case 70:
                break;
            case 80:
                if (LocalMultiplayerManager.GetInstance().GetNumPlayers() > 1)
                    CreateRayEnemy(3, 3, 1f, 1.5f);
                CreateRayEnemy(3, 1.8f, 1.2f);
                break;
            case 90:
                CreateRayEnemy(5, 1.5f, 1.5f, 1);
                break;

            default:
                break;
        }


    }

    private void CreateRayEnemy(int timesToShot, float loadTime, float rayTime, float speed = 2.5f)
    {
        Vector3 position = new Vector3(Camera.main.orthographicSize * Camera.main.aspect + enemyPrefabs[1].transform.localScale.x * 2, Random.Range(Camera.main.orthographicSize, -Camera.main.orthographicSize), 0);

        GameObject rayEnemy = Instantiate(enemyPrefabs[1], position, Quaternion.identity);
        RayEnemyMovement enemigoScript = rayEnemy.GetComponent<RayEnemyMovement>();
        enemigoScript.railsParent = rayEnemyDestination;
        enemigoScript.timeFiring = rayTime;
        enemigoScript.timeToLoad = loadTime;
        enemigoScript.timesToShoot = timesToShot;
        enemigoScript.velocity = speed;
        rayEnemy.GetComponent<CameraShake>().mainCamera = Camera.main;
    }

    public BasicEnemy AddEnemy(int targetIndex)
    {
        int rand = Random.Range(0, enemyPrefabs.Length);
        GameObject clon = Instantiate(enemyPrefabs[0]);

        BasicEnemy basicEnemy = clon.GetComponent<BasicEnemy>();
        if (basicEnemy != null)
        {
            if (targetIndex == -1)
                basicEnemy.target = targetWagons[Random.Range(0, targetWagons.Length)];
            else
                basicEnemy.target = targetWagons[targetIndex];

            basicEnemy.enemySpawner = this;
        }

        int randPos = Random.Range(0, spawnPoints.Length);
        clon.transform.position = spawnPoints[randPos].position;

        enemysAlive.Add(clon);

        return basicEnemy;
    }
}
