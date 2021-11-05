using UnityEngine;
using System.Collections;

public class Vulnerable : MonoBehaviour
{
    public float LifePointsTotal = 10;
    public float LifePoints { get; protected set; }
    public float MinDamageToTake = 0;
    private MeshExploder exploder;
    private AreaSpawner spawner;
    protected new Collider collider;
    private Vector3 spawnPosition;

    protected AudioSource audioSource;
    [SerializeField] AudioClip spawnSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip hitSound;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        exploder = GetComponent<MeshExploder>();
        collider = GetComponent<Collider>();
        spawner = GetComponent<AreaSpawner>();
        spawnPosition = transform.position;
        LifePoints = LifePointsTotal;

        audioSource = GetComponent<AudioSource>();
        if (audioSource != null && spawnSound != null)
        {
            audioSource.PlayOneShot(spawnSound);
        }
    }

    protected virtual void Update()
    {
        if (IsAlive() && transform.position.y < -50)
            Die();
    }

    public bool IsAlive()
    {
        return LifePoints > 0;
    }

    public virtual float Cure(float points)
    {
        LifePoints = Mathf.Min(LifePoints + points, LifePointsTotal);
        return LifePoints;
    }

    public virtual float TakeDamage(float damage)
    {
        return TakeDamage(damage, true);
    }

    public virtual float TakeDamage(float damage, bool flinch = true)
    {
        LifePoints -= damage >= MinDamageToTake ? damage : 0;
        if (LifePoints <= 0)
        {
            Die();
        }
        else
        {
            if (audioSource != null && hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }

            if (flinch)
            {
                Player p = this.GetComponent<Player>();
                if (p != null)
                p.GetHurt();
            }
        }
        return LifePoints;
    }

    protected virtual void Die()
    {
        float timeToDestroy = 0f;
        if(audioSource != null && deathSound != null)
        {
            timeToDestroy = deathSound.length;
            audioSource.PlayOneShot(deathSound);
        }

        if(exploder != null)
        {
            exploder.Explode();
        }

        gameObject.transform.localScale = Vector3.zero;

        LifePoints = 0;
        StartCoroutine(SpawnLoot(0.2f));
        Invoke(nameof(DestroyObject), timeToDestroy);
    }

    bool IsGrounded()
    {
        int layers = LayerMask.GetMask("Ground");
        float distToGround = collider.bounds.extents.y;
        return Physics.Raycast(transform.position, Vector3.down, distToGround + 0.1f, layers);
    }

    public IEnumerator SpawnLoot(float delay)
    {
        if (IsGrounded())
        {
            yield return new WaitForSeconds(delay);
            if(spawner != null)
                spawner.BeginSpawning(0, 1, transform.position);
        }
        else
        {
            if (spawner != null)
                spawner.BeginSpawning(0, 1, spawnPosition);
        }
    }

    protected void DestroyObject()
    {
        Destroy(gameObject);
    }

    public void Step()
    {
        if(collider != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, collider.bounds.size.y))
            {
                Platform platform = hit.collider.gameObject.GetComponentInParent<Platform>();
                if(platform != null)
                {
                    platform.StepOn();
                }
            }
        }
    }


}
