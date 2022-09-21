using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrainManager : MonoBehaviour
{
    static TrainManager instance;

    static TrainManager GetInstance()
    { return instance; }

    int MainVelocity = 0;

    [Header("Referencias")]
    [SerializeField] Transform deliverCoal;
    [SerializeField] TMP_Text MainVelocity_text;

    [SerializeField] WagonLogic[] wagons;
    [SerializeField] Transform[] columns;
    [SerializeField] Transform[] rows;

    //Health
    [SerializeField] float health, maxHealth;
    [SerializeField] Slider healthSlider;


    // Manager
    [SerializeField] Transform changeRail_Prefab;
    [SerializeField] Transform rails;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        health = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
    }


    float spawnTimer;
    IEnumerator SpawnChangeRail()
    {
        spawnTimer = Random.Range(2, 6);

        while (spawnTimer > 0)
        {
            spawnTimer -= Time.deltaTime;

            yield return 0;
        }

        Instantiate(changeRail_Prefab, rails);

        StartCoroutine(SpawnChangeRail());
    }





    #region wevadas

    public void CoalDelivered(OnTriggerDelegation delegation)
    {

        if (!delegation.Other.CompareTag("Coal")) return;

        GrabbableItem coal = delegation.Other.transform.GetComponent<GrabbableItem>();
        coal.ItemGrabbed(null);

        // Deliver time
        float deliverTime = .3f;
        StartCoroutine(Utils.MoveItemSmooth(coal.transform, deliverCoal, deliverTime));
        StartCoroutine(DestroyCoal(coal, deliverTime));
    }

    IEnumerator DestroyCoal(GrabbableItem coal, float seconds)
    {
        yield return new WaitForSeconds(seconds + .1f);
        if (coal != null)
            Destroy(coal.gameObject);

        AddVelocity();
    }

    void AddVelocity()
    {
        MainVelocity += 5;
        MainVelocity_text.text = MainVelocity.ToString() + " / 100 Km";
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
       
        if (health <= 0) health = 0;
        healthSlider.value = health;
    }

    #endregion
}
