using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JodyMovement : MonoBehaviour
{
    public float speed = 0.7f;
    public Transform coachTarget;
    public Animator jodyAnim;

    void Update ()
    {
        Vector3 targetPosition = coachTarget.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if(Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            transform.position = targetPosition;
            jodyAnim.applyRootMotion = true;
            Turn();
            this.enabled = false;
        }
    }

    public void Turn()
    {
        jodyAnim.Play("Turn");        
    }
}
