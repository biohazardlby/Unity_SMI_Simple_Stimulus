//cameraFacingBillboard.cs v02
//by Neil Carter (NCarter)
//modified by Juan Castaneda (juanelo)
//
//added in-between GRP object to perform rotations on
//added auto-find main camera
//added un-initialized state, where script will do nothing
using UnityEngine;
using System.Collections;


public class Camera_Facing_Sprite : MonoBehaviour
{

    public Camera m_Camera;
    public bool amActive = false;
    GameObject myContainer;

    SpriteRenderer renderer;
    void Awake()
    {
        myContainer = new GameObject();
        myContainer.name = "GRP_" + transform.gameObject.name;
        myContainer.transform.position = transform.position;
        transform.parent = myContainer.transform;
        renderer = GetComponent<SpriteRenderer>();
    }

    //Orient the camera after all movement is completed this frame to avoid jittering
    void LateUpdate()
    {
        m_Camera = Camera.main;
        if (amActive == true)
        {
            renderer.enabled = true;
            myContainer.transform.LookAt(myContainer.transform.position + m_Camera.transform.rotation * Vector3.back, m_Camera.transform.rotation * Vector3.up);
        }
        else
        {
            renderer.enabled = false;
        }
    }
}