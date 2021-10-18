using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float damage = 5;
    public float range;
    private Vector3 startPosition;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(startPosition, transform.position) > range)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vulnerable vulnerable = collision.gameObject.GetComponent<Vulnerable>();
        if(vulnerable != null)
        {
            vulnerable.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
