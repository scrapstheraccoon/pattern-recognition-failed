using UnityEngine;
using System.Collections;

public class ChipPanel : Interactable
{
    [SerializeField] private float uploadTime = 80f;
    [SerializeField] private Animator chipAnim;
    private Chip chip;

    private float uploadTimer;
    private UploadState currentState = UploadState.Idle;

    enum UploadState
    {
        Idle,
        Uploading,
        Complete
    }

    private void Update()
    {
        if (isFocused && currentState == UploadState.Uploading)
        {
            UIManager.Instance.UpdateBanner($"UPLOADING DATA CHIP\nTime Remaining: {FormatTime(uploadTimer)}", false);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        uploadTimer = uploadTime;
        chipAnim.gameObject.SetActive(false);
    }

    private void completeUpload()
    {
        UIManager.Instance.UpdateBanner("Upload Complete.\nHuman survival probability stabilized", true);
    }

    public override void Interact(GameObject player)
    {
        if (currentState != UploadState.Idle)
            return;

        PlayerInteraction pi = player.GetComponent<PlayerInteraction>();
        if (pi == null)
            return;

        if (pi.HeldObject is Chip heldChip)
        {
            chip = heldChip;
            StartCoroutine(UploadRoutine());
        }
    }


    private IEnumerator UploadRoutine()
    {
        chip.Upload();
        yield return new WaitForSeconds(0.7f);
        chipAnim.gameObject.SetActive(true);
        chipAnim.SetTrigger("upload");

        currentState = UploadState.Uploading;
        canInteract = false;

        uploadTimer = uploadTime;

        while (uploadTimer > 0)
        {
            uploadTimer -= Time.deltaTime;
            Debug.Log(uploadTimer);
            yield return null;
        }

        currentState = UploadState.Complete;
        completeUpload();
    }

    public override void SetFocus(bool value)
    {
        base.SetFocus(value);

        if (value)
        {
            if (currentState == UploadState.Uploading)
            {
                UIManager.Instance.UpdateBanner($"Time Remaining: {FormatTime(uploadTimer)}", false);
            }
        }
        else
        {
            UIManager.Instance.CloseBanner();
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return $"{minutes:00}:{seconds:00}";
    }
}
