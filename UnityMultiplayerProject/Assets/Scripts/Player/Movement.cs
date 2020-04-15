using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 10f;
    public MouseLook playerLook;
    public Controller controller;
    public float walkSpeed;
    public float sprintSpeed;
    public float maxSpeed;

    private void Update()
    {
        float _xMove = Input.GetAxisRaw("Horizontal");
        float _zMove = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _xMove;
        Vector3 _moveVertical = transform.forward * _zMove;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * speed;

        controller.Move(_velocity);
    }

    void Sprint()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            maxSpeed = sprintSpeed;
        if (Input.GetKeyUp(KeyCode.LeftShift))
            maxSpeed = walkSpeed;
    }
}
