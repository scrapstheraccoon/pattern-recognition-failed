using UnityEngine;
using System.Collections;

public class blinkController : MonoBehaviour
{
    private PlayerMovement playerMovement;
    void Start()
    {
        StartCoroutine(startUpRoutine());
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        playerMovement.toggleMovement();
    }

    private IEnumerator startUpRoutine()
    {
        yield return new WaitForSeconds(13f);
        playerMovement.toggleMovement();
        gameObject.SetActive(false);
    }
   
}
