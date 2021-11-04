using System;
using System.Collections;
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

    private ManualAnimator manualAnimator;
    private GameObject player;
    private Gun fistShooter;

    private IEnumerator darkBallsCoroutine;

    private GameObject forceField;

    protected override void Start()
    {
        base.Start();

        manualAnimator = GetComponent<ManualAnimator>();
        fistShooter = GetComponent<Gun>();

        player = GameObject.Find("Player");

        manualAnimator.PlayContinuous("Floating");

        ActivateForceField(StartVulnerablePhase);

        Invoke(nameof(StartForceFieldPhase), 4f);
    }

    void StartForceFieldPhase()
    {
        StopReleasingDarkBalls();
        ActivateForceField(StartVulnerablePhase);
        StartShootingLightning(Math.Max(initialLightningTimeBetween * (LifePoints / LifePointsTotal), minLightningTimeBetween));
    }

    void StartVulnerablePhase()
    {
        StopShootingLightning();
        StartReleasingDarkBalls(Math.Max(initialDarkBallsTimeBetween * (LifePoints / LifePointsTotal), minDarkBallsTimeBetween));

        Invoke(nameof(StartForceFieldPhase), Math.Max(initialVulnerableTime * (LifePoints / LifePointsTotal), minVulnerableTime));
    }

    protected override void Update()
    {
        base.Update();

        transform.LookAt(player.transform);
    }

    void ActivateForceField(Action callback = null)
    {
        if (forceField != null)
        {
            return;
        }

        manualAnimator.PlayAbrupt("Spell Charge");

        forceField = Instantiate(forceFieldTemplate, forceFieldTemplate.transform.position, Quaternion.identity);
        if(callback != null)
            forceField.GetComponent<ForceField>().setDeathCallback(callback);
        forceField.SetActive(true);
    }

    void StartReleasingDarkBalls(float timeInBetween, int? amount = null, Action callback = null)
    {
        darkBallsCoroutine = DarkBallsCoroutine(timeInBetween, amount, callback);
        StartCoroutine(darkBallsCoroutine);
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
            fistShooter.bullet.gameObject.GetComponent<Creature>().walkingSpeed = Math.Min(maxDarkBallsSpeed, initialDarkBallsSpeed * (LifePointsTotal / LifePoints));
            fistShooter.bullet.gameObject.GetComponent<Creature>().runningSpeed = Math.Min(maxDarkBallsSpeed, initialDarkBallsSpeed * (LifePointsTotal / LifePoints));
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
