using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private bool m_IsWalking;
        [SerializeField] private bool m_IsCrouching;
        [SerializeField] public float m_WalkSpeed;
        [SerializeField] public float m_RunSpeed;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private float m_StepInterval;
        [SerializeField] private AudioClip m_JumpSound;
        [SerializeField] private AudioClip m_LandSound;

        public Image currentHealthbar;
        public Text ratioText;
        public string level;
        public float stamina = 100f;
        public float health = 100f;
        public Camera m_Camera;
        private bool m_Jump;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;
        private AudioSource m_AudioSource;
        public NetWorkMoving networkMoving;

        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle / 2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
            m_MouseLook.Init(transform, m_Camera.transform);
            networkMoving = GetComponent<NetWorkMoving>();

        }

        private void Update()
        {
            if (stamina < 100)
                stamina += 15f * Time.deltaTime;
            if (Input.GetKey(KeyCode.LeftShift))
                stamina -= 30f * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, 100);
            RotateView();
            if (!m_Jump && m_CharacterController.isGrounded)
                m_Jump = Input.GetButtonDown("Jump");
            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
                m_MoveDir.y = 0f;
            m_PreviouslyGrounded = m_CharacterController.isGrounded;
        }

        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }

        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);
            Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo, m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;
            m_MoveDir.x = desiredMove.x * speed;
            m_MoveDir.z = desiredMove.z * speed;
            if (m_CharacterController.isGrounded)
            {
                networkMoving.isGrounded = true;
                m_MoveDir.y = -m_StickToGroundForce;
                if (m_Jump)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    PlayJumpSound();
                    m_Jump = false;
                    m_Jumping = true;
                }
            }
            else
            { 
                m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
            networkMoving.isGrounded = false;
        }
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);
            ProgressStepCycle(speed);
            m_MouseLook.UpdateCursorLock();
           
        }

        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }

        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * (m_IsWalking ? 1f : m_RunstepLenghten))) *
                             Time.fixedDeltaTime;
            }
            if (!(m_StepCycle > m_NextStep))
                return;
            m_NextStep = m_StepCycle + m_StepInterval;
        }

        private void GetInput(out float speed)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
            m_IsCrouching = Input.GetKey(KeyCode.LeftControl);
            speed = m_IsCrouching ? m_WalkSpeed : m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);
            if (m_Input.sqrMagnitude > 1)
                m_Input.Normalize();
        }

        private void RotateView()
        {
            m_MouseLook.LookRotation(transform, m_Camera.transform);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            if (m_CollisionFlags == CollisionFlags.Below)
                return;
            if (body == null || body.isKinematic)
                return;
            body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }
    }
}