using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Vulnerable trackingVulnerable;

    private void Start()
    {
        slider.maxValue = trackingVulnerable.LifePointsTotal;
        slider.value = trackingVulnerable.LifePointsTotal;
    }

    private void Update()
    {
        slider.value = trackingVulnerable.LifePoints;
    }
}
