using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class Button_PK : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("SET UP")]
    [SerializeField] protected TMP_Text thisText;
    Transform thisText_Transform; float thisText_InitScale;
    protected MenuManager_PK menuManager;

    // VARIABLES INTERNAS
    protected int buttonIndex;
    float buttonScaleVel = .2f;

    void Awake()
    {
        // SET UP
        // ThisText
        thisText_Transform = thisText.GetComponent<Transform>();
        thisText_InitScale = thisText_Transform.localScale.x;
        // MenuManager
        menuManager = GetComponentInParent<MenuManager_PK>();

        ExtraAwake();
    }
    protected virtual void ExtraAwake() { }

    // Este metodo se llama desde el menuManager de este boton al principio
    // del juego y le asigna su index correspondiente
    public void SetIndex(int index)
    { buttonIndex = index; }

    #region PointerEvents
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Informar al menuManager de que se ha cambiado el boton seleccionado
        menuManager.ChangeSelectButton(buttonIndex);

        menuManager.MouseOverSelectedButton(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Informar al menuManager que no hay ningun boton seleccionado
        menuManager.ChangeSelectButton(-1);

        menuManager.MouseOverSelectedButton(false);
    }

    #endregion

    #region Selection

    // Se llama desde menuManager
    public void SelectThis()
    {
        if (GameManager.GetInstance().duringTransition)
            return;

        // Sound
        AudioManager_PK.GetInstance().Play("ButtonOnselect", 1);

        // Animation
        thisText.GetComponent<Transform>().DOScale(Vector3.one * thisText_InitScale * 1.3f, buttonScaleVel).SetUpdate(true);
    }

    // Se llama desde menuManager
    public void UnselectThis()
    {
        // Animation
        thisText.GetComponent<Transform>().DOScale(Vector3.one * thisText_InitScale, buttonScaleVel).SetUpdate(true);
    }

    #endregion
}
