using System.Collections;
using UnityEngine;
public class PowerUpObject : MonoBehaviour
{
    [SerializeField] PowerUpsEnum PowerUp;
    [SerializeField] Material Material;
    [SerializeField] float Duration;
    [SerializeField] LineRenderer LaserLineRenderer;
    [SerializeField] float LaserMaxSize = 1f;
    [SerializeField] float LaserAnimationTime = 0.7f;

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
            if(LaserLineRenderer != null)
            {
                StopAllCoroutines();
                StartCoroutine(GrowLaserCoroutine(LaserAnimationTime, LaserMaxSize));
            }

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

    private IEnumerator GrowLaserCoroutine(float timeTakes, float laserMaxSize)
    {
        float elapsedTime = 0;
        Vector3 startPoint = new Vector3(0f, 0f, 0f);
        Vector3 endPoint = new Vector3(0f, laserMaxSize, 0f);

        LaserLineRenderer.gameObject.SetActive(true);
        LaserLineRenderer.SetPosition(1, startPoint);

        while (elapsedTime < timeTakes)
        {
            LaserLineRenderer.SetPosition(0, Vector3.Lerp(startPoint, endPoint, (elapsedTime / timeTakes)));

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1);

        LaserLineRenderer.gameObject.SetActive(false);
    }
}
