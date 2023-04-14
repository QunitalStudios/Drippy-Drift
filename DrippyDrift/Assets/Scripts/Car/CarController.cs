using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarController : MonoBehaviour
{
    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentbreakForce;
    private bool isBreaking;
    public bool isGroundedRL = false;
    public bool isGroundedRR = false;
    public float distToGround;
    private bool isLightActive = true;
    public float driftAngle;

    public float playerScore = 0f;
    public string currentMultiplier = "";
    public Color currentMPColor = Color.red;

    public float timer = 0f;
    public float comboTimer = 0f;

    public float currentSpeed = 0f;

    // Settings
    [SerializeField] private float motorForce, breakForce, maxSteerAngle;
    [SerializeField] private FixedJoystick joystick;

    // Wheel Colliders
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    // Wheels
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;
    [SerializeField] private CameraAScript cameraScript;
    [SerializeField] private ParticleSystem smoke1;
    [SerializeField] private ParticleSystem smoke2;
    [SerializeField] private TrailRenderer trail1;
    [SerializeField] private TrailRenderer trail2;

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    private void Start()
    {
        trail1.emitting = false;
        trail2.emitting = false;
        smoke1.enableEmission=false;
        smoke2.enableEmission = false;
        this.GetComponent<Rigidbody>().velocity = new Vector3(-0.2f, 0f, 0f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }

        isGroundedRR = Physics.Raycast(rearRightWheelCollider.transform.position, -Vector3.up, 0.5f);
        isGroundedRL = Physics.Raycast(rearLeftWheelCollider.transform.position, -Vector3.up, 0.5f);

        currentSpeed = this.GetComponent<Rigidbody>().velocity.magnitude * 2;

        TrailController();
        ResetPlayerCar();
        LightController();

        AddScore();

        if(rearLeftWheelCollider.motorTorque < 0 && Input.GetAxis("Vertical") < 0)
        {
            transform.GetChild(4).transform.GetChild(4).GetComponent<Light>().intensity = 4f;
            transform.GetChild(4).transform.GetChild(5).GetComponent<Light>().intensity = 4f;
            cameraScript.FlareLayer.enabled = true;
        }
        else
        {
            transform.GetChild(4).transform.GetChild(4).GetComponent<Light>().intensity = 2;
            transform.GetChild(4).transform.GetChild(5).GetComponent<Light>().intensity = 2;
            cameraScript.FlareLayer.enabled = false;
        }
    }

    public bool isGoingForward()
    {
        if (frontLeftWheelCollider.rpm >= 0 && frontRightWheelCollider.rpm >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public float CheckDriftAngle()
    {
        if (this.GetComponent<Rigidbody>().velocity != new Vector3(0f, 0f, 0f))
        {
            Vector3 driftValue = transform.InverseTransformVector(this.GetComponent<Rigidbody>().velocity);
            driftAngle = (Mathf.Atan2(driftValue.x, driftValue.z) * Mathf.Rad2Deg);

        }
        return Mathf.Abs(driftAngle);
    }

    private void AddScore()
    {
        if (((CheckDriftAngle() >= 15) && (CheckDriftAngle() <= 90)) && isGoingForward() && cameraScript.iffollow == true && isGroundedRL && isGroundedRR && currentSpeed > 1)
        {
            if(isGroundedRL&&isGroundedRR)
            {
                trail1.emitting = true;
                trail2.emitting = true;
                smoke1.enableEmission = true;
                smoke2.enableEmission = true;
            }
            comboTimer = 0f;
            timer += Time.deltaTime;

            if (timer >= 0.5 && timer < 3)
            {
                currentMultiplier = "";
                playerScore += (CheckDriftAngle() * 3 * Time.deltaTime) * 0.7f;
            }
            if (timer >= 3 && timer < 5)
            {
                currentMultiplier = "x2";
                playerScore += (CheckDriftAngle() * 3 * Time.deltaTime) * 0.9f;
                currentMPColor = Color.yellow;
            }
            if (timer >= 5 && timer < 7)
            {
                currentMultiplier = "x3";
                playerScore += (CheckDriftAngle() * 3 * Time.deltaTime) * 2;
                currentMPColor = Color.green;
            }
            if (timer >= 7 && timer < 9)
            {
                currentMultiplier = "x4";
                playerScore += (CheckDriftAngle() * 3 * Time.deltaTime) * 4;
                currentMPColor = Color.blue;
            }
            if (timer >= 9)
            {
                currentMultiplier = "x5";
                playerScore += (CheckDriftAngle() * 3 * Time.deltaTime) * 8;
                currentMPColor = Color.red;
            }
        }
        else
        {
            trail1.emitting = false;
            trail2.emitting = false;
            smoke1.enableEmission = false;
            smoke2.enableEmission = false;
            comboTimer += Time.deltaTime;
            if (comboTimer >= 0.7)
            {
                timer = 0f;
                currentMultiplier = "";
            }
        }
    }

    private void LightController()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (isLightActive)
            {
                transform.GetChild(4).transform.GetChild(2).transform.gameObject.SetActive(false);
                transform.GetChild(4).transform.GetChild(3).transform.gameObject.SetActive(false);
                isLightActive = false;
            }
            else
            {
                transform.GetChild(4).transform.GetChild(2).transform.gameObject.SetActive(true);
                transform.GetChild(4).transform.GetChild(3).transform.gameObject.SetActive(true);
                isLightActive = true;
            }
        }
    }

    private void TrailController()
    {
        if (!isGroundedRL)
            rearLeftWheelCollider.transform.GetChild(0).GetComponent<TrailRenderer>().emitting = false;
        else
            rearLeftWheelCollider.transform.GetChild(0).GetComponent<TrailRenderer>().emitting = true;
        if (!isGroundedRR)
            rearRightWheelCollider.transform.GetChild(0).GetComponent<TrailRenderer>().emitting = false;
        else
            rearRightWheelCollider.transform.GetChild(0).GetComponent<TrailRenderer>().emitting = true;
    }

    private void ResetPlayerCar()
    {
        if (Input.GetKeyDown(KeyCode.R) && !cameraScript.iffollow)
        {
            cameraScript.iffollow = true;
            isGroundedRL = false;
            isGroundedRR = false;
            transform.rotation = new Quaternion(transform.rotation.x, 180, transform.rotation.z, transform.rotation.w);
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        }
    }

    private void GetInput()
    {
        // Steering Input
        horizontalInput = joystick.Horizontal;

        // Acceleration Input
        verticalInput = joystick.Vertical;

        // Breaking Input
        isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        rearLeftWheelCollider.motorTorque = verticalInput * motorForce;
        rearRightWheelCollider.motorTorque = verticalInput * motorForce;
        if (isBreaking)
        {
            currentbreakForce = breakForce;
        }
        else
        {       
            currentbreakForce = 0f;
        }
        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        //  frontRightWheelCollider.brakeTorque = currentbreakForce;
        // frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
}