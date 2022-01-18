using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

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
    public bool hasWon;
    private bool dying;
	
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
    [SerializeField] AudioClip stunnedSound;
    [SerializeField] AudioClip victoryTheme;

    private void ReduceHealthByTime()
    {
        if(!hasWon && !dying && !ControlledByCinematic)
            TakeDamage(recurrentHealthLost, false, false);
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

        hasWon = false;
        dying = false;
        animator.SetBool("isAlive", true);
        InvokeRepeating(nameof(ReduceHealthByTime), recurrentHealthLostTime, recurrentHealthLostTime);

        raymanBody.GetComponent<Renderer>().material.color = overlayColor;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if(!hasWon && !dying && !PauseMenu.gameIsPaused)
        {
            isGrounded = IsGrounded();
            CalculateMovingSpeedAndApplyRotation();
            HandleMovementCases();
            SetAnimatorParameters();
            GetInputs();

            if (!ControlledByCinematic)
            {
                DecreasePowerupTime(); 
            }

        }
        if(hasWon)
            rigidBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }

    public void ToggleHelicopter(bool status)
    {
        if(status)
        {
            audioSource.clip = this.powerUp == PowerUpsEnum.HELICOPTER ? helicopterPowerUpSound : helicopterSound;
            audioSource.Play();
            isUsingHelicopter = true;
            raymanBody.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(22, 100);
        } else
        {
            audioSource.clip = null;
            isUsingHelicopter = false;
            raymanBody.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(22, 0);
        }
    }

    void HandleMovementCases()
    {
        if (isGrounded)
            ToggleHelicopter(false);

        if (isUsingHelicopter)
        {
            playerSpeedMultiplier = helicopterMovementSpeed;
            if(powerUp != PowerUpsEnum.HELICOPTER)
                rigidBody.velocity = new Vector3(rigidBody.velocity.x, helicopterDescendingSpeed, rigidBody.velocity.z);
            else
                rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpSpeed, rigidBody.velocity.z);
        }
        else
        {
            playerSpeedMultiplier = movementSpeed;
        }
    }

    void OnJump()
    {
        if (ControlledByCinematic || dying || hasWon || PauseMenu.gameIsPaused)
            return;
        jumpInput = true;
        if (!stunned)
        {
            if (isGrounded)
            {
                if (audioSource != null && jumpSound != null)
                    audioSource.PlayOneShot(jumpSound);
                rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpSpeed, rigidBody.velocity.z);
            }
            else
            {
                ToggleHelicopter(!isUsingHelicopter);
            }
        }
    }

    void OnAttack()
    {
        if (ControlledByCinematic || dying || hasWon || PauseMenu.gameIsPaused)
            return;
        Gun gun = powerUp == PowerUpsEnum.STRENGTH ? fistShooterStrengthPowerUp : fistShooter;
        if (gun != null)
        {
            if (gun.Attack(null))
                StartCoroutine(AnimatePunch());
        }
        SceneController.ResumeMusic();
    }

    void CalculateMovingSpeedAndApplyRotation()
    {
        Vector3 resultVelocity = !stunned ? new Vector3(horizontalAxisInput, 0, verticalAxisInput) : Vector3.zero;
        resultVelocity.Normalize();
        resultVelocity = Quaternion.AngleAxis(rotation, Vector3.up) * resultVelocity;
        resultVelocity *= playerSpeedMultiplier;
        rigidBody.velocity = new Vector3(resultVelocity.x, rigidBody.velocity.y, resultVelocity.z);
        if (Mathf.Abs(horizontalAxisInput) > 0.01f || Mathf.Abs(verticalAxisInput) > 0.01f)
            transform.rotation = Quaternion.Euler(0, GetFlatVelocityAbsoluteAngle(new Vector3(horizontalAxisInput, 0, verticalAxisInput)), 0);
    }

    public void OnMovement(InputValue value)
    {
        if (ControlledByCinematic || dying || hasWon)
            return;
        if(PauseMenu.gameIsPaused)
        {
            horizontalAxisInput = 0;
            verticalAxisInput = 0;
            return;
        }
        Vector2 direction = value.Get<Vector2>();
        horizontalAxisInput = direction.x;
        verticalAxisInput = direction.y;
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

    void GetInputs()
    {
        jumpInput = false;
        if (ControlledByCinematic)
        {
            horizontalAxisInput = 0;
            verticalAxisInput = 0;
            jumpInput = false;
        }
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
        dying = true;
        isUsingHelicopter = false;
        animator.SetBool("IsUsingHelicopter", false);
        raymanBody.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(22, 0);
        StartCoroutine(AnimateDeath());
        base.Die();
    }

    IEnumerator AnimateDeath()
    {
        if (transform.position.y > Vulnerable.AutoKillHeight)
        {
            animator.SetBool("isAlive", false);
            yield return new WaitForSeconds(4f);
        }
        else
            yield return new WaitForSeconds(2f);
        SceneTransitions sceneTransitions = FindObjectOfType<SceneTransitions>();
        sceneTransitions.ReloadScene();
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
            raymanBody.GetComponent<Renderer>().material.color = overlayColor;
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
        raymanBody.GetComponent<Renderer>().material.color = overlayColor;
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
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("isPunching", false);
    }

    public void GetHurt()
    {
        if(!hasWon && !dying)
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

    public Coroutine Celebrate()
    {
        return StartCoroutine(CelebrateCoroutine());
    }

    IEnumerator CelebrateCoroutine()
    {
        hasWon = true;
        isUsingHelicopter = false;
        raymanBody.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(22, 0);

        SceneController.StopMusic();
        if (audioSource != null && victoryTheme != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(victoryTheme);
        }

        transform.Rotate(new Vector3(0, 180, 0));
        animator.SetBool("IsUsingHelicopter", isUsingHelicopter);
        animator.SetBool("isCelebrating", true);
        animator.SetInteger("celebrationStage", 0);
        yield return new WaitForSeconds(1.5f);
        animator.SetInteger("celebrationStage", 1);
        yield return new WaitForSeconds(4f);
        animator.SetBool("isCelebrating", false);
        animator.SetInteger("celebrationStage", 0);
        animator.SetFloat("VerticalSpeedValue", 0);
        animator.SetFloat("FlatSpeedAbsoluteValue", 1);
        rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        transform.Rotate(new Vector3(0, 180, 0));

        audioSource.Play();
    }
}
