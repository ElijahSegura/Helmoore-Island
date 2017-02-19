using UnityEngine;
using System.Collections;

public class CameraControll : MonoBehaviour {
    // Update is called once per frame
    private Vector3 Rotation = Vector3.zero;
    public float sensitivity;

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
        
    }
}
