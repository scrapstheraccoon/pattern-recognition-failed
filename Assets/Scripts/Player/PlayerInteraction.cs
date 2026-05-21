using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private Transform cam;
    [SerializeField] private GameObject interactPrompt;
    [SerializeField] private GameObject scanPrompt;

    [Header("Input Action References")]
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private InputActionReference scanAction;

    [Header("Interaction Variables")]
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private float interactRadius = 0.15f;

    private Interactable currentInteractable;
    private Interactable heldObject;

    public Interactable HeldObject => heldObject;

    private void OnEnable()
    {
        interactAction.action.Enable();
        scanAction.action.Enable();
    }

    private void OnDisable()
    {
        interactAction.action.Disable();
        scanAction.action.Disable();
    }

    private void Start()
    {
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }

        if (scanPrompt != null)
        {
            scanPrompt.SetActive(false);
        }
    }
    void Update()
    {
        HandleRaycast();
        HandleInteractionInput();
        HandleScanActionInput();
    }

private void HandleRaycast()
    {
        RaycastHit hit;
        Interactable newInteractable = null;

        if (Physics.SphereCast(
            cam.position,
            interactRadius,
            cam.forward,
            out hit,
            interactDistance))
        {
            newInteractable = hit.collider.GetComponentInParent<Interactable>();
        }

        if (newInteractable == currentInteractable)
            return;

        // Remove focus from old
        if (currentInteractable != null)
            currentInteractable.SetFocus(false);

        currentInteractable = newInteractable;

        // Apply focus to new
        if (currentInteractable != null)
        {
            currentInteractable.SetFocus(true);
            ShowPrompts(true);
        }
        else
        {
            ShowPrompts(false);
        }
    }

    private void HandleInteractionInput()
    {
        if (!interactAction.action.WasPressedThisFrame())
            return;

        if (UIManager.Instance != null && UIManager.Instance.IsScanMenuOpen)
            return;

        if (currentInteractable != null && currentInteractable.RequiresScan && !currentInteractable.IsScanned)
            return;

            // IF HOLDING SOMETHING
            if (heldObject != null)
            {
                if (heldObject is Chip && currentInteractable is ChipPanel panel)
                {
                    panel.Interact(gameObject);
                    return;
                }

                if (heldObject is Basketball basketball)
                {
                    basketball.ThrowBall(cam.forward);
                }
                else
                {
                    heldObject.Interact(gameObject); // drop
                }

                heldObject = null;
                return;
            }

        // NOTHING HELD = try interacting
        if (currentInteractable == null)
            return;

        currentInteractable.Interact(gameObject);

        // After interaction, check if it became held
        if (currentInteractable.IsHeld)
        {
            heldObject = currentInteractable;
        }
    }

    private void HandleScanActionInput()
    {
        if (!scanAction.action.WasPressedThisFrame())
            return;

        if (UIManager.Instance != null && UIManager.Instance.IsScanMenuOpen)
        {
            UIManager.Instance.CloseScanLog();
            return;
        }

        if (currentInteractable == null)
            return;

        if (currentInteractable.RequiresScan)
        {
            currentInteractable.StartScan(this, gameObject);
        }
    }

    public void SetHeldBall(Basketball basketball)
    {
        heldObject = basketball;
    }

    private void ShowPrompts(bool show)
    {
        if (!show || currentInteractable == null || UIManager.Instance.IsScanMenuOpen || currentInteractable.CanInteract == false)
        {
            interactPrompt.SetActive(false);
            scanPrompt.SetActive(false);
            return;
        }

        if (currentInteractable.RequiresScan)
        {
            scanPrompt.SetActive(true);
            interactPrompt.SetActive(currentInteractable.IsScanned);
        }
        else
        {
            scanPrompt.SetActive(false);
            interactPrompt.SetActive(true);
        }
    }

    private void OnDrawGizmos()
    {
        if (cam == null) return;

        // Draw the forward ray of the SphereCast
        Gizmos.color = Color.red;
        Vector3 rayEnd = cam.position + cam.forward * interactDistance;
        Gizmos.DrawLine(cam.position, rayEnd);

        // Draw the sphere at the end point (just for reference)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(rayEnd, interactRadius);

        // Optional: Draw sphere along the start to visualize radius
        Gizmos.color = new Color(0, 1, 1, 0.2f); // transparent cyan
        Gizmos.DrawWireSphere(cam.position, interactRadius);
    }



}
