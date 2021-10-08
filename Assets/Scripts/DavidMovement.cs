using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DavidMovement : MonoBehaviour
{
    public float speed = 0.7f;
    public Transform coachTarget;
    public Animator davidAnim;

    void Update ()
    {
        Vector3 targetPosition = coachTarget.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed);

        if(Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            transform.position = targetPosition;
            davidAnim.applyRootMotion = true;
            Turn();
            this.enabled = false;
        }
    }

    public void Turn()
    {
        davidAnim.Play("Turn");        
    }
}
