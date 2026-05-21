using System.Collections.Generic;
using UnityEngine;

public class Lego : Interactable
{
    [Header("Follow Settings")]
    [SerializeField] private float hoverHeight = 0.7f;
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float forwardDistance = 1.2f;

    public override string RobotDescription => robotDescriptions[descriptionIndex];

    private bool isSelected = false;
    private static bool triggeredLevel1 = false;
    private static bool triggeredLevel2 = false;

    private Rigidbody rb;
    private Transform followTarget;

    public override bool IsHeld => isSelected;

    private static float heldTime = 0f;

    protected override void Awake()
    {
        base.Awake();
        isScanned = true;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!isSelected || followTarget == null)
            return;

        Vector3 targetPosition = followTarget.position + followTarget.forward * forwardDistance + Vector3.up * hoverHeight;
        Vector3 newPosition = Vector3.Lerp(rb.position, targetPosition, followSpeed * Time.fixedDeltaTime);

        rb.MovePosition(newPosition);
    }

    private void Update()
    {
        if (!isSelected)
            return;

        heldTime += Time.deltaTime;

        if (heldTime > 30f && !triggeredLevel1)
        {
            triggeredLevel1 = true;
            descriptionIndex = 1;
            UIManager.Instance.UpdateBanner($"New interpretation of [{objectName}] achieved.\nUpdate scan.log!", true);
        }

        if (heldTime > 60f && !triggeredLevel2)
        {
            triggeredLevel2 = true;
            descriptionIndex = 2;
            UIManager.Instance.UpdateBanner($"New interpretation of [{objectName}] achieved.\nUpdate scan.log!", true);
        }
    }

    public override void Interact(GameObject player)
    {
        isSelected = !isSelected;

        if (isSelected)
        {
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            followTarget = player.transform;
            rb.freezeRotation = true;
        }
        else
        {
            rb.useGravity = true;
            followTarget = null;
            rb.freezeRotation = false;
        }
    }
}
