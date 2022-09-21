using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    static UIManager instance;

    [SerializeField] GameObject spawnTimerPrefab;
    static UIManager GetInstance()
    {
        return instance;
    }
    // Start is called before the first frame update
    void Awake()
    {
        if (!instance)
            instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void CreateSpawnTimer(Transform pos)
    {
        Instantiate(spawnTimerPrefab, pos);
    }


     
}
