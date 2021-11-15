using UnityEngine;

[RequireComponent(typeof(MeshExploder)), RequireComponent(typeof(Resizable))]
public class Balloon : MonoBehaviour
{
    [SerializeField] float bouncingForce = 600;
    [SerializeField] float respawnSeconds = 3;

    [SerializeField] AudioClip popSound;
    private AudioSource audioSource;

    private Resizable resizable;
    private MeshExploder exploder;
    private Vector3 initialScale;

    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rigidBody = collision.transform.GetComponent<Rigidbody>();
        if (rigidBody != null)
        {
            rigidBody.velocity = Vector3.zero;
            rigidBody.AddForce(Vector3.up * bouncingForce);

            if(audioSource != null && popSound != null)
            {
                audioSource.PlayOneShot(popSound);
                exploder.Explode();

                if (respawnSeconds == float.PositiveInfinity)
                {
                    Destroy(gameObject);
                }
                else
                {
                    transform.localScale = Vector3.zero;
                    Invoke(nameof(Respawn), respawnSeconds);
                }
            }

            return;

            
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        exploder = GetComponent<MeshExploder>();
        resizable = GetComponent<Resizable>();

        initialScale = transform.localScale;
    }

    private void Respawn()
    {
        resizable.ScaleOverTime(initialScale, 1.2f);
        //gameObject.SetActive(true);
    }
}
