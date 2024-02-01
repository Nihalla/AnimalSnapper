using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal_Behaviour : MonoBehaviour
{
    GameObject gameManager; 

    public bool aggressive_to_player = false;
    [SerializeField] public Animal_State current_state;
    private GameObject player;
    public bool attacked = false;
    public float ctimer = 0;
    private float range_for_reaction = 20f;
    private float range_for_attack = 5f;
    private float range_for_flee = 10f;
    private Rand_move move_script;
    [SerializeField] private float idle_timer_max;
    private float idle_timer;
    [SerializeField] private float movement_timer_max;
    private float movement_timer;
    private NavMeshAgent agent;
    [SerializeField] private GameObject home_point;

    public GameObject foodSource;
    [Range(1, 10)] public float eatCD;
    public float eatTimer = 0;
    public bool isEating = false;
    public bool isCarnivore;

    public float maxAnimalDistance;

    public GameObject poopObject; 
    public GameObject pawPrint;
    public GameObject pawParent;
    public GameObject[] pawPrints;
    private float pawTimer = 0;
    public float pawTimerCD = 1;
    private int pawPointer = 0;

    [Range(1, 50)] public int maxPaws;

    private Hashing hash;
    public Animator anim;

    public enum Animal_State
    {
        IDLE = 0,
        MOVING = 1,
        REACTING = 2,
        FLEEING = 3,
        ATTACKING = 4,
        GOING_HOME = 5, 
        EATING = 6
    };


    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager"); 
        current_state = Animal_State.IDLE;
        player = GameObject.FindGameObjectWithTag("Player");
        idle_timer = idle_timer_max;
        movement_timer = movement_timer_max;
        move_script = gameObject.GetComponent<Rand_move>();

        anim = gameObject.GetComponentInChildren<Animator>();
        anim?.SetLayerWeight(0, 1f);
        hash = GameObject.FindGameObjectWithTag("GameController").GetComponent<Hashing>();
    }
    private void Start()
    {
        agent = move_script.GetAgent();
        this.transform.parent = gameManager.transform.Find("Animals");

        if (pawPrint != null)
        {
            pawPrints = new GameObject[maxPaws];
            pawParent = new GameObject();
            pawParent.name = "paws of " + this.name;
            pawParent.transform.parent = gameManager.transform.Find("Paws"); 
            pawParent.transform.position = transform.position;
            int temp = Random.Range(0, pawPrints.Length);
            //bug.Log("temp " + temp);
            for (int i = 0; i < pawPrints.Length; i++)
            {
                //Debug.Log("creating paw " + i);
                if (temp != i)
                {
                    pawPrints[i] = Instantiate(pawPrint);
                }
                else
                {
                    pawPrints[i] = Instantiate(poopObject);
                }
                pawPrints[i].transform.parent = pawParent.transform;
                pawPrints[i].transform.localScale = new Vector3(2, 2, 2);
                //pawPrints[i].transform.parent = this.transform;
                //pawPrints[i].GetComponent<PawPrintScript>().setParent(this.gameObject);
                //bool isPlaced = pawPrints[i].AddComponent<bool>() as bool; 
            } 
        }
    }

    void Update()
    {
        switch (current_state)
        {
            case Animal_State.IDLE:
                UpdateAnimationState(Animal_State.IDLE);
                break;
            case Animal_State.EATING:
                UpdateAnimationState(Animal_State.EATING);
                break;
            default:
                UpdateAnimationState(Animal_State.MOVING);
                break;
        }
    }

    private void UpdateAnimationState(Animal_State effective_state)
    {
        /*switch (effective_state)
        {
            case Animal_State.IDLE:
                anim?.SetBool(hash.WalkingBool, false);
                anim?.SetBool(hash.IdleBool, true);
                break;
            case Animal_State.MOVING:
                anim?.SetBool(hash.IdleBool, false);
                anim?.SetBool(hash.WalkingBool, true);
                break;
            case Animal_State.EATING:
                anim?.SetBool(hash.WalkingBool, false);
                anim?.SetBool(hash.IdleBool, false);
                anim?.SetBool(hash.EatingBool, true);
                break;
         }*/
        if (effective_state == Animal_State.IDLE && isEating)
        {
            anim?.SetBool(hash.IdleBool, false);
            anim?.SetBool(hash.WalkingBool, false);
            anim?.SetBool(hash.EatingBool, true);
        }
        else if (effective_state == Animal_State.MOVING)
        {
            anim?.SetBool(hash.IdleBool, false);
            anim?.SetBool(hash.WalkingBool, true);
            anim?.SetBool(hash.EatingBool, false);
        }
        else if (effective_state == Animal_State.IDLE)
        {
            anim?.SetBool(hash.IdleBool, true);
            anim?.SetBool(hash.WalkingBool, false);
            anim?.SetBool(hash.EatingBool, false);
        }
        /*else
        {
            anim?.SetBool(hash.IdleBool, true);
            anim?.SetBool(hash.WalkingBool, false);
            anim?.SetBool(hash.EatingBool, false);
        }*/

    }
    private void FixedUpdate()
    {        
        // Character can only react to player if in range and only if Idle or Moving
        if (DistanceToPlayer() <= range_for_reaction && (current_state == Animal_State.IDLE || current_state == Animal_State.MOVING))
        {
            current_state = Animal_State.REACTING;
        }

        switch (current_state)
        {
            case Animal_State.IDLE:
                ResolveIdle();
                break;
            case Animal_State.MOVING:
                ResolveMovement();
                break;
            case Animal_State.REACTING:
                ResolveReaction();
                break;
            case Animal_State.FLEEING:
                ResolveFlee();
                break;
            case Animal_State.ATTACKING:
                ResolveAttack();
                break;
            case Animal_State.GOING_HOME:
                CheckHome();
                break;
            case Animal_State.EATING:
                CheckFood();
                if (isEating)
                {
                    eatTimer += Time.deltaTime; 
                    if (eatTimer < eatCD)
                    {
                        current_state = Animal_State.IDLE;
                    }
                    else
                    {
                        isEating = false;
                        eatTimer = 0; 
                    }
                }
                break; 
        }

        if (current_state != Animal_State.IDLE)
        {
            if (pawTimer > pawTimerCD)
            {
                Vector3 temp = this.transform.position;
                pawPrints[pawPointer].transform.position = new Vector3(temp.x, temp.y + 0.01f, temp.z);
                pawPrints[pawPointer].transform.rotation = this.transform.rotation;
                pawTimer = 0;
                pawPointer++;
                //Debug.Log("Pawprinted " + pawPointer);
                if (pawPointer == pawPrints.Length)
                {
                    pawPointer = 0;
                }
            }
            else
            {
                pawTimer += Time.deltaTime;
            }
        }

        if (attacked)
        {
            ctimer = 5;
            attacked = false;
        }

        if (ctimer > 0)
        {
            ctimer -= Time.deltaTime;
            current_state = Animal_State.FLEEING;
        }

        if (ctimer <= 0 && current_state == Animal_State.FLEEING)
        {
            current_state = Animal_State.IDLE;
        }



        // Character checks if it is too far away from home point after resolving each state
        // This prevents the character from straying into different character territories
        GoHome();
    }

    /// <summary>
    /// When character is Idle after a timer runs out switch to Moving
    /// </summary>
    private void ResolveIdle()
    {
        if (idle_timer <= 0)
        {
            //Debug.Log("1");
            idle_timer = idle_timer_max;
            move_script.online = true;
            isEating = false; 
            current_state = Animal_State.MOVING;
        }
        else
        {
            //Debug.Log("2");
            agent.destination = transform.position;
            idle_timer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// When character is Moving after a timer runs out switch to Idle
    /// </summary>
    private void ResolveMovement()
    {
        if(movement_timer <= 0)
        {
            movement_timer = movement_timer_max;
            move_script.online = false;
            agent.destination = transform.position;
            current_state = Animal_State.IDLE;
        }
        else
        {
            movement_timer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// When character is too close to player either set to Attack or Flee based on character aggression
    /// </summary>
    private void ResolveReaction()
    {
        move_script.online = false;
        current_state = aggressive_to_player ? Animal_State.ATTACKING : Animal_State.FLEEING;
    }

    /// <summary>
    /// When character is too close to player based on the direction to player set a new destination away from them
    /// </summary>
    private void ResolveFlee()
    {
        if (DistanceToPlayer() <= range_for_flee)
        {
            var direction_to_player = transform.position - player.transform.position;
            agent.destination = transform.position + direction_to_player;
        }
        if(DistanceToPlayer() >= range_for_reaction)
        {
            current_state = Animal_State.IDLE;
        }
    }

    /// <summary>
    /// When character is too close to player set it to follow the player.
    /// 
    /// Can possibly implement an attack as well?
    /// </summary>
    private void ResolveAttack()
    {
        if (DistanceToPlayer() >= range_for_attack)
        {
            agent.destination = player.transform.position;
        }
        else
        {
            //attack player?
        }
        if (DistanceToPlayer() >= range_for_reaction)
        {
            current_state = Animal_State.IDLE;
        }
    }

    /// <summary>
    /// Check if character is too far away from home point. If it is set it to go home.
    /// </summary>
    private void GoHome()
    {
        var distance_to_home = Vector3.Distance(home_point.transform.position, transform.position);
        if (distance_to_home > maxAnimalDistance)
        {
            agent.destination = home_point.transform.position;
            current_state = Animal_State.GOING_HOME;
        }
    }

    /// <summary>
    /// Check if character is back home. If yes it goes back to being idle
    /// </summary>
    private void CheckHome()
    {
        agent.destination = home_point.transform.position;
        if(Vector3.Distance(home_point.transform.position, transform.position) <= 5)
        {
            current_state = Animal_State.IDLE;
        }
    }

    private void CheckFood()
    {
        if (foodSource != null)
        {
            //Debug.Log("food found");
            if(Vector3.Distance(foodSource.transform.position, transform.position) <=  3/* && !isEating*/)
            {
                //Debug.Log("STARTED EATING");
                isEating = true;
                current_state = Animal_State.IDLE;
                //isEating = true;
                //eatTimer = 0; 
                //move_script.online = false; 
            }
            else
            {
                //Debug.Log("going to food"); 
                agent.destination = foodSource.transform.position;
            }
            //eatTimer += Time.deltaTime;
        }

        /*if (isEating)
        {
            Debug.Log("Is Eating");
            current_state = Animal_State.IDLE;
            if (eatTimer > eatCD / 2)
            {

                Debug.Log("STOPPED EATING");
                isEating = false;
                ResolveIdle();
            }
        }

        if (eatTimer > eatCD)
        {
            eatTimer = 0; 
        }*/
    }

    /// <summary>
    /// Returns distance to player from character
    /// </summary>
    private float DistanceToPlayer()
    {
        return Vector3.Distance(player.transform.position, transform.position);
    }

    public void SetHpoint(GameObject new_spawnpoint)
    {
        home_point = new_spawnpoint;
    }

    public GameObject GetHPoint()
    {
        return home_point;
    }

    /*private void OnTriggerStay(Collider collider)
    {
        //if (collider.transform.Find("EatTrigger").tag == "EatTrigger")
        Debug.Log("finding food");
        if (collider.gameObject.tag == "EatTrigger")
        {
            Debug.Log("FOOD FOUND");
            foodSource = collider.gameObject;
            agent.destination = collider.transform.position;
        }
    }*/
    public void SetAgentDestination(Vector3 position)
    {
        agent.destination = position;
    }
}
