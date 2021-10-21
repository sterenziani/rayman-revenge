using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    public float degreesPerSecondX = 0;
    public float degreesPerSecondY = 25;
    public float degreesPerSecondZ = 0;

    void Update()
    {
        transform.Rotate(degreesPerSecondX * Time.deltaTime, degreesPerSecondY * Time.deltaTime, degreesPerSecondZ * Time.deltaTime);
    }
}
