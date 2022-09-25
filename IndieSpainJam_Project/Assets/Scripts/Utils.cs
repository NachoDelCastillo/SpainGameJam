using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Utils : MonoBehaviour
{
    static AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    static public IEnumerator MoveItemSmooth(Transform item, Transform finalSpot, float seconds)
    {
        Vector3 initialPosition = item.position;

        float c = 0;
        while (item != null && Vector3.Distance(finalSpot.position, item.position) > 0.001f)
        {
            item.transform.position = Vector3.Lerp(initialPosition, finalSpot.position, animationCurve.Evaluate(c));
            //item.transform.position = Vector3.Lerp(initialPosition, assembleSpot.position, c);
            c += (Time.deltaTime / seconds);
            c = Mathf.Clamp(c, 0, 1);
            yield return new WaitForSeconds(0);
        }
    }

    static public IEnumerator WriteThis(string fulltext, TMP_Text text_, float secondsBetweenLetters)
    {
        text_.text = "";
        int i = 0;
        foreach (char s in fulltext)
        {
            text_.text += s;
            i++;

            yield return new WaitForSeconds(secondsBetweenLetters);
        }
    }
}
