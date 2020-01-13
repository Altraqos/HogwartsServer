using UnityEngine;

public class NetWorkMoving : MonoBehaviour
{
    public float speed;
    public NetworkManager nManager;
    public string playerName;
    public GameObject startPos;
    Vector3 playerSpawnPos;

    public Animator anim;

    public float inputX;
    public float inputZ;

    public void OnEnable()
    {
        nManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        try
        {
            playerSpawnPos = startPos.transform.position;
        }
        catch
        {
            playerSpawnPos = new Vector3(0,0,0);
        }
        SendPosition();
    }

    void Update()
    {
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 5.0f;
        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);
        if (Input.GetKey(KeyCode.W))
            inputZ = 1.0f;
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
        anim.SetFloat("InputX", inputX);
        anim.SetFloat("InputZ", inputZ);

        if (nManager != null)
        {
            //if (x != 0 || z != 0)
                SendPosition();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "PlayArea")
            transform.position = playerSpawnPos;
    }

    public void SendPosition()
    {
        nManager.ForwardPlayerPos(
            nManager.playerID + "#" + 
            transform.position.x.ToString("n4") + "*" + 
            transform.position.y.ToString("n4") + "*" + 
            transform.position.z.ToString("n4") + "#" + 
            transform.rotation.eulerAngles.x.ToString("n4") + "*" + 
            transform.rotation.eulerAngles.y.ToString("n4") + "*" + 
            transform.rotation.eulerAngles.z.ToString("n4") + "#" +
            inputX + "*" + 
            inputZ);
    }
}
