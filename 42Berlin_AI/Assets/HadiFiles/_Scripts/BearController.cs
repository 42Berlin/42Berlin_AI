using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
public class BearController : MonoBehaviour
{
    static public BearController instance;
    
    [Tooltip("Body Animator Controller")]
    public Animator bodyAnimatorController;
    
    [Tooltip("Face Animator Controller")]
    public Animator faceAnimatorController;
    
    // [SerializeField]
    // [Tooltip("Constraint Component which points the rotation of Neck to the viewer")]
    // RotationConstraint constraint;


    void Awake()
    {
        instance = this;
        
    }
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ConstraintActivate();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ConstraintDeactivate();
        }

    }


    public void ConstraintActivate()
    {
        // constraint.weight = 1;
    }

    public void ConstraintDeactivate()
    {
        // constraint.weight = 0;
    }

    public void Idle()
    {
        ConstraintActivate();
        bodyAnimatorController.SetBool("idle", true);
        faceAnimatorController.SetBool("talk", false);
        bodyAnimatorController.SetBool("talk", false);
        bodyAnimatorController.SetBool("greeting", false);
        bodyAnimatorController.SetBool("goodbye", false);
        faceAnimatorController.updateMode = AnimatorUpdateMode.AnimatePhysics;

    }
    public void StartTalking()
    {
        ConstraintActivate();
        faceAnimatorController.SetBool("talk", true);
        bodyAnimatorController.SetBool("talk", true);
        bodyAnimatorController.SetBool("idle", false);
        bodyAnimatorController.SetBool("greeting", false);
        bodyAnimatorController.SetBool("goodbye", false);
        faceAnimatorController.updateMode = AnimatorUpdateMode.Normal;

    }

    public void Walking()
    {
       //To do

    }
    public void Greeting()
    {
        ConstraintActivate();
        faceAnimatorController.SetBool("talk", true);
        bodyAnimatorController.SetBool("greeting", true);
        bodyAnimatorController.SetBool("idle", false);
        bodyAnimatorController.SetBool("goodbye", false);
        faceAnimatorController.updateMode = AnimatorUpdateMode.Normal;
    }


    public void GoodBye()
    {
        ConstraintActivate();
        faceAnimatorController.SetBool("talk", true);
        bodyAnimatorController.SetBool("goodbye", true);
        bodyAnimatorController.SetBool("greeting", false);
        bodyAnimatorController.SetBool("idle", false);
        faceAnimatorController.updateMode = AnimatorUpdateMode.Normal;
    }


    public void CurrentAnimation(int currentAnimation)
    {
        switch (currentAnimation)
        {
            case 0:
                Idle();
                break;
            case 1:
                StartTalking();
                break;
            case 2:
                Greeting();
                break;
            case 3:
                Greeting();
                break;
            default:
                break;
        }
    }

}
