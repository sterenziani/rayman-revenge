using System;
using UnityEngine;

public class ForceField : Vulnerable
{
    private Resizable resizable;
    private Vector3 startLocalScale;
    private Action callback;

    private AudioSource audioSource;

    [SerializeField] AudioClip activationSound;
    [SerializeField] AudioClip deactivationSound;
    [SerializeField] AudioClip impactSound;

    public float damageOnTouch = 5;
    public float stunSeconds = 1;

    protected override void Start()
    {
        base.Start();

        startLocalScale = transform.localScale;
        resizable = gameObject.GetComponent<Resizable>();

        audioSource = gameObject.GetComponent<AudioSource>();

        if (audioSource != null && activationSound != null)
        {
            audioSource.PlayOneShot(activationSound);
        }

        if (resizable != null)
        {
            transform.localScale = Vector3.zero;
            resizable.ScaleOverTime(startLocalScale, 1.5f);
        }
    }

    public override float TakeDamage(float damage)
    {
        float lifePointsOld = LifePoints;
        float lifePointsNew = base.TakeDamage(damage);
        if (lifePointsOld > lifePointsNew)
        {
            if (audioSource != null && impactSound != null)
            {
                audioSource.PlayOneShot(activationSound);
            }
        }

        return lifePointsNew;
    }

    public void Deactivate()
    {
        Die();
    }

    public void setDeathCallback(Action callback)
    {
        this.callback = callback;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Die()
    {
        callback?.Invoke();

        if (resizable == null)
        {
            base.Die();
        }
        else
        {
            if(audioSource != null && deactivationSound != null)
            {
                audioSource.PlayOneShot(deactivationSound);
            }

            resizable.ScaleOverTime(Vector3.zero, 1.5f, base.Die);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (damageOnTouch == 0 && stunSeconds == 0)
            return;

        Player v = collision.gameObject.GetComponent<Player>();
        if (v != null && v.gameObject != null)
        {
            if (stunSeconds > 0)
            {
                Player player = v.gameObject.GetComponent<Player>();
                if (player != null)
                    player.Stun(stunSeconds);
            }

            v.TakeDamage(damageOnTouch);
        }
    }
}
