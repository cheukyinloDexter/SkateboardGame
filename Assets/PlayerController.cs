using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private SkateboardController m_skateboardController;
    [SerializeField] private Slider JumpSlider;
    public float maxChargeTime = 1f;  // Maximum time to charge the jump
    public float minJumpForce = 5f;  // Minimum jump force
    public float maxJumpForce = 10f; // Maximum jump force

    private Rigidbody m_rigidbody;
    private float jumpChargeTime = 0f;
    private bool isCharging = false; // Whether the jump is being charged

    [SerializeField] private GameObject leftArm;
    [SerializeField] private GameObject rightArm;

    [SerializeField] private GameObject leftArmTarget;
    [SerializeField] private GameObject rightArmTarget;

    //[SerializeField] private SphereCollider TargetTrigger;

    private TwoBoneIKConstraint leftArmIk;
    private MultiRotationConstraint leftArmRotation;

    private TwoBoneIKConstraint rightArmIk;
    private MultiRotationConstraint rightArmRotation;

    public List<GameObject> targets = new List<GameObject>();
    public GameObject target;

    //private Transform previousTarget;
    //private float transitionProgress = 1f; // 1 = fully reached the new target
    //[SerializeField] private float transitionSpeed = 1f; // Adjust speed for smoother transitions




    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        //TargetTrigger = GetComponent<SphereCollider>();
        if (leftArm != null || rightArm != null)
        {
            leftArmIk = leftArm.GetComponent<TwoBoneIKConstraint>();
            leftArmRotation = leftArm.GetComponent<MultiRotationConstraint>();
            rightArmIk = rightArm.GetComponent<TwoBoneIKConstraint>();
            rightArmRotation = rightArm.GetComponent<MultiRotationConstraint>();

        }

        if (JumpSlider != null)
        {
            JumpSlider.value = 0f; // Ensure slider starts at 0
            JumpSlider.gameObject.SetActive(false); // Deactivate slider initially
        }
    }

    private void OnMouseDown()
    {
        // Check if the skateboard is on the surface
        if (m_skateboardController != null && JumpSlider != null)
        {
            isCharging = true;
            JumpSlider.gameObject.SetActive(true);
            jumpChargeTime = 0f; // Reset jump charge time
            Debug.Log("Mouse down - Charging jump");
            JumpSlider.value = 0f;
        }
    }

    private void OnMouseDrag()
    {
        // Continue charging while dragging
        if (isCharging && JumpSlider != null)
        {
            jumpChargeTime += Time.deltaTime; // Increase charge time
            float chargeProgress = Mathf.Clamp(jumpChargeTime / maxChargeTime, 0f, 1f);
            JumpSlider.value = chargeProgress;
            Debug.Log($"Charging jump: {jumpChargeTime}");
        }

        // show the charge level in the ui
    }

    private void OnMouseUp()
    {
        // Release jump when mouse is released
        if (isCharging && JumpSlider != null && m_skateboardController != null)
        {
            isCharging = false;
            if (m_skateboardController.m_onSurface)
            {
                // Calculate jump force based on charge time
                float jumpForce = minJumpForce + Mathf.Clamp(jumpChargeTime / maxChargeTime, 0f, 1f) * (maxJumpForce - minJumpForce);

                // Apply the jump force to the Rigidbody
                m_rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                Debug.Log($"Mouse up - Jump force applied: {jumpForce}");

                if (jumpChargeTime >= maxChargeTime)
                {
                    m_skateboardController.Flip();

                }
            }
            // Reset charge time
            jumpChargeTime = 0f;
            JumpSlider.value = 0f;
            JumpSlider.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            targets.Add(other.gameObject);
            //target = other.gameObject;
            //Destroy(other.gameObject);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            targets.Remove(other.gameObject);
            //target = null;
            //Destroy(other.gameObject);
        }
    }

    void Update()
    {
        //foreach (GameObject i in targets)
        //{
        //    //get closses target
        //    target = i;
        //}
        if (targets.Count >= 1)
        {
            //target = targets.OrderBy(t => Vector3.Distance(transform.position, t.transform.position)).FirstOrDefault();
            
            GameObject newTarget = targets.OrderBy(t => Vector3.Distance(transform.position, t.transform.position)).FirstOrDefault();

            if (newTarget != target)
            {
                //previousTarget = target != null ? target.transform : null; // Store previous target
                //transitionProgress = 0f; // Reset transition progress
            }

            target = newTarget;
        }
        else
        {
            target = null;
        }

        if (leftArmIk != null && target != null)
        {
            leftArmIk.weight = Mathf.MoveTowards(leftArmIk.weight, 1f, Time.deltaTime * 2f);
            leftArmRotation.weight = Mathf.MoveTowards(leftArmRotation.weight, 1f, Time.deltaTime * 3f);

            //transitionProgress = Mathf.Clamp01(transitionProgress + Time.deltaTime * transitionSpeed);

            // Get smooth transition position
            //Vector3 startPosition = previousTarget != null ? previousTarget.position : leftArmTarget.transform.position;
            Vector3 targetPosition = target.transform.position;
            //leftArmTarget.transform.position = Vector3.Lerp(startPosition, targetPosition, transitionProgress);
            leftArmTarget.transform.position = Vector3.MoveTowards(leftArmTarget.transform.position, targetPosition, Time.deltaTime * 30f);

            // Get smooth transition rotation
            //Quaternion startRotation = previousTarget != null ? previousTarget.rotation : leftArmTarget.transform.rotation;
            Vector3 direction = transform.position - target.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.AngleAxis(270, Vector3.right) * Quaternion.AngleAxis(90, Vector3.up);
            leftArmTarget.transform.rotation = Quaternion.RotateTowards(leftArmTarget.transform.rotation, targetRotation, Time.deltaTime * 3600f);//turn 3600 degree in i sec
        }
        else
        {
            if (leftArmIk != null)
            {
                leftArmIk.weight = Mathf.MoveTowards(leftArmIk.weight, 0f, Time.deltaTime * 2f);
                leftArmRotation.weight = Mathf.MoveTowards(leftArmRotation.weight, 0f, Time.deltaTime * 5f);
            }
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