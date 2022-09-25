using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;


public class TrainManager : MonoBehaviour
{
    [SerializeField] Light2D globalLight;
    public static TrainManager Instance { get; private set; }

    float elapsedTime = 0;
    float timeToMuteExplosions = 5f;
    public bool gameLost = false, letterOfFinalWhenLoseAlreadyOut = false;
    bool showingResults, waterTankExploding = false;

    [SerializeField]
    int MainVelocity = 0;

    Vector3[] initialPosOfWagons;
    [SerializeField] CameraShake cameraShake;

    [SerializeField] SpriteRenderer wheelNeon;
    Material wheelMaterial;
    Vector4 wheelColor, wheelBaseColor;
    [SerializeField] float maxIntensity, normalIntensity, timeToReachMax;

    [Header("Referencias")]
    [SerializeField] ParticleSystem smoke;
    [SerializeField] Transform deliverCoal;
    [SerializeField] TMP_Text MainVelocity_text;

    [SerializeField] TMP_Text resultText;
    [SerializeField] Image resultPanel;
    [SerializeField] TMP_Text pressAnything;
    [SerializeField] Color winColor;
    [SerializeField] Color loseColor;


    [SerializeField] WagonLogic[] wagons;
    [SerializeField] Transform[] columns;
    [SerializeField] Transform[] rows;



    [SerializeField] int velocityGainedByCoal = 5;

    //Health
    [SerializeField] float health, maxHealth;
    [SerializeField] Slider healthSlider;


    // Manager
    [SerializeField] Transform changeRail_Prefab;
    [SerializeField] Transform rails;
    [SerializeField] Transform[] spawnChangeRails;

    // Listas con los cambios de vias
    List<ChangeRail>[] changeRail_Lists;

    //Agua
    [SerializeField] public Slider waterSlider;
    [SerializeField] AnimationCurve waterCurve;
    [SerializeField] AnimationCurve waterCurveMax;
    //Updated upstream
    [SerializeField] public float currentWater, maxWater, waterSubstracPerSecond, dmgWhenWater0PerSecond;
    [SerializeField] Color[] waterColorSlider;
    [SerializeField] Image waterFillImage;
    [SerializeField] float timeForWaterDown;
    [SerializeField] Transform waterDanger, waterDangerIniPos, waterDangerFinalPos;
    [SerializeField] ParticleSystem waterParticles;
    Vector3 waterDangerTarget;
    bool rotateRight;
    public bool waterDown;
    float waterTimer;

    [SerializeField] GameObject sparkSys;

    float thisText_InitScale;
    public float GetmainVelocity()
    {
        return MainVelocity;
    }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        changeRail_Lists = new List<ChangeRail>[3];

        changeRail_Lists[0] = new List<ChangeRail>();
        changeRail_Lists[1] = new List<ChangeRail>();
        changeRail_Lists[2] = new List<ChangeRail>();


