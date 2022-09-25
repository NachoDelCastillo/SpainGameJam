using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
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



        //Invoke("CreateEnemys", 1);

        AddEnemy(1).transform.position = new Vector3(4, 10, 0);
    }

    // Update is called once per frame
    void Update()
    {
        multiplier = animationCurve.Evaluate(TrainManager.Instance.GetmainVelocity() / 100);
    }

    void CreateEnemys()
    {
        int numEnmy = Random.Range(Mathf.RoundToInt(minEnemys * multiplier), Mathf.RoundToInt(maxEnemys * multiplier));

        for (int i = 0; i < numEnmy; i++)
            AddEnemy(-1);

        Invoke("CreateEnemys", Random.Range(minTimeForNextWave * (1 / multiplier), maxTimeForNextWave * (1 / multiplier)));
    }


    public void CheckEnemiesOnSpeedChange()
    {
        switch (TrainManager.Instance.GetmainVelocity())
        {
            case <= 10:
                break;
            case <= 20:
                break;
            case <= 30:
                CreateRayEnemy(1, 3, 0.8f);
                break;
            case <= 40:
                break;
            case <= 50:
                break;
            case <= 60:
                CreateRayEnemy(3, 2, 1.2f);
                break;
            case <= 70:
                break;
            case <= 80:
                CreateRayEnemy(3, 1.8f, 1.2f);
                break;
            case <= 90:
                CreateRayEnemy(5, 1.5f, 1.5f);

                break;
        }


    }

    private void CreateRayEnemy(int timesToShot, float loadTime, float rayTime)
    {
        Vector3 position = new Vector3(Camera.main.orthographicSize * Camera.main.aspect + enemyPrefabs[1].transform.localScale.x * 2, Random.Range(Camera.main.orthographicSize, -Camera.main.orthographicSize), 0);

        GameObject rayEnemy = Instantiate(enemyPrefabs[1], position, Quaternion.identity);
        RayEnemyMovement enemigoScript = rayEnemy.GetComponent<RayEnemyMovement>();
        enemigoScript.railsParent = rayEnemyDestination;
        enemigoScript.leavingPoint = rayEnemyLeavingPoint;
        enemigoScript.timeFiring = rayTime;
        enemigoScript.timeToLoad = loadTime;
        enemigoScript.timesToShoot = timesToShot;
        rayEnemy.GetComponent<CameraShake>().mainCamera = Camera.main;
    }

    BasicEnemy AddEnemy(int targetIndex)
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
