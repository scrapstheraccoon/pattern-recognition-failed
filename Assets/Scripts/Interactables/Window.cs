using UnityEngine;

public class Window : Interactable
{
    [SerializeField] private Camera cam;
    private bool isEnabled = false;

    public override void Interact(GameObject player)
    {
        isEnabled = !isEnabled;
        cam.enabled = isEnabled;

        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.toggleMovement();
    }
}
