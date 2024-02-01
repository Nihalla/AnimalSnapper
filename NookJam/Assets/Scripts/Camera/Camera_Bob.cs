using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Camera_Bob : MonoBehaviour
{
    private float origin_point;
    [SerializeField] private float distance_to_player;
    private GameObject player;
    [SerializeField] private GameObject look_at;
    private Player_Mov_Controller player_script;
    [SerializeField] private bool moving = false;
    private float move_point = 1f;
    [System.NonSerialized] public Vector3 initial_position;
    public bool online = true;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player_script = player.GetComponent<Player_Mov_Controller>();
        look_at = gameObject.GetComponent<CinemachineVirtualCamera>().Follow.gameObject;
        origin_point = transform.position.y - look_at.transform.position.y;
        distance_to_player = origin_point;
        
        player_script.controls.Player.Move.performed += ctx => Moving();
        player_script.controls.Player.Move.canceled += ctx => Idle();
    }

    void Update()
    {
        distance_to_player = transform.position.y - player.transform.position.y;
        if (!player_script.journal_mode && online)
        {
            if (moving)
            {
                if (distance_to_player >= 0.7f)
                {
                    move_point = -1f;

                }
                else if (distance_to_player <= 0.2f)
                {
                    move_point = 1f;

                }

            }
            else
            {
                if (distance_to_player >= 0.7f)
                {
                    move_point = -0.25f;

                }
                else if (distance_to_player <= 0.2f)
                {
                    move_point = 0.25f;

                }
            }
            look_at.transform.position = new Vector3(look_at.transform.position.x, look_at.transform.position.y + (move_point * Time.deltaTime), look_at.transform.position.z);
        }
    }

    public void Moving()
    {
        moving = true;
    }

    private void Idle()
    {
        moving = false;
    }

    public void ResetMovement(Vector3 new_position)
    {
        online = false;
        look_at.transform.position = new Vector3(look_at.transform.position.x, new_position.y, look_at.transform.position.z);
    }
    public void SetOnline(bool value)
    {
        online = value;
    }
}
