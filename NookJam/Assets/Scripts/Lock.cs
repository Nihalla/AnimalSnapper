using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour
{
    public GameObject cam;
    public MeshRenderer mesh_render;

    private void Awake()
    {
        cam = GameObject.FindGameObjectWithTag("Camera").gameObject;
        mesh_render = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        VisualCheck();
        Reposition();
    }
    private void VisualCheck()
    {
        if(cam)
        {
            RendererToggle(true);
        }
        else
        {
            RendererToggle(false);
        }
    }
    private void RendererToggle(bool value)
    {
        if(mesh_render.enabled != value)
        {
            mesh_render.enabled = value;
            var children = GetComponentsInChildren<MeshRenderer>();
            foreach (var child in children)
            {
                child.enabled = value;
            }
        }
    }
    private void Reposition()
    {
        if (cam == null)
        {
            cam = GameObject.FindGameObjectWithTag("Camera").gameObject;
        }
        else if (!GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Mov_Controller>().holding_camera)
        {
            transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y + 3.0f, cam.transform.position.z);
        }
    }
}
