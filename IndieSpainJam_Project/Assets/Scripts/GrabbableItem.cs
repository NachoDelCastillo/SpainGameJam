using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableItem : MonoBehaviour
{
    public enum ItemType { coal}
    [SerializeField] ItemType itemType;

    [HideInInspector] public PlayerController_2D playerGrabbingThis;

    [HideInInspector] public Rigidbody2D rb;
    public BoxCollider2D col;

    public bool coalReady;

    public Transform initialParent;

    public CoalWagon wagon;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        col.isTrigger = true;
    }


    public void ItemGrabbed(PlayerController_2D playerGrabbingThis_)
    {
        TutorialManager.GetInstance().TryToChangePhase(TutorialManager.tutPhases.agarrarCarbonParaTorreta);
        TutorialManager.GetInstance().TryToChangePhase(TutorialManager.tutPhases.agarrarCarbonParaMotor);

        AudioManager_PK.instance.Play("PickUp", Random.Range(0.9f, 1.1f));

        playerGrabbingThis = playerGrabbingThis_;

        rb.isKinematic = true;
        col.isTrigger = true;

        if (wagon.clon != null && wagon.clon == gameObject) wagon.inWagon = false;
        transform.GetChild(0).GetComponent<SpriteRenderer>().sortingLayerName = "Items";
        transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sortingLayerName = "Items";
    }

    public void ItemDropped()
    {
        playerGrabbingThis = null;

        rb.isKinematic = false;
        col.isTrigger = false;
    }
}
