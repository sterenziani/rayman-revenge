using UnityEngine;

public class Balloon : MonoBehaviour
{
    [SerializeField] float bouncingForce = 600;
    [SerializeField] float respawnSeconds = 3;

    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rigidBody = collision.transform.GetComponent<Rigidbody>();
        if (rigidBody != null)
        {
            gameObject.SetActive(false);

            rigidBody.AddForce(Vector3.up * bouncingForce);

            if (respawnSeconds == float.PositiveInfinity)
            {
                Destroy(gameObject);
            }
            else
            {
                Invoke(nameof(Respawn), respawnSeconds);
            }
        }
    }

    private void Respawn()
    {
        gameObject.SetActive(true);
    }
}
