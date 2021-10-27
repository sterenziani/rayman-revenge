using UnityEngine;

public class Vulnerable : MonoBehaviour
{
    public float LifePointsTotal = 10;
    public float LifePoints { get; protected set; }

    private HealthBar healthBar;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        LifePoints = LifePointsTotal;
        healthBar = GetComponent<HealthBar>();
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
        LifePoints -= damage;
        if(LifePoints <= 0)
        {
            Die();
        }

        return LifePoints;
    }

    protected virtual void Die()
    {

        DestroyObject();
    }

    protected void DestroyObject()
    {
        if(healthBar != null)
            Destroy(healthBar.gameObject);

        Destroy(gameObject);
    }


}
