using UnityEngine;

public class StickyPlatform : MonoBehaviour
{
    private GameObject target = null;
    private Vector3 offset;

    void Start()
    {
        target = null;
    }

    private void OnCollisionStay(Collision collision)
    {
        target = collision.gameObject;
        offset = target.transform.position - transform.position;
    }

    private void OnCollisionExit(Collision collision)
    {
        target = null;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            target.transform.position = transform.position + offset;
        }
    }
}
