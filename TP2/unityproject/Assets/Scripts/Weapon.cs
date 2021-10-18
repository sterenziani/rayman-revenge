using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected float damage;
    [SerializeField] protected float cooldownTime;
    public bool CanUse { get; protected set; } = true;

    public virtual bool Attack(GameObject target)
    {
        if (!CanUse)
            return false;

        Vulnerable vulnerable = target.GetComponent<Vulnerable>();
        if(vulnerable == null)
            return false;

        vulnerable.TakeDamage(damage);

        CanUse = false;
        Invoke(nameof(CooldownFinished), cooldownTime);

        return true;
    }

    protected void CooldownFinished()
    {
        CanUse = true;
    }
}
