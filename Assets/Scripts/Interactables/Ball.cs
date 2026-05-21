using UnityEngine;

public class Ball : Interactable
{
    [SerializeField] private GameObject basketballPrefab;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Interact(GameObject player)
    {
        // Spawn the real basketball
        GameObject newBall = Instantiate(
            basketballPrefab,
            transform.position,
            transform.rotation
        );

        // If the basketball also has an Interactable component:
        Interactable interactable = newBall.GetComponent<Interactable>();
        if (interactable != null)
        {
            interactable.Interact(player); // auto-pickup
        }

        // Destroy the placeholder sphere
        Destroy(gameObject);
    }
}
