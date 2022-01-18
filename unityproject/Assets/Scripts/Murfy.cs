using UnityEngine;

public class Murfy : MonoBehaviour
{
    [SerializeField] Transform following;

    void Update()
    {
        transform.LookAt(following);
    }
}
