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

    public bool coalReady, inWagon;

    public Transform initialParent;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        col.isTrigger = true;
        inWagon = true;
    }

    public void ItemGrabbed(PlayerController_2D playerGrabbingThis_)
    {
        TutorialManager.GetInstance().TryToChangePhase(TutorialManager.tutPhases.agarrarCarbonParaTorreta);
        TutorialManager.GetInstance().TryToChangePhase(TutorialManager.tutPhases.agarrarCarbonParaMotor);

        playerGrabbingThis = playerGrabbingThis_;

        rb.isKinematic = true;
        col.isTrigger = true;
    }

    public void ItemDropped()
    {
        playerGrabbingThis = null;

        rb.isKinematic = false;
        col.isTrigger = false;
    }
}