        waterSlider.maxValue = maxWater;
    }


    private void Start()
    {
        izqWagon = wagons[0];
        midWagon = wagons[1];
        derWagon = wagons[2];

        thisText_InitScale = MainVelocity_text.transform.localScale.x;

        cameraShake = GetComponent<CameraShake>();
        gameLost = false;
        health = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;

        //currentWater = 0;
        //waterSlider.value = currentWater;
        waterDanger.gameObject.SetActive(false);
        waterParticles.Stop();
        maxWater = waterCurveMax.Evaluate(MainVelocity / maxWheelVelocity);
        waterSlider.maxValue = maxWater;

        AudioManager_PK.instance.sounds[6].source.mute = false;
        AudioManager_PK.instance.sounds[7].source.mute = false;

        smoke.Pause();

        // StartCoroutine(SpawnChangeRail());

        initialPosOfWagons = new Vector3[4];
        for (int i = 0; i < wagons.Length; i++)
        {
            initialPosOfWagons[i] = wagons[i].transform.position;
            wagons[i].RailColumn = i;
        }

        wagons[3].locomotora = true;

        wheelMaterial = wheelNeon.material;
        wheelBaseColor = wheelMaterial.GetColor("_Color");
        wheelColor = new Vector4(wheelBaseColor.x * normalIntensity, wheelBaseColor.y * normalIntensity, wheelBaseColor.z * normalIntensity, wheelBaseColor.w);
        wheelMaterial.SetColor("_Color", wheelColor);
    }

    float spawnTimer;
    public IEnumerator SpawnChangeRail()
    {
        //spawnTimer = Random.Range(8, 10);
        spawnTimer = 3;

        while (spawnTimer > 0)
        {
            spawnTimer -= Time.deltaTime;

            yield return 0;
        }

        int[] possibleRows = new int[4] {
            wagons[0].RailRow,
            wagons[1].RailRow,
            wagons[2].RailRow,
            wagons[3].RailRow
        };

        int selectedRow = possibleRows[Random.Range(0, 4)];

        bool[] railsways_b =
        { false, false, false };

        bool correctBools;
        do
        {
            correctBools = true;

            RandomRail(ref railsways_b[0]);
            RandomRail(ref railsways_b[1]);
            RandomRail(ref railsways_b[2]);

            if (selectedRow == 0)
                railsways_b[0] = false;
            else if (selectedRow == 2)
                railsways_b[2] = false;


            bool selectedRowIsOccupied = false;
            int numOfWays = 0;
            for (int i = 0; i < railsways_b.Length; i++)
            {
                // Si el rail de enfrente es true, poner esta variable a true
                if (i == 1)
                    selectedRowIsOccupied = railsways_b[i];

                if (railsways_b[i])
                    numOfWays++;
            }

            // Si solo hay una via, y justo es la que va recta, no aceptarla
            if (selectedRowIsOccupied && numOfWays == 1) correctBools = false;

            if (numOfWays == 0) correctBools = false;

            // Si hay un rail que va de frente, 
            if (railsways_b[1] == true)
                if (Random.Range(0, 2) == 0)
                    correctBools = false;

            if (!railsways_b[0] && !railsways_b[1] && !railsways_b[2]) correctBools = false;

        } while (!correctBools);


        ChangeRail newChangeRail = Instantiate(changeRail_Prefab, spawnChangeRails[selectedRow].position + new Vector3(30, 0), Quaternion.identity, rails).GetComponent<ChangeRail>();

        // Meter el cambio de via en la lista de cambio de via correspondiente
        changeRail_Lists[selectedRow].Add(newChangeRail);

        newChangeRail.SetRailWays(selectedRow, railsways_b);


        StartCoroutine(MoveWagonsHorizontally());
    }


    WagonLogic izqWagon, midWagon, derWagon;

    IEnumerator MoveWagonsHorizontally()
    {
        for (int i = 0; i < 3; i++)
        {
            int posibilidad = Random.Range(0, 3);

            if (posibilidad == 0)
            {
                if (izqWagon.RailRow == midWagon.RailRow)
                    continue;

                // Los dos de la izquierda
                izqWagon.transform.DOMoveX(columns[1].position.x, 1);
                midWagon.transform.DOMoveX(columns[0].position.x, 1);

                izqWagon.RailColumn = 1;
                midWagon.RailColumn = 0;

                WagonLogic auxWagon = izqWagon;
                izqWagon = midWagon;
                midWagon = auxWagon;

                yield break;
            }

            else if (posibilidad == 1)
            {
                if (derWagon.RailRow == midWagon.RailRow)
                    continue;

                // Los dos de la derecha
                midWagon.transform.DOMoveX(columns[2].position.x, 1);
                derWagon.transform.DOMoveX(columns[1].position.x, 1);

                midWagon.RailColumn = 2;
                derWagon.RailColumn = 1;

                WagonLogic auxWagon = derWagon;
                derWagon = midWagon;
                midWagon = auxWagon;

                yield break;
            }

            else if (posibilidad == 2)
            {
                if (izqWagon.RailRow == derWagon.RailRow)
                    continue;

                if (izqWagon.RailRow == midWagon.RailRow)
                    continue;

                if (derWagon.RailRow == midWagon.RailRow)
                    continue;


                // Los dos de la derecha
                izqWagon.transform.DOMoveX(columns[2].position.x, 1);
                derWagon.transform.DOMoveX(columns[0].position.x, 1);

                izqWagon.RailColumn = 2;
                derWagon.RailColumn = 0;

                WagonLogic auxWagon = derWagon;
                derWagon = izqWagon;
                izqWagon = auxWagon;

                yield break;
            }
        }

        yield return new WaitForSeconds(3);

        StartCoroutine(SpawnChangeRail()); ;
    }

    IEnumerator MoveWagonsHorizontally_()
    {

        int currentwWagonIndex = Random.Range(0, 3);
        int numberOfWagonsTried = 0;

        // Informacion importante
        WagonLogic wagon;
        int finalDirectionOfTheWagon = 0;
        int wagonColumn;

        do
        {
            numberOfWagonsTried++;

            currentwWagonIndex++;
            if (currentwWagonIndex >= 3)
                currentwWagonIndex = 0;

            wagon = wagons[currentwWagonIndex];

            // Elegir direccion a la que se puede mover
            wagonColumn = wagon.RailColumn;

            // Averiguar si este vagon no tiene posibilidad de moverse
            bool canGoLeft = true, canGoRight = true;

            // Bordes
            if (wagonColumn == 0) canGoLeft = false;
            if (wagonColumn == 2) canGoRight = false;

            foreach (WagonLogic thisWagon in wagons)
            {
                // Comprobar que no es el mismo
                if (wagon != thisWagon)
                {
                    // Misma linea
                    if (wagon.RailRow == thisWagon.RailRow)
                    {
                        // Comprobar los dos lados
                        if (wagonColumn - 1 == thisWagon.RailColumn)
                            canGoLeft = false;
                        if (wagonColumn + 1 == thisWagon.RailColumn)
                            canGoRight = false;
                    }
                }
            }

            // Si se puede para alguno de los lados, seleccionarlo
            if (canGoLeft && !canGoRight)
                finalDirectionOfTheWagon = -1;
            else if (!canGoLeft && canGoRight)
                finalDirectionOfTheWagon = 1;
            else if (canGoLeft && canGoRight)
            {
                // Si se puede ir para los dos lados, elegir uno aleatoriamente
                if (Random.Range(0, 2) == 0)
                    finalDirectionOfTheWagon = 1;
                else finalDirectionOfTheWagon = -1;
            }

            // Si el vagon tiene direccion y no se han llegado a intentar todos los vagones, seguir
        } while (finalDirectionOfTheWagon == 0 && numberOfWagonsTried <= 3);

        int goToThisColumnIndex = wagonColumn + finalDirectionOfTheWagon;
        Vector3 goToThisPosition = columns[goToThisColumnIndex].position;

        // Con el vagon ya seleccionado, lo movemos
        wagon.transform.DOMoveX(goToThisPosition.x, 1);

        yield return new WaitForSeconds(1);

        //StartCoroutine(MoveWagonsHorizontally());
    }

    void RandomRail(ref bool railway_b)
    {
        if (Random.Range(0, 2) == 0)
            railway_b = true;
        else
            railway_b = false;
    }

    private void Update()
    {
        if (health <= 0)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > 0.1f)
            {
                timeToMuteExplosions -= elapsedTime;
                elapsedTime = 0;
                if (timeToMuteExplosions > 0)
                {
                    AudioManager_PK.instance.Play("SmallExplosion", Random.Range(0.8f, 1.1f));
                }
            }
            if ((Input.anyKey || Input.anyKeyDown) && letterOfFinalWhenLoseAlreadyOut)
                GameManager.instance.ChangeScene("MainMenu_Scene");
            return;
        }

        UpdateWater();


        RotateWheel();

        if (Input.anyKeyDown)
            pressAnything_b = true;

        if (showingResults) return;

        // Mover vagones
        CheckWagons();

        // Destruir los cambios de vagon que se van por la izquierda
        RemoveRailChanges(0);
        RemoveRailChanges(1);
        RemoveRailChanges(2);

        //DebugRow(0);
        //DebugRow(1);
        //DebugRow(2);
    }

    void UpdateWater()
    {
        if (waterDown)
        {
            WaterDown();
        }
        else
        {
            if (TutorialManager.GetInstance().duringTutorial) return;
            maxWater = waterCurveMax.Evaluate(MainVelocity / maxWheelVelocity);
            waterSlider.maxValue = maxWater;
            //Updated upstream
            // Si el tren esta quieto no joder el vagon de agua
            //if (MainVelocity <= 0) return;

            // Debug.Log("currentWater = " + currentWater);

            currentWater += waterSubstracPerSecond * Time.deltaTime;

            currentWater = Mathf.Clamp(currentWater, 0, maxWater);

            if (currentWater >= maxWater)
            {
                health -= dmgWhenWater0PerSecond * Time.deltaTime;
                setGlobalLightColor(Color.red);
                cameraShake.ShakeIt();
                //Loop audio cuando se sale
                if (!AudioManager_PK.instance.sounds[16].source.isPlaying)
                    AudioManager_PK.instance.Play("WaterExplosion", Random.Range(0.7f, 0.75f));
            }
            TakeDamage(0);
            currentWater = Mathf.Clamp(currentWater, 0, maxWater);

            if (currentWater >= maxWater * 0.5f)
            {
                //rotacion slider -5 -- 5 en z 
                if (waterFillImage.GetComponent<RectTransform>().rotation.eulerAngles.z < 85)
                {
                    rotateRight = true;

                }
                else if (waterFillImage.GetComponent<RectTransform>().rotation.eulerAngles.z > 95)
                {
                    rotateRight = false;
                }

                if (rotateRight) waterFillImage.GetComponent<RectTransform>().Rotate(new Vector3(0, 0, Time.deltaTime * 2));
                else waterFillImage.GetComponent<RectTransform>().Rotate(new Vector3(0, 0, -Time.deltaTime * 2));
            }
            else
            {
                if (waterFillImage.GetComponent<RectTransform>().rotation.eulerAngles.z < 90)
                {
                    waterFillImage.GetComponent<RectTransform>().Rotate(new Vector3(0, 0, Time.deltaTime * 1.5f));
                }
                else if (waterFillImage.GetComponent<RectTransform>().rotation.eulerAngles.z > 90)
                {
                    waterFillImage.GetComponent<RectTransform>().Rotate(new Vector3(0, 0, -Time.deltaTime * 1.5f));
                }
                //si la rotacion en z no es 0 hacer rotacion volver a 0 en z smooth
            }

            if (currentWater >= maxWater * 0.85f)
            {
                if (!waterParticles.isPlaying) waterParticles.Play();

                var main = waterParticles.main;
                main.startColor = new ParticleSystem.MinMaxGradient(waterFillImage.color);
            }
            else if (waterParticles.isPlaying) waterParticles.Stop();

            if (currentWater >= maxWater)
            {
                health -= dmgWhenWater0PerSecond * Time.deltaTime;
                TakeDamage(0);
                WaterDanger();
            }
            else
            {
                waterDanger.gameObject.SetActive(false);
                TutorialManager.GetInstance().HideTutorialItems(TutorialManager.tutPhases.repararVagonAgua);
            }


            waterSlider.value = currentWater;
        }

        ColorWater();
    }

    public void setGlobalLightColor(Color color)
    {
        globalLight.color = color;
    }

    [SerializeField] SpriteRenderer dangerSprite;
    public void ColorWater()
    {
        float value = currentWater / maxWater;
        waterFillImage.color = (value > 0.5) ? Color.Lerp(waterColorSlider[1], waterColorSlider[2], (value - 0.5f) * 2) : Color.Lerp(waterColorSlider[1], waterColorSlider[0], Mathf.Abs(value - 0.5f) * 2);
    }

    public void RechargeWater()
    {
        currentWater = 0;
        waterSlider.value = currentWater;
    }

    void WaterDown()
    {
        waterTimer += Time.deltaTime;
        if (waterCurve.Evaluate(waterTimer) * maxWater <= currentWater) currentWater = waterCurve.Evaluate(waterTimer) * maxWater;
        waterSlider.value = currentWater;

        if (waterTimer >= timeForWaterDown)
        {
            waterTimer = 0;
            waterDown = false;
        }
    }

    void WaterDanger()
    {
        if (!waterDanger.gameObject.activeInHierarchy)
        {
            TutorialManager.GetInstance().ShowTutorialItems(TutorialManager.tutPhases.repararVagonAgua);

            waterDanger.gameObject.SetActive(true);
            StartCoroutine(AppearDanger());
        }
        else
        {
            //Mover
        }
    }

    IEnumerator AppearDanger()
    {
        waterDanger.localScale = Vector3.zero;
        while (waterDanger.localScale.x < 1)
        {
            waterDanger.localScale += new Vector3(Time.deltaTime * 2, Time.deltaTime * 2, Time.deltaTime * 2);
            yield return null;
        }
    }

    [SerializeField]

    // Se encarga de comprobar cada una de las posiciones de los vagones
    // para ver si estan en un cambio de via, elegir su via y cambiarlo
    void CheckWagons()
    {
        if (MainVelocity <= 0) return;

        // Comprobar la posicion y linea de todos los vagones para ver si estan en un cambio de via
        for (int i = 0; i < wagons.Length; i++)
        {
            // Comprobar que este vagon no se este cambiando de via justo en este momento

            WagonLogic thisWagon = wagons[i];

            // Fila en la que se encuentra este vagon
            int wagonRailRow = thisWagon.RailRow;

            // Todos los cambios de via que hay en esta fila ahora mismo
            List<ChangeRail> changeRailsInThisRow = changeRail_Lists[wagonRailRow];

            if (changeRailsInThisRow.Count == 0) return;

            foreach (ChangeRail changeRail in changeRailsInThisRow)
            {
                // Si en este frame este vagon a pasado a un cambio de via un cambio de via
                if (changeRail.transform.position.x - 2 < thisWagon.transform.position.x
                    && thisWagon.transform.position.x < changeRail.transform.position.x + 2)
                {
                    //if (changeRail.)


                    // Si este vagon ya ha usado este cambio de via, ignorarlo
                    if (changeRail.wagonsThatAlreadyUsedThis.Contains(thisWagon)) continue;


                    // Seleccionar a la via a la que se va a mover este vagon
                    int[] possibleRailWays = changeRail.GetPossibleRailWays();

                    int selectedRow;
                    if (possibleRailWays.Length == 0)
                        continue;
                    else if (possibleRailWays.Length == 1)
                        selectedRow = possibleRailWays[0];
                    else
                        selectedRow = possibleRailWays[Random.Range(0, possibleRailWays.Length)];

                    // Si la via a la que se quiere ir coinide con la via en la que ya se esta
                    // Mover el vagon a la via 

                    if (selectedRow != wagonRailRow)
                    {
                        bool canGoThatDirection = true;
                        //foreach (WagonLogic otherWagon in wagons)
                        //{
                        //    if (otherWagon != thisWagon)
                        //    {
                        //        // Si no es la locomotora
                        //        if (otherWagon.wagonIndex != 3)
                        //        {
                        //            // Si estan en la misma columna
                        //            if (otherWagon.RailColumn == thisWagon.RailColumn)
                        //            {
                        //                // Si hay un vagon justo donde se planea ir, no ir
                        //                if (otherWagon.RailRow == selectedRow)
                        //                    canGoThatDirection = false;
                        //            }
                        //        }
                        //    }
                        //}

                        //if (selectedRow == 2) Debug.Log("dv");
                        //else

                        if (canGoThatDirection)
                        {
                            thisWagon.transform.DOMoveY(rows[selectedRow].position.y, 4);
                            thisWagon.RailRow = selectedRow;
                            changeRail.wagonsThatAlreadyUsedThis.Add(thisWagon);
                        }
                    }

                    // Informar al vagon de que ha cambiado de via

                    // Como este vagon ya ha usado este cambio de via,
                    // informarselo al cambio de via para que no lo vuelva a usar
                }
            }
        }
    }

    void RemoveRailChanges(int i)
    {
        List<ChangeRail> changeRailsInThisRow = changeRail_Lists[i];

        if (changeRailsInThisRow.Count == 0) return;

        ChangeRail deleteThisChange = null;

        foreach (ChangeRail thisChangeRail in changeRailsInThisRow)
        {
            if (thisChangeRail.transform.position.x < -40)
                deleteThisChange = thisChangeRail;
        }

        if (deleteThisChange != null)
        {
            changeRail_Lists[i].Remove(deleteThisChange);
            Destroy(deleteThisChange.gameObject);
        }
    }

    void DebugRow(int i)
    {
        string s = "";
        if (i == 0) s = "ROW UP = ";
        else if (i == 1) s = "ROW MID = ";
        else if (i == 2) s = "ROW LOW = ";

        List<ChangeRail> rowUP = changeRail_Lists[i];
        foreach (ChangeRail thisChangeRail in rowUP)
            s += thisChangeRail.name + ", ";
        Debug.Log(s);
    }

    IEnumerator GlowWheel()
    {
        float i = normalIntensity;
        while (i < maxIntensity)
        {
            wheelColor = new Vector4(wheelBaseColor.x * i, wheelBaseColor.y * i, wheelBaseColor.z * i, wheelBaseColor.w);
            wheelMaterial.SetColor("_Color", wheelColor);
            i += (maxIntensity - normalIntensity) / 10;
            yield return new WaitForSeconds(timeToReachMax / 10);
        }

        i = maxIntensity;
        while (i > normalIntensity)
        {
            wheelColor = new Vector4(wheelBaseColor.x * i, wheelBaseColor.y * i, wheelBaseColor.z * i, wheelBaseColor.w);
            wheelMaterial.SetColor("_Color", wheelColor);
            i -= (maxIntensity - normalIntensity) / 10;
            yield return new WaitForSeconds(timeToReachMax / 10);
        }
    }

    #region wevadas

    [SerializeField] Transform wheel;
    [SerializeField] Transform innerWheel;
    [SerializeField] float initialWheelVelocity;
    [SerializeField] float maxWheelVelocity;

    void RotateWheel()
    {
        float speed = (maxWheelVelocity / 100) * maxWheelVelocity;

        float rotateMainVelocity = -(speed + initialWheelVelocity) * Time.deltaTime;

        //Debug.Log("rotateMainVelocity = " + rotateMainVelocity);
        wheel.Rotate(new Vector3(0, 0, rotateMainVelocity));
    }

    void RotateWheelFast()
    {
        innerWheel.DORotate(new Vector3(0, 0, -1080), 3 - (maxWheelVelocity / 100), RotateMode.FastBeyond360);
    }

    public void CoalDelivered(OnTriggerDelegation delegation)
    {
        if (!delegation.Other.CompareTag("Coal")) return;


        if (TutorialManager.GetInstance().duringTutorial &&
        TutorialManager.GetInstance().GetCurrentPhase() != TutorialManager.tutPhases.meterCarbonEnMotor)
            return;

        TutorialManager.GetInstance().TryToChangePhase(TutorialManager.tutPhases.meterCarbonEnMotor);


        RotateWheelFast();

        GrabbableItem coal = delegation.Other.transform.GetComponent<GrabbableItem>();
        coal.ItemGrabbed(null);

        coal.transform.GetChild(0).DORotate(new Vector3(0, 0, -720), 1, RotateMode.FastBeyond360);
        coal.transform.GetChild(0).DOScale(0, 1);

        // Deliver time
        float deliverTime = .05f;
        StartCoroutine(Utils.MoveItemSmooth(coal.transform, deliverCoal, deliverTime));
        StartCoroutine(DestroyCoal(coal, deliverTime));
    }

    IEnumerator DestroyCoal(GrabbableItem coal, float seconds)
    {
        yield return new WaitForSeconds(.05f);
        if (coal != null)
            Destroy(coal.gameObject);

        AddVelocity();

        AudioManager_PK.instance.Play("Combust", 0.8f + ((float)MainVelocity / 100f) * 0.5f);

        if (MainVelocity >= 100 && !showingResults)
        {
            //yield return new WaitForSeconds(1);
            StartCoroutine(ShowResult(true));
        }
    }

    IEnumerator ShowResult(bool win)
    {
        showingResults = true;

        string fullString;
        if (win)
        {
            fullString = "SIUUUUUU";
            resultPanel.color = winColor;

            // Poner todos los vagones en fila

            for (int j = 0; j < wagons.Length; j++)
                wagons[j].transform.DOMove(initialPosOfWagons[j], 1);
        }
        else
        {
            gameLost = true;
            fullString = "CAGASTE";
            resultPanel.color = loseColor;
        }

        resultPanel.DOFade(.1f, 1);
        yield return new WaitForSeconds(1);

        StartCoroutine(Utils.WriteThis(fullString, resultText, .15f));

        yield return new WaitForSeconds(1);

        if (win)
        {
            // WIN
            yield return new WaitForSeconds(1);
            GameManager.instance.ChangeScene("MainMenu_Scene");
        }
        else
        {
            // LOSE
            float timeBetweenLetters = .05f;
            string s = "Press anything to try again";
            StartCoroutine(Utils.WriteThis(s, pressAnything, timeBetweenLetters));
            yield return new WaitForSeconds(timeBetweenLetters * s.Length);

            yield return new WaitForSeconds(1);

            letterOfFinalWhenLoseAlreadyOut = true;
            ShowAnyKeyButton();

            CancelInvoke();
            yield return 0;
        }


        showingResults = false;
    }

    bool pressAnything_b;

    void ShowAnyKeyButton()
    {
        pressAnything.gameObject.SetActive(true);

        Invoke("RemoveAnyKeyButton", .5f);
    }

    void RemoveAnyKeyButton()
    {
        pressAnything.gameObject.SetActive(false);

        Invoke("ShowAnyKeyButton", .5f);
    }

    void AddVelocity()
    {
        if (showingResults) return;

        if (MainVelocity == 0)
        {
            smoke.Play();
        }

        StartCoroutine(UpdateTextSpeed());

        GameObject clon = Instantiate(sparkSys, wheel.position, Quaternion.identity);
        clon.transform.parent = wheel;
        StartCoroutine(GlowWheel());
        var aux = smoke.emission;
        aux.rateOverTime = 2 + (((float)MainVelocity / 100f) * 18f);

        ParticleSystem.MinMaxCurve a = new();
        a = smoke.velocityOverLifetime.x;
        a.curveMultiplier = 0.5f + (((float)MainVelocity / 100f) * 50f);
        var aux2 = smoke.velocityOverLifetime;
        aux2.x = a;

        EnemySpawner.myDelegate?.Invoke();

    }

    IEnumerator UpdateTextSpeed()
    {
        MainVelocity_text.GetComponent<Transform>().DOScale(Vector3.one * thisText_InitScale * 1.3f, 0.2f).SetUpdate(true);
        int lastMainVel = MainVelocity;
        MainVelocity += velocityGainedByCoal;
        float add = 0;

        while (add <= velocityGainedByCoal)
        {
            add += Time.deltaTime * velocityGainedByCoal;
            int text = lastMainVel + Mathf.RoundToInt(add);
            MainVelocity_text.text = text.ToString() + " / 100 Km";
            yield return null;
        }

        MainVelocity_text.GetComponent<Transform>().DOScale(Vector3.one * thisText_InitScale, 0.2f).SetUpdate(true);
    }


    public void TakeDamage(float amount)
    {
        if (showingResults)
            return;

        health -= amount;

        globalLight.color = new Color(1, globalLight.color.g, globalLight.color.b, 1);
        if (health <= 0) health = 0;

        healthSlider.value = health;

        if (health <= 0 && !showingResults)
        {
            StartCoroutine(ShowResult(false));

            MainVelocity = 0;

            AudioManager_PK.instance.sounds[6].source.mute = true;

            ParticleSystem.MinMaxCurve a = new();
            a = smoke.velocityOverLifetime.x;
            a.curveMultiplier = 0;
            var aux2 = smoke.velocityOverLifetime;
            aux2.x = a;

            foreach (WagonLogic wagon in wagons)
            {
                wagon.Died();
            }
        }

    }
}
    #endregion
