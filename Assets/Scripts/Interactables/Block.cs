using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : Interactable
{
    [SerializeField] private GameObject legoPrefab;

    private static bool blocksScanned = false;
    public override bool IsScanned => blocksScanned;
    private static List<Block> allBlocks = new List<Block>();


    protected override void Awake()
    {
        base.Awake();
        allBlocks.Add(this);
    }

    private void OnDestroy()
    {
        allBlocks.Remove(this);
    }

    public override void Interact(GameObject player)
    {
        if (!blocksScanned)
            return;

        TransformIntoLego();
    }

    public override void StartScan(MonoBehaviour runner, GameObject interactor)
    {
        if (IsScanning)
            return;

        scanInteractor = interactor;
        runner.StartCoroutine(ScanRoutine());
    }

    protected override IEnumerator ScanRoutine()
    {
        isScanning = true;

        UIManager.Instance.ShowScanText(true);

        yield return new WaitForSeconds(scanDuration);

        UIManager.Instance.ShowScanText(false);

        isScanning = false;
        blocksScanned = true; // mark all blocks scanned

        foreach (Block block in allBlocks.ToArray())
        {
            block.TransformIntoLego();
        }

        UIManager.Instance.ShowRobotInterpretation(RobotDescription);

        if (runInteractAfterScan)
        {
            Interact(scanInteractor);
        }

    }

    private void TransformIntoLego()
    {
        Instantiate(legoPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }

}

