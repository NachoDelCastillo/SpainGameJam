using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    // Si esta a true, haces el tutorial
    [HideInInspector] public bool doTutorial;


    static TutorialManager instance;
    public static TutorialManager GetInstance()
    { return instance; }

    // Fases del tutorial
    public enum tutPhases
    {
        agarrarCarbonParaTorreta, meterCarbonEnTorreta,
        meterseEnTorreta, matarEnemigoTorreta, salirDeTorreta, repararVagonAgua,
        agarrarCarbonParaMotor, meterCarbonEnMotor, trenEnMarcha
    }

    // El currentPhase muestra lo que el jugador necesita hacer
    tutPhases currentPhase = tutPhases.agarrarCarbonParaTorreta;

    [HideInInspector] public bool duringTutorial;

    [Serializable]
    public struct tutItems
    {
        public tutPhases phase;
        public SpriteRenderer panel;
        public SpriteRenderer[] infoImages;
        public TMP_Text[] infoTexts;
    }
    [SerializeField] tutItems[] tutorialElements;


    private void Awake()
    {
        if (instance == null)
            instance = this;

        HideEverything();

        //StartCoroutine(DoTutorial());
    }

    private void Start()
    {
        doTutorial = false;

        if (doTutorial)
        {
            duringTutorial = true;

            // Empezar primera parte del tutorial
            ShowTutorialItems((tutPhases)0);

            // Joder vagon de agua
            TrainManager.Instance.currentWater = TrainManager.Instance.maxWater;
            TrainManager.Instance.waterSlider.value = TrainManager.Instance.maxWater;
            TrainManager.Instance.ColorWater();

            // Joder Torreta
            Turret turret = FindObjectOfType<Turret>();
            turret.currentAmmo = 0;
            turret.UpdateAmmoSlider();
        }
        else
        {
            EndTutorial();
        }
    }

    void HideEverything()
    {
        foreach (var tutElement in tutorialElements)
        {
            Color panelColor = tutElement.panel.color;
            tutElement.panel.color = new Color(panelColor.r, panelColor.g, panelColor.b, 0);

            // Hide texts
            foreach (TMP_Text item in tutElement.infoTexts)
            {
                Color itemColor = item.color;
                item.color = new Color(itemColor.r, itemColor.g, itemColor.b, 0);
            }

            // Hide info Images
            foreach (SpriteRenderer item in tutElement.infoImages)
            {
                Color itemColor = item.color;
                item.color = new Color(itemColor.r, itemColor.g, itemColor.b, 0);
            }
        }
    }

    private void Update()
    {
    }

    public tutPhases GetCurrentPhase()
    { return currentPhase; }

    // Este metodo se llama cuando se realiza alguna de las acciones necesarias para completar el tutorial
    public void TryToChangePhase(tutPhases phaseDone)
    {
        if (doTutorial)
            StartCoroutine(TryToChangePhase_IEnumerator(phaseDone));
    }

    // Devuelve true si se esta pasando de fase en este momento
    bool changingPhase;
    IEnumerator TryToChangePhase_IEnumerator(tutPhases phaseDone)
    {
        yield return new WaitUntil(() => !changingPhase);

        if (currentPhase == phaseDone)
        {
            //Encontrar las referencias de los items de la parte del tutorial actual
            HideTutorialItems(currentPhase);

            currentPhase++;

            changingPhase = true;
            yield return new WaitForSeconds(1);
            changingPhase = false;


            if (currentPhase == tutPhases.matarEnemigoTorreta)
                FindObjectOfType<EnemySpawner>().AddEnemy(1).transform.position = new Vector3(4, 10, 0);

            //STOP
            if (currentPhase == tutPhases.trenEnMarcha)
            {
                duringTutorial = false;
                EndTutorial();
                yield break;
            }

            //Si no es el final, mostrar el siguiente
            ShowTutorialItems(currentPhase);
        }
    }

    void ShowTutorialItems(tutPhases tutPhase)
    {
        if (changingPhase) return;

        // Encontrar las referencias de los items de la parte del tutorial actual
        tutItems tutItem = Array.Find(tutorialElements, tutItem => tutItem.phase == tutPhase);
        StartCoroutine(ShowTutorialItems(tutItem));
    }
    IEnumerator ShowTutorialItems(tutItems phaseItem)
    {
        // Tiempo que tarda en aparecer el panel
        float panelShowTime = 1;

        // Tiempo que tarda en aparecer la informacion del panel (texxtos e imagenes)
        float infoPanelShowTime = .5f;


        // PANEL

        // Fade
        SpriteRenderer panel = phaseItem.panel;
        panel.DOFade(1, panelShowTime);

        // Position
        Vector3 panelPosition = panel.transform.position;
        phaseItem.panel.transform.position =
            new Vector3(panelPosition.x, panelPosition.y - 1);
        phaseItem.panel.transform.DOMoveY(panelPosition.y, panelShowTime);


        yield return new WaitForSeconds(panelShowTime);

        if (!changingPhase)
        {
            // Info in the Panel
            SpriteRenderer[] infoImages = phaseItem.infoImages;
            TMP_Text[] infoTexts = phaseItem.infoTexts;

            foreach (SpriteRenderer infoImage in infoImages)
                infoImage.DOFade(1, infoPanelShowTime);

            foreach (TMP_Text infoText in infoTexts)
                infoText.DOFade(1, infoPanelShowTime);
        }
    }

    void HideTutorialItems(tutPhases tutPhase)
    {
        if (changingPhase) return;

        // Encontrar las referencias de los items de la parte del tutorial actual
        tutItems tutItem = Array.Find(tutorialElements, tutItem => tutItem.phase == tutPhase);
        StartCoroutine(HideTutorialItems(tutItem));
    }
    IEnumerator HideTutorialItems(tutItems phaseItem)
    {
        // Tiempo que tarda en aparecer el panel
        float panelHideTime = 1;

        // Tiempo que tarda en aparecer la informacion del panel (texxtos e imagenes)
        float infoHideTime = .5f;


        // INFO IN THE PANEL
        SpriteRenderer[] infoImages = phaseItem.infoImages;
        TMP_Text[] infoTexts = phaseItem.infoTexts;

        foreach (SpriteRenderer infoImage in infoImages)
            infoImage.DOFade(0, infoHideTime);

        foreach (TMP_Text infoText in infoTexts)
            infoText.DOFade(0, infoHideTime);



        yield return new WaitForSeconds(infoHideTime);

        // PANEL

        // Fade
        SpriteRenderer panel = phaseItem.panel;
        panel.DOFade(0, panelHideTime);

        // Position
        Vector3 panelPosition = panel.transform.position;
        phaseItem.panel.transform.DOMoveY(panelPosition.y - 1, panelHideTime);
    }



    void EndTutorial()
    {
        FindObjectOfType<EnemySpawner>().CreateEnemys();
    }
}
