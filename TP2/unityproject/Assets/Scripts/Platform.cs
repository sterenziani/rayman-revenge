using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Platform : MonoBehaviour
{
    [SerializeField] AudioClip stepSound;
    [SerializeField] AudioClip enterSound;
    [SerializeField] AudioClip leaveSound;
    private AudioSource audioSource;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void StepOn()
    {
        if(audioSource != null && stepSound != null)
        {
            audioSource.PlayOneShot(stepSound);
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        OnTriggerEnter(collision.collider);
    }

    protected virtual void OnCollisionExit(Collision collision)
    {
        OnTriggerExit(collision.collider);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (audioSource != null && enterSound != null)
        {
            audioSource.PlayOneShot(enterSound);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (audioSource != null && leaveSound != null)
        {
            audioSource.PlayOneShot(leaveSound);
        }
    }
}
