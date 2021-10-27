using UnityEngine;

public class DarkRayman : Vulnerable
{
    private GameObject forceFieldTemplate;
    private ManualAnimator manualAnimator;
    private GameObject player;

    private GameObject forceField;

    protected override void Start()
    {
        base.Start();

        manualAnimator = GetComponent<ManualAnimator>();
        player = GameObject.Find("Player");

        forceFieldTemplate = transform.Find("ForceField").gameObject;

        ActivateForceField();

        //manualAnimator.PlayContinuous("Floating");
    }

    protected override void Update()
    {
        base.Update();

        transform.LookAt(player.transform);
    }

    void ActivateForceField()
    {
        if (forceField != null)
        {
            return;
        }

        forceField = Instantiate(forceFieldTemplate, forceFieldTemplate.transform.position, Quaternion.identity);
        forceField.GetComponent<ForceField>().setDeathCallback(ForceFieldDestroyed);
        forceField.SetActive(true);
    }

    void ForceFieldDestroyed()
    {
        Invoke(nameof(ActivateForceField), 5f);
    }
}
