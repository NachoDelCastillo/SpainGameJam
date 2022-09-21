using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Regula el comportamiento del cubo con valores de ruido en su posici�n
/// </summary>
public class Noise : MonoBehaviour
{
    /// <summary>
    /// M�quina de estados que controla el scan del rudio
    /// </summary>
    enum IncrementState { ascending, descending }
    IncrementState incrementState;

    /// <summary>
    /// Posici�n inicial del objeto
    /// </summary>
    Vector3 initialPosition;

    //Variable auxiliar para el scan del ruido en el eje x de perlin para el eje X del objeto
    float xX;
    //Variable auxiliar para el scan del ruido en el eje x de perlin para el eje Y del objeto
    float xY;
    //Variable auxiliar para el scan del ruido en el eje x de perlin para el eje Z del objeto

    //Variable auxiliar para el scan del ruido en el eje y
    float y;
    //Variable auxiliar para el m�ximo nivel de scan del ruido en el eje y
    float maxY = 1000;
    //Variable auxiliar para el m�nimo nivel de scan del ruido en el eje y
    float minY = 10;

    //Variable auxiliar para el m�ximo nivel de scan del ruido en el eje y
    float maxX = 1000;
    //Variable auxiliar para el m�nimo nivel de scan del ruido en el eje y
    float minX = 10;

    [Range(0, 5)]
    public float noiseMultiplier = 0.25f, noiseFrecuency = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        //Establece el estado inicial
        incrementState = IncrementState.ascending;

        //Guarda la posici�n inicial
        initialPosition = transform.localPosition;

        //Aletario entre dos valores
        xX = Random.Range(minX, maxX);
        xY = Random.Range(minX, maxX);
    }

    // Update is called once per frame
    void Update()
    {
        #region Incrementa o decrementa el ruido
        if (incrementState == IncrementState.ascending)
            y += Time.deltaTime * noiseFrecuency;
        else
            y -= Time.deltaTime * noiseFrecuency;
        #endregion

        #region Establece el ruido en la posici�n del objeto
        float ruidoX = Mathf.PerlinNoise(xX, y);  //Toma el ruido
        float ruidoY = Mathf.PerlinNoise(xY, y);  //Toma el ruido

        transform.localPosition = new Vector3(initialPosition.x + (ruidoX * noiseMultiplier), initialPosition.y + (ruidoY * noiseMultiplier), 0);
        #endregion

        #region Gestion del estado del incrmento
        if (y >= maxY)
            incrementState = IncrementState.descending;
        else if (y < minY)
            incrementState = IncrementState.ascending;
        #endregion
    }
}
