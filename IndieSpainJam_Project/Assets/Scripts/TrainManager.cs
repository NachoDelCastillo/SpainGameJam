using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
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

    // Listas con los cambios de vias
    List<ChangeRail>[] changeRail_Lists;


    private void Awake()
    {
        if (instance == null)
            instance = this;

        changeRail_Lists = new List<ChangeRail>[3];

        changeRail_Lists[0] = new List<ChangeRail>();
        changeRail_Lists[1] = new List<ChangeRail>();
        changeRail_Lists[2] = new List<ChangeRail>();
    }

    private void Start()
    {
        health = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;

        StartCoroutine(SpawnChangeRail());
    }

    float spawnTimer;
    IEnumerator SpawnChangeRail()
    {
        spawnTimer = Random.Range(2, 3);

        while (spawnTimer > 0)
        {
            spawnTimer -= Time.deltaTime;

            yield return 0;
        }

        int selectedRow = Random.Range(0, 3);

        ChangeRail newChangeRail = Instantiate(changeRail_Prefab, rows[selectedRow].position + new Vector3(30, 0), Quaternion.identity, rails).GetComponent<ChangeRail>();

        // Meter el cambio de via en la lista de cambio de via correspondiente
        changeRail_Lists[selectedRow].Add(newChangeRail);

        bool[] railsways_b =
        { true, false, true };

        RandomRail(ref railsways_b[0]);
        RandomRail(ref railsways_b[1]);
        RandomRail(ref railsways_b[2]);

        if (selectedRow == 0)
            railsways_b[0] = false;
        else if (selectedRow == 2)
            railsways_b[2] = false;

        newChangeRail.SetRailWays(railsways_b);

        StartCoroutine(SpawnChangeRail());
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
        // Mover vagones
        CheckWagons();

        // Destruir los cambios de vagon que se van por la izquierda
        RemoveRailChanges(0);
        RemoveRailChanges(1);
        RemoveRailChanges(2);

        DebugRow(0);
        DebugRow(1);
        DebugRow(2);
    }

    // Se encarga de comprobar cada una de las posiciones de los vagones
    // para ver si estan en un cambio de via, elegir su via y cambiarlo
    void CheckWagons()
    {
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
                if (changeRail.transform.position.x <= thisWagon.transform.position.x)
                { 
                    // Si este vagon ya ha usado este cambio de via, ignorarlo
                    if (changeRail.wagonsThatAlreadyUsedThis.Contains(thisWagon)) continue;


                    // Seleccionar a la via a la que se va a mover este vagon
                    int[] possibleRailWays = changeRail.GetPossibleRailWays();

                    if (possibleRailWays.Length == 0)
                        continue;
                    int selectedRow = possibleRailWays[Random.Range(0, possibleRailWays.Length-1)];

                    // Si la via a la que se quiere ir coinide con la via en la que ya se esta
                    // Mover el vagon a la via 
                    if (selectedRow != wagonRailRow)
                        thisWagon.transform.DOMoveY(rows[selectedRow].position.y, 1);

                    // Informar al vagon de que ha cambiado de via
                    thisWagon.RailRow = selectedRow;

                    // Como este vagon ya ha usado este cambio de via,
                    // informarselo al cambio de via para que no lo vuelva a usar
                    changeRail.wagonsThatAlreadyUsedThis.Add(thisWagon);
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
