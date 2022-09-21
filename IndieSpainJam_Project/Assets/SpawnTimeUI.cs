using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SpawnTimeUI : MonoBehaviour
{
    // Start is called before the first frame update
    float elapsedTime;
    float timeToRespawn;
    float velocity = 0.5f;

    Transform timeTextPos;
    TextMeshProUGUI text;
    void Start()
    {
        elapsedTime = 0;
        timeTextPos = transform.GetChild(0);
        text = timeTextPos.gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if(elapsedTime >= timeToRespawn)
        {
            Destroy(this.gameObject);
        }


        text.text = ((int)timeToRespawn - ((int)elapsedTime)).ToString();
        timeTextPos.position += new Vector3(0, velocity * Time.deltaTime, 0);
        //timeTextPos.position += new Vector3(transform.position.x, velocity * Time.deltaTime, 0);
        //timeTextPos.position =  new Vector2(transform.position.x, transform.position.y + elapsedTime % ((int)elapsedTime));
    }


    public void SetTimeToRespawn(float time)
    {
        timeToRespawn = time;
    }
}
