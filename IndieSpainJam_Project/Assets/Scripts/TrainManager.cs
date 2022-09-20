using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainManager : MonoBehaviour
{
    [SerializeField] Transform deliverCoal;



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
    }
}
