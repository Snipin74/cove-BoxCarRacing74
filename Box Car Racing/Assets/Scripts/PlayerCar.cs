using UnityEngine;

public class PlayerCar : MonoBehaviour
{
    public float maxTurnAngle = 10;
    public float maxTorque = 10;
    public float maxBrakeTorque = 100;
    private bool applyHandBrake = false;
    public float handbrakeForwardSlip = 0.04f;
    public float handbrakesidwaysSlip = 0.08f;

    public WheelCollider FrontRight;
    public WheelCollider FrontLeft;
    public WheelCollider RightRear;
    public WheelCollider LeftRear;
    public Rigidbody rigidbody;

    public float spoilerRatio = 0.1f;

    public Transform wheelTransformFr;
    public Transform wheelTransformFl;
    public Transform wheelTransformBr;
    public Transform wheelTransformBl;

    public float declerationTorque = 30;

    private float CurrentSpeed = 0;
    public float maxSpeed = 150;

    public GameObject leftBrakeLight;
    public GameObject RightBrakeLight;
    public Texture2D idleLightTex;
    public Texture2D brakeLightTex;
    public Texture2D ReverseLightTex;

    public Texture2D speedometer;
    public Texture2D needle;

    private void Update()
    {
        float rotationThisFrame = 360 * Time.deltaTime;
        wheelTransformFr.Rotate(0, -FrontRight.rpm / rotationThisFrame, 0);
        wheelTransformFl.Rotate(0, -FrontLeft.rpm / rotationThisFrame, 0);
        wheelTransformBr.Rotate(0, -RightRear.rpm / rotationThisFrame, 0);
        wheelTransformBl.Rotate(0, -LeftRear.rpm / rotationThisFrame, 0);

        //adjust the wheels heights based on the suspension
        UpdateWheelPosition();

        //Determine What texture to use
        DetermineBrakeLightState();
    }

    void DetermineBrakeLightState()
    {
        if((CurrentSpeed > 0 && Input.GetAxis("Vertical") < 0) || (CurrentSpeed < 0 && Input.GetAxis("Vertical") > 0)
            || applyHandBrake)
        {
            leftBrakeLight.GetComponent<Renderer>().material.mainTexture = brakeLightTex;
            RightBrakeLight.GetComponent<Renderer>().material.mainTexture = brakeLightTex;
        }
        else if (CurrentSpeed < 0 && Input.GetAxis("Vertical") < 0)
        {
            leftBrakeLight.GetComponent<Renderer>().material.mainTexture = ReverseLightTex;
            RightBrakeLight.GetComponent<Renderer>().material.mainTexture = ReverseLightTex;
        }
        else
        {
            leftBrakeLight.GetComponent<Renderer>().material.mainTexture = idleLightTex;
            RightBrakeLight.GetComponent<Renderer>().material.mainTexture = idleLightTex;
        }
    }

    void UpdateWheelPosition()
    {
        WheelHit contact = new WheelHit();

        if(FrontRight.GetGroundHit(out contact))
        {
            Vector3 temp = FrontRight.transform.position;
            temp.y = (contact.point + (FrontRight.transform.up * FrontRight.radius)).y;
            wheelTransformFr.position = temp;
        }
        if (FrontLeft.GetGroundHit(out contact))
        {
            Vector3 temp = FrontLeft.transform.position;
            temp.y = (contact.point + (FrontLeft.transform.up * FrontLeft.radius)).y;
            wheelTransformFl.position = temp;
        }
        if (RightRear.GetGroundHit(out contact))
        {
            Vector3 temp = RightRear.transform.position;
            temp.y = (contact.point + (RightRear.transform.up * RightRear.radius)).y;
            wheelTransformBr.position = temp;
        }
        if (FrontRight.GetGroundHit(out contact))
        {
            Vector3 temp = LeftRear.transform.position;
            temp.y = (contact.point + (LeftRear.transform.up * LeftRear.radius)).y;
            wheelTransformBl.position = temp;
        }

    }

    public Vector3 centerOfMassAdjustment = new Vector3(0f, -0.9f, 0f);
    
    private void Start()
    {
        //lower center of mass for roll-over resistance
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.centerOfMass += centerOfMassAdjustment;
    }

