using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Murfy : MonoBehaviour
{
    [SerializeField] Transform following;

    void Update()
    {
        transform.LookAt(following);
    }
}
