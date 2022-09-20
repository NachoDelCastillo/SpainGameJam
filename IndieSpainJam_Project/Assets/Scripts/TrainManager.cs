using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrainManager : MonoBehaviour
{
    int MainVelocity = 0;

    [Header("Referencias")]
    [SerializeField] Transform deliverCoal;
    [SerializeField] TMP_Text MainVelocity_text;

    //Health
    [SerializeField] float health, maxHealth;
    [SerializeField] Slider healthSlider;

    private void Start()
    {
        health = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
    }

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
}
