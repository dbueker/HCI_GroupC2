using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))] 

public class IKControl : MonoBehaviour {
    
    protected Animator animator;
    
    public bool ikActive = false;
    public float ikWeight = 0.0f;
    public Transform rightHandObj = null;
    public Transform lookObj = null;
    public RealisticEyeMovements.LookTargetController lookTargetController = null;

    private float ikWeightLastFrame = 0.0f;

    void Start () 
    {
        animator = GetComponent<Animator>();
    }
    
    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if(animator) {

            if(ikWeight > 0.0f && ikWeightLastFrame == 0.0f) {

                if(lookObj != null) {
                    lookTargetController.LookAtPoiDirectly(rightHandObj.GetChild(0), -1);
                }
            }

            if(ikWeight == 0.0f && ikWeightLastFrame > 0.0f) {

                if(lookObj != null) {
                    lookTargetController.LookAtPoiDirectly(rightHandObj.GetChild(0), 0);
                }
            }

            ikWeightLastFrame = ikWeight;
            
            //if the IK is active, set the position and rotation directly to the goal. 
            if(ikActive) {

                // Set the look target position, if one has been assigned
                if(lookObj != null) {
                    //head.SetLookAtWeight(1);
                    //head.SetLookAtPosition(lookObj.position);
                    //float lookDuration = ikWeight > 0 ? -1 : 0;
                    //lookTargetController.LookAtPoiDirectly(rightHandObj.GetChild(0), lookDuration);
                }    

                // Set the right hand target position and rotation, if one has been assigned
                if(rightHandObj != null) {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,ikWeight);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,ikWeight);  
                    animator.SetIKPosition(AvatarIKGoal.RightHand,rightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand,rightHandObj.rotation);
                }        
                
            }
            
            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else {          
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand,0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand,0); 
                //head.SetLookAtWeight(0);
            }
        }
    } 
}