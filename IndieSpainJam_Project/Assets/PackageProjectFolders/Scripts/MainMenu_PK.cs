using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainMenu_PK : MenuManager_PK
{

    AllMenuManager_PK allMenuManager;

    protected override void ExtraAwake()
    {
        allMenuManager = GetComponentInParent<AllMenuManager_PK>();
    }

    protected override void buttonPressed(int index)
    {
        base.buttonPressed(index);

        //if (index == 0) allMenuManager.PressPlay();
        if (index == 0) allMenuManager.PressPlay();
        else if (index == 1) allMenuManager.PressSettings();
        else if (index == 2) allMenuManager.PressControls();
        else if (index == 3) allMenuManager.PressCredits();
        else if (index == 4) PressQuit();
    }

    void PressQuit()
    {
        Application.Quit();
    }
}
