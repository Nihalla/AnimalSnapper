using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using TMPro;

public class Player_Mov_Controller : MonoBehaviour
{
    public NookJamControls controls;
    private CharacterController controller;
    private Vector2 movement_inputs = Vector2.zero;
    private Vector2 rotate_inputs = Vector2.zero;
    /*public Camera char_camera;*/
    public Transform cam_transform;
    public AudioSource walk;
    public AudioSource shutter;
    public CinemachineVirtualCamera main_cam;
    public CinemachinePOV composer;
    public Canvas cam;
    public TMP_Text species;
    public TMP_Text dropped;
    public GameObject textbord;
    public GameObject dropped_cam;
    public GameObject held_cam;
    public GameObject marker;
    public Camera_Logic cam_logic;
    public Vector2 base_cam_speed;
    public GameObject pickup;
    public Image flash_panel;

    public float mark_timer;
    public float flash_duration = 0.5f;
    private float flash_timer = 0.0f;
    private bool flash_active = false;
    private bool known = false;
    public bool holding_camera = true;
    public bool camera_mode = false;
    public bool journal_mode = false;
    private bool near_cam = false;
    [Range(20, 50)] public float max_cast = 20f;

    private List<GameObject> cam_ui_elements = new();
    private List<GameObject> journal_ui_elements = new();

    public float move_speed = 1.0f;
    public float cam_sensitivity_debuff = 0.5f;

    public float deadzone = 0.1f;
    public float turn_smooth_time = 0.1f;
    private float turn_smooth_velocity;
    public Vector3 hit_direction;

    public int number_jumps = 1;
    public float knockback_force = 10.0f;
    public float knockback_velocity = 1.0f;
    public float jump_force = 10.0f;
    private float jump_velocity = 0.0f;
    [System.NonSerialized] public int jump_attempts = 0;

    public float gravity = 9.8f;
    public float additional_decay = 0.0f;
    public float decay_multiplier = 0.2f;
    public bool move_online = true;
    public bool flying = false;

    public bool jumping = false;
    public bool looking = false;
    public bool landing = false;
    private bool rotate_lock = false;
    public float backwards_multiplier = -0.5f;
    public GameObject[] animals;
    private Screenshot cam_script;
    private string update_tag = "";

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        cam_script = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Screenshot>();
        pickup.gameObject.SetActive(false);
        flash_panel.color = new Color(255,255,255,0);
        composer = main_cam.GetCinemachineComponent<CinemachinePOV>();
        cam_transform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        base_cam_speed = new Vector2(composer.m_HorizontalAxis.m_MaxSpeed, composer.m_VerticalAxis.m_MaxSpeed);
        textbord = GameObject.FindGameObjectWithTag("TextBord");
        cam_logic = gameObject.GetComponent<Camera_Logic>();
        foreach (Transform child in cam.transform)
        {
            if(child.gameObject.tag == "Cam_UI")
            {
                cam_ui_elements.Add(child.gameObject);
            }
            else if(child.gameObject.tag == "Journal_UI")
            {
                journal_ui_elements.Add(child.gameObject);
            }
        }
        SetUpControls();
    }
    /// <summary>
    /// Sets up new input system controls and their functions
    /// </summary>
    private void SetUpControls()
    {
        controls = new();
        controls.Player.Move.performed += ctx => movement_inputs = SanInput(ctx.ReadValue<Vector2>());
        controls.Player.Move.canceled += ctx => movement_inputs = Vector2.zero;
        controls.Player.Move.canceled += ctx => walk.enabled = false;
        controls.Player.Look.performed += ctx => rotate_inputs = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => rotate_inputs = Vector2.zero;
        controls.Player.Jump.performed += ctx => Jump();
        controls.Player.Fire.performed += ctx => Fire();
        controls.Player.PhotoMode.performed += ctx => PhotoMode();
        controls.Player.Journal.performed += ctx => Journal();
        controls.Player.JournalLeft.performed += ctx => JournalLeft();
        controls.Player.JournalRight.performed += ctx => JournalRight();
        controls.Player.Lookatcamera.performed += ctx => rotateTowards();
        controls.Player.Lookatcamera.canceled += ctx => looking = false;
        controls.Player.Interact.performed += ctx => HandleInteraction();
        controls.Player.Debug_Leave.performed += ctx => Application.Quit();
    }

    private void OnTriggerEnter(Collider other)
    {  
       if (other.gameObject.layer == 6 && other.gameObject.GetComponent<Animal_Behaviour>().aggressive_to_player == true)
        {
            other.gameObject.GetComponent<Animal_Behaviour>().attacked = true;
            if (holding_camera)
            {
                var temp_cam = Instantiate(dropped_cam, transform.position, transform.rotation);
                temp_cam.GetComponent<Rigidbody>().isKinematic = false;
            }
            move_online = false;
            hit_direction = (other.gameObject.transform.forward);
            Knock();
            holding_camera = false;
            camera_mode = false;
            cam_logic.RevertZoom();
            if (known == false)
            {
                known = true;
                dropped.gameObject.SetActive(true);
            }
            foreach (var element in cam_ui_elements)
            {
                element.SetActive(false);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "Camera")
        {
            pickup.gameObject.SetActive(true);
            near_cam = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Camera")
        {
            pickup.gameObject.SetActive(false);
            near_cam = false;
        }
      
    }

    private void FixedUpdate()
    {
        HandleJump();
        HandleMovement();

        if (!holding_camera)
        {
            held_cam.SetActive(false);
        }

        if (Physics.Raycast(cam_transform.transform.position, cam_transform.transform.forward, out RaycastHit hit, max_cast) && (hit.transform.gameObject.layer == 6)
             && camera_mode == true)
        {
            if (textbord != null && !textbord.activeInHierarchy)
            {
                textbord.SetActive(true);
            }

            if (hit.transform.gameObject.GetComponent<Animal>().marked == false)
            {
                species.text = "???";
                species.gameObject.SetActive(true);
            }

            else if (hit.transform.gameObject.GetComponent<Animal>().marked == true)
            {
                species.text = hit.transform.gameObject.tag;
                if(hit.transform.gameObject.tag == "chicken")
                {
                    species.text = "Chicken";
                }
                species.gameObject.SetActive(true);
            }
        }
        else
        {
            species.text = null;

            if(textbord != null && textbord.activeInHierarchy)
            {
                textbord.SetActive(false);
            }
        }
    }

    private void Update()
    {
        HandleCinemachine();

        if (mark_timer > 0)
        {
            mark_timer -= Time.deltaTime;
        }

        if (mark_timer <= 0)
        {
            marker.SetActive(false);
        }

        if (flash_active)
        {
            flash_timer += Time.deltaTime;
            if (flash_timer < 0)
            {
                var gradient = Math3Rule(flash_duration/2, 100, flash_timer + flash_duration / 2);
                flash_panel.color = new Color(255, 255, 255, gradient * 0.01f);
            }
            else if (flash_timer < flash_duration / 2)
            {
                var gradient = Math3Rule(flash_duration / 2, 100, flash_timer);
                gradient = 100 - gradient;
                flash_panel.color = new Color(255, 255, 255, gradient * 0.01f);
            }
            else
            {
                flash_active = false;
            }
        }

        if(update_tag != "")
        {
            cam_script.CallScreenShot(Screen.width, Screen.height, update_tag);
            update_tag = "";
        }
    }

    private void HandleInteraction()
    {
        if (near_cam)
        {
            holding_camera = true;
            near_cam = false;
            Destroy(GameObject.FindGameObjectsWithTag("Camera")[0]);
            pickup.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// Function that deals with the logic behind jumping state
    /// </summary>
    private void HandleJump()
    {
        if (controller.isGrounded)
        {
            if (jumping && additional_decay >= decay_multiplier)
            {
                jumping = false;
                additional_decay = 0.0f;
                jump_attempts = 0;
                if (flying)
                {
                    flying = false;
                    move_online = true;
                }
            }
            
        }
        if (jumping)
        {
            jump_velocity -= (gravity * Time.deltaTime) + additional_decay;
            additional_decay += (Time.deltaTime * move_speed * decay_multiplier);
        }

    }
    /// <summary>
    /// Main function that handles movement and coordenates the other functions into working.
    /// </summary>
    private void HandleMovement()
    {
        if(Compare2Deadzone(movement_inputs.x) || Compare2Deadzone(movement_inputs.y))
        {
            walk.enabled = true;
        }
        else
        {
            walk.enabled = false;
        }
        if (move_online)
        {
            Vector3 input_direction = new(movement_inputs.x, 0.0f, movement_inputs.y);
        }
        Vector3 rotation_direction = new(rotate_inputs.x, 0.0f, rotate_inputs.y);
        Vector3 rotate = RotateCalc(rotation_direction, cam_transform.transform.eulerAngles.y);
        Vector3 movement = XZMoveCalc(rotate);

        if (jumping)
        {
            walk.enabled = false;
        }

        movement.y = jump_velocity;
        var movement_motion = move_speed * Time.deltaTime * movement;
        if (rotate_lock)
        {
            movement_motion *= backwards_multiplier;
        }
        controller.Move(movement_motion);
    }

    private void HandleCinemachine()
    {
        
        if(camera_mode)
        {
            composer.m_VerticalAxis.m_MaxSpeed = base_cam_speed.y * cam_sensitivity_debuff;
            composer.m_HorizontalAxis.m_MaxSpeed = base_cam_speed.x * cam_sensitivity_debuff;
        }
        else
        {
            composer.m_VerticalAxis.m_MaxSpeed = base_cam_speed.y;
            composer.m_HorizontalAxis.m_MaxSpeed = base_cam_speed.x;
        }
    }
    /// <summary>
    /// Checks if player moving forwards or backwards.
    /// Its a bit jank, but needs to be here due to bug when moving backwards.
    /// </summary>
    private Vector2 SanInput(Vector2 input)
    {
        rotate_lock = false;
        if (input.y < 0)
        {
            rotate_lock = true;
        }
        if(move_online)
        {
            return input;
        }
        return (Vector2.zero);
    }
    /// <summary>
    /// The starting function for the jump logic that handles logic on input
    /// </summary>
    private void Jump()
    {
        if (jump_attempts < number_jumps && !landing)
        {
            jumping = true;
            jump_velocity = jump_force;
            additional_decay = 0.0f;
            jump_attempts += 1;
        }
    }
    private void Knock()
    {
        if (!landing)
        {
            flying = true;
            jumping = true;
            jump_velocity = knockback_force;
            additional_decay = 0.0f;
            jump_attempts += 1;
        }
    }
    /// <summary>
    /// Handles the player rotation by finding the angle formed by the inputs and then rotating the player to match the angle using the desired anchor (usually Forward)
    /// </summary>
    /// <param name="inputs">Inputs - Vector that stores the inputs being used to calculate rotation</param>
    /// <param name="anchor_rotation">Anchor - Vector used as reference to direction to apply the rotation (for normal character rotation use forward)</param>
    /// <returns>Vector representative of current rotation on Y-axis</returns>
    private Vector3 RotateCalc(Vector3 inputs, float anchor_rotation)
    {
        inputs.Normalize();
        float rotateAngle = Mathf.Atan2(inputs.x, inputs.z) * Mathf.Rad2Deg + anchor_rotation;
        float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotateAngle, ref turn_smooth_velocity, turn_smooth_time);
        transform.rotation = Quaternion.Euler(0.0f, smoothAngle, 0.0f);

        return new Vector3(0.0f, rotateAngle, 0.0f);
    }
    /// <summary>
    /// Handles the step of movement on the given direction.
    /// </summary>
    /// <param name="direction">Direction - Direction of movement</param>
    /// <returns>Vector representing total movement</returns>
    private Vector3 XZMoveCalc(Vector3 direction)
    {
        Vector3 move_direction;
        Vector3 forward;
        Vector3 right;
        if (move_online)
        {
            move_direction = direction;
            forward = Quaternion.Euler(move_direction) * Vector3.forward;
            if (movement_inputs.y < 0)
            {
                forward = Quaternion.Euler(move_direction) * Vector3.back;
            }
            right = Quaternion.Euler(move_direction) * Vector3.right;
            if (movement_inputs.x < 0)
            {
                right = Quaternion.Euler(move_direction) * Vector3.left;
            }
        }
        else
        {
            move_direction = hit_direction;
            forward = Quaternion.Euler(move_direction) * Vector3.forward;
            if (hit_direction.z < 0)
            {
                forward = Quaternion.Euler(move_direction) * Vector3.back;
            }
            right = Quaternion.Euler(move_direction) * Vector3.right;
            if (hit_direction.x < 0)
            {
                right = Quaternion.Euler(move_direction) * Vector3.left;
            }
        }

        

        Vector3 movement_y = forward;
        Vector3 movement_x = right;
        movement_x.Normalize();
        movement_y.Normalize();
        /*Vector3 forward = Quaternion.Euler(direction).normalized * Vector3.forward;*/
        
        if (!flying && !Compare2Deadzone(movement_inputs.x))
        {
            movement_x = Vector3.zero;
        }
        if (!flying && !Compare2Deadzone(movement_inputs.y))
        {
            movement_y = Vector3.zero;
        }
        var movement = movement_y + movement_x;

        if (move_online)
        {
            movement *= move_speed;
        }
        else
        {
            movement *= knockback_velocity;
        }
        
        return movement;
    }
    /// <summary>
    /// Compares the input given by the player to the deadzone value.
    /// Serves to avoid stick drift.
    /// </summary>
    /// <param name="value">
    /// Value - Refers to input_value</param>
    /// <returns></returns>
    private bool Compare2Deadzone(float value)
    {
        if (value < deadzone)
        {
            if (value > -deadzone)
            {
                return false;
            }
        }
        return true;
    }
    private void Fire()
    {
        if (camera_mode)
        {
            RaycastHit hit;

            if (Physics.Raycast(cam_transform.transform.position, cam_transform.transform.forward, out hit, max_cast))
            {
                shutter.Play();
                if(hit.transform.gameObject.layer == 6)
                {
                    animals = GameObject.FindGameObjectsWithTag(hit.transform.tag);
                    update_tag = hit.transform.tag;
                    foreach (GameObject animal in animals)
                    {
                        if (animal.layer == 6)
                        { animal.transform.GetComponent<Animal>().marked = true; }
                    }
                    if(!flash_active)
                    {
                        flash_active = true;
                        flash_timer = -flash_duration / 2;
                    }
                }
            }
        }
    }
    private void PhotoMode()
    {
        if (holding_camera && !journal_mode)
        {
            camera_mode = !camera_mode;
            if(!camera_mode)
            {
                CameraUI(camera_mode);
            }
            cam_logic.PlayAnimation(camera_mode);
            /*CameraUI(camera_mode);*/
        }
    }
    private void PhotoMode( bool value)
    {
        if (holding_camera && !cam_logic.isAnimationPlaying())
        {
            camera_mode = value;
            if (!camera_mode)
            {
                CameraUI(camera_mode);
            }
            cam_logic.PlayAnimation(camera_mode);
            /*CameraUI(camera_mode);*/

        }
    }
    public void CameraUI(bool value)
    {
        foreach (var element in cam_ui_elements)
        {
            element.SetActive(camera_mode);
        }
        cam_logic.RevertZoom();
    }
    private void Journal()
    {
        journal_mode = !journal_mode;
        if (journal_mode == true && camera_mode == true)
        { 
            PhotoMode(false);
        }
        foreach(var element in journal_ui_elements)
        {
            element.SetActive(journal_mode);
        }
    }
    private void JournalLeft()
    {
        foreach (var element in journal_ui_elements)
        {
            if (element.GetComponent<AnimalLog>() == true)
            {
                element.GetComponent<AnimalLog>().menu_slider.value -= 1;
                if (element.GetComponent<AnimalLog>().menu_slider.value < element.GetComponent<AnimalLog>().menu_slider.minValue)
                {
                    element.GetComponent<AnimalLog>().menu_slider.value = element.GetComponent<AnimalLog>().menu_slider.maxValue;
                }
            }
        }
        cam_script.UpdatePhoto();
    }
    private void JournalRight()
    {
        foreach (var element in journal_ui_elements)
        {
            if (element.GetComponent<AnimalLog>() == true)
            {
                element.GetComponent<AnimalLog>().menu_slider.value += 1;
                if (element.GetComponent<AnimalLog>().menu_slider.value > element.GetComponent<AnimalLog>().menu_slider.maxValue)
                {
                    element.GetComponent<AnimalLog>().menu_slider.value = element.GetComponent<AnimalLog>().menu_slider.minValue;
                }
            }
        }
        cam_script.UpdatePhoto();
    }

    /// <summary>
    /// Enables usage of the Player input map from new input system.
    /// </summary>
    public void EnableInput()
    {
        controls.Player.Enable();
    }
    /// <summary>
    /// Diables usage of the Player input map from the new input system
    /// </summary>
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
    private float Math3Rule( float a, float b, float A)
    {
        return ((A * b) / a);
    }

    protected void rotateTowards()
    {
        //Debug.Log("Look");
        looking = true;
        if (!holding_camera)
        {
            if (mark_timer <= 0)
            {
                mark_timer = 5.0f;
            }
            marker.SetActive(true);

        }
    }

    public bool GetMode()
    {
        return camera_mode;
    }
}
