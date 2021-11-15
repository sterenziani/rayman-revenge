using UnityEngine;

public class PointCollectable : MonoBehaviour
{
    [SerializeField] int points;
    [SerializeField] AudioClip audioClip;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if(player != null)
        {
            player.Cure(points);

            if (audioSource != null && audioClip != null)
            {
                audioSource.PlayOneShot(audioClip);

                gameObject.transform.localScale = new Vector3(0, 0, 0);
                Component[] lights = gameObject.GetComponentsInChildren<Light>();
                if(lights != null)
                {
                    foreach(Light light in lights)
                        light.enabled = false;
                }
                gameObject.GetComponent<Collider>().enabled = false;

                Invoke(nameof(Destroy), audioClip.length);
            } 
            else
            {
                Destroy();
            }

        }
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
