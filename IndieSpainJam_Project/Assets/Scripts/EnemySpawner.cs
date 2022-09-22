using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject[] enemyPrefabs;

    Transform[] spawnPoints;

    [SerializeField] float timeBetweenEnemys;

    [SerializeField] int minEnemys, maxEnemys;

    [SerializeField] Transform[] targetWagons;

    [HideInInspector] public List<GameObject> enemysAlive = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        spawnPoints = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            spawnPoints[i] = transform.GetChild(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(enemysAlive.Count <= 0)
        {
            CreateEnemys();
        }
    }

    void CreateEnemys()
    {
        int numEnmy = Random.Range(minEnemys, maxEnemys);

        for (int i = 0; i < numEnmy; i++)
        {
            int rand = Random.Range(0, enemyPrefabs.Length);
            GameObject clon = Instantiate(enemyPrefabs[rand]);

            BasicEnemy basicEnemy = clon.GetComponent<BasicEnemy>();
            if (basicEnemy != null)
            {
                basicEnemy.target = targetWagons[Random.Range(0, targetWagons.Length)];
                basicEnemy.enemySpawner = this;
            }

            int randPos = Random.Range(0, spawnPoints.Length);
            clon.transform.position = spawnPoints[randPos].position;

            enemysAlive.Add(clon);

            //yield return new WaitForSeconds(timeBetweenEnemys);
        }
    }
}
