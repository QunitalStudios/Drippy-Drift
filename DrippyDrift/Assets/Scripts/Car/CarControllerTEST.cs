using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarControllerTEST : MonoBehaviour
{
    private Rigidbody playerRB;
    public WheelColliders colliders;
    public WheelMeshes wheelMeshes;
    public WheelParticles wheelParticles;
    public float gasInput;
    public float brakeInput;
    public float steeringInput;
    public float motorPower;
    public float brakePower;
    public float slipAngle;
    private float speed;
    public AnimationCurve steeringCurve;
    public int maxSteerAngle;
    public float movingDirection;
    [SerializeField] private CameraAScript cameraScript;

    public float driftAngle;
    public float currentSpeed = 0f;
    public float playerScore = 0f;
    public string currentMultiplier = "";
    public Color currentMPColor = Color.red;

    public float timer = 0f;
    public float comboTimer = 0f;

    public bool isGroundedRL = false;
    public bool isGroundedRR = false;
    private bool isLightActive = true;

    [SerializeField] private ParticleSystem smoke1;
    [SerializeField] private ParticleSystem smoke2;
    [SerializeField] private TrailRenderer trail1;
    [SerializeField] private TrailRenderer trail2;

    // Start is called before the first frame update
    void Start()
    {
        playerRB = gameObject.GetComponent<Rigidbody>();
    }


    // Update is called once per frame

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }

        speed = playerRB.velocity.magnitude;
        currentSpeed = playerRB.velocity.magnitude * 2;

        isGroundedRR = Physics.Raycast(colliders.RRWheel.transform.position, -Vector3.up, 0.5f);
        isGroundedRL = Physics.Raycast(colliders.RLWheel.transform.position, -Vector3.up, 0.5f);

        CheckInput();
        ApplyMotor();
        ApplySteering();
        ApplyBrake();
        ApplyWheelPositions();
        TrailController();
        LightController();

        AddScore();

        if (colliders.RLWheel.motorTorque < 0 && Input.GetAxis("Vertical") < 0)
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

    void CheckInput()
    {
        gasInput = Input.GetAxisRaw("Vertical");      
        steeringInput = Input.GetAxis("Horizontal");
        
        slipAngle = Vector3.Angle(transform.forward, playerRB.velocity - transform.forward);

        movingDirection = Vector3.Dot(transform.forward, playerRB.velocity);
        if (movingDirection < -0.5f && gasInput > 0)
        {
            brakeInput = Mathf.Abs(gasInput);
        }
        else if (movingDirection > 0.5f && gasInput < 0)
        {
            brakeInput = Mathf.Abs(gasInput);
        }
        else
        {
            brakeInput = 0;
        }
    }

    public bool isGoingForward()
    {
        if (colliders.FLWheel.rpm >= 0 && colliders.FRWheel.rpm >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void AddScore()
    {
        if (((slipAngle >= 15) && (slipAngle <= 90)) && isGoingForward() && cameraScript.iffollow == true && isGroundedRL && isGroundedRR && currentSpeed > 1)
        {
            if (isGroundedRL && isGroundedRR)
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
                playerScore += (slipAngle * 3 * Time.deltaTime) * 0.7f;
            }
            if (timer >= 3 && timer < 5)
            {
                currentMultiplier = "x2";
                playerScore += (slipAngle * 3 * Time.deltaTime) * 0.9f;
                currentMPColor = Color.yellow;
            }
            if (timer >= 5 && timer < 7)
            {
                currentMultiplier = "x3";
                playerScore += (slipAngle * 3 * Time.deltaTime) * 2;
                currentMPColor = Color.green;
            }
            if (timer >= 7 && timer < 9)
            {
                currentMultiplier = "x4";
                playerScore += (slipAngle * 3 * Time.deltaTime) * 4;
                currentMPColor = Color.blue;
            }
            if (timer >= 9)
            {
                currentMultiplier = "x5";
                playerScore += (slipAngle * 3 * Time.deltaTime) * 8;
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

    public bool fsad=false;
    void ApplyBrake()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            colliders.RRWheel.brakeTorque = brakePower ;
            colliders.RLWheel.brakeTorque = brakePower ;
            fsad = true;
        }
        else
        {
            fsad = false;
            colliders.FRWheel.brakeTorque = brakeInput * brakePower * 0.7f;
            colliders.FLWheel.brakeTorque = brakeInput * brakePower * 0.7f;
            colliders.RRWheel.brakeTorque = brakeInput * brakePower * 0.3f;
            colliders.RLWheel.brakeTorque = brakeInput * brakePower * 0.3f;
        }
    }
    void ApplyMotor()
    {

        colliders.RRWheel.motorTorque = motorPower * gasInput;
        colliders.RLWheel.motorTorque = motorPower * gasInput;

    }
    void ApplySteering()
    {

        float steeringAngle = steeringInput * maxSteerAngle * steeringCurve.Evaluate(speed);
        if (slipAngle < 120f && movingDirection>0f )
        {
            steeringAngle += Vector3.SignedAngle(transform.forward*0.1f, playerRB.velocity * 0.1f + transform.forward, Vector3.up);
        }
        steeringAngle = Mathf.Clamp(steeringAngle, -90f, 90f);
        colliders.FRWheel.steerAngle = steeringAngle;
        colliders.FLWheel.steerAngle = steeringAngle;
    }

    void ApplyWheelPositions()
    {
        UpdateWheel(colliders.FRWheel, wheelMeshes.FRWheel);
        UpdateWheel(colliders.FLWheel, wheelMeshes.FLWheel);
        UpdateWheel(colliders.RRWheel, wheelMeshes.RRWheel);
        UpdateWheel(colliders.RLWheel, wheelMeshes.RLWheel);
    }
   
    void UpdateWheel(WheelCollider coll, MeshRenderer wheelMesh)
    {
        Quaternion quat;
        Vector3 position;
        coll.GetWorldPose(out position, out quat);
        wheelMesh.transform.position = position;
        wheelMesh.transform.rotation = quat;
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
            colliders.RLWheel.transform.GetChild(0).GetComponent<TrailRenderer>().emitting = false;
        else
            colliders.RLWheel.transform.GetChild(0).GetComponent<TrailRenderer>().emitting = true;
        if (!isGroundedRR)
            colliders.RRWheel.transform.GetChild(0).GetComponent<TrailRenderer>().emitting = false;
        else
            colliders.RRWheel.transform.GetChild(0).GetComponent<TrailRenderer>().emitting = true;
    }
}
[System.Serializable]
public class WheelColliders
{
    public WheelCollider FRWheel;
    public WheelCollider FLWheel;
    public WheelCollider RRWheel;
    public WheelCollider RLWheel;
}
[System.Serializable]
public class WheelMeshes
{
    public MeshRenderer FRWheel;
    public MeshRenderer FLWheel;
    public MeshRenderer RRWheel;
    public MeshRenderer RLWheel;
}
[System.Serializable]
public class WheelParticles
{
    public ParticleSystem FRWheel;
    public ParticleSystem FLWheel;
    public ParticleSystem RRWheel;
    public ParticleSystem RLWheel;

}