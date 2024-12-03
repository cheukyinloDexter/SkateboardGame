using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkateboardController : MonoBehaviour
{
    public Transform m_skateboard;
    public float m_alignSpeed = 5;
    public float m_rayDistance = 5f;
    [Range(-1, 1)]
    public float m_Forward;
    public float m_FwdForce = 10;
    private Vector3 m_surfaceNormal = new Vector3();
    private Vector3 m_collisionPoint = new Vector3();
    public bool m_useRaycast = true;
    public bool m_onSurface;
    private Collision m_surfaceCollisionInfo;
    private Rigidbody m_rigidbody;

    private Vector3 _startDragPosition;
    private Vector3 _releasePosition;
    private bool _isDragging = false;
    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //ProcessInputs();
    }

    private void FixedUpdate()
    {
        AlignToSurface();
        //ProcessForce();
    }

    private void ProcessInputs()
    {
        //m_Forward = Input.GetAxis("Vertical");
    }

    private void ProcessForce()
    {
        if (!m_onSurface)
            return;

        m_rigidbody.AddForce(m_skateboard.forward * m_FwdForce * m_Forward);
    }

    private void OnCollisionStay(Collision other)
    {
        m_onSurface = true;
        m_surfaceCollisionInfo = other;
        m_surfaceNormal = other.GetContact(0).normal;
        m_collisionPoint = other.GetContact(0).point;
    }

    private void OnCollisionExit(Collision other)
    {
        m_surfaceCollisionInfo = null;
        m_onSurface = false;
    }

    void AlignToSurface()
    {
        if (m_useRaycast)
        {
            var hit = new RaycastHit();
            var onSurface = Physics.Raycast(transform.position, Vector3.down, out hit, m_rayDistance);
            if (onSurface)
            {
                var localRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                var euler = localRot.eulerAngles;
                euler.y = 0;
                localRot.eulerAngles = euler;
                m_skateboard.localRotation = Quaternion.LerpUnclamped(m_skateboard.localRotation, localRot, m_alignSpeed * Time.fixedDeltaTime);
            }
        }
        else
        {
            if (m_onSurface)
            {
                var localRot = Quaternion.FromToRotation(transform.up, m_surfaceNormal) * transform.rotation;
                var euler = localRot.eulerAngles;
                euler.y = 0;
                localRot.eulerAngles = euler;
                m_skateboard.localRotation = Quaternion.LerpUnclamped(m_skateboard.localRotation, localRot, m_alignSpeed * Time.fixedDeltaTime);
            }
        }
    }

    public void Flip()
    {
        // rotate m_skateboard 360 degree in z axis in 1/4 of a sec
    }

    void OnMouseDown()
    {
        // Store the position where the drag starts
        _startDragPosition = Input.mousePosition;
        _isDragging = true;
    }

    void OnMouseDrag()
    {
        // Optional: You can add visual feedback for the drag
        // maybe add a line renderer of the trajectory
    }

    void OnMouseUp()
    {
        if (!m_onSurface)
            return;

        if (_isDragging)
        {
            // Store the position where the drag ends
            _releasePosition = Input.mousePosition;

            // Calculate the drag direction
            Vector3 dragDirection = _releasePosition - _startDragPosition;

            // Convert the drag direction to world space (you may need to adjust this based on your camera orientation)
            Vector3 worldDirection = m_skateboard.forward * -dragDirection.y;

            // Calculate the magnitude of the force
            float forceMagnitude = worldDirection.magnitude * m_FwdForce;

            // Cap the force to a maximum value
            float maxForce = 400f; // Replace this with your desired maximum force value
            if (forceMagnitude > maxForce)
            {
                // Scale the direction vector to maintain direction but cap the force
                worldDirection = worldDirection.normalized * maxForce;
            }

            // Apply the force
            m_rigidbody.AddForce(worldDirection);

            Debug.Log($"Force applied: {worldDirection} (magnitude: {worldDirection.magnitude})");
            _isDragging = false;
        }
    }


}
