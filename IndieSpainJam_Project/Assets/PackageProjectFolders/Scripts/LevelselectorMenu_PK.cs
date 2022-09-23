using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelselectorMenu_PK : MenuManager_PK
{
    AllMenuManager_PK allMenuManager;

    protected override void ExtraAwake()
    {
        allMenuManager = GetComponentInParent<AllMenuManager_PK>();
    }

    protected override void buttonPressed(int index)
    {
        base.buttonPressed(index);
        
        if (index == nButtons-1) allMenuManager.BackButton();

        else
        {
            // Informar al gamemanager del numero actual de jugadores
            GameManager.numPlayersPlaying = index + 1;

            GameManager.GetInstance().ChangeScene("Gameplay");
        }
    }

}
