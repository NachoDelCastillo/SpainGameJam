using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeRail : MonoBehaviour
{
    [SerializeField] public int thisRow;

    // Si esta a true puede moverse a esa posicion
    [SerializeField] bool[] railWays;

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

    public void SetRailWays(int thisRow_, bool[] railWays_)
    {
        thisRow = thisRow_;

        railWays = railWays_;

        for (int i = 0; i < 3; i++)
            railGfx[i].gameObject.SetActive(railWays[i]);
    }

    public int[] GetPossibleRailWays()
    {
        List<int> possibleRailWays = new List<int>();

        for (int i = 0; i < railWays.Length; i++)
            if (railWays[i])
            {
                int numI = - 1;
                if (thisRow == 0)
                    numI = i - 1;
                else if (thisRow == 1)
                    numI = i;
                else if (thisRow == 2)
                    numI = i + 1;

                possibleRailWays.Add(numI);
            }

        return possibleRailWays.ToArray();
    }
}