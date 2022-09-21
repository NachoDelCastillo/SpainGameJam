using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeRail : MonoBehaviour
{
    bool[] railWays;

    [SerializeField] Transform[] railGfx;

    private void Awake()
    {
        for (int i = 0; i < 3; i++)
            railGfx[i].gameObject.SetActive(false);
    }

    public void SetRailWays(bool[] railWays_)
    {
        railWays = railWays_;


        for (int i = 0; i < 3; i++)
            railGfx[i].gameObject.SetActive(true);
    }
}
