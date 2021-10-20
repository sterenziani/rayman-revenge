using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    [SerializeField] float degreesPerSecondX = 0;
    [SerializeField] float degreesPerSecondY = 25;
    [SerializeField] float degreesPerSecondZ = 0;

    void Update()
    {
        transform.Rotate(degreesPerSecondX * Time.deltaTime, degreesPerSecondY * Time.deltaTime, degreesPerSecondZ * Time.deltaTime);
    }
}
