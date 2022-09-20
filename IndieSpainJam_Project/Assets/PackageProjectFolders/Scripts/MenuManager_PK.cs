using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager_PK : MonoBehaviour
{
    [Header("MENU MANAGER SETUP")]
    [SerializeField]Transform buttonGroup; // Todos los botones son hijos de esto

    // VARIABLES INTERNAS
    protected int nButtons;
    protected Button_PK[] buttons;

    // Valor del index del boton que esta activo, -1 si ninguno esta seleccionado
    [HideInInspector] public int buttonSelected = -1;

    // Devuelve true si el menu acepta un cambio de boton con el mando
    // Se usa para que haya un delay entre botones
    // Tambien lo cambia el pauseMenu a false cuando esta no esta pausado durante el juego
    protected bool canUseControllerSelection = true;

    // Devuelve true si actualmente el raton esta sobre el boton seleccionado
    bool mouseOverSelectedButton;

    void Awake()
    {
        // Almacenar botones
        nButtons = buttonGroup.childCount;
        buttons = new Button_PK[nButtons];
        for (int i = 0; i < nButtons; i++)
        {
            buttons[i] = buttonGroup.GetChild(i).GetComponent<Button_PK>();
            buttons[i].SetIndex(i);
        }

        ExtraAwake();
    }
    protected virtual void ExtraAwake() { }

    private void Update()
    {
        if (GameManager.GetInstance().duringTransition) return;

        // Comprobar si se ha pulsado el boton seleccionado (ya sea con raton o mando)
        pressButton_Input();

        // Comprobar si se ha seleccionado un boton con un mando
        controller_Input();

        ExtraUpdate();
    }
    protected virtual void ExtraUpdate() { }


    void pressButton_Input()
    {
        if (Input.GetButtonDown("Action")
            || (Input.GetMouseButtonDown(0) && mouseOverSelectedButton))
        {
            buttonPressed(buttonSelected);

            // Sound
            bool canSound = false;

            PauseMenu_PK posiblePauseMenu = GetComponent<PauseMenu_PK>();

            if (posiblePauseMenu != null && posiblePauseMenu.paused) return;

            if (buttonSelected == -1) return;

            if (!buttons[buttonSelected].GetComponent<AudioSlider_PK>())
            {
                if (posiblePauseMenu == null)
                    canSound = true;
                else if (posiblePauseMenu.paused)
                    canSound = true;
            }

            if (canSound)
                AudioManager_PK.GetInstance().Play("ButtonPress", 1);
        }
    }
    protected virtual void buttonPressed(int index) { }


    #region Select/Unselect buttons
    void controller_Input()
    {
        // Comprobar si se ha terminado el delay
        if (canUseControllerSelection)
        {
            // Actualizar variable del input
            int axis = 0;
            if (Input.GetAxisRaw("Vertical") > .3f) axis = 1;
            else if (Input.GetAxisRaw("Vertical") < -.3f) axis = -1;

            // Si se ha presionado arriba o abajo
            if (axis != 0)
            {
                // Asegurarse de que este codigo no se pueda volver a ejecutar en X tiempo
                canUseControllerSelection = false;
                StartCoroutine(CanUseControllerSelection());

                // Deseleccionar boton actual, si hay alguno
                if (buttonSelected != -1)
                    UnselectButton(buttonSelected);
                else
                {
                    // Si no hay ningun boton seleccionado, seleccionar el primero
                    ChangeSelectButton(0); return;
                }

                if (axis < 0) // Abajo
                {
                    // Calcular nuevo boton seleccionado
                    buttonSelected++;
                    if (buttonSelected >= nButtons) // Limitador
                        buttonSelected = 0;

                    ChangeSelectButton(buttonSelected);
                }
                else // Arriba
                {
                    // Calcular nuevo boton seleccionado
                    buttonSelected--;
                    if (buttonSelected <= -1) // Limitador
                        buttonSelected = buttons.Length - 1;

                    ChangeSelectButton(buttonSelected);
                }
            }
        }
    }
    IEnumerator CanUseControllerSelection()
    {
        yield return new WaitForSecondsRealtime(.25f);
        canUseControllerSelection = true;
    }

    // Esta funcion se llama desde cada boton al ser seleccionado para informar al menuManager
    public void ChangeSelectButton(int buttonIndex)
    {
        // Deseleccionar el boton anterior
        if (buttonSelected != -1)
            UnselectButton(buttonSelected);

        buttonSelected = buttonIndex;

        // Seleccionar el nuevo boton, solo si no se han deseleccionado todos los botones
        if (buttonSelected != -1)
            SelectButton(buttonSelected);
    }

    void SelectButton(int index)
    { buttons[index].SelectThis(); }

    void UnselectButton(int index)
    { buttons[index].UnselectThis(); }

    // Esta funcion se llama desde cada boton cuando el raton esta sobre ellos
    public void MouseOverSelectedButton(bool value) 
    { mouseOverSelectedButton = value; }

    private void OnEnable()
    {
        for (int i = 0; i < nButtons; i++)
            buttons[i].enabled = true;
    }

    private void OnDisable()
    {
        for (int i = 0; i < nButtons; i++)
            buttons[i].enabled = false;
    }

    #endregion
}
