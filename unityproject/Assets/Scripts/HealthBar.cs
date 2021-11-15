using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Vulnerable trackingVulnerable;
    [SerializeField] Image icon;
    private void Start()
    {
        if(trackingVulnerable == null)
        {
            gameObject.SetActive(false);
            return;
        }

        slider.maxValue = trackingVulnerable.LifePointsTotal;
        slider.value = trackingVulnerable.LifePointsTotal;
        icon.sprite = trackingVulnerable.sprite;
    }

    private void Update()
    {
        if(trackingVulnerable == null || trackingVulnerable.gameObject == null || trackingVulnerable.LifePoints <= 0)
            Destroy(gameObject);
        else
            slider.value = trackingVulnerable.LifePoints;
    }
}
