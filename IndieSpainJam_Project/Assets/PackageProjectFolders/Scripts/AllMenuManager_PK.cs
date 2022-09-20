using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AllMenuManager_PK : MonoBehaviour
{
    [Header("SETUP")]
    [SerializeField] Transform cameraObj;

    [SerializeField] float cameraSpeed;

    float cameraDistanceX = 25;
    float cameraDistanceY = 15;


    // Referencias a todos los menus
    MainMenu_PK mainMenu;
    SettingsMenu_PK settingsMenu;
    LevelselectorMenu_PK levelSelectorMenu;

    private void Awake()
    {
        mainMenu = FindObjectOfType<MainMenu_PK>();
        settingsMenu = FindObjectOfType<SettingsMenu_PK>();
        levelSelectorMenu = FindObjectOfType<LevelselectorMenu_PK>();

        mainMenu.enabled = true;
        settingsMenu.enabled = false;
        levelSelectorMenu.enabled = false;
    }


    private void Update()
    {
        if (Input.anyKeyDown && Mathf.Abs(cameraObj.position.x) == cameraDistanceX) 
        {
            // Sound
            AudioManager_PK.GetInstance().Play("ButtonPress", 1);

            StartCoroutine(EnableMenu(mainMenu, true, cameraSpeed));
            cameraObj.DOMoveX(0, cameraSpeed);
        }
    }

    // Activa/Desactiva el script indicado dentro de Xsegundos
    // Esto se usa para activar los menus una vez la camara ha llegado
    IEnumerator EnableMenu(MonoBehaviour menuScript, bool value, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        menuScript.enabled = value;
    }

    #region buttonPress

    #region mainMenu Buttons
    public void PressPlay()
    {
        mainMenu.enabled = false;
        StartCoroutine(EnableMenu(levelSelectorMenu, true, cameraSpeed/2));

        cameraObj.DOMoveY(cameraDistanceY, cameraSpeed);
    }

    public void PressSettings()
    {
        mainMenu.enabled = false;
        StartCoroutine(EnableMenu(settingsMenu, true, cameraSpeed/2));

        cameraObj.DOMoveY(-cameraDistanceY, cameraSpeed);
    }

    public void PressControls()
    {
        mainMenu.enabled = false;

        cameraObj.DOMoveX(-cameraDistanceX, cameraSpeed);
    }

    public void PressCredits()
    {
        mainMenu.enabled = false;

        cameraObj.DOMoveX(cameraDistanceX, cameraSpeed);
    }

    #endregion

    #region Settings Buttons

    public void BackButton()
    {
        settingsMenu.enabled = false;
        levelSelectorMenu.enabled = false;
        StartCoroutine(EnableMenu(mainMenu, true, cameraSpeed/2));

        cameraObj.DOMoveY(0, cameraSpeed);
    }

    #endregion

    #endregion
}
