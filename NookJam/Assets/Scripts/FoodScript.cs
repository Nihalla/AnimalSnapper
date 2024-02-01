using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodScript : MonoBehaviour
{
    [SerializeField] private GameObject animal;
    public bool isMeat;
    public bool isWater; 

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider collider)
    {
        //Debug.Log("ANIMAL ENTERED " + collider.gameObject.name); 
        /*if (((collider.gameObject.tag == "Pig" || collider.gameObject.tag == "chicken" ||
            collider.gameObject.tag == "Rooster" || collider.gameObject.tag == "Sheep" ||
            collider.gameObject.tag == "Turkey" || collider.gameObject.tag == "Cow") &&
            !isMeat) 
            ||
            (collider.gameObject.tag == "Dog" ))*/
        if (collider.gameObject.layer == 6 /*Animal*/)
        {
            if ((collider.GetComponent<Animal_Behaviour>().isCarnivore == false && isMeat == false)
                || isWater == true)
            {
                animal = collider.gameObject;
                collider.GetComponent<Animal_Behaviour>().foodSource = this.gameObject;
                collider.GetComponent<Animal_Behaviour>().current_state = Animal_Behaviour.Animal_State.EATING; 
                //collider.GetComponent<Animal_Behaviour>().SetAgentDestination(this.transform.position);
            }
            else if ((collider.GetComponent<Animal_Behaviour>().isCarnivore == true && isMeat == true) 
                || isWater == true)
            {
                animal = collider.gameObject;
                collider.GetComponent<Animal_Behaviour>().foodSource = this.gameObject;
                collider.GetComponent<Animal_Behaviour>().current_state = Animal_Behaviour.Animal_State.EATING;
            }
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        //Debug.Log("ANIMAL EXIT ");// + collider.gameObject.name); 
        if(collider.gameObject.layer == 6 /*Animal*/)
        {
            collider.GetComponent<Animal_Behaviour>().foodSource = null;
        }
    }
    private void OnCollisionExit(Collision collider)
    {
        if (collider.gameObject.tag == "Animal")
        {
            //collider.gameObject.GetComponent<Animal_Behaviour>().isEating = false;
        }
    }
}
