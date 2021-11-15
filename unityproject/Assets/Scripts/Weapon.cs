using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected float damage;
    [SerializeField] protected float cooldownTime;

    [SerializeField] protected AudioClip attackSound;
    protected AudioSource audioSource;

    public bool CanUse { get; protected set; } = true;

    protected virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public virtual bool Attack(GameObject target)
    {
        if (!CanUse)
            return false;

        if(audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        Vulnerable vulnerable = target.GetComponent<Vulnerable>();
        if(vulnerable == null)
            return false;

        vulnerable.TakeDamage(damage, true);

        CanUse = false;
        Invoke(nameof(CooldownFinished), cooldownTime);

        return true;
    }

    protected void CooldownFinished()
    {
        CanUse = true;
    }
}
