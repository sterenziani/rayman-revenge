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
        icon.color = trackingVulnerable.overlayColor;
    }

    private void Update()
    {
        if(trackingVulnerable == null || trackingVulnerable.gameObject == null || trackingVulnerable.LifePoints <= 0)
        {
            gameObject.SetActive(false);
        } 
        else
        {
            gameObject.SetActive(true);
            slider.value = trackingVulnerable.LifePoints;
        }
    }

    public void SetTrackingVulnerable(Vulnerable vulnerable)
    {
        trackingVulnerable = vulnerable;
        gameObject.SetActive(true);
    } 
}
