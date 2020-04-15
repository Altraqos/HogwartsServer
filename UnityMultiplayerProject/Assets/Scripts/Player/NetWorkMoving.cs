using UnityEngine;

public class NetWorkMoving : MonoBehaviour
{
    public int playerID;
    public bool isHost;
    public NetworkManager nManager;
    public string playerName;
    public GameObject startPos;
    public Animator anim;
    public float inputX;
    public float inputZ;
    public bool isShooting = false;
    public bool isAiming = false;
    public bool isSprinting = false;
    public bool isGrounded;
    public bool isJumping;
    public float JumpForce = 1000;
    public bool isVR = false;
    public GameObject LeftHand;
    public GameObject RightHand;
    public GameObject Body;
    public GameObject Head;

    public void OnEnable()
    {
        nManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        if (nManager != null)
            SendPosition();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            if (Input.GetKey(KeyCode.LeftShift))
                inputZ = 2.0f;
            else
                inputZ = 1.0f;
        }
        else if (Input.GetKey(KeyCode.S))
            inputZ = -1.0f;
        else
            inputZ = 0.0f;
        if (Input.GetKey(KeyCode.A))
            inputX = -1.0f;
        else if (Input.GetKey(KeyCode.D))
            inputX = 1.0f;
        else
            inputX = 0.0f;
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            anim.SetBool("isAiming", true);
            isAiming = true;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            anim.SetBool("isShooting", true);
            isShooting = true;
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            anim.SetBool("isAiming", false);
            isAiming = false;
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            anim.SetBool("isShooting", false);
            isShooting = false;
        }
        if (isGrounded)
        {
            anim.SetBool("isGrounded", true);
            anim.SetBool("isJumping", false);
            isJumping = false;
        }
        else if (!isGrounded)
        {
            anim.SetBool("isGrounded", false);
            anim.SetBool("isJumping", true);
            isJumping = true;
        }

        anim.SetFloat("InputX", inputX);
        anim.SetFloat("InputZ", inputZ);
        if (nManager != null)
            SendPosition();
    }

    public void SendPosition()
    {
        if (!isVR)
            nManager.ForwardPlayerPos(
                isVR + "#" +
                nManager.playerID + "#" + 
                transform.position.x.ToString("n4") + "*" +
                transform.position.y.ToString("n4") + "*" +
                transform.position.z.ToString("n4") + "#" +
                transform.rotation.eulerAngles.x.ToString("n4") + "*" +
                transform.rotation.eulerAngles.y.ToString("n4") + "*" +
                transform.rotation.eulerAngles.z.ToString("n4") + "#" +
                inputX + "*" +
                inputZ + "*" +
                isAiming + "*" +
                isShooting + "*" +
                isJumping + "*" +
                isGrounded);
        if (isVR)
            nManager.ForwardPlayerPos(
                isVR + "#" +
                nManager.playerID + "#" + 
                LeftHand.transform.position.x.ToString("n4") + "*" +
                LeftHand.transform.position.y.ToString("n4") + "*" +
                LeftHand.transform.position.z.ToString("n4") + "#" +
                LeftHand.transform.rotation.eulerAngles.x.ToString("n4") + "*" +
                LeftHand.transform.rotation.eulerAngles.y.ToString("n4") + "*" +
                LeftHand.transform.rotation.eulerAngles.z.ToString("n4") + "#" +

                RightHand.transform.position.x.ToString("n4") + "*" +
                RightHand.transform.position.y.ToString("n4") + "*" +
                RightHand.transform.position.z.ToString("n4") + "#" +
                RightHand.transform.rotation.eulerAngles.x.ToString("n4") + "*" +
                RightHand.transform.rotation.eulerAngles.y.ToString("n4") + "*" +
                RightHand.transform.rotation.eulerAngles.z.ToString("n4") + "#" +

                Body.transform.position.x.ToString("n4") + "*" +
                Body.transform.position.y.ToString("n4") + "*" +
                Body.transform.position.z.ToString("n4") + "#" +
                Body.transform.rotation.eulerAngles.x.ToString("n4") + "*" +
                Body.transform.rotation.eulerAngles.y.ToString("n4") + "*" +
                Body.transform.rotation.eulerAngles.z.ToString("n4") + "#" +

                Head.transform.position.x.ToString("n4") + "*" +
                Head.transform.position.y.ToString("n4") + "*" +
                Head.transform.position.z.ToString("n4") + "#" +
                Head.transform.rotation.eulerAngles.x.ToString("n4") + "*" +
                Head.transform.rotation.eulerAngles.y.ToString("n4") + "*" +
                Head.transform.rotation.eulerAngles.z.ToString("n4") + "#" +

                inputX + "*" +
                inputZ + "*" +
                isAiming + "*" +
                isShooting + "*" +
                isJumping + "*" +
                isGrounded);
    }
}
