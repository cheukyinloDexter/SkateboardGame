using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public SkateboardController m_skateboardController;
    public float maxChargeTime = 1f;  // Maximum time to charge the jump
    public float minJumpForce = 5f;  // Minimum jump force
    public float maxJumpForce = 10f; // Maximum jump force

    private Rigidbody m_rigidbody;
    private float jumpChargeTime = 0f;
    private bool isCharging = false; // Whether the jump is being charged

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void OnMouseDown()
    {
        // Check if the skateboard is on the surface
        if (m_skateboardController != null && m_skateboardController.m_onSurface)
        {
            isCharging = true;
            jumpChargeTime = 0f; // Reset jump charge time
            Debug.Log("Mouse down - Charging jump");
        }
    }

    private void OnMouseDrag()
    {
        // Continue charging while dragging
        if (isCharging)
        {
            jumpChargeTime += Time.deltaTime; // Increase charge time
            Debug.Log($"Charging jump: {jumpChargeTime}");
        }
    }

    private void OnMouseUp()
    {
        // Release jump when mouse is released
        if (isCharging && m_skateboardController != null && m_skateboardController.m_onSurface)
        {
            isCharging = false;

            // Calculate jump force based on charge time
            float jumpForce = minJumpForce + Mathf.Clamp(jumpChargeTime / maxChargeTime, 0f, 1f) * (maxJumpForce - minJumpForce);

            // Apply the jump force to the Rigidbody
            m_rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            Debug.Log($"Mouse up - Jump force applied: {jumpForce}");

            // Reset charge time
            jumpChargeTime = 0f;
        }
    }

/*    void Update()
    {
        ProcessInputs();
    }

    private void ProcessInputs()
    {
        bool m_onSurface = m_skateboardController != null && m_skateboardController.m_onSurface;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began && m_onSurface)
            {
                jumpChargeTime = 0f;
            }
            else if (touch.phase == TouchPhase.Stationary && m_onSurface)
            {
                jumpChargeTime += Time.deltaTime;
            }
            else if (touch.phase == TouchPhase.Ended && m_onSurface)
            {
                float jumpForce = minJumpForce + Mathf.Clamp(jumpChargeTime / maxChargeTime, 0f, 1f) * maxJumpForce;
                m_rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                jumpChargeTime = 0f;
            }
        }
    }*/
}
