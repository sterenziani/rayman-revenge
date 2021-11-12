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

    private ManualAnimator manualAnimator;
    private GameObject player;
    private Gun fistShooter;

    private Coroutine darkBallsCoroutine;

    private GameObject forceField;

    private Vector3 initialPosition;

    public override void SetControlledByCinematic(bool controlledByCinematic)
    {
        base.SetControlledByCinematic(controlledByCinematic);

        if(controlledByCinematic)
        {
            StopReleasingDarkBalls();
            StopShootingLightning();
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

        initialPosition = transform.position;

        manualAnimator = GetComponent<ManualAnimator>();
        fistShooter = GetComponent<Gun>();

        player = GameObject.Find("Player");

        manualAnimator.PlayContinuous("Floating");
    }

    async Task Speak()
    {
        DialogueUI dialogueUI = GameObject.Find("Dialogue UI").GetComponent<DialogueUI>();
        await Task.Delay(100);
        await dialogueUI.ShowDialogue(this, "You <b>will</b> die!");
        await dialogueUI.ShowDialogue(player.GetComponent<Player>(), "No, U!");
        await dialogueUI.ShowDialogue(this, "NOOOOOO");
        await dialogueUI.ShowDialogue(player.GetComponent<Player>(), "yeet");

        await Task.Delay(1000);

        await dialogueUI.ShowTutorial("Press the <b>SPACE BAR</b> to jump!", 2000);
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
