using System;
using System.Collections;
using UnityEngine;

public class Resizable : MonoBehaviour
{
    private bool isScaling = false;

    public void ScaleOverTime(Vector3 toScale, float duration, Action callback = null)
    {
        StartCoroutine(scaleOverTime(transform, toScale, duration, callback));
    }

    private IEnumerator scaleOverTime(Transform objectToScale, Vector3 toScale, float duration, Action then = null)
    {
        //Make sure there is only one instance of this function running
        if (isScaling)
        {
            yield break; ///exit if this is still running
        }
        isScaling = true;

        float counter = 0;

        //Get the current scale of the object to be moved
        Vector3 startScaleSize = objectToScale.localScale;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            objectToScale.localScale = Vector3.Lerp(startScaleSize, toScale, counter / duration);

            /*gameObject.GetComponent<MeshRenderer>().material.color = new Color(
                gameObject.GetComponent<MeshRenderer>().material.color.r,
                gameObject.GetComponent<MeshRenderer>().material.color.g,
                gameObject.GetComponent<MeshRenderer>().material.color.b,
                counter / duration);*/

            yield return null;
        }

        isScaling = false;

        then?.Invoke();
    }
}
