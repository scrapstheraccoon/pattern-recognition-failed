using UnityEngine;

public class Cylinder : Interactable
{
    [SerializeField] private Animator binAnim;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Interact(GameObject player)
    {
        binAnim.SetTrigger("show");
        Destroy(this.gameObject);
    }
}
