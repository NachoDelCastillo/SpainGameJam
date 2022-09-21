using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UfosManager : MonoBehaviour
{
    [SerializeField] GameObject ufoPrefab;
    [SerializeField] int numberOfUfos = 2;
    [SerializeField] float timeBetweenUfos;
    [SerializeField] Transform initialUfoPos;
    [SerializeField] Transform[] wagonsPos;

    List<Transform> availableWagons = new List<Transform>();

    GameObject[] clones;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < wagonsPos.Length; i++)
        {
            availableWagons.Add(wagonsPos[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CreateUfos()
    {    
        clones = new GameObject[numberOfUfos];
        for (int i = 0; i < numberOfUfos; i++)
        {
            clones[i] = Instantiate(ufoPrefab);
            clones[i].transform.position = initialUfoPos.position;
        }

        int j = 0;
        while(availableWagons.Count > 0)
        {
            int rand = Random.Range(0, availableWagons.Count);
            Vector3 targetPos = new Vector3(availableWagons[rand].position.x, initialUfoPos.position.y, availableWagons[rand].position.z);
            clones[j].GetComponent<Ufo>().GoToLane(targetPos);
            availableWagons.Remove(availableWagons[rand]);
            j++;
            yield return new WaitForSeconds(timeBetweenUfos);
        }

        for (int i = 0; i < wagonsPos.Length; i++)
        {
            availableWagons.Add(wagonsPos[i]);
        }
    }
}
