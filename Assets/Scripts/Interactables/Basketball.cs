using UnityEditor;
using UnityEngine;

public class Basketball : Interactable
{
    [Header("Follow Settings")]
    [SerializeField] private float hoverHeight = 0.7f;
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float forwardDistance = 1.2f;

    [Header("Throw Settings")]
    [SerializeField] private float throwForce = 15f;
    [SerializeField] private float upwardBoost = 0.5f;

    public override string RobotDescription => robotDescriptions[descriptionIndex];

    private Rigidbody rb;
    private Transform followTarget;

    private bool isSelected = false;

    private int basketHits = 0;


    protected override void Awake()
    {
        base.Awake();
        isScanned = true;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!isSelected || followTarget == null)
            return;

        Vector3 targetPosition = followTarget.position + followTarget.forward * forwardDistance + Vector3.up * hoverHeight;
        Vector3 newPosition = Vector3.Lerp(rb.position, targetPosition, followSpeed * Time.deltaTime);

        rb.MovePosition(newPosition);
    }

    public override void Interact(GameObject player)
    {
        isSelected = !isSelected;

        PlayerInteraction playerInteraction = player.GetComponent<PlayerInteraction>();

        if (isSelected)
        {
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            followTarget = player.transform;
            rb.freezeRotation = true;

            if (playerInteraction != null)
                playerInteraction.SetHeldBall(this);
        }
        else
        {
            rb.useGravity = true;
            followTarget = null;
            rb.freezeRotation = false;

            if (playerInteraction != null)
                playerInteraction.SetHeldBall(null);
        }
    }

    public void ThrowBall(Vector3 direction)
    {
        isSelected = false;
        followTarget = null;

        rb.useGravity = true;
        rb.freezeRotation = false;

        rb.linearVelocity = Vector3.zero;

        Vector3 throwDirection = (direction + Vector3.up * upwardBoost).normalized;
        rb.AddForce(throwDirection.normalized * throwForce, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("LaundryBasket"))
        {
            basketHits++;
            Debug.Log("Basket hits: " + basketHits);

            if (basketHits == 2)
            {
                UIManager.Instance.UpdateBanner($"New interpretation of [{objectName}] achieved.\nUpdate scan.log!", true);
                this.descriptionIndex = 1;
            }

            if (basketHits == 4)
            {
                UIManager.Instance.UpdateBanner($"New interpretation of [{objectName}] achieved.\nUpdate scan.log!", true);
                this.descriptionIndex = 2;
            }
        }
    }

}
