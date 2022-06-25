using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Inspector Variables

    [Header("Movement Settings")]
    [SerializeField] private float forwardSpeedAmount = 10f;
    [SerializeField] private float swerveSpeed = 0.5f;
    [SerializeField] private float maxSwerveAmount = 1f;
    [SerializeField] private float gravityAmount = 9.8f;
    [Space(10)]
    [Header("Ground Check")]
    [Range(0.1f, 1f)]
    [SerializeField] private float groundCheckAmount = 0.5f; //Diameter of the sphere controlling the ground
    [SerializeField] private LayerMask checkLayerMask; //Ground sphere Layer Mask
    [SerializeField] private float groundRayAmount = 1f; //Ground ray

    #endregion

    public bool isPlayerActive = true; //Can the character play

    #region Private Variables

    //Character components
    private SwerveInputSystem _swerveInputSystem;
    private Rigidbody _playerRigidbody;
    private AnimManager _animManager;

    private Rotator rotaterObject;
    private GameManager _gameManager;

    private float gravity;
    private int forwardSpeed = 1;
    private bool isPush;
    private bool canMove = false;
    private float pushForce;
    private Vector3 pushDirection;

    private bool isStuned = false;
    private bool wasStuned = false;


    #endregion

    #region Properties

    public bool CanMove { get { return canMove; } set { canMove = value; } }
    public Rigidbody PlayerRigidbody { get { return _playerRigidbody; } set { _playerRigidbody = value; } }
    public AnimManager AnimManager => _animManager;

    #endregion

    private void Awake()
    {
        //Component Variables
        _swerveInputSystem = GetComponent<SwerveInputSystem>();
        _playerRigidbody = GetComponent<Rigidbody>();
        _gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        _animManager = GetComponent<AnimManager>();
    }

    private void FixedUpdate()
    {
        if (isPush) //Is the character being pushed
        {
            //SetPush function is called
            SetPush(rotaterObject.p_Direction, rotaterObject.p_DirectionMultiplier, rotaterObject.p_Speed);
        }

        if (canMove) //Does the character move
        {
            _animManager.SetAnimationState(AnimManager.AnimationStates.Run);
            Move();
        }
        else //When the character is not moving
        {
            _playerRigidbody.velocity = Vector3.zero;
            _animManager.SetAnimationState(AnimManager.AnimationStates.Idle);
            _playerRigidbody.velocity = pushDirection * pushForce;
        }
    }

    private void Update()
    {
        //The character's contact with the ground is checked.
        CheckGround();

        //If the character is playable and the left mouse button is pressed
        if (Input.GetMouseButtonDown(0) && !canMove && isPlayerActive)
        {
            _gameManager.isDestination = true; //Artificial intelligence movement begins
            SetMovement(true); //Player starts moving
            _gameManager.SetUI(UIManager.UIState.Gameplay); //The gameplay screen is displayed
        }
    }

    #region Character Movement

    //Character movement calculation
    private void Move()
    {
        //The difference value between the swerve movement is taken
        float swerveAmount = Time.fixedDeltaTime * swerveSpeed * _swerveInputSystem.MoveFactorX;
        swerveAmount = Mathf.Clamp(swerveAmount, -maxSwerveAmount, maxSwerveAmount);
        _playerRigidbody.velocity = new Vector3(swerveAmount, -gravity * Time.fixedDeltaTime, forwardSpeed * forwardSpeedAmount * Time.fixedDeltaTime);
    }

    //Function that adjusts the motion state
    public void SetMovement(bool value)
    {
        canMove = value;
        if (canMove) //If the player is moving
        {
            //Rigidbody adjustments are made
            _playerRigidbody.constraints = RigidbodyConstraints.None;
            _playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else //If the player is not moving
        {
            _playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    #endregion

    #region Character Physics
    //Function that controls the character's contact with the ground
    private void CheckGround()
    {
        //We draw a sphere under the character
        bool isGround = Physics.CheckSphere(transform.position, groundCheckAmount, checkLayerMask);
        SetGravity(isGround);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundRayAmount))
        {
            if (hit.transform.CompareTag("RotatingPlatform"))
            {
                rotaterObject = hit.transform.parent.GetComponent<Rotator>();
                isPush = true;
            }
            else
            {
                isPush = false;
            }
        }
    }

    //Function that adjusts the gravity value
    public void SetGravity(bool state)
    {
        if (state)
        {
            gravity = 0f;
            forwardSpeed = 1;
        }
        else
        {
            gravity = gravityAmount * 75f;
            forwardSpeed = 0;
        }
    }

    //Function used to push the character
    private void SetPush(Rotator.Direction axis, int direction, float speed)
    {
        Vector3 directionAxis = new Vector3();
        switch (axis)
        {
            case Rotator.Direction.X_Axis:
                directionAxis = Vector3.up;
                break;

            case Rotator.Direction.Y_Axis:
                directionAxis = Vector3.forward;
                break;

            case Rotator.Direction.Z_Axis:
                directionAxis = Vector3.right;
                break;
        }

        _playerRigidbody.AddForce(directionAxis * speed * 250f * -direction * Time.fixedDeltaTime, ForceMode.Force);
    }

    //Function that will run when the player is hit by an obstacle
    public void HitPlayer(Vector3 velocityF, float time)
    {
        _playerRigidbody.velocity = velocityF;

        pushForce = velocityF.magnitude;
        pushDirection = Vector3.Normalize(velocityF);
        StartCoroutine(Decrease(velocityF.magnitude, time));
    }

    #endregion


    #region Enumarators
    //
    private IEnumerator Decrease(float value, float duration)
    {
        if (isStuned)
            wasStuned = true;
        isStuned = true;
        canMove = false;
        //SetMovement(false);

        float delta = 0;
        delta = value / duration;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            yield return null;

            _playerRigidbody.AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0)); //Add gravity
        }

        if (wasStuned)
        {
            wasStuned = false;
        }
        else
        {
            isStuned = false;
            canMove = true;
            //SetMovement(true);
        }
    }
        
    #endregion

    //ON DRAW GIZMOS
    private void OnDrawGizmos()
    {
        //GroundCheck sphere visualization
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, groundCheckAmount);
    }
}
