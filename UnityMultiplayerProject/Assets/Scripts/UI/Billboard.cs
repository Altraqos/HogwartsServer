using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Camera m_Camera;

    private void OnEnable()
    {
        m_Camera = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Camera>();
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward, m_Camera.transform.rotation * Vector3.up);
    }
}