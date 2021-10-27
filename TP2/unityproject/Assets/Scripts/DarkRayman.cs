using UnityEngine;

public class DarkRayman : Vulnerable
{
    private GameObject player;
    private ManualAnimator manualAnimator;
    private new Collider collider;
    private GameObject forceField;

    protected override void Start()
    {
        player = GameObject.Find("Player");
        manualAnimator = GetComponent<ManualAnimator>();
        collider = GetComponent<Collider>();
        forceField = transform.Find("ForceField").gameObject;

        manualAnimator.PlayContinuous("Floating");
        collider.isTrigger = true;
    }

    protected override void Update()
    {
        transform.LookAt(player.transform.position);
    }

    private void ToggleForceField(bool activated)
    {
        forceField.SetActive(activated);
    }
}
