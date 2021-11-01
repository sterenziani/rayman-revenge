using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningStrike : MonoBehaviour
{
    [SerializeField] ParticleSystem strikeFX;
    [SerializeField] ParticleSystem summonFX;

    public float damage = 10;
    public float stunSeconds = 0;

    private HashSet<Vulnerable> VulnerablesInRange = new HashSet<Vulnerable>();

    private void OnTriggerEnter(Collider other)
    {
        Vulnerable vulnerable = other.GetComponent<Vulnerable>();
        if(vulnerable != null)
        {
            VulnerablesInRange.Add(vulnerable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Vulnerable vulnerable = other.GetComponent<Vulnerable>();
        if (vulnerable != null)
        {
            VulnerablesInRange.Remove(vulnerable);
        }
    }

    private void Strike()
    {
        strikeFX.Play();

        foreach(Vulnerable v in VulnerablesInRange)
        {
            if(v != null && v.gameObject != null)
            {
                v.TakeDamage(damage);

                if(stunSeconds > 0)
                {
                    //TODO STUN
                }
            }
        }

        Invoke(nameof(Destroy), strikeFX.main.duration);
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        summonFX.Play();

        Invoke(nameof(Strike), summonFX.main.duration);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
