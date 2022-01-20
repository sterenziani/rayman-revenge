using UnityEngine;
using UnityEngine.UI;

public class PowerupBar : MonoBehaviour
{
    public Image powerupBarFill;
    public Player trackedPlayer;

    private void Start()
    {
        if (trackedPlayer == null)
        {
            gameObject.SetActive(false);
            return;
        }
    }

    private void Update()
    {
        if (trackedPlayer == null || trackedPlayer.gameObject == null || trackedPlayer.LifePoints <= 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            float percentage = trackedPlayer.GetRemainingPowerUpPercentage();
            powerupBarFill.fillAmount = percentage;
        }
    }

    public void SetTrackedPlayer(Player player)
    {
        trackedPlayer = player;
        gameObject.SetActive(true);
    }
}
