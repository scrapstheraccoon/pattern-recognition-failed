using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameObject scanText;
    [SerializeField] private GameObject blinker;

    [SerializeField] private Animator scanMenuAnim;
    [SerializeField] private Animator bannerAnim;

    [SerializeField] private TextMeshProUGUI scanTextComponent;
    [SerializeField] private TextMeshProUGUI bannerTextComponent;

    [SerializeField] private float bannerVisibilityTime = 3f;

    public bool IsScanMenuOpen { get; private set; }
    private bool bannerOpen = false;

    private void Awake()
    {
        Instance = this;

        if (scanText != null)
            scanText.SetActive(false);

        if(blinker != null)
            blinker.SetActive(true);
    }

    private void Start()
    {
        StartCoroutine(StartUpRoutine());
    }

    public void ShowScanText(bool show)
    {
        if (show)
            scanText.SetActive(true);
        else 
            scanText.SetActive(false);
    }

    public void ShowRobotInterpretation(string robotDescription)
    {
        scanTextComponent.text = robotDescription;
        scanMenuAnim.SetTrigger("openMenu");
        IsScanMenuOpen = true;
    }

    public void CloseScanLog()
    {
        scanMenuAnim.SetTrigger("closeMenu");
        IsScanMenuOpen = false;
    }

    public void UpdateBanner(string sentence, bool timed)
    {
        if (!bannerOpen)
        {
            bannerAnim.SetTrigger("open");
            bannerOpen = true;
        }

        bannerTextComponent.text = sentence;

        if (timed)
            StartCoroutine(bannerRoutine());
    }

    public void CloseBanner()
    {
        if (bannerOpen)
        {
            bannerAnim.SetTrigger("close");
            bannerOpen = false;
        }
    }

    private IEnumerator bannerRoutine()
    {
        yield return new WaitForSeconds(bannerVisibilityTime);

        bannerAnim.SetTrigger("close");
        bannerOpen = false;
    }

    private IEnumerator StartUpRoutine()
    {
        yield return new WaitForSeconds(5.1f);

        UIManager.Instance.UpdateBanner("C3PO online. Systems nominal.", false);
        yield return new WaitForSeconds(2f);

        UIManager.Instance.UpdateBanner("Human units endangered due to system malfunction.", false);
        yield return new WaitForSeconds(3f);

        UIManager.Instance.UpdateBanner("Repair the malfunction by synthesizing a new data chip.", false);
        yield return new WaitForSeconds(3f);

        CloseBanner();
    }

}
