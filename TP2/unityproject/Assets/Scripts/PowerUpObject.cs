using UnityEngine;
public class PowerUpObject : MonoBehaviour
{
    [SerializeField] PowerUpsEnum PowerUp;
    [SerializeField] Material Material;
    [SerializeField] float Duration;

    private AudioSource audioSource;
    [SerializeField] AudioClip rechargeSound;

    private void OnTriggerStay(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.ApplyPowerUp(PowerUp, Duration, Material);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            if (audioSource != null && rechargeSound != null)
            {
                audioSource.PlayOneShot(rechargeSound);
            }
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
}
