using UnityEngine;

public class LaundryBasket : Interactable
{
    protected override void Awake()
    {
        base.Awake();
        isScanned = true;
    }

    public override void Interact(GameObject interactor)
    {
        throw new System.NotImplementedException();
    }
}
