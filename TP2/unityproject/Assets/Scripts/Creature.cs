using UnityEngine;
using UnityEngine.AI;

public class Creature : Vulnerable
{
    private NavMeshAgent agent;
    private GameObject player;
    [SerializeField] LayerMask playerLayer;

    [SerializeField] float sightRange;
    [SerializeField] float attackRange;
    [SerializeField] float walkingSpeed = 1;
    [SerializeField] float runningSpeed = 3;

    private ManualAnimator manualAnimator;

    private Weapon weapon;

    [SerializeField] GameObject[] patrolWaypoints;
    int currentPatrolWaypointIndex = 0;
    private bool walkPointSet;

    private bool playerInSightRange, playerInAttackRange;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Quitar miembros privados no utilizados", Justification = "<pendiente>")]
    protected override void Start()
    {
        base.Start();

        player = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();
        weapon = GetComponent<Weapon>();
        manualAnimator = GetComponent<ManualAnimator>();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Quitar miembros privados no utilizados", Justification = "<pendiente>")]
    protected override void Update()
    {
        base.Update();

        if (!IsAlive())
            return;

        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!playerInSightRange && !playerInAttackRange && patrolWaypoints.Length == 0) Idling();
        if (!playerInSightRange && !playerInAttackRange && patrolWaypoints.Length > 0) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void Idling()
    {
        manualAnimator.PlayContinuous("Idle");
        agent.isStopped = true;
    }

    private void Patroling()
    {
        agent.isStopped = false;
        manualAnimator.PlayContinuous("Walking");
        agent.speed = walkingSpeed;
        agent.SetDestination(patrolWaypoints[currentPatrolWaypointIndex].transform.position);

        if (Vector3.Distance(transform.position, patrolWaypoints[currentPatrolWaypointIndex].transform.position) < 0.5f)
        {
            currentPatrolWaypointIndex = (currentPatrolWaypointIndex + 1) % patrolWaypoints.Length;
        }
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player.transform);

        manualAnimator.PlayContinuous("Taunting");

        if (weapon != null && weapon.CanUse)
        {
            manualAnimator.PlayAbrupt("Punching");

            Invoke(nameof(PerformAttack), manualAnimator.GetCurrentAnimationTotalDuration() / 3);
        }
    }

    private void PerformAttack()
    {
        weapon.Attack(player);
    }

    private void ChasePlayer()
    {
        agent.isStopped = false;
        manualAnimator.PlayContinuous("Running");
        agent.speed = runningSpeed;
        agent.SetDestination(player.transform.position);
    }

    protected override void Die()
    {
        agent.isStopped = true;
        manualAnimator.PlayForceContinuous("Dying");

        gameObject.GetComponent<Collider>().enabled = false;

        Invoke(nameof(DestroyObject), manualAnimator.GetCurrentAnimationTotalDuration() + 3);
    }

    public override float TakeDamage(float damage)
    {
        float remainingLife = base.TakeDamage(damage);

        if (damage > 0 && IsAlive())
        {
            agent.isStopped = true;
            manualAnimator.PlayAbrupt("Taking hit");
            Invoke(nameof(RemoveAgentStop), manualAnimator.GetCurrentAnimationTotalDuration());
        }

        return remainingLife;
    }

    private void RemoveAgentStop()
    {
        agent.isStopped = false;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Quitar miembros privados no utilizados", Justification = "<pendiente>")]
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }










    /*WaypointFollower defaultWaypointFollower;
    WaypointFollower waypointFollower;

    new Collider collider;

    [SerializeField] float maxViewDistance = 200;

    // Start is called before the first frame update
    void Start()
    {
        //waypointFollower = defaultWaypointFollower = GetComponent<WaypointFollower>();
        collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        bool m_HitDetect = Physics.BoxCast(collider.bounds.center, transform.localScale, transform.forward, out RaycastHit m_Hit, transform.rotation, maxViewDistance);
        if (m_HitDetect)
        {
            if(m_Hit.collider.name == "Player")
            {
                
            }
        }
    }*/
}
