using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    [SerializeField] float hitDamage = 0;
    [SerializeField] bool useGravity = false;

    [Range(0f, 1f)] [SerializeField] float bounciness = 0;

    public float maxRange = float.PositiveInfinity;
    [SerializeField] int maxCollisions = int.MaxValue;
    [SerializeField] float maxLifetime = float.PositiveInfinity;
    private float remainingSafeTime = 1f;

    [SerializeField] bool explodeOnTouch = false;
    [SerializeField] ParticleSystem explosionEffect;
    [SerializeField] float explosionDamage = 0;
    [SerializeField] float explosionRange = 0;
    [SerializeField] float explosionForce = 0;
    [SerializeField] bool onlyCollideWithPlayer = false;

    private AudioSource audioSource;
    [SerializeField] AudioClip collideSound;
    [SerializeField] AudioClip explosionSound;

    private Rigidbody rigidBody;
    private Vector3 startPosition;

    private bool exploding = false;

    private int collisionCount = 0;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        startPosition = transform.position;
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        rigidBody.useGravity = useGravity;

        PhysicMaterial physics_mat = new PhysicMaterial();
        physics_mat.bounciness = bounciness;
        physics_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        physics_mat.bounceCombine = PhysicMaterialCombine.Maximum;
        GetComponent<Collider>().material = physics_mat;
    }

    private void Explode()
    {
        if (exploding)
            return;

        exploding = true;

        //GameObject grafico explision
        if (explosionEffect != null)
        {
            GameObject expInstance = Instantiate(explosionEffect.gameObject, transform.position, Quaternion.identity);
            expInstance.SetActive(true);
            ParticleSystem particleSystem = expInstance.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();
            }
        }

        transform.localScale = Vector3.zero;

        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange);
        for (int i = 0; i < enemies.Length; i++)
        {
            //Daño
            Vulnerable vulnerable = enemies[i].GetComponent<Vulnerable>();
            if(vulnerable != null)
            {
                vulnerable.TakeDamage(explosionDamage);
            }

            //Fuerza de la explosion (si objetivo tiene rigidBody)
            if (enemies[i].GetComponent<Rigidbody>())
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRange);
        }

        float timeToExplode = 0.05f;
        if(audioSource != null && explosionSound != null)
        {
            audioSource.clip = explosionSound;
            audioSource.Play();
            timeToExplode = explosionSound.length;
        }

        //Para estar seguros de que llegamos a hacer todo
        Invoke(nameof(DestroyObject), timeToExplode);
    }
    private void DestroyObject()
    {
        Destroy(gameObject);
    }

    protected virtual void Update()
    {
        if (collisionCount > maxCollisions)
            Explode();

        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0)
            Explode();

        if (Vector3.Distance(startPosition, transform.position) > maxRange)
            Explode();

        remainingSafeTime -= Time.deltaTime;
    }

    protected virtual private void OnCollisionEnter(Collision collision)
    {
        if (exploding)
            return;

        collisionCount++;

        if(audioSource != null && collideSound != null)
        {
            audioSource.PlayOneShot(collideSound);
        }

        if (onlyCollideWithPlayer && !collision.gameObject.GetComponent<Player>())
            return;
        if (remainingSafeTime > 0 && collision.gameObject.GetComponent<Player>() != null)
            return;

        Vulnerable vulnerable = collision.gameObject.GetComponent<Vulnerable>();

        if(vulnerable != null && vulnerable.MinDamageToTake <= hitDamage)
        {
            vulnerable.TakeDamage(hitDamage);

            if(explodeOnTouch)
            {
                Explode();
            }
        }
    }
}
