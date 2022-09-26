using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aaa : MonoBehaviour
{
    ParticleSystem sys;

    private void Start()
    {
        sys = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        float speed = TrainManager.Instance.GetmainVelocity();

        ParticleSystem.MinMaxCurve a = new();
        a = sys.velocityOverLifetime.x;
        a.curveMultiplier = 0.5f + (((float)speed / 100f) * 30f);
        var aux2 = sys.velocityOverLifetime;
        aux2.x = a;
    }
}
