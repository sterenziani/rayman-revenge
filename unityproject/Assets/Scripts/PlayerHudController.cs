using UnityEngine;

public class PlayerHudController : MonoBehaviour
{
    [SerializeField] HealthBar healthBar;
    [SerializeField] PowerupBar powerupBar;

    public void SetTrackingVulnerable(Vulnerable vulnerable)
    {
        healthBar.SetTrackingVulnerable(vulnerable);
    }

    public void SetTrackingVulnerable(Player player)
    {
        healthBar.SetTrackingVulnerable(player);
        powerupBar?.SetTrackedPlayer(player);
    }
}
