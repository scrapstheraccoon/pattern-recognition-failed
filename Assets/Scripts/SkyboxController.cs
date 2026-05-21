using UnityEngine;

public class SkyboxController : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 2f;

    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotateSpeed);
    }
}
