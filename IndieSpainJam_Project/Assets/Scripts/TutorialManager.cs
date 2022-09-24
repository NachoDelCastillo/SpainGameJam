using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    // Fases del tutorial
    public enum tutPhases { agarrarCarbonParaTorreta, meterCarbonEnTorreta, 
        meterseEnTorreta, matarEnemigoTorreta, salirDeTorreta, agarrarCarbonParaMotor, meterCarbonEnMotor,
        trenEnMarcha}

    // El currentPhase muestra lo que el jugador necesita hacer
    tutPhases currentPhase = tutPhases.agarrarCarbonParaTorreta;


    [Serializable]
    public struct tutItems
    {
        public tutPhases phase;
        public SpriteRenderer panel;
        public SpriteRenderer[] infoImages;
        public TMP_Text[] infoTexts;
    }
    [SerializeField] tutItems[] TutorialElements;


    private void Awake()
    {
        StartCoroutine(DoTutorial());
    }

    IEnumerator DoTutorial()
    {
        yield return new WaitForSeconds(1);

        // TUTORIAL DE AGARRAR EL CARBON

        yield return new WaitUntil(() => currentPhase == tutPhases.meterCarbonEnTorreta);

        // TUTORIAL DE METER CARBON EN LA TORRETA

        yield return new WaitUntil(() => currentPhase == tutPhases.meterseEnTorreta);

        // TUTORIAL DE COMO METERSE EN LA TORRETA

    }


    // Este metodo se llama cuando se realiza alguna de las acciones necesarias para completar el tutorial
    public void TryToChangePhase(tutPhases phaseDone)
    {
        if (currentPhase == phaseDone)
            currentPhase++;

        // STOP
        //if (currentPhase = TutPhases.trenEnMarcha)
    }

    IEnumerator ShowTutorialItems(tutItems phaseItem)
    {
        // Tiempo que tarda en aparecer el panel
        float panelShowTime = 1;

        // Tiempo que tarda en aparecer la informacion del panel (texxtos e imagenes)
        float infoPanelShowTime = .5f;



        // Panel

        // Fade
        SpriteRenderer panel = phaseItem.panel;
        panel.color = new Color(1, 1, 1, 0);
        panel.DOFade(1, panelShowTime);

        // Position
        Vector3 panelPosition = panel.transform.position;
        phaseItem.panel.transform.position = 
            new Vector3(panelPosition.x, panelPosition.y - 1);
        phaseItem.panel.transform.DOMoveY(panelPosition.y, panelShowTime);


        yield return new WaitForSeconds(panelShowTime);

        // Info in the Panel
        SpriteRenderer[] infoImages = phaseItem.infoImages;
        TMP_Text[] infoTexts = phaseItem.infoTexts;

        foreach (SpriteRenderer infoImage in infoImages)
        {
            infoImage.color = new Color(1, 1, 1, 0);
            infoImage.DOFade(1, panelShowTime);
        }

        foreach (TMP_Text infoText in infoTexts)
        {
            infoText.color = new Color(1, 1, 1, 0);
            infoText.DOFade(1, panelShowTime);
        }
    }


    // Cambios graficos en la escena

    void AgarrarCarbonParaTorreta()
    {

    }

    void MeterCarbonEnTorreta()
    {

    }
}
