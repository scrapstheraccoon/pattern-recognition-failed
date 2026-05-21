using TMPro;
using UnityEngine;

public class ScanningText : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float speed = 0.5f;

    private int dotCount = 0;
    private float timer = 0f;
    private bool increasing = true;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= speed)
        {
            timer = 0f;

            if (increasing)
            {
                dotCount++;
                if (dotCount >= 3)
                    increasing = false;
            }
            else
            {
                dotCount--;
                if (dotCount <= 0)
                    increasing = true;
            }

            textComponent.text = "Scanning" + new string('.', dotCount);
        }
    }
}
