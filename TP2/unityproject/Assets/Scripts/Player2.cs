using UnityEngine;
using UnityEngine.SceneManagement;

public class Player2 : Vulnerable
{
    private Rigidbody rigidBody;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float movementSpeed = 9f;
    [SerializeField] float helicopterMovementSpeed = 3.0f;

    float playerSpeedMultiplier;
    private float helicopterDescendingSpeed = -1.0f;

    private Animator animator;
    private float distToGround;
    private float horizontalAxisInput;
    private float verticalAxisInput;
    private bool jumpInput;
    private bool isUsingHelicopter;
    private bool isGrounded;

    private bool hitInput;
    private Gun fistShooter;

    private new Collider collider;
    private float rotation = 80;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider>();
        distToGround = collider.bounds.extents.y;
        fistShooter = GetComponent<Gun>();
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
    }

    void HandleShoot()
    {
        if (hitInput)
        {
            if(fistShooter != null)
            {
                fistShooter.Attack(null);
            }
        }
    }

    void HandleMovementCases()
    {
        if (jumpInput && isGrounded)
        {
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpSpeed, rigidBody.velocity.z);
        }

        if (isGrounded || (jumpInput && isUsingHelicopter))
        {
            //audioSource.Stop();
            isUsingHelicopter = false;
        }

        if (jumpInput && !isGrounded && !isUsingHelicopter)
        {
            //audioSource.Play();
            isUsingHelicopter = true;
        }

        if (jumpInput && isUsingHelicopter)
        {
            //AudioSource.PlayClipAtPoint (helicopterStopSound, transform.position, 0.75f);
        }

        if (isUsingHelicopter)
        {
            playerSpeedMultiplier = helicopterMovementSpeed;
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, helicopterDescendingSpeed, GetComponent<Rigidbody>().velocity.z);
        }
        else
        {
            playerSpeedMultiplier = movementSpeed;
        }
    }

    void CalculateMovingSpeedAndApplyRotation()
    {
        Vector3 resultVelocity = new Vector3(horizontalAxisInput, 0, verticalAxisInput);
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
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f) || Mathf.Abs(rigidBody.velocity.y) < 0.1f;
    }

    public void SetRotation(float rotation)
    {
        this.rotation = rotation;
    }

    protected override void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
