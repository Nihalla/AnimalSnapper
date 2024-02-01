using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Four_Point_Movement))]
public class Camera_Transitions : MonoBehaviour
{
    public GameObject player_camera;
    public Player_Mov_Controller player;
    public Four_Point_Movement movement;
    public GameObject mesh;
    public float direction = 1f;
    public bool play = false;
    public bool complete_signal = false;

    private float progress = 0;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Mov_Controller>();
        mesh = transform.Find("Mesh").gameObject;
        movement = GetComponent<Four_Point_Movement>();
        movement.move_object = player_camera.transform;
    }
    private void OnValidate()
    {
        if(movement == null)
        {
            movement = GetComponent<Four_Point_Movement>();
            movement.move_object = player_camera.transform;
        }
    }

    private void FixedUpdate()
    {
        if(play)
        {
            if ((progress < 1 && direction > 0) || (progress > 0 && direction < 0))
            {
                progress = movement.Move(Time.deltaTime);
            }
            else 
            {
                play = false;
                complete_signal = true;
            }
        }
        
    }
    public void PlayAnimation(bool forward)
    {
        movement.SetDirection(forward);
        direction = 1.0f;
        if(!forward)
        {
            direction = -1.0f;
        }
        play = true;
    }
    public Vector3 GetFinalPointHeight()
    {
        return movement.GetFinalPointHeight();
    }
}
