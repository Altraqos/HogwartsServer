using UnityEngine;

public class Spinning : MonoBehaviour
{
    public float RotateX = 20;
    public float RotateY = 20;
    public float RotateZ = 20;
    public float rotateSpeed = 10;
    public int ID = 0;
    public Transform[] wayPointList;
    public int currentWayPoint = 0;
    Transform targetWayPoint;
    public float moveSpeed = 4f;
    public NetworkManager nManager;

    public void OnEnable()
    {
        nManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        if (nManager != null)
            SendPosition();
    }

    void Update()
    {
        transform.Rotate(new Vector3(RotateX, RotateY, RotateZ), rotateSpeed / 10);
        if (currentWayPoint < wayPointList.Length)
        {
            if (targetWayPoint == null)
                targetWayPoint = wayPointList[currentWayPoint];
            walk();
        }
        else
        {
            targetWayPoint = wayPointList[0];
            currentWayPoint = 0;
            walk();
        }
    }

    void walk()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetWayPoint.position, moveSpeed * Time.deltaTime);
        if (transform.position == targetWayPoint.position && currentWayPoint < wayPointList.Length +1)
        {
            currentWayPoint++;
            targetWayPoint = wayPointList[currentWayPoint - 1];
        }
    }

    public void SendPosition()
    {
        nManager.ForwardPlayerPos(
            ID + "#" +
            transform.position.x.ToString("n4") + "*" +
            transform.position.y.ToString("n4") + "*" +
            transform.position.z.ToString("n4") + "#" +
            transform.rotation.eulerAngles.x.ToString("n4") + "*" +
            transform.rotation.eulerAngles.y.ToString("n4") + "*" +
            transform.rotation.eulerAngles.z.ToString("n4") + "#" +
            0 + "*" +
            0 + "*" +
            false + "*" +
            false + "*" +
            false + "*" +
            false);
    }
}
