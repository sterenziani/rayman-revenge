using System.Collections.Generic;
using UnityEngine;

public class StickyPlatform : MonoBehaviour
{
    private Dictionary<GameObject, Vector3> targetsAndOffsets = new Dictionary<GameObject, Vector3>();

    private void OnCollisionStay(Collision collision)
    {
        GameObject target = collision.gameObject;
        Vector3 offset = target.transform.position - transform.position;

        targetsAndOffsets[target] = offset;
    }

    private void OnCollisionExit(Collision collision)
    {
        GameObject target = collision.gameObject;
        targetsAndOffsets.Remove(target);
    }

    void LateUpdate()
    {
        foreach (GameObject target in targetsAndOffsets.Keys)
        {
            target.transform.position = transform.position + targetsAndOffsets[target];
        }
    }
}
