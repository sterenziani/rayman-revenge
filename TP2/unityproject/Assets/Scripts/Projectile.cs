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

    [SerializeField] bool explodeOnTouch = false;
    [SerializeField] ParticleSystem explosion;
    [SerializeField] float explosionDamage = 0;
    [SerializeField] float explosionRange = 0;
    [SerializeField] float explosionForce = 0;
    [SerializeField] bool onlyCollideWithPlayer = false;

    private Rigidbody rigidBody;
    private Vector3 startPosition;

    private int collisionCount = 0;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        startPosition = transform.position;
        rigidBody = GetComponent<Rigidbody>();

        rigidBody.useGravity = useGravity;

        PhysicMaterial physics_mat = new PhysicMaterial();
        physics_mat.bounciness = bounciness;
        physics_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        physics_mat.bounceCombine = PhysicMaterialCombine.Maximum;
        GetComponent<Collider>().material = physics_mat;
    }

    private void Explode()
    {
        //GameObject grafico explision
        if (explosion != null)
        {
            GameObject expInstance = Instantiate(explosion.gameObject, transform.position, Quaternion.identity);
            expInstance.SetActive(true);
            ParticleSystem particleSystem = expInstance.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();
            }
        }

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

        //Para estar seguros de que llegamos a hacer todo
        Invoke(nameof(DestroyObject), 0.05f);
    }
    private void DestroyObject()
    {
        Destroy(gameObject);
    }

    protected virtual void Update()
    {
        if (collisionCount > maxCollisions) Explode();

        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0) Explode();

        if (Vector3.Distance(startPosition, transform.position) > maxRange)
        {
            Explode();
        }
    }

    protected virtual private void OnCollisionEnter(Collision collision)
    {
        collisionCount++;

        if (onlyCollideWithPlayer && !collision.gameObject.GetComponent<Player>())
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
