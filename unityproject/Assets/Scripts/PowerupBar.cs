using UnityEngine;
using UnityEngine.UI;

public class PowerupBar : MonoBehaviour
{
    public Image powerupBarFill;
    public Player trackedPlayer;

    private void Start()
    {

    }

    private void Update()
    {
        float percentage = trackedPlayer.GetRemainingPowerUpPercentage();
        powerupBarFill.fillAmount = percentage;
    }
}
