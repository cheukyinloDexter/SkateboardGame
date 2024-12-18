using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageIK : MonoBehaviour
{
    Animator animator;
    public bool ikActive = true;
    public Transform objectTarget;
    public float lookWeight;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnAnimatorIK()
    {
        if(animator)
        {
            if (ikActive)
            {
                if(objectTarget != null)
                {
                    animator.SetLookAtWeight(lookWeight);
                    animator.SetLookAtPosition(objectTarget.position);
                }
            }
            else
            {
                animator.SetLookAtWeight(0);
            }
        }
    }
}
