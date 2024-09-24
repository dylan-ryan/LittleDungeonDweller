using UnityEngine;

public class Billboard : MonoBehaviour
{
    //Put this script on Enemies that need to face camera (Does not work for player)
    
    private Transform cameraTransform;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + cameraTransform.rotation * Vector3.forward, cameraTransform.rotation * Vector3.up);
    }
}
