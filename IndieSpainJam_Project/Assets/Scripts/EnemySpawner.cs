using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject[] enemyPrefabs;

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

    BasicEnemy AddEnemy(int targetIndex)
    {
        int rand = Random.Range(0, enemyPrefabs.Length);
        GameObject clon = Instantiate(enemyPrefabs[rand]);

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
