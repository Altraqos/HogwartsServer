using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoints : MonoBehaviour
{
    public Transform[] wayPointList;
    public int currentWayPoint = 0;
    Transform targetWayPoint;
    public float speed = 4f;

    void Update()
    {
        if (currentWayPoint < wayPointList.Length)
        {
            if (targetWayPoint == null)
                targetWayPoint = wayPointList[currentWayPoint];
            walk();
        }
        else
        {
            targetWayPoint = wayPointList[0];
            walk();
        }
    }

    void walk()
    {
        transform.forward = Vector3.RotateTowards(transform.forward, targetWayPoint.position - transform.position, 1 * Time.deltaTime, 0.0f);
        transform.position = Vector3.MoveTowards(transform.position, targetWayPoint.position, speed * Time.deltaTime);
        if (transform.position == targetWayPoint.position)
        {
            currentWayPoint++;
            targetWayPoint = wayPointList[currentWayPoint];
        }
    }
}