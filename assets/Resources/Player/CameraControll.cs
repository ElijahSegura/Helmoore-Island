using UnityEngine;
using System.Collections;

public class CameraControll : MonoBehaviour {
    // Update is called once per frame
    private Vector3 Rotation = Vector3.zero;
    public float sensitivity;
    private float MaxDistance;
    Vector3 og;
    private bool controlable = true;

    [SerializeField]
    private Camera c;

    void Start()
    {
        
        MaxDistance = Vector3.Distance(transform.position, c.transform.position);
        og = c.transform.localPosition;
    }

    RaycastHit hit;
	void Update () {
        Vector3 to = c.transform.position - transform.position;
        if (Physics.Raycast(transform.position, to, out hit, MaxDistance))
        {
            c.transform.position = Vector3.Lerp(transform.position, hit.point, 0.8f);
        }
        else
        {
            c.transform.localPosition = og;
        }

        



        if(controlable)
        {
            Rotation.x += Input.GetAxis("Mouse Y") * -1 * sensitivity;
            if (Rotation.x > 90.00)
            {
                Rotation.x = 90;
            }
            else if (Rotation.x < -90)
            {
                Rotation.x = -90;
            }
            if (Rotation.x < 90.00 && Rotation.x > -90.00)
            {
                transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * -1 * sensitivity, 0, 0));
            }
            transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * sensitivity, 0), Space.World);
        }
    }

    public void reset(Vector3 rot)
    {
        transform.Rotate(new Vector3(0,rot.y-transform.rotation.eulerAngles.y,0), Space.World);
    }

    public void setControlable(bool controlable)
    {
        this.controlable = controlable;
    }
}
