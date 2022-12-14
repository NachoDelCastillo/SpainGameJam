using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CoalWagon : MonoBehaviour
{
    public GameObject coal;

    [SerializeField] Transform iniPos, finalPos;

    [SerializeField] float speed;

    public bool coalStarted, coalReady, inWagon;

    [HideInInspector] public GameObject clon;
    // Start is called before the first frame update
    void Start()
    {
        coalStarted = false;
        coalReady = false;
        inWagon = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!coalStarted && !coalReady) CreateCoal();


        if (coal == null || !inWagon) coalReady = false;
    }

    void CreateCoal()
    {
        if (TrainManager.Instance.coalsInScreen.Count >= 2) return;

        coalStarted = true;
        inWagon = true;

        clon = Instantiate(coal, transform.parent);
        TrainManager.Instance.coalsInScreen.Add(clon.GetComponent<GrabbableItem>());
        clon.GetComponent<GrabbableItem>().initialParent = transform.parent;
        clon.GetComponent<GrabbableItem>().wagon = this;
        clon.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingLayerName = "MidTrain";
        clon.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sortingLayerName = "MidTrain";
        clon.transform.position = iniPos.position;

        // Smooooooooooooooooooooth
        SpriteRenderer sp = coal.GetComponentInChildren<SpriteRenderer>();
        sp.color = new Color(1, 1, 1, 0);
        sp.DOFade(1, 2);

        clon.transform.GetChild(0).DORotate(new Vector3(0, 0, -720), 2, RotateMode.FastBeyond360);
        clon.transform.GetChild(0).localScale = new Vector3(0, 0, 0);
        clon.transform.GetChild(0).DOScale(1, 2);

        StartCoroutine(MoveCoal(clon));
    }

    IEnumerator MoveCoal(GameObject coal)
    {
        coal.GetComponent<Rigidbody2D>().isKinematic = true;

        while (coal.transform.position.y < finalPos.position.y - 0.2f)
        {
            //Debug.Log("en corrutina");
            coal.transform.Translate(coal.transform.up * speed * Time.deltaTime);
            //Debug.Log(coal.transform.position);
            yield return null;
        }

        coal.transform.position = finalPos.position;

        coal.GetComponent<GrabbableItem>().coalReady = true;
        coalReady = true;
        coalStarted = false;
    }
}
