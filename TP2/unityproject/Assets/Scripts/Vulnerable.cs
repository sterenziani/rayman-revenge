using UnityEngine;

public class Vulnerable : MonoBehaviour
{
    public float LifePointsTotal = 10;
    public float LifePoints { get; protected set; }

    public float MinDamageToTake = 0;

    private MeshExploder exploder;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        exploder = GetComponent<MeshExploder>();
        LifePoints = LifePointsTotal;
    }

    protected virtual void Update()
    {
        if (transform.position.y < -50)
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
        else if(flinch)
        {
            Player p = this.GetComponent<Player>();
            if (p != null)
                p.GetHurt();
        }
        return LifePoints;
    }

    protected virtual void Die()
    {
        if(exploder != null)
        {
            exploder.Explode();
        }

        DestroyObject();
    }

    protected void DestroyObject()
    {
        Destroy(gameObject);
    }

    public void Step()
    {
        Collider collider = gameObject.GetComponent<Collider>();

        if(collider != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, collider.bounds.size.y))
            {
                Platform platform = hit.collider.gameObject.GetComponent<Platform>();
                if(platform != null)
                {
                    platform.StepOn();
                }
            }
        }
    }


}
