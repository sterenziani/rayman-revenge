using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Vulnerable : MonoBehaviour
{
    public float LifePointsTotal = 10;
    public float LifePoints { get; protected set; }
    public float MinDamageToTake = 0;
    private MeshExploder exploder;
    private AreaSpawner spawner;
    protected new Collider collider;
    private Vector3 spawnPosition;

    public string Name;
    public Sprite sprite;

    protected bool ControlledByCinematic = false;

    protected AudioSource audioSource;
    [SerializeField] AudioClip spawnSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] protected AudioClip hittedSound;

    [SerializeField] List<GameObject> inmuneTo;

    [SerializeField] bool hideInmediatlyOnDestroy = true;
    [SerializeField] float timeToDestroy = 0f;

    public static float AutoKillHeight = -50;

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

    public virtual void SetControlledByCinematic(bool controlledByCinematic)
    {
        this.ControlledByCinematic = controlledByCinematic;
    }

    protected virtual void Update()
    {
        if (IsAlive() && transform.position.y < AutoKillHeight)
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

    public virtual float TakeDamage(float damage, bool flinch = true, bool playSound = true, bool destroyInmediatly = true)
    {
        Player p = this.GetComponent<Player>();
        if (p == null || (p != null && !p.hasWon))
            LifePoints -= damage >= MinDamageToTake ? damage : 0;
        if (LifePoints <= 0)
        {
            Die();
        }
        else if (p == null || (p != null && !p.hasWon))
        {
            if (playSound && audioSource != null && hittedSound != null)
            {
                audioSource.PlayOneShot(hittedSound);
            }

            if (flinch)
            {
                if (p != null)
                    p.GetHurt();
            }
        }
        return LifePoints;
    }

    protected virtual void Die()
    {
        Rigidbody rigidBody = GetComponent<Rigidbody>();
        if(rigidBody != null)
            rigidBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        float timeToDestroy = this.timeToDestroy;
        if(audioSource != null && deathSound != null)
        {
            timeToDestroy = Math.Max(deathSound.length, timeToDestroy);
            audioSource.PlayOneShot(deathSound);
        }

        if(exploder != null)
            exploder.Explode();

        LifePoints = 0;
        SpawnLoot();

        if (GetComponent<Player>() == null)
        {
            if (hideInmediatlyOnDestroy)
            {
                gameObject.transform.localScale = Vector3.zero;
                // Special case for crates, otherwise messes up weighted platforms in Level 2
                if (gameObject.tag == "Crate")
                    gameObject.transform.position = new Vector3(0, -500, 0);
            }
            Invoke(nameof(DestroyObject), timeToDestroy);
        }
    }

    protected bool IsGrounded()
    {
        int layers = LayerMask.GetMask("Ground", "Enemies");
        float distToGround = collider.bounds.extents.y;
        return Physics.Raycast(transform.position, Vector3.down, distToGround + 0.1f, layers);
    }

    private void SpawnLoot()
    {
        if (IsGrounded())
        {
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
