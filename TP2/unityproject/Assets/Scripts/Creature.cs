using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class Creature : Vulnerable
{
    private enum State
    {
        IDLE = 0,
        PATROLLING = 1,
        CHASING = 2,
        ATTACKING = 3,
        DEAD = 4,
    }

    private State state;

    private NavMeshAgent agent;
    private GameObject player;
    [SerializeField] LayerMask playerLayer;

    [SerializeField] float sightRange;
    [SerializeField] float attackRange;
    public float walkingSpeed = 1;
    public float runningSpeed = 3;

    private ManualAnimator manualAnimator;
    private Weapon weapon;

    [SerializeField] GameObject[] patrolWaypoints;
    private Vector3 currentPatrollingDestination;
    int currentPatrolWaypointIndex = 0;
    private bool walkPointSet;
    [SerializeField] bool randomPatrol;

    [SerializeField] AudioClip sawEnemySound;
    [SerializeField] AudioClip attackSound;

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
        if (!playerInSightRange && !playerInAttackRange && (patrolWaypoints.Length > 0 || randomPatrol)) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void Idling()
    {
        state = State.IDLE;
        manualAnimator?.PlayContinuous("Idle");
        agent.isStopped = true;
    }

    private void Patroling()
    {
        state = State.PATROLLING;

        agent.isStopped = false;
        manualAnimator?.PlayContinuous("Walking");
        agent.speed = walkingSpeed;
        agent.SetDestination(currentPatrollingDestination);

        if (Vector3.Distance(transform.position, currentPatrollingDestination) < 0.5f || agent.path.status != NavMeshPathStatus.PathComplete)
        {
            SetNextPatrollingDestination();
        }
    }

    private void SetNextPatrollingDestination()
    {
        if(randomPatrol)
        {
            currentPatrollingDestination = GetRandomNavMeshPoint(transform.position, (int)Random.Range(1, 20)) ?? transform.position;
        }
        else if(patrolWaypoints.Length > 0)
        {
            currentPatrolWaypointIndex = (currentPatrolWaypointIndex + 1) % patrolWaypoints.Length;

            currentPatrollingDestination = patrolWaypoints[currentPatrolWaypointIndex].transform.position;
        }
    }

    private void AttackPlayer()
    {
        state = State.ATTACKING;

        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player.transform);

        manualAnimator?.PlayContinuous("Taunting");

        if (weapon != null && weapon.CanUse)
        {
            manualAnimator?.PlayAbrupt("Punching");

            Invoke(nameof(PerformAttack), manualAnimator.GetCurrentAnimationTotalDuration() / 3);
        }
    }

    private void PerformAttack()
    {
        if(weapon.Attack(player))
        {
            if(audioSource != null && attackSound != null)
            {
                audioSource.PlayOneShot(attackSound);
            }
        }
    }

    private void ChasePlayer()
    {
        if(state != State.CHASING && audioSource != null && sawEnemySound != null)
        {
            audioSource.PlayOneShot(sawEnemySound);
        }

        state = State.CHASING;
        agent.isStopped = false;
        manualAnimator?.PlayContinuous("Running");
        agent.speed = runningSpeed;
        agent.SetDestination(player.transform.position);
    }

    protected override void Die(float timeToDie = 0, bool hideInmediatly = false)
    {
        if (agent != null)
            agent.isStopped = true;

        LifePoints = 0;

        gameObject.GetComponent<Collider>().enabled = false;
        AnimateDeath();

        base.Die(5f, hideInmediatly);
    }

    void AnimateDeath()
    {
        manualAnimator?.PlayForceContinuous("Dying");
    }

    public override float TakeDamage(float damage)
    {
        float remainingLife = base.TakeDamage(damage, false);

        if (damage > 0 && IsAlive())
        {
            agent.isStopped = true;
            manualAnimator?.PlayAbrupt("Taking hit");

            if(manualAnimator != null)
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

    Vector3? GetRandomNavMeshPoint(Vector3 center, float range)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        return null;
    }
}
