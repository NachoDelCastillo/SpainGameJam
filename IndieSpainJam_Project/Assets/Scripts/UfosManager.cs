using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UfosManager : MonoBehaviour
{
    [SerializeField] GameObject ufoPrefab;
    [SerializeField] int numberOfUfos = 2;
    [SerializeField] float timeBetweenUfosIn, timeBetweenUfosOut;
    [SerializeField] Transform initialUfoPos, finalUfoPos;
    [SerializeField] Transform[] wagonsPos;

    List<Transform> availableWagons = new List<Transform>();

    GameObject[] clones;
    bool readyToAttack, ufosCreated, attacking;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < wagonsPos.Length; i++)
        {
            availableWagons.Add(wagonsPos[i]);
        }

        StartCoroutine(CreateUfos());
    }

    // Update is called once per frame
    void Update()
    {
        if (!ufosCreated) return;

        CheckIfReadyToAttack();
    }

    void CheckIfReadyToAttack()
    {
        readyToAttack = true;

        for (int i = 0; i < numberOfUfos; i++)
        {
            Ufo ufo = clones[i].GetComponent<Ufo>();
            if (!ufo.inPlace)
            {
                readyToAttack = false;
                break;
            }
        }

        if (readyToAttack && !attacking)
        {
            for (int j = 0; j < numberOfUfos; j++)
            {
                clones[j].GetComponent<Ufo>().Attack();
            }

            attacking = true;
        }

        if (Ufo.attacckedFinished)
        {
            Leave();
            Ufo.attacckedFinished = false;
        }

    }

    public IEnumerator CreateUfos()
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
            clones[j].GetComponent<Ufo>().target = targetPos;
            clones[j].GetComponent<Ufo>().canMove = true;
            availableWagons.Remove(availableWagons[rand]);
            j++;
            yield return new WaitForSeconds(timeBetweenUfosIn);
        }

        for (int i = 0; i < wagonsPos.Length; i++)
        {
            availableWagons.Add(wagonsPos[i]);
        }

        ufosCreated = true;
    }

    public void Leave()
    {
        for (int i = 0; i < numberOfUfos; i++)
        {
            clones[i].GetComponent<Ufo>().target = finalUfoPos.position;
            clones[i].GetComponent<Ufo>().canMove = true;
        }
    }
}
