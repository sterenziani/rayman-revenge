using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningStrike : MonoBehaviour
{
    [SerializeField] ParticleSystem strikeFX;
    [SerializeField] ParticleSystem summonFX;

    [SerializeField] List<Vulnerable> immuneList;

    private AudioSource audioSource;
    [SerializeField] AudioClip impactSound;

    public float damage = 10;
    public float stunSeconds = 0;

    private HashSet<Vulnerable> VulnerablesInRange = new HashSet<Vulnerable>();

    private void OnTriggerEnter(Collider other)
    {
        Vulnerable vulnerable = other.GetComponent<Vulnerable>();
        if(vulnerable != null && !immuneList.Contains(vulnerable))
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
                if(stunSeconds > 0)
                {
                    Player player = v.gameObject.GetComponent<Player>();
                    if(player != null)
                        player.Stun(stunSeconds);
                }

                v.TakeDamage(damage);
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
        audioSource = GetComponent<AudioSource>();

        if(audioSource != null && impactSound != null)
        {
            audioSource.PlayOneShot(impactSound);
        }

        summonFX.Play();

        Invoke(nameof(Strike), summonFX.main.duration);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
