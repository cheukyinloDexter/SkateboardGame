using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;
using UnityEngine;
using Unity.VisualScripting;

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

    public bool IsFlipping = false; // Tracks if the skateboard is flipping
    public float flipDuration = 0.25f; // Time taken to complete the flip
    public float flipStartTime = 0.125f;
    public float flipElapsedTime = 0f; // Tracks time during the flip
    public Quaternion initialFlipRotation; // Initial rotation before flip
    public Quaternion targetFlipRotation; // Final rotation after flip

    [SerializeField] private Rig rig;
    [SerializeField] private GameObject leftLegTarget;
    [SerializeField] private GameObject rightLegTarget;

    private TwoBoneIKConstraint leftLegIk;
    private MultiRotationConstraint leftLegRotation;

    private TwoBoneIKConstraint rightLegIk;
    private MultiRotationConstraint rightLegRotation;


    [SerializeField] private ManageIK headIK;

    // a list or targets and aim at the closest

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();

        if (leftLegTarget != null || rightLegTarget != null)
        {
            leftLegIk = leftLegTarget.GetComponent<TwoBoneIKConstraint>();
            leftLegRotation = leftLegTarget.GetComponent<MultiRotationConstraint>();
            rightLegIk = rightLegTarget.GetComponent<TwoBoneIKConstraint>();
            rightLegRotation = rightLegTarget.GetComponent<MultiRotationConstraint>();

        }
    }

    // Update is called once per frame
    void Update()
    {
        //ProcessInputs();
        if(!IsFlipping)
        {
            //if (rig != null)
            //{
            //    rig.weight = Mathf.MoveTowards(rig.weight, 1f, Time.deltaTime * 25f);
            //    headIK.lookWeight = Mathf.MoveTowards(headIK.lookWeight, 1f, Time.deltaTime * 3f);
            //}
            if (leftLegIk != null)
            {
                leftLegIk.weight = Mathf.MoveTowards(leftLegIk.weight, 1f, Time.deltaTime * 7f);
                leftLegRotation.weight = Mathf.MoveTowards(leftLegRotation.weight, 1f, Time.deltaTime * 7f);

                rightLegIk.weight = Mathf.MoveTowards(rightLegIk.weight, 1f, Time.deltaTime * 7f);
                rightLegRotation.weight = Mathf.MoveTowards(rightLegRotation.weight, 1f, Time.deltaTime * 7f);

                headIK.lookWeight = Mathf.MoveTowards(headIK.lookWeight, 1f, Time.deltaTime * 3f);
            }
                
        }
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
        if (IsFlipping) // If flipping, handle the flip logic
        {
            HandleFlip();
            return; // Skip surface alignment while flipping
        }

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
        else if (m_onSurface)
        {
            var localRot = Quaternion.FromToRotation(transform.up, m_surfaceNormal) * transform.rotation;
            var euler = localRot.eulerAngles;
            euler.y = 0;
            localRot.eulerAngles = euler;
            m_skateboard.localRotation = Quaternion.LerpUnclamped(m_skateboard.localRotation, localRot, m_alignSpeed * Time.fixedDeltaTime);
        }
    }

    public void Flip()
    {
        Debug.Log("Flip");
        IsFlipping = true;
        flipElapsedTime = 0f;
        initialFlipRotation = m_skateboard.localRotation;
        targetFlipRotation = initialFlipRotation * Quaternion.Euler(0f, 0f, 360f);
    }

    private void HandleFlip()
    {
        // Increment elapsed time
        flipElapsedTime += Time.deltaTime;

        // Wait for flipStartTime before starting the flip
        if (flipElapsedTime < flipStartTime)
        {
            // If the elapsed time is less than the start time, do nothing and return
            headIK.lookWeight = Mathf.Lerp(headIK.lookWeight, 0f, Time.deltaTime * 10f);
            return;

        }

        // Normalize time value (0 to 1) after flipStartTime has passed
        float t = (flipElapsedTime - flipStartTime) / flipDuration;

        if (t >= 1f)
        {
            // Flip complete: Ensure it ends exactly at 360 degrees
            m_skateboard.localRotation = Quaternion.Euler(0f, 0f, 360f);
            IsFlipping = false;
            flipElapsedTime = 0f;  // Reset elapsed time for future flips
            Debug.Log("Flip done");
            // start setting the rig back to 1;

            //rig.weight = 1;

            //leftLegIk.weight = 1;
            //leftLegRotation.weight = 1;


            //rightLegIk.weight = 1;
            //rightLegRotation.weight = 1;
            
            //headIK.lookWeight = 1;
        }
        else
        {
            // Smoothly increment rotation on the Z-axis from 0 to 360 degrees
            float zRotation = Mathf.Lerp(0f, 360f, t);  // Calculate the rotation from 0 to 360 degrees

            // Set the skateboard's local rotation based on the calculated Z-axis rotation
            m_skateboard.localRotation = Quaternion.Euler(0f, 0f, zRotation);
            Debug.Log("Flipping");
            //rig.weight = 0;
            leftLegIk.weight = Mathf.Lerp(leftLegIk.weight, 0f, Time.deltaTime * 15f);
            leftLegRotation.weight = Mathf.Lerp(leftLegRotation.weight, 0f, Time.deltaTime * 15f);


            rightLegIk.weight = Mathf.Lerp(rightLegIk.weight, 0f, Time.deltaTime * 15f);
            rightLegRotation.weight = Mathf.Lerp(rightLegRotation.weight, 0f, Time.deltaTime * 15f);

            headIK.lookWeight = Mathf.Lerp(headIK.lookWeight, 0f, Time.deltaTime * 15f);

            //rig.weight = Mathf.Lerp(rig.weight, 0f, Time.deltaTime * 15f);

        }
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
                // Normalize the world direction and apply the capped magnitude
                worldDirection = worldDirection.normalized * maxForce;
            }
            else
            {
                // Apply the scaled force if it's within the cap
                worldDirection *= m_FwdForce;
            }

            // Apply the force
            m_rigidbody.AddForce(worldDirection);

            Debug.Log($"Force applied: {worldDirection} (magnitude: {worldDirection.magnitude})");
            _isDragging = false;
        }

    }


}
