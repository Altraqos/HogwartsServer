using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRController : MonoBehaviour
{
    public bool leftHanded = false;
    public bool isActive = true;

    public float Sensitivity = 0.1f;
    public float maxSpeed = 1;
    public float gravity = 30;
    public float rotationIncrement = 45;

    public SteamVR_Action_Boolean rotatePressLeft;
    public SteamVR_Action_Boolean rotatePressRight;
    public SteamVR_Action_Boolean movePress;
    public SteamVR_Action_Vector2 moveValue;
    public SteamVR_Action_Boolean gripPress;

    public GameObject wandObject;

    public Transform leftHand;
    public Transform rightHand;

    private float speed = 0;

    private CharacterController cController;
    
    private Transform headPos;


    void Awake()
    {
        cController = GetComponent<CharacterController>();
        setHand(leftHanded);
    }

    void Start()
    {
        headPos = SteamVR_Render.Top().head;
    }

    public void setHand(bool isLeft)
    {
        if (isLeft)
        {
            wandObject.transform.position = leftHand.position;
            wandObject.transform.parent = leftHand;
        }
        if (!isLeft)
        {
            wandObject.transform.position = rightHand.position;
            wandObject.transform.parent = rightHand;
        }
    }

    void Update()
    {
        handleHeight();
        calculateMovement();
        snapRotation();
        useGrip();
    }

    public void useGrip()
    {
        if (leftHanded)
        {
            if (gripPress.GetStateDown(SteamVR_Input_Sources.LeftHand))
            {
                isActive = !isActive;
                wandObject.SetActive(isActive);
            }
        }
        if (!leftHanded)
        {
            if (gripPress.GetStateDown(SteamVR_Input_Sources.RightHand))
            { 
                isActive = !isActive;
                wandObject.SetActive(isActive);
            }
        }
    }

    public void handleHeight()
    {
        float headHeight = Mathf.Clamp(headPos.localPosition.y, 1, 2);
        cController.height = headHeight;
        Vector3 newCenter = Vector3.zero;
        newCenter.y = cController.height / 2;
        newCenter.y += cController.skinWidth;
        newCenter.x = headPos.localPosition.x;
        newCenter.z = headPos.localPosition.z;
        cController.center = newCenter;
    }

    public void calculateMovement()
    {
        Quaternion orientation = calculateOrientation();
        Vector3 movement = Vector3.zero;
        if (leftHanded)
        {
            if (moveValue.GetAxis(SteamVR_Input_Sources.LeftHand) == new Vector2(0, 0))
                speed = 0;

            speed += moveValue.axis.magnitude * Sensitivity;
            speed = Mathf.Clamp(speed, -maxSpeed, maxSpeed);
            movement += orientation * (speed * Vector3.forward);
        }
        if (!leftHanded)
        {
            if (moveValue.GetAxis(SteamVR_Input_Sources.RightHand) == new Vector2(0, 0))
                speed = 0;

            speed += moveValue.axis.magnitude * Sensitivity;
            speed = Mathf.Clamp(speed, -maxSpeed, maxSpeed);
            movement += orientation * (speed * Vector3.forward);
        }
        movement.y -= gravity * Time.deltaTime;
        cController.Move(movement * Time.deltaTime);
    }

    public Quaternion calculateOrientation()
    {
        float rotation = Mathf.Atan2(moveValue.axis.x, moveValue.axis.y);
        rotation *= Mathf.Rad2Deg;
        Vector3 orientationEuler = new Vector3(0, headPos.eulerAngles.y + rotation, 0);
        return Quaternion.Euler(orientationEuler);
    }

    public void snapRotation()
    {
        float snapVal = 0;
        if (leftHanded)
        {
            if (rotatePressLeft.GetStateDown(SteamVR_Input_Sources.LeftHand))
                snapVal = -Mathf.Abs(rotationIncrement);
            if (rotatePressRight.GetStateDown(SteamVR_Input_Sources.LeftHand))
                snapVal = Mathf.Abs(rotationIncrement);
        }
        if(!leftHanded)
        {
            if (rotatePressLeft.GetStateDown(SteamVR_Input_Sources.RightHand))
                snapVal = -Mathf.Abs(rotationIncrement);
            if (rotatePressRight.GetStateDown(SteamVR_Input_Sources.RightHand))
                snapVal = Mathf.Abs(rotationIncrement);
        }
        transform.RotateAround(headPos.position, Vector3.up, snapVal);
    }
}
