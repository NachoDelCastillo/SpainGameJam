using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeRail : MonoBehaviour
{
    // Si esta a true puede moverse a esa posicion
    bool[] railWays;

    [SerializeField] Transform[] railGfx;

    // Lista con los vagones que ya han usado este cambio de via
    [HideInInspector] public List<WagonLogic> wagonsThatAlreadyUsedThis;

    private void Awake()
    {
        for (int i = 0; i < 3; i++)
            railGfx[i].gameObject.SetActive(false);
    }

    private void Update()
    {
        transform.Translate(new Vector3(-10 * Time.deltaTime, 0));
    }

    public void SetRailWays(bool[] railWays_)
    {
        railWays = railWays_;

        for (int i = 0; i < 3; i++)
            railGfx[i].gameObject.SetActive(railWays[i]);
    }

    public int[] GetPossibleRailWays()
    {
        List<int> possibleRailWays = new List<int>();

        for (int i = 0; i < railWays.Length; i++)
            if (railWays[i])
                possibleRailWays.Add(i);

        return possibleRailWays.ToArray();
    }
}