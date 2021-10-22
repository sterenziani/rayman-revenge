using UnityEngine;
public class PowerUpObject : MonoBehaviour
{
    [SerializeField] PowerUpsEnum PowerUp;
    [SerializeField] Material Material;
    [SerializeField] float Duration;

    private void OnTriggerStay(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.ApplyPowerUp(PowerUp, Duration, Material);
        }
    }
}
