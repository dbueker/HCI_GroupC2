using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SophieMovement : MonoBehaviour
{
    public float speed = 0.7f;
    public Transform coachTarget;
    public Animator sophieAnim;

    void Update ()
    {
        Vector3 targetPosition = coachTarget.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if(Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            transform.position = targetPosition;
            sophieAnim.applyRootMotion = true;
            Turn();
            this.enabled = false;
        }
    }

    public void Turn()
    {
        sophieAnim.Play("Turn");        
    }
}
