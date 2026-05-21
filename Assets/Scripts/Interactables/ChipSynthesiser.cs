using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChipSynthesiser : Interactable
{
    [SerializeField] private float synthesisTime = 90f;
    [SerializeField] private Animator chipAnim;
    [SerializeField] private GameObject chipPrefab;

    private Interactable interactableComponent;

    private float synthesisTimer;
    private SynthState currentState = SynthState.Idle;

    enum SynthState
    {
        Idle,
        Synthesising,
        Complete
    }

    protected override void Awake()
    {
        base.Awake();
        synthesisTimer = synthesisTime;
        interactableComponent = GetComponent<Interactable>();

        chipPrefab.SetActive(false);
    }

    private void Update()
    {
        if (isFocused && currentState == SynthState.Synthesising)
        {
            UIManager.Instance.UpdateBanner($"SYNTHESISING DATA CHIP\nTime Remaining: {FormatTime(synthesisTimer)}", false);
        }
    }

    private void completeSynthesis()
    {
        chipAnim.SetTrigger("show");
        outline.enabled = false;

        UIManager.Instance.UpdateBanner("Chip synthesis Complete.\nUpload data chip to control panel", true);
    }

    public override void Interact(GameObject player)
    {
        if (currentState != SynthState.Idle)
            return;

        StartCoroutine(SynthesisRoutine());
    }

    private IEnumerator SynthesisRoutine()
    { 
        currentState = SynthState.Synthesising;
        canInteract = false;

        synthesisTimer = synthesisTime;

        while (synthesisTimer > 0)
        {
            synthesisTimer -= Time.deltaTime;
            yield return null;
        }

        currentState = SynthState.Complete;
        interactableComponent.enabled = false;
        completeSynthesis();
        
        yield return new WaitForSeconds(1f);
        chipPrefab.SetActive(true);
        chipAnim.gameObject.SetActive(false);
    }

    public override void SetFocus(bool value)
    {
        base.SetFocus(value);

        if (value)
        {
            if (currentState == SynthState.Synthesising)
            {
                UIManager.Instance.UpdateBanner($"Time Remaining: {FormatTime(synthesisTimer)}", false);
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
