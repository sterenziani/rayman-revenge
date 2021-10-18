using UnityEngine;
using UnityEngine.AI;

public class Vulnerable : MonoBehaviour
{
    [SerializeField] float lifePointsTotal = 10;
    public float LifePoints { get; private set; }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        LifePoints = lifePointsTotal;
    }

    protected virtual void Update()
    {
        if (transform.position.y < -100)
            Die();
    }

    public bool IsAlive()
    {
        return LifePoints > 0;
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
        Destroy(gameObject);
    }


}