    void SetSlipValues(float forward, float sideways)
    {
        //change the stiffness values of wheel  friction curve and the reapply it.
        WheelFrictionCurve tempStruct = LeftRear.forwardFriction;
        tempStruct.stiffness = forward;
        LeftRear.forwardFriction = tempStruct;

        tempStruct = LeftRear.sidewaysFriction;
        tempStruct.stiffness = sideways;
        LeftRear.sidewaysFriction = tempStruct;

        tempStruct = RightRear.forwardFriction;
        tempStruct.stiffness = forward;
        RightRear.forwardFriction = tempStruct;

        tempStruct = RightRear.sidewaysFriction;
        tempStruct.stiffness = sideways;
        RightRear.sidewaysFriction = tempStruct;
            
    }

    //FixedUpdate is called once per Physics frame
    private void FixedUpdate()
    {
        //handbrake controls
        if (Input.GetButton("Jump"))
        {
            applyHandBrake = true;
            FrontRight.brakeTorque = maxBrakeTorque;
            FrontLeft.brakeTorque = maxBrakeTorque;

            //Power slide
            if(GetComponent<Rigidbody>().velocity.magnitude > 1)
            {
                SetSlipValues(handbrakeForwardSlip, handbrakesidwaysSlip);
            }
            else // skid to a stop, regular friction enabled.
            {
                SetSlipValues(1f, 1f);
            }
        }
        else
        {
            applyHandBrake = false;
            FrontRight.brakeTorque = 0;
            FrontLeft.brakeTorque = 0;
            SetSlipValues(1f, 1f);
        }

        //Front wheel steering
        FrontRight.steerAngle = Input.GetAxis("Horizontal") * maxTurnAngle;
        FrontLeft.steerAngle = Input.GetAxis("Horizontal") * maxTurnAngle;

        //rear wheel steering
        RightRear.motorTorque = Input.GetAxis("Vertical") * maxTorque;
        LeftRear.motorTorque = Input.GetAxis("Vertical") * maxTorque;

        //spoilers add down pressure based on the cars speed. (upside-down lift)
        Vector3 localVelocity = transform.InverseTransformDirection(rigidbody.velocity);
        rigidbody.AddForce(-transform.up * (localVelocity.z * spoilerRatio), ForceMode.Impulse);

        //apply decleration when pressing the break or lightly when not pressing the gas.
        if(!applyHandBrake && ((Input.GetAxis("Vertical") <= -0.5f && localVelocity.z > 0) ||
            (Input.GetAxis("Vertical") <= -0.5f && localVelocity.z > 0)))
        {
            RightRear.brakeTorque = declerationTorque + maxTorque;
            LeftRear.brakeTorque = declerationTorque + maxTorque;
        }
        else if(!applyHandBrake && Input.GetAxis("Vertical") == 0)
        {
            RightRear.brakeTorque = declerationTorque;
            LeftRear.brakeTorque = declerationTorque;
        }
        else
        {
            RightRear.brakeTorque = 0;
            LeftRear.brakeTorque = 0;
        }

        //calculate max speed in km/h (condensed calculation)
        CurrentSpeed = RightRear.radius * RightRear.rpm * Mathf.PI * 0.12f;
        if(CurrentSpeed < maxSpeed)
        {
            //rear wheel drive.
            RightRear.motorTorque = Input.GetAxis("Vertical") * maxTorque;
            LeftRear.motorTorque = Input.GetAxis("Vertical") * maxTorque;
        }
        else
        {
            //cant go faster, already at top speed that engine produces
            RightRear.motorTorque = 0;
            LeftRear.motorTorque = 0;
        }
    }
    void OnGUI()
    {
        GUI.DrawTexture(new Rect(Screen.width - 300, Screen.height - 150, 300, 150), speedometer);
        float speedFactor = CurrentSpeed / maxSpeed;
        float rotationAngle = Mathf.Lerp(0, 180, Mathf.Abs(speedFactor));
        GUIUtility.RotateAroundPivot(rotationAngle, new Vector2(Screen.width - 150, Screen.height));
        GUI.DrawTexture(new Rect(Screen.width - 300, Screen.height - 150, 300, 300), needle);
    }


}
