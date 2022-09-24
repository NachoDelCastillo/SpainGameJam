using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    // Fases del tutorial
    public enum TutPhases { agarrarCarbonParaTorreta, meterCarbonEnTorreta, 
        meterseEnTorreta, matarEnemigoTorreta, salirDeTorreta, agarrarCarbonParaMotor, meterCarbonEnMotor,
        trenEnMarcha}

    // El currentPhase muestra lo que el jugador necesita hacer
    TutPhases currentPhase = TutPhases.agarrarCarbonParaTorreta;


    [Serializable]
    public struct tutorialItems
    {
        public TutPhases phase;
        public SpriteRenderer panels;
        public SpriteRenderer[] infoPanels;
        public TMP_Text[] texts;
    }
    [SerializeField] tutorialItems[] TutorialElements;


    private void Awake()
    {
        StartCoroutine(DoTutorial());
    }

    IEnumerator DoTutorial()
    {
        yield return new WaitForSeconds(1);

        // TUTORIAL DE AGARRAR EL CARBON

        yield return new WaitUntil(() => currentPhase == TutPhases.meterCarbonEnTorreta);

        // TUTORIAL DE METER CARBON EN LA TORRETA

        yield return new WaitUntil(() => currentPhase == TutPhases.meterseEnTorreta);

        // TUTORIAL DE COMO METERSE EN LA TORRETA

    }


    // Este metodo se llama cuando se realiza alguna de las acciones necesarias para completar el tutorial
    public void TryToChangePhase(TutPhases phaseDone)
    {
        if (currentPhase == phaseDone)
            currentPhase++;

        // STOP
        //if (currentPhase = TutPhases.trenEnMarcha)
    }

    void ShowTutorialItems(TutPhases)
    {

    }


    // Cambios graficos en la escena

    void AgarrarCarbonParaTorreta()
    {

    }

    void MeterCarbonEnTorreta()
    {

    }
}
