using UnityEngine;
using System.Collections;

public class CameraControll : MonoBehaviour {
    // Update is called once per frame
    private Vector3 Rotation = Vector3.zero;
    public float sensitivity;

    Transform mainCamera;
    Vector3 cameraOffset;
    float cameraDistance = 16f;
    float cameraHeight = 16f;

    

    private void Start()
    {
        mainCamera = Camera.main.transform;
        cameraOffset = new Vector3(0f, cameraHeight, -cameraDistance);
    }

    void Update () {
        Rotation.x += Input.GetAxis("Mouse Y") * -1 * sensitivity;
        if(Rotation.x > 90.00)
        {
            Rotation.x = 90;
        }
        else if(Rotation.x < -90)
        {
            Rotation.x = -90;
        }
        if(Rotation.x < 90.00 && Rotation.x > -90.00)
        {
            transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * -1 * sensitivity, 0, 0));
        }

        mainCamera.position = transform.parent.transform.position;
        mainCamera.rotation = transform.parent.transform.rotation;
        mainCamera.Translate(cameraOffset);
    }
}
