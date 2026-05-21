using UnityEngine;
using System.Collections;
using JetBrains.Annotations;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected string objectName;

    [Header("Scan Settings")]
    [SerializeField] private bool requiresScan = true;
    [SerializeField] protected bool runInteractAfterScan;
    [SerializeField] protected float scanDuration = 2f;
    [TextArea]
    [SerializeField] protected string[] robotDescriptions;

    [Header("Outline Settings")]
    [SerializeField] private float outlineWidth = 5f;

    [Header("Interaction Settings")]
    [SerializeField] protected bool canInteract = true;
    public bool CanInteract => canInteract;

    // Scan vairables
    protected int descriptionIndex;
    protected bool isScanning = false;
    public bool RequiresScan => requiresScan;
    protected bool isScanned = false;
    public virtual bool IsScanned => isScanned;
    public bool IsScanning => isScanning;
    public virtual string RobotDescription => robotDescriptions[descriptionIndex];
    protected GameObject scanInteractor;
    public virtual bool IsHeld => false;

    // Outline Variables
    protected Outline outline;
    protected bool isFocused;

    protected virtual void Awake()
    {
        outline = GetComponentInChildren<Outline>();

        if (outline != null)
        {
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineColor = Color.turquoise;
            outline.OutlineWidth = 0f;
        }
    }

    public virtual void SetFocus(bool value)
    {
        if (isFocused == value) return; // Prevent duplicate calls

        isFocused = value;

        if (outline == null) return;

        outline.OutlineWidth = isFocused ? outlineWidth : 0f;
    }

    public virtual void StartScan(MonoBehaviour runner, GameObject interactor)
    {
        if (!requiresScan || isScanning)
            return;

        scanInteractor = interactor;
        runner.StartCoroutine(ScanRoutine());
    }

    protected virtual IEnumerator ScanRoutine()
    {
        isScanning = true;

        UIManager.Instance.ShowScanText(true);

        yield return new WaitForSeconds(scanDuration);

        UIManager.Instance.ShowScanText(false);

        isScanning = false;
        isScanned = true;

        int safeIndex = Mathf.Clamp(descriptionIndex, 0, robotDescriptions.Length - 1);
        UIManager.Instance.ShowRobotInterpretation(robotDescriptions[safeIndex]);

        yield return new WaitForSeconds(scanDuration);

        if (runInteractAfterScan)
        {
            Interact(scanInteractor);
        }
    }

    public abstract void Interact(GameObject interactor);
}
