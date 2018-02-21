using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeRunnng : MonoBehaviour {
    private CharacterController control;
    private float timer = 5f;
    public AnimationCurve upWall, withWall;
    private Vector3 ogPos;
    void Start()
    {
        control = GetComponent<CharacterController>();
    }

    private bool col = false;

    void LateUpdate()
    {
        
        if(col)
        {
            timer -= Time.deltaTime;
        }
        else if (!control.isGrounded && !col)
        {
            GetComponent<Character>().stopFreeRun();
            GetComponent<Character>().Gravity = new Vector3(0, 9.8f, 0);
        }
        if(control.isGrounded || !Input.GetButton("Jump"))
        {
            col = false;
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!control.isGrounded && Input.GetButton("Jump"))
        {
            if (Mathf.Round(hit.normal.x) != 0.0f || Mathf.Round(hit.normal.z) != 0.0f)
            {
                col = true;
                GetComponent<Character>().Gravity = transform.forward - hit.normal;
                GetComponent<Character>().Gravity *= 9.8f;
                GetComponent<Character>().startFreeRun();
                if(ogPos != null)
                {
                    ogPos = transform.position;
                    Vector3 normal = transform.forward - hit.normal;
                }
            }
        }
        else
        {
            //GetComponent<Character>().stopFreeRun();
        }
    }
    
}
