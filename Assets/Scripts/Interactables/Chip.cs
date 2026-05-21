using System.Collections;
using UnityEngine;

public class Chip : Interactable
{
    [Header("Follow Settings")]
    [SerializeField] private float hoverHeight = 0.7f;
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float forwardDistance = 1.2f;

    [SerializeField] private Transform uploadPos;

    private bool isSelected = false;
    public bool IsSelected => isSelected;

    private Rigidbody rb;
    private Transform followTarget;

    public override bool IsHeld => isSelected;

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

    public void Upload()
    {
        Debug.Log("uploaded chip");

        isSelected = false;
        followTarget = null;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;

        StartCoroutine(MoveToUploadPosition());
    }

    private IEnumerator MoveToUploadPosition()
    {
        while (Vector3.Distance(transform.position, uploadPos.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                uploadPos.position,
                followSpeed * Time.deltaTime
            );

            yield return null;
        }

        Destroy(gameObject, 0.75f);
    }



}
