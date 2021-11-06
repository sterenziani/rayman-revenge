using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Player : Vulnerable
{
    private Rigidbody rigidBody;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float movementSpeed = 9f;
    [SerializeField] float helicopterMovementSpeed = 3.0f;

    private PowerUpsEnum powerUp = PowerUpsEnum.NONE;

    float playerSpeedMultiplier;
    private float helicopterDescendingSpeed = -1.0f;

    private bool stunned = false;

    private Animator animator;
    private float distToGround;
    private float horizontalAxisInput;
    private float verticalAxisInput;
    private bool jumpInput;
    private bool isUsingHelicopter;
    private bool isGrounded;

    private bool hitInput;
    private Gun fistShooter;
    private Gun fistShooterStrengthPowerUp;

    private float rotation = 80;
    private Vector3 mouseLookingAt;

	private GameObject raymanBody;

    public float recurrentHealthLost = 1;
    public float recurrentHealthLostTime = 1;

    private float remainingPowerUpTime = 0;
    private float currentPowerupDuration = 100;

    private Material defaultMaterial;

    [SerializeField] AudioClip helicopterSound;
    [SerializeField] AudioClip helicopterPowerUpSound;
    [SerializeField] AudioClip powerUpSound;
    [SerializeField] AudioClip endPowerUpSound;
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip hittedSound;
    [SerializeField] AudioClip stunnedSound;

    private void ReduceHealthByTime()
    {
        TakeDamage(recurrentHealthLost, false);
    }

    public void Stun(float time)
    {
        stunned = true;
        isUsingHelicopter = false;

        if (audioSource != null && stunnedSound != null)
        {
            audioSource.PlayOneShot(stunnedSound);
        }

        Invoke(nameof(RemoveStun), time);
    }

    public void RemoveStun()
    {
        stunned = false;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        audioSource.clip = helicopterSound;

        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider>();
        distToGround = GetComponent<Collider>().bounds.extents.y;
        fistShooter = GetComponent<Gun>();
		raymanBody = this.gameObject.transform.Find("rayman").gameObject.transform.Find("Body").gameObject;
        fistShooter = this.gameObject.transform.Find("FistShooter").GetComponent<Gun>();
        fistShooterStrengthPowerUp = this.gameObject.transform.Find("FistShooterStrengthPowerUp").GetComponent<Gun>();
        defaultMaterial = raymanBody.GetComponent<Renderer>().material;

        //TODO va a traer problemas cuando se recargue la escena?
        animator.SetBool("isAlive", true);
        InvokeRepeating(nameof(ReduceHealthByTime), recurrentHealthLostTime, recurrentHealthLostTime);
	}

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        GetInputs();
        GetCircumstances();
        CalculateMovingSpeedAndApplyRotation();
        HandleMovementCases();
        HandleShoot();
        SetAnimatorParameters();
        DecreasePowerupTime();
    }

    void HandleShoot()
    {
        if (hitInput)
        {
            Gun gun = powerUp == PowerUpsEnum.STRENGTH ? fistShooterStrengthPowerUp : fistShooter;
            if(gun != null)
            {
                if(gun.Attack(null))
                {
                    StartCoroutine(AnimatePunch());
                }
            }
        }
    }

    void HandleMovementCases()
    {
        if (jumpInput && isGrounded && !stunned)
        {
            if (audioSource != null && jumpSound != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }

            rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpSpeed, rigidBody.velocity.z);
        }

        if (isGrounded)
        {
            audioSource.clip = null;
            isUsingHelicopter = false;
			raymanBody.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(22, 0);
		}

        if(jumpInput && !stunned)
        {
            if(isUsingHelicopter)
            {
                audioSource.clip = null;
                isUsingHelicopter = false;
				raymanBody.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(22, 0);
			}
            else if(!isGrounded && !isUsingHelicopter)
            {
                audioSource.clip = this.powerUp == PowerUpsEnum.HELICOPTER ? helicopterPowerUpSound : helicopterSound;
                audioSource.Play();
                isUsingHelicopter = true;
				raymanBody.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(22, 100);
			}
        }

        if (isUsingHelicopter)
        {
            playerSpeedMultiplier = helicopterMovementSpeed;

            if(powerUp != PowerUpsEnum.HELICOPTER)
            {
                rigidBody.velocity = new Vector3(rigidBody.velocity.x, helicopterDescendingSpeed, rigidBody.velocity.z);
            }
            else
            {
                rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpSpeed, rigidBody.velocity.z);
            }
        }
        else
        {
            playerSpeedMultiplier = movementSpeed;
        }
    }

    void CalculateMovingSpeedAndApplyRotation()
    {
        Vector3 resultVelocity = !stunned ? new Vector3(horizontalAxisInput, 0, verticalAxisInput) : Vector3.zero;
        resultVelocity.Normalize();
        resultVelocity = Quaternion.AngleAxis(rotation, Vector3.up) * resultVelocity;
        resultVelocity *= playerSpeedMultiplier;
        rigidBody.velocity = new Vector3(resultVelocity.x, rigidBody.velocity.y, resultVelocity.z);
        if (CurrentlyMoving())
        {
            transform.rotation = Quaternion.Euler(0,
                GetFlatVelocityAbsoluteAngle(new Vector3(horizontalAxisInput, 0, verticalAxisInput)),
                0);
        }
    }

    bool CurrentlyMoving()
    {
        return Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.01f || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.01f;
    }

    float GetFlatVelocityAbsoluteAngle(Vector3 flatVelocity)
    {
        Vector3 forwardVector = new Vector3(0, 0, 1.0f);
        Vector3 standardizedFlatVelocity = Quaternion.AngleAxis(0.0f, Vector3.down) * flatVelocity;
        bool right = standardizedFlatVelocity.x >= 0.0f;
        float angle = Vector3.Angle(forwardVector, flatVelocity);
        return right ? angle + rotation : 360f - angle + rotation;
    }

    void SetAnimatorParameters()
    {
        animator.SetFloat("FlatSpeedAbsoluteValue", new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z).magnitude);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("JumpPressed", jumpInput);
        animator.SetFloat("VerticalSpeedValue", rigidBody.velocity.y);
        animator.SetBool("IsUsingHelicopter", isUsingHelicopter);
        animator.SetBool("isStunned", stunned);
    }

    void GetCircumstances()
    {
        isGrounded = IsGrounded();
    }

    void GetInputs()
    {
        horizontalAxisInput = Input.GetAxisRaw("Horizontal");
		verticalAxisInput = Input.GetAxisRaw("Vertical");
        jumpInput = Input.GetButtonDown("Jump");

        hitInput = Input.GetMouseButtonDown(0);
    }

	bool IsGrounded()
	{
        int layers = LayerMask.GetMask("Ground", "Enemies");
        return Physics.Raycast(transform.position, Vector3.down, distToGround + 0.1f, layers);
	}

	public void SetRotation(float rotation)
    {
        this.rotation = rotation;
    }

    public void SetMouseLookingAt(Vector3 mouseLookingAt)
    {
        this.mouseLookingAt = mouseLookingAt;
    }

    protected override void Die()
    {
        StartCoroutine(AnimateDeath());
        base.Die();
    }

    IEnumerator AnimateDeath()
    {
        animator.SetBool("isAlive", false);
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ApplyPowerUp(PowerUpsEnum powerUp, float duration, Material material)
    {
        CancelInvoke(nameof(UndoPowerUp));

        remainingPowerUpTime = duration;
        currentPowerupDuration = duration;
        Invoke(nameof(UndoPowerUp), duration);

        if (this.powerUp != powerUp)
        {
            audioSource.PlayOneShot(powerUpSound);

            raymanBody.GetComponent<Renderer>().material = material;
            this.powerUp = powerUp;

            if (this.powerUp == PowerUpsEnum.HELICOPTER && isUsingHelicopter)
            {
                audioSource.clip = helicopterPowerUpSound;
                audioSource.Play();
            }
        }
    }

    private void UndoPowerUp()
    {
        audioSource.PlayOneShot(endPowerUpSound);

        if(this.powerUp == PowerUpsEnum.HELICOPTER && isUsingHelicopter)
        {
            audioSource.clip = helicopterSound;
            audioSource.Play();
        }

        raymanBody.GetComponent<Renderer>().material = defaultMaterial;
        this.powerUp = PowerUpsEnum.NONE;
        remainingPowerUpTime = 0;
    }

    private void DecreasePowerupTime()
    {
        if(remainingPowerUpTime >= 0)
            remainingPowerUpTime -= Time.deltaTime;
        if (remainingPowerUpTime < 0)
            remainingPowerUpTime = 0;
    }

    public float GetRemainingPowerUpPercentage()
    {
        return remainingPowerUpTime / currentPowerupDuration;
    }

    IEnumerator AnimatePunch()
    {
        animator.SetBool("isPunching", true);
        yield return new WaitForSeconds(0.4f);
        animator.SetBool("isPunching", false);
    }

    public void GetHurt()
    {
        StartCoroutine(AnimateTakeDamage());
    }

    IEnumerator AnimateTakeDamage()
    {
        if(!stunned && audioSource != null && hittedSound != null)
        {
            audioSource.PlayOneShot(hittedSound);
        }

        animator.SetBool("isTakingDamage", true);
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("isTakingDamage", false);
    }
}
