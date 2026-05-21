using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void SetMoving(bool isMoving)
    {
        anim.SetBool("isMoving", isMoving);
    }

    public void SetGrounded(bool isGrounded)
    {
        anim.SetBool("isGrounded", isGrounded);
    }

    public void TriggerJump()
    {
        anim.SetTrigger("jump");
    }

    public void TriggerLand()
    {
        anim.SetTrigger("land");
    }
}
