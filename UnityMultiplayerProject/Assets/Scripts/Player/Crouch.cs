using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Crouch : MonoBehaviour
{
    private CharacterController m_CharacterController;
    public KeyCode crouchKey = KeyCode.LeftAlt;
    public PlayerMovement pMovement;
    public bool m_Crouch = false;
    private float m_OriginalHeight;
    public float m_CrouchHeight = 0.5f;
    public float crouchTimeDown = 1.0f;
    public float crouchTimeUp = 1.0f;
    private bool isCeiling = false;


    private void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_OriginalHeight = m_CharacterController.height;
    }

    
    void Update()
    {
        if (Input.GetKey(crouchKey) )
        {
            if (!isCeiling) {
                m_Crouch = !m_Crouch ? true : false;
                CheckCrouch();
            }           
        }
        if (Input.GetKey(crouchKey))
        {
            m_Crouch = true;
            CheckCrouch();
        }
        else
        {
            m_Crouch = false;
            CheckCrouch();
        }

        

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out hit, 2.0f))
        {
            if (hit.collider.gameObject != null)
            {
                isCeiling = true;
            }       
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * 2, Color.yellow);
        }
        else
        {
            isCeiling = false;
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * 2, Color.white);
           // Debug.Log("Did not Hit");
        }
    }

    void CheckCrouch()
    {
        if (m_Crouch == true)
        {
            m_CharacterController.height = m_CrouchHeight;
            crouchTimeUp = crouchTimeDown;
            gameObject.GetComponent<PlayerMovement>().m_WalkSpeed = 2.0f;
            gameObject.GetComponent<PlayerMovement>().m_RunSpeed = 2.0f;
        }
        else
        {
            m_CharacterController.height = m_OriginalHeight;
            //pMovement.m_WalkSpeed = 4.0f;
            //pMovement.m_RunSpeed = 7.0f;
        }
    }
}
