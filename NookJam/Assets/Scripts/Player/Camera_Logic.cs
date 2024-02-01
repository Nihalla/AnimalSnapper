using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Camera_Logic : MonoBehaviour
{
    public CinemachineVirtualCamera main_cam;
    public CinemachinePOV composer;
    public Player_Mov_Controller player_mov;
    public Camera_Transitions camera_anim;
    public Camera_Bob camera_bob;
    private NookJamControls controls;
    private bool zoom_in = false;
    private bool zoom_out = false;

    private void Awake()
    {
        player_mov = GetComponent<Player_Mov_Controller>();
        if(player_mov != null)
        {
            main_cam = player_mov.main_cam;
        }
        composer = main_cam.GetCinemachineComponent<CinemachinePOV>();
        controls = new();
        controls.Player.ZoomIn.performed += ctx => ZoomIn();
        controls.Player.ZoomIn.canceled += ctx => Idle();
        controls.Player.ZoomOut.performed += ctx => ZoomOut();
        controls.Player.ZoomOut.canceled += ctx => Idle();
    }
    private void Start()
    {
        camera_bob.initial_position = camera_anim.GetFinalPointHeight();
    }
    public void Update()
    {
        if(camera_anim.complete_signal)
        {
            camera_anim.complete_signal = false;
            if(camera_anim.direction > 0)
            {
                camera_anim.mesh.SetActive(false);
                player_mov.CameraUI(true);
            }
            camera_bob.ResetMovement(camera_anim.GetFinalPointHeight());
            camera_bob.SetOnline(true);
        }
        if (player_mov.GetMode())
        {
            if (zoom_in && main_cam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView > 20)
            {
                main_cam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView -= 1;
                RenderSettings.fogStartDistance += 2;
                player_mov.max_cast++;
            }
            if (zoom_out && main_cam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView < 50)
            {
                main_cam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView += 1;
                RenderSettings.fogStartDistance -= 2;
                player_mov.max_cast--;
            }
        }
    }
    private void ZoomIn()
    {
        zoom_in = true;
    }

    private void ZoomOut()
    {
        zoom_out = true;
    }

    private void Idle()
    {
        zoom_in = false;
        zoom_out = false;
    }
    public void PlayAnimation(bool new_state)
    {
        if(!camera_anim.play)
        {
            if (!camera_anim.mesh.activeInHierarchy)
            {
                camera_anim.mesh.SetActive(true);
            }
            if(camera_bob != null)
            {
                camera_bob.ResetMovement(camera_anim.GetFinalPointHeight());
            }
            camera_anim.PlayAnimation(new_state);
        }
    }
    public void RevertZoom()
    {
        main_cam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = 50;
        RenderSettings.fogStartDistance = 1;
        player_mov.max_cast = 20;
    }

    public void EnableInput()
    {
        controls.Player.Enable();
    }

    public void DisableInput()
    {
        controls.Player.Disable();
    }
    private void OnDisable()
    {
        DisableInput();
    }
    private void OnEnable()
    {
        EnableInput();
    }

    public bool isAnimationPlaying()
    {
        return camera_anim.play;
    }
}
