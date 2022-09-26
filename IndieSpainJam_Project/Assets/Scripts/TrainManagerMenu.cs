using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;


public class TrainManagerMenu : MonoBehaviour
{
    public static TrainManagerMenu Instance { get; private set; }

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


    [SerializeField] WagonLogic[] wagons;
    [SerializeField] Transform[] columns;
    [SerializeField] Transform[] rows;

    [SerializeField] int velocityGainedByCoal;

    // Manager
    [SerializeField] Transform changeRail_Prefab;
    [SerializeField] Transform rails;
    [SerializeField] Transform[] spawnChangeRails;

    // Listas con los cambios de vias
    List<ChangeRail>[] changeRail_Lists;

    [SerializeField] GameObject sparkSys;

    public float GetmainVelocity()
    {
        return MainVelocity;
    }
    private void Awake()
    {
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
    }


    private void Start()
    {
        izqWagon = wagons[0];
        midWagon = wagons[1];
        derWagon = wagons[2];

        cameraShake = GetComponent<CameraShake>();

        AudioManager_PK.instance.sounds[6].source.mute = false;
        AudioManager_PK.instance.sounds[7].source.mute = false;

        smoke.Pause();
        AudioManager_PK.GetInstance().AudioVolume(0.5f, true);
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


        UpdateTextSpeed();
        SpawnChangeRail();
        StartCoroutine(MoveWagonsHorizontally());
    }

    [SerializeField] public float moveIntensity = 8;
    float spawnTimer;
    public IEnumerator SpawnChangeRail()
    {
        Debug.Log("SpawnChangeRail");

        //spawnTimer = Random.Range(8, 10);
        spawnTimer = moveIntensity;

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

            railsways_b[1] = true;

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


        StartCoroutine(SpawnChangeRail());
    }


    WagonLogic izqWagon, midWagon, derWagon;

    public IEnumerator MoveWagonsHorizontally()
    {
        Debug.Log("MoveWagonsHorizontally");

        float moveTime = 3;

        for (int i = 0; i < 3; i++)
        {
            int posibilidad = Random.Range(0, 3);

            if (posibilidad == 0)
            {
                if (izqWagon.RailRow == midWagon.RailRow)
                    continue;

                // Los dos de la izquierda
                izqWagon.transform.DOMoveX(columns[1].position.x, moveTime);
                midWagon.transform.DOMoveX(columns[0].position.x, moveTime);

                izqWagon.RailColumn = 1;
                midWagon.RailColumn = 0;

                WagonLogic auxWagon = izqWagon;
                izqWagon = midWagon;
                midWagon = auxWagon;

                goto suuuu;
            }

            else if (posibilidad == 1)
            {
                if (derWagon.RailRow == midWagon.RailRow)
                    continue;

                // Los dos de la derecha
                midWagon.transform.DOMoveX(columns[2].position.x, moveTime);
                derWagon.transform.DOMoveX(columns[1].position.x, moveTime);

                midWagon.RailColumn = 2;
                derWagon.RailColumn = 1;

                WagonLogic auxWagon = derWagon;
                derWagon = midWagon;
                midWagon = auxWagon;
                
                goto suuuu;
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
                izqWagon.transform.DOMoveX(columns[2].position.x, moveTime);
                derWagon.transform.DOMoveX(columns[0].position.x, moveTime);

                izqWagon.RailColumn = 2;
                derWagon.RailColumn = 0;

                WagonLogic auxWagon = derWagon;
                derWagon = izqWagon;
                izqWagon = auxWagon;

                goto suuuu;
            }
        }

        suuuu:

        yield return new WaitForSeconds(moveIntensity);

        StartCoroutine(MoveWagonsHorizontally()); ;
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

        StartCoroutine(MoveWagonsHorizontally());


        RotateWheel();

        if (Input.anyKeyDown)
            pressAnything_b = true;

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
    bool pressAnything_b;

    
   

    void AddVelocity()
    {
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
        int lastMainVel = MainVelocity;
        MainVelocity += velocityGainedByCoal;
        float add = 0;

        while (add <= velocityGainedByCoal)
        {
            add += Time.deltaTime * velocityGainedByCoal;
            int text = lastMainVel + Mathf.RoundToInt(add);
            MainVelocity_text.text = text.ToString() + " / 100 Km/h";
            yield return null;
        }
    }
}
    #endregion
