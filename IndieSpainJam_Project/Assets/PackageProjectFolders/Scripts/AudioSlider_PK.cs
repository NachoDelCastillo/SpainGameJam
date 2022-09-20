using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSlider_PK : Button_PK
{
    [Header("SET UP")]
    [SerializeField]
    bool isMusicSlider;

    [HideInInspector]
    public Slider slider;

    // Devuelve true si se puede usar el mando para cambiar el valor del slider
    // Esto se usa para mantener un delay entre valores si se mantiene el joystick
    bool canChangeValueWithController = true;

    protected override void ExtraAwake()
    {
        // ASIGNAR VARIABLES
        slider = GetComponentInChildren<Slider>();
        UpdateText();
    }

    void Update()
    {
        // Si esta seleccionado este slider, y se mueve horizontalmente, cambiar su valor
        horizontalControllerInput();
    }

    #region Horizontal Control
    void horizontalControllerInput()
    {
        if (menuManager.buttonSelected == buttonIndex && canChangeValueWithController
            && Input.GetAxisRaw("Horizontal") != 0)
        {
            canChangeValueWithController = false;
            StartCoroutine("CanChangeValueWithController");

            if (Input.GetAxisRaw("Horizontal") > 0 && slider.value != slider.maxValue)
                OnChangeValue(slider.value + 1);
            else if (Input.GetAxisRaw("Horizontal") < 0 && slider.value != 0)
                OnChangeValue(slider.value - 1);
        }
    }

    IEnumerator CanChangeValueWithController()
    {
        yield return new WaitForSecondsRealtime(.25f);
        canChangeValueWithController = true;
    }

    // Este metodo se llama cada vez que se cambia el valor del slider
    public void OnChangeValue(float newValue)
    {
        // Sound
        AudioManager_PK.GetInstance().Play("ButtonPress", 1);

        // Logic
        FindObjectOfType<AudioManager_PK>().AudioVolume(newValue, isMusicSlider); // Cambiar volumen en el "AudioManager"

        slider.value = newValue; // Actualizar slider

        UpdateText();
    }

    #endregion

    void UpdateText()
    {
        // Cambiar texto
        if (slider.value != 10) // Si el nuevo valor es menor que 10
        {
            if (isMusicSlider) thisText.text = "MUSIC   " + slider.value; // Si es el slider del Sonido
            else thisText.text = "SOUND   " + slider.value; // Si es el slider de la musica
        }
        else
        {
            if (isMusicSlider) thisText.text = "MUSIC  " + 10; // Si es el slider del Sonido 
            else thisText.text = "SOUND  " + 10; // Si es el slider de la musica
        }
    }
}
