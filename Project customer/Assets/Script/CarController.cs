using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentbreakForce;
    private bool isBreaking;
    private bool phoneUp=false;

    private Rigidbody rb;
    private BoxCollider bc;

    [SerializeField] private AudioSource music;
    [SerializeField] private GameObject phonePrompt;


    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;
    [SerializeField] private float speedLimit;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheeTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    [SerializeField] private Transform steeringWheel;
    [SerializeField] private Transform dashBoard;
    [SerializeField] private Transform phone;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();
    }
    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        
    }
    void Update()
    {
        handlePhone();

        if (!bc.enabled)
        {
            music.Stop();
            
        }
        if(!bc.enabled || Input.GetKeyDown(KeyCode.E))
        {
            phonePrompt.SetActive(false);
        }
    }
    private void handlePhone()
    {
        if (Input.GetKeyUp(KeyCode.E) && phoneUp == false)
        {
            phone.transform.localPosition = Vector3.Lerp(phone.transform.localPosition, new Vector3(phone.transform.localPosition.x, 1.1f, phone.transform.localPosition.z), 1);
            phoneUp = true;
        }
        else
        if (Input.GetKeyUp(KeyCode.E) && phoneUp)
        {
            phone.transform.localPosition = Vector3.Lerp(phone.transform.localPosition, new Vector3(phone.transform.localPosition.x, 0, phone.transform.localPosition.z), 1);
            phoneUp = false;
        }
    }
    private void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBreaking = Input.GetKey(KeyCode.Space);
        
    }


    
    private void HandleMotor()
    {

        //adding force to the two front wheels 
        rearLeftWheelCollider.motorTorque = verticalInput * motorForce;
        rearRightWheelCollider.motorTorque = verticalInput * motorForce;
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce;

        //cheking if the breaking or not, if not break force equal 0
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();

        //update dash board
        dashBoard.eulerAngles = new Vector3(dashBoard.eulerAngles.x,
        dashBoard.eulerAngles.y, 80-(rb.velocity.magnitude*15));
    }

    private void ApplyBreaking()
    {
        //limit speed
        if(rb.velocity.magnitude > speedLimit)
        {
            Vector3 normalisedVelocity = rb.velocity.normalized;
            Vector3 brakeVelocity = normalisedVelocity * 20000;

            rb.AddForce(-brakeVelocity);
        }
        
        //break force
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;

        
    }

    private void HandleSteering()
    {
        //check for input and steer the wheel with maximum angle
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;

        //match steering wheel model to input
        steeringWheel.eulerAngles = new Vector3(steeringWheel.eulerAngles.x,
        steeringWheel.eulerAngles.y, -currentSteerAngle);
    }

    private void UpdateWheels()
    {
        //update wheels visiual
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheeTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot; 
        //get position of wheels and tranform
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
}
