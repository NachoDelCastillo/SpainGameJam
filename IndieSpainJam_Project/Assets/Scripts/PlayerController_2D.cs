using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using Random = UnityEngine.Random;

public class PlayerController_2D : MonoBehaviour
{
    [HideInInspector] public int playerIndex;

    [SerializeField] float speed = 10;

    // Components
    Rigidbody2D rb;
    [SerializeField] SpriteRenderer gfx;
    [SerializeField] Animator anim;
    [SerializeField] Transform grabSpot;
    [SerializeField] Color colorRespawn;

    PlayerInputActions playerControl;

    // Movement
    float input_hor;
    float input_ver;
    int dir = 1;

    // Jump
    [SerializeField] float jumpForce;
    float smoothTime = .1f;
    private Vector3 m_Velocity = Vector3.zero;
    bool canDoubleJump;
    float jumpRemember = .2f;
    float jumpRememberTimer = -1;

    float groundRemember = .2f;
    float groundRememberTimer = -1;

    // Turret
    bool currentlyInTurretWagon = false;
    bool usingTurret = false;
    bool enteringTurret = false;
    [SerializeField] public GameObject turret;
    float turretEnteringRadius = 0.1f;
    Turret turretControl;
    GameObject turretOutline;

    // Ground checker
    [SerializeField] Transform groundCheck_tr;
    [SerializeField] LayerMask groundLayer;
    bool onGround;
    float groundCheck_radius = .2f;
    bool isDropping = false;

    [SerializeField] Collision2D basicAttack_Side;
    [SerializeField] Collision2D basicAttack_Up;

    [SerializeField] float screenMargin;
    float screenLimit;

    CoalWagon coalWagon;

    [Header("Particle Systems")]
    [SerializeField] ParticleSystem jumping;
    [SerializeField] ParticleSystem doubleJumping;
    [SerializeField] ParticleSystem landing1;
    [SerializeField] ParticleSystem landing2;


    //muerte
    [HideInInspector] public bool killable;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerControl = new PlayerInputActions();
        turret = FindObjectOfType<Turret>().gameObject;
        turretControl = turret.GetComponent<Turret>();
        turretOutline = turretControl.outline;
        killable = true;
        coalWagon = FindObjectOfType<CoalWagon>();
    }

    private void Start()
    {
        Vector3 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        screenLimit = screenBounds.x - screenMargin;
        killable = true;

    }

    private void LeaveTurret(bool muerto = false)
    {
        if (TutorialManager.GetInstance().duringTutorial &&
            TutorialManager.GetInstance().GetCurrentPhase() != TutorialManager.tutPhases.salirDeTorreta)
            return;

        TutorialManager.GetInstance().TryToChangePhase(TutorialManager.tutPhases.salirDeTorreta);
        AudioManager_PK.instance.Stop("TurretRotate");

        if (!muerto)
        {
            jumpRemember = .2f;
            jumpRememberTimer = 1;
            canDoubleJump = true;
            onGround = true;
            groundRemember = .2f;
            groundRememberTimer = 1;
        }


        turretControl.beingUsed = false;
        transform.SetParent(null);
        rb.isKinematic = false;
        usingTurret = false;
        enteringTurret = false;
        turretOutline.SetActive(false);
        turretControl.changeShooting(false);

        gfx.enabled = true;
    }
    public void GotKilled()
    {
        LeaveTurret(true);

        //si teiene un objeto lo suelta
        if (grabbedItem != null)
        {
            // Lanzar el objeto
            grabbedItem.ItemDropped();
            grabbedItem.rb.velocity = new Vector2(10 * dir, 15);
            grabbedItem.transform.SetParent(null);
            grabbedItem = null;
            droppingAnItem = false;
        }
    }

    public SpriteRenderer GetGFX()
    {
        return gfx;
    }
    void Update()
    {

        UpdateFlipGfx();

        Jump_check();


        // Limit down velocity
        if (rb.velocity.y < -50)
            rb.velocity = new Vector2(rb.velocity.x, -50);


        //float maxVelocity = 
        //else if (rb.velocity.y > 21)
        //    rb.velocity = new Vector2(rb.velocity.x, 21);


        // Update animator
        if (input_hor > .1f || input_hor < -.1f)
            anim.SetFloat("HorVel", Mathf.Abs(rb.velocity.x));
        else
            anim.SetFloat("HorVel", 0);


        anim.SetFloat("VerVel", rb.velocity.y);

        anim.SetBool("OnGround", onGround);

        if (playerIndex == 0)
        {
            string s = "";

            if (reachableItems.Count == 0)
            {
                //Debug.Log("NO ITEMS");
            }
            else
            {
                foreach (GrabbableItem item in reachableItems)
                    if (item != null) s += item.name + ", ";
                //Debug.Log(s);
            }
        }



        if(transform.position.y  < -Camera.main.orthographicSize)
        {
            LocalMultiplayerManager.GetInstance().Respawn(this);
        }
        //Debug.Log("grabbedItem = " + grabbedItem);
    }


    #region Input
    public void Jump_Input(InputAction.CallbackContext context)
    {

        if (context.started)
        {
            jumpRememberTimer = jumpRemember;

            // Solo se puede salir de la torreta si has pulsado la tecla despu?s de haber entrado en ella o mientras se est? subiendo
            if (usingTurret || enteringTurret)
            {
                //Debug.Log("Salta de la torreta");
                LeaveTurret();
            }
        }

        if (context.canceled && rb.velocity.y > 0 && input_ver >= 0)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * .55f);
    }


    // Moves an item to a position in X seconds
    [Space(12)]
    [SerializeField] AnimationCurve animationCurve;
    bool grabbingAnItem; // Devuelve true si se esta moviendo un objeto del suelo a las manos de este jugador
    bool droppingAnItem; // Devuelve true si se esta soltando un objeto

    public void Grab_Input(InputAction.CallbackContext context)
    {

        // Si est? en la torreta, dispara
        if (usingTurret)
        {
            //permitir disparar o no
            if (context.started)
            {
                turretControl.changeShooting(true);
            }
            else if (context.canceled)
                turretControl.changeShooting(false);
            return;
        }


        //A partir de ahora solo vale la pulsaci?n inicial de tecla
        if (!context.started) return;





        // Si se est? subiendo a la torreta no recoge nada
        if (enteringTurret) return;

        // Si est? en el vag?n de la torreta pero NO la est? usando, se sube y no agarra nada m?s
        if (currentlyInTurretWagon && !usingTurret && !enteringTurret && !turretControl.beingUsed)
        {
            if (TutorialManager.GetInstance().duringTutorial &&
            TutorialManager.GetInstance().GetCurrentPhase() != TutorialManager.tutPhases.meterseEnTorreta)
                return;

            //Debug.Log("Entra en torreta");
            TutorialManager.GetInstance().TryToChangePhase(TutorialManager.tutPhases.meterseEnTorreta);
            turretControl.beingUsed = true;
            rb.isKinematic = true;
            rb.velocity = new Vector2(0, 0);
            enteringTurret = true;
            AudioManager_PK.instance.Play("TurretUp", 1f);
            return;
        }



        // Si se esta en el proceso de coger un objeto, no seguir
        if (grabbingAnItem || droppingAnItem) return;





        // Si ya se tiene un objeto en las manos
        if (grabbedItem != null)
        {
            // Lanzar el objeto
            grabbedItem.ItemDropped();
            grabbedItem.rb.velocity = new Vector2(10 * dir, 15);
            grabbedItem.transform.SetParent(null);
            grabbedItem = null;

            droppingAnItem = true;
            Invoke("EndDropping", .3f);
        }
        // Si no se tiene un objeto en las manos
        else
        {
            int operativeCoals = 0;
            foreach (GrabbableItem item in reachableItems)
                if (item != null)
                    operativeCoals++;

            //GrabbableItem deleteThis = null;
            //foreach (GrabbableItem item in reachableItems)
            //    if (item == null)
            //        deleteThis = item;
            //if (deleteThis != null)
            //    reachableItems.Remove(deleteThis);

            // Si no se esta intentando cojer ningun objeto del vagon del carbon
            // Comprobar si se quiere cojer un objeto del suelo
            if (operativeCoals != 0)
            {
                //// Si no hay ningun objeto cerca, no seguir
                //if (reachableItems.Count == 0) return;


                GrabbableItem nearestItem = reachableItems[0];

                if (reachableItems.Count == 1)
                {
                    if (reachableItems[0] == null)
                        return;
                    else
                        nearestItem = reachableItems[0];
                }


                else
                {
                    if (nearestItem == null)
                        nearestItem = reachableItems[1];

                    for (int i = 0; i < reachableItems.Count; i++)
                    {
                        if (reachableItems[i] == null) continue;

                        float shortestDistance = 200;

                        if (nearestItem != null)
                            shortestDistance = Vector3.Distance(nearestItem.transform.position, transform.position);

                        float newDistance = Vector3.Distance(reachableItems[i].transform.position, transform.position);

                        if (newDistance < shortestDistance)
                            nearestItem = reachableItems[i];
                    }
                }

                // Coger el objeto
                if (nearestItem != null)
                {
                    if (!nearestItem.coalReady) return;

                    nearestItem.transform.SetParent(grabSpot);
                    grabbedItem = nearestItem;
                    grabbedItem.ItemGrabbed(this);

                }
                // Mover el objeto
                float grabTime = .2f;
                if (nearestItem != null)
                    StartCoroutine(Utils.MoveItemSmooth(nearestItem.transform, grabSpot.transform, grabTime));
                grabbingAnItem = true;
                Invoke("EndGrabbing", grabTime + .1f);

            }

            // Comprobar si se esta intentando agarrar uno del vagon del carbon
            else if (currentlyInCoalWagon)
            {
                //if (!coalWagon.coalReady) return;

                //coalWagon.clon.transform.parent = grabSpot;
                //StartCoroutine(Utils.MoveItemSmooth(coalWagon.clon.transform, grabSpot.transform, 0.2f));
                //grabbedItem = coalWagon.clon.GetComponent<GrabbableItem>();
                //grabbedItem.inWagon = false;
                //grabbedItem.col.isTrigger = false;
                //grabbedItem.ItemGrabbed(this);
                //coalWagon.coalReady = false;
                //grabbingAnItem = true;
                //Invoke("EndGrabbing", 0.2f + .1f);
            }

            else if (currentlyInWaterWagon)
            {
                if (TutorialManager.GetInstance().duringTutorial &&
                TutorialManager.GetInstance().GetCurrentPhase() != TutorialManager.tutPhases.repararVagonAgua)
                    return;
                TrainManager.Instance.setGlobalLightColor(Color.white);

                TutorialManager.GetInstance().TryToChangePhase(TutorialManager.tutPhases.repararVagonAgua);

                //TrainManager.Instance.RechargeWater();

                TrainManager.Instance.waterDown = true;
                AudioManager_PK.instance.Stop("WaterExplosion");
                AudioManager_PK.instance.Play("WaterDown", Random.Range(0.9f, 1f));
            }
        }
    }

    IEnumerator MoveCoal(GameObject coal)
    {
        while(Vector3.Distance(coal.transform.localPosition, grabSpot.localPosition) >= 0.1f)
        {
            coal.transform.localPosition = Vector3.MoveTowards(coal.transform.localPosition, grabSpot.localPosition, 6 * Time.deltaTime);
            yield return 0;
        }

        coal.transform.localPosition = grabSpot.localPosition;
    }

    void EndGrabbing()
    { grabbingAnItem = false; }

    void EndDropping()
    { droppingAnItem = false; }


    #endregion

    #region GrabSystem


    GrabbableItem grabbedItem;

    [Header("Grabbing Mechanic")]
    public List<GrabbableItem> reachableItems;

    [SerializeField] GameObject coalPrefab;

    public void ReachableItemTriggerEnter(OnTriggerDelegation delegation)
    {
        GrabbableItem g = delegation.Other.GetComponent<GrabbableItem>();
        if (g != null && g.playerGrabbingThis == null)
            AddReachableItem(g);
    }

    public void ReachableItemTriggerExit(OnTriggerDelegation delegation)
    {
        GrabbableItem g = delegation.Other.GetComponent<GrabbableItem>();
        if (g != null)
            RemoveReachableItem(g);
    }

    // A?adir/Quitar objeto a la lista de objetos cercanos
    void AddReachableItem(GrabbableItem g)
    {
        reachableItems.Add(g);
    }

    public void RemoveReachableItem(GrabbableItem g)
    {
        reachableItems.Remove(g);
    }

    // Comporbar si se entra en el vagon del carbon

    bool currentlyInCoalWagon;
    bool currentlyInWaterWagon;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Turret>())
            currentlyInTurretWagon = true;

        if (collision.CompareTag("CoalWagon"))
            currentlyInCoalWagon = true;

        if (collision.CompareTag("WaterWagon"))
            currentlyInWaterWagon = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Turret>())
            currentlyInTurretWagon = false;


        if (collision.CompareTag("CoalWagon"))
            currentlyInCoalWagon = false;

        if (collision.CompareTag("WaterWagon"))
            currentlyInWaterWagon = false;
    }

    #endregion

    #region Movility

    public void GetMoveInput(InputAction.CallbackContext context)
    {
        //Debug.Log("context.ReadValue<Vector2>().x = " + context.ReadValue<Vector2>().x);

        input_hor = context.ReadValue<Vector2>().x;
        input_ver = context.ReadValue<Vector2>().y;
    }

    void UpdateFlipGfx()
    {
        // Cambiar direccion actual
        if (input_hor != 0)
        {
            if (input_hor < 0)
            { dir = -1; gfx.transform.localScale = new Vector3(-1, 1, 0); }
            else
            { dir = 1; gfx.transform.localScale = new Vector3(1, 1, 0); }
        }
    }

    void FixedUpdate()
    {
        Move();

        CheckGround();
    }

    void Move()
    {
        //if (!stunned)
        //{
        //    Vector3 targetVelocity = new Vector2(realInputHor * speed, rb.velocity.y);
        //    // And then smoothing it out and applying it to the character
        //    rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, smoothTime);
        //}
        //else
        //{
        //    Vector3 targetVelocity = new Vector2(0, rb.velocity.y);
        //    // And then smoothing it out and applying it to the character
        //    rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, .5f);
        //}

        if (usingTurret)
        {
            turretControl.RotateCannon(input_hor);
            return;
        }


        if (enteringTurret)
        {
            rb.position = Vector3.Lerp(rb.position, turret.transform.position, 0.45f);

            if (Vector2.Distance(rb.position, turret.transform.position) < turretEnteringRadius)
            {
                turretOutline.SetActive(true);
                usingTurret = true;
                transform.position = turret.transform.position;
                transform.SetParent(turret.transform);
                enteringTurret = false;
                gfx.enabled = false;

            }

            return;
        }



        Vector3 targetVelocity = new Vector2(input_hor * speed, rb.velocity.y);

        WagonLogic wagonLogic = transform.GetComponentInParent<WagonLogic>();
        if (wagonLogic != null)
            targetVelocity += new Vector3(wagonLogic.transform.GetComponent<Rigidbody2D>().velocity.x, wagonLogic.transform.GetComponent<Rigidbody2D>().velocity.y + 10 * Time.deltaTime);

        //check if in screen bounds
        if ((transform.position.x >= screenLimit && targetVelocity.x > 0) ||
            (transform.position.x <= -screenLimit && targetVelocity.x < 0))
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            // And then smoothing it out and applying it to the character
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, smoothTime);

        }

        //Clamp pos so you dont go out of bounds
        //float x = Mathf.Clamp(transform.position.x, leftMargin.position.x, rightMargin.position.x);
        //transform.position = new Vector2(x, transform.position.y);
    }

    void CheckGround()
    {
        if (!isDropping)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck_tr.position, groundCheck_radius, groundLayer);

            if (colliders.Length == 0)
                onGround = false;
            else
            {
                //if (!onGround)
                //{
                //    landing1.Play();
                //    landing2.Play();
                //}
                onGround = true;
                canDoubleJump = true;

                groundRememberTimer = groundRemember;

                // Se da por finalizado el salto despues del dash
            }
        }
        else
        {
            onGround = false;
        }

        //if (!onGround_Remember && onGround && !isRespawning)
        //    AudioManager_PK.instance.Play("Fall", Random.Range(.3f, .6f));

        onGround_Remember = onGround;
    }

    bool onGround_Remember;

    void Jump_check()
    {
        jumpRememberTimer -= Time.deltaTime;
        groundRememberTimer -= Time.deltaTime;

        if (rb.velocity.y > 4)
            groundRememberTimer = 0;


        if (jumpRememberTimer > 0 && onGround && input_ver < -.8f && !isDropping)
        {
            jumpRememberTimer = 0;
            groundRememberTimer = 0;

            onGround = false;
            StartCoroutine(Dropping());
        }


        if (jumpRememberTimer > 0 && groundRememberTimer > 0)
        {
            jumpRememberTimer = 0;
            groundRememberTimer = 0;

            onGround = false;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0f, jumpForce));

            AudioManager_PK.instance.Play("Jump", Random.Range(0.7f, 0.8f));
            jumping.Play(true);
        }

        if (jumpRememberTimer > 0 && canDoubleJump)
        {
            jumpRememberTimer = 0;

            canDoubleJump = false;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0f, 3000));

            AudioManager_PK.instance.Play("Jump", Random.Range(1.3f, 1.5f));
            jumping.Play(true);
            doubleJumping.Play(true);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        landing1.Play();
        landing2.Play();
    }

    private IEnumerator Dropping()
    {
        isDropping = true;
        Collider2D playerCol = GetComponent<Collider2D>();
        Collider2D platformCol = transform.parent.GetComponent<Collider2D>();

        Physics2D.IgnoreCollision(playerCol, platformCol);

        yield return new WaitForSeconds(0.4f);

        isDropping = false;
        Physics2D.IgnoreCollision(playerCol, platformCol, false);
    }





    #endregion

    public SpriteRenderer triangulo;

    public IEnumerator RespawnColor(float respawnTime, int timesToBlink)
    {

        while(timesToBlink > 0)
        {
            gfx.enabled = true;
            yield return new WaitForSeconds(respawnTime / 5 * 0.5f);
            gfx.enabled = false;
            yield return new WaitForSeconds(respawnTime / 5  *0.5f);
            
            timesToBlink--;
            
        }
        gfx.enabled = true;
        killable = true;
        //float timePassed = 0;
        //gfx.color = colorRespawn;
        //while (timePassed < respawnTime)
        //{
        //    // Code to go left here
        //    timePassed += Time.deltaTime;
        //    gfx.color = Color.Lerp(gfx.color, Color.white, timePassed/respawnTime);

        //    yield return null;
        //}

        //killable = true;

    }
}
