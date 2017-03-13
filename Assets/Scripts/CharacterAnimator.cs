using UnityEngine;
using System.Collections;

public class CharacterAnimator : MonoBehaviour {

    [SerializeField]
    Animator animator;
    [SerializeField]
    CharacterController controller;

    void Start()
    {
        controller.OnJump += ()=>animator.SetTrigger("Jump");
    }

    void FixedUpdate()
    {
        animator.SetBool("Facing Left", controller.facingLeft);
        animator.SetFloat("Speed", controller.normalizedSpeed);
    }
}
