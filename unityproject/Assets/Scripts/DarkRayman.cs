using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class DarkRayman : Vulnerable
{
    [SerializeField] GameObject forceFieldTemplate;
    [SerializeField] AreaSpawner lightningSpawner;

    [SerializeField] float initialVulnerableTime = 10;
    [SerializeField] float minVulnerableTime = 4;
    [SerializeField] float initialDarkBallsTimeBetween = 2;
    [SerializeField] float minDarkBallsTimeBetween = 0.3f;
    [SerializeField] float initialDarkBallsSpeed = 2f;
    [SerializeField] float maxDarkBallsSpeed = 8f;
    [SerializeField] float initialLightningTimeBetween = 0.8f;
    [SerializeField] float minLightningTimeBetween = 0.3f;

    public ManualAnimator manualAnimator;
    private GameObject player;
    private Gun fistShooter;

    private Coroutine darkBallsCoroutine;

    private GameObject forceField;

    public Vector3 initialPosition;

    private bool started = false;

    protected override void Die()
    {
        StopReleasingDarkBalls();
        StopShootingLightning();

        StopAllCoroutines();

        CancelInvoke(nameof(StartForceFieldPhase));

        base.Die();
    }

    public override void SetControlledByCinematic(bool controlledByCinematic)
    {
        base.SetControlledByCinematic(controlledByCinematic);

        if (!started)
            return;

        if(controlledByCinematic)
        {
            StopReleasingDarkBalls();
            StopShootingLightning();

            StopAllCoroutines();

            CancelInvoke(nameof(StartForceFieldPhase));
        } else
        {
            Invoke(nameof(StartForceFieldPhase), 1f);
        }
    }

    private float CalculateCurrentValueBasedOnBossHealth(float start, float finish)
    {
        float bossProgressPercentage = 1 - (LifePoints / LifePointsTotal);

        float difference = finish - start;

        return start + (difference * bossProgressPercentage); 
    }

    protected override void Start()
    {
        base.Start();

        started = true;

        initialPosition = transform.position;

        manualAnimator = gameObject.GetComponent<ManualAnimator>();
        fistShooter = gameObject.GetComponent<Gun>();

        player = GameObject.Find("Player");

        manualAnimator.PlayContinuous("Floating");

        if(!ControlledByCinematic)
            Invoke(nameof(StartForceFieldPhase), 1.5f);
    }

    void StartForceFieldPhase()
    {
        StopReleasingDarkBalls();
        ActivateForceField(StartVulnerablePhase);
        StartShootingLightning(CalculateCurrentValueBasedOnBossHealth(initialLightningTimeBetween, minLightningTimeBetween));
    }

    void StartVulnerablePhase()
    {
        StopShootingLightning();
        StartReleasingDarkBalls(CalculateCurrentValueBasedOnBossHealth(initialDarkBallsTimeBetween, minDarkBallsTimeBetween));

        Invoke(nameof(StartForceFieldPhase), CalculateCurrentValueBasedOnBossHealth(initialVulnerableTime, minVulnerableTime));
    }

    protected override void Update()
    {
        base.Update();

        transform.LookAt(player.transform);
        transform.transform.position = initialPosition;
    }

    void ActivateForceField(Action callback = null)
    {
        if (forceField != null)
        {
            return;
        }

        manualAnimator.PlayAbrupt("Spell Charge");

        forceField = Instantiate(forceFieldTemplate, forceFieldTemplate.transform.position, Quaternion.identity);

        forceField.transform.parent = transform;

        if(callback != null)
            forceField.GetComponent<ForceField>().setDeathCallback(callback);
        forceField.SetActive(true);
    }

    void StartReleasingDarkBalls(float timeInBetween, int? amount = null, Action callback = null)
    {
        StopReleasingDarkBalls();

        darkBallsCoroutine = StartCoroutine(DarkBallsCoroutine(timeInBetween, amount, callback));
    }

    void StopReleasingDarkBalls()
    {
        if (darkBallsCoroutine != null)
            StopCoroutine(darkBallsCoroutine);
    }

    IEnumerator DarkBallsCoroutine(float timeInBetween, int? amount = null, Action callback = null)
    {
        while (amount == null || amount > 0)
        {
            manualAnimator.PlayAbrupt("Spell Front");
            fistShooter.bullet.gameObject.GetComponent<Creature>().walkingSpeed = CalculateCurrentValueBasedOnBossHealth(initialDarkBallsSpeed, maxDarkBallsSpeed);
            fistShooter.bullet.gameObject.GetComponent<Creature>().runningSpeed = CalculateCurrentValueBasedOnBossHealth(initialDarkBallsSpeed, maxDarkBallsSpeed);
            fistShooter.Attack(player);

            if(amount.HasValue)
                amount--;

            yield return new WaitForSeconds(timeInBetween);
        }

        callback?.Invoke();
    }

    void StartShootingLightning(float timeBetweenSpawns = 0.7f)
    {
        manualAnimator.PlayContinuous("Spell Up");
        
        lightningSpawner.BeginSpawning(timeBetweenSpawns);
    }

    void StopShootingLightning()
    {
        manualAnimator.PlayContinuous("Floating");

        lightningSpawner.StopSpawning();
    }
}
