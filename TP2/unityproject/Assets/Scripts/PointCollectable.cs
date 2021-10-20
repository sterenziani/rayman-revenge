using UnityEngine;

public class PointCollectable : MonoBehaviour
{
    [SerializeField] int points;

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if(player != null)
        {
            player.Cure(points);
            Destroy(gameObject);
        }
    }
}
