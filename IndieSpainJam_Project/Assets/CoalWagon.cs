using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CoalWagon : MonoBehaviour
{
    [SerializeField] GameObject coal;

    [SerializeField] Transform iniPos, finalPos;

    [SerializeField] float speed;

    public bool coalStarted, coalReady;

    [HideInInspector] public GameObject clon;
    // Start is called before the first frame update
    void Start()
    {
        coalStarted = false;
        coalReady = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!coalStarted && !coalReady) CreateCoal();
    }

    void CreateCoal()
    {
        coalStarted = true;

        clon = Instantiate(coal, transform.parent);
        clon.transform.position = iniPos.position;

        // Smooooooooooooooooooooth
        SpriteRenderer sp = coal.GetComponentInChildren<SpriteRenderer>();
        sp.color = new Color(1, 1, 1, 0);
        sp.DOFade(1, 1);

        clon.transform.GetChild(0).DORotate(new Vector3(0, 0, -720), 1, RotateMode.FastBeyond360);
        clon.transform.GetChild(0).localScale = new Vector3(0, 0, 0);
        clon.transform.GetChild(0).DOScale(1, 1);

        StartCoroutine(MoveCoal(clon));
    }

    IEnumerator MoveCoal(GameObject coal)
    {
        coal.GetComponent<Rigidbody2D>().isKinematic = true;

        while (Mathf.Abs(coal.transform.position.y - finalPos.position.y) >= 0.3f)
        {
            coal.transform.Translate(coal.transform.up * speed * Time.deltaTime);
            Debug.Log(coal.transform.position);
            yield return null;
        }

        coal.transform.position = finalPos.position;

        coal.GetComponent<GrabbableItem>().coalReady = true;
        coal.GetComponent<GrabbableItem>().createdPos = coal.transform.position;
        coalReady = true;
        coalStarted = false;
    }
}
