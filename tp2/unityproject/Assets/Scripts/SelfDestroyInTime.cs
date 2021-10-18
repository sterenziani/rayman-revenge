using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroyInTime : MonoBehaviour
{
    [SerializeField] float lifeTime;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
