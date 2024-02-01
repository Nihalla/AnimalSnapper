using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    GameObject spawnPoint;
    GameObject gameManager; 
    public List<GameObject> animalList;

    public GameObject player;
    public GameObject chickenPrefab;
    public GameObject pigPrefab;
    public GameObject dogPrefab;
    public GameObject sheepPrefab;
    public GameObject cowPrefab;
    public GameObject cockPrefab;
    public GameObject turkeyPrefab;

    [SerializeField] private bool canSpawn = false;

    public float firstSpawn;
    [Range(1, 10)] public float spawnCD;

    [Range(1, 300)] public float maxAnimalDistance;
    [Range(1, 300)] public float spawnDistance; 

    public int numberToSpawn = 0;
    private int numberSpawned = 0;

    public enum AnimalType
    {
        CHICKEN,
        PIG,
        DOG, 
        SHEEP,
        COW,
        COCK, 
        TURKEY
    }

    public AnimalType animalType;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gameManager = GameObject.FindGameObjectWithTag("GameManager"); 

        animalList = new List<GameObject>();
    }


    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = this.transform.Find("SpawnPoint").gameObject;
        //InvokeRepeating("CreateAnimal", firstSpawn, spawnCD);
        InvokeRepeating("CreateAnimal", firstSpawn, spawnCD);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void CreateAnimal()
    {
        /*Debug.Log("size of animal list: " + animalList.Count + " CD " + spawnCD + 
            " firstSpawn " + firstSpawn);*/
        if (Vector3.Distance(player.transform.position, this.transform.position) < spawnDistance)
        {
            canSpawn = true; 
        }
        else 
        {
            canSpawn = false;
        }

        if (canSpawn && numberSpawned < numberToSpawn && 
            gameManager.GetComponent<GameManager>().animalList.Count < 
            gameManager.GetComponent<GameManager>().capacity)
        {
            GameObject tempAnimal;
            if (animalType == AnimalType.CHICKEN)
            {
                tempAnimal = Instantiate(chickenPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            }
            else if (animalType == AnimalType.PIG)
            {
                tempAnimal = Instantiate(pigPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            }
            else if (animalType == AnimalType.DOG)
            {
                tempAnimal = Instantiate(dogPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            }
            else if (animalType == AnimalType.SHEEP)
            {
                tempAnimal = Instantiate(sheepPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            }
            else if (animalType == AnimalType.COW)
            {
                tempAnimal = Instantiate(cowPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            }
            else if (animalType == AnimalType.COCK)
            {
                tempAnimal = Instantiate(cockPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            }
            else
            {
                tempAnimal = Instantiate(turkeyPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            }

            //tempAnimal.transform.localScale = new Vector3(50, 50, 50);
            Debug.Log("Temp animal position at spawn " + tempAnimal.transform.position);
            tempAnimal.GetComponent<Animal_Behaviour>().maxAnimalDistance = maxAnimalDistance; 
            /*tempAnimal.transform.position = spawnPoint.transform.position;
            tempAnimal.transform.rotation = spawnPoint.transform.rotation;*/
            //tempAnimal.transform.parent = gameManager.transform.Find("Animals"); 

            var animal_behavior = tempAnimal.gameObject.GetComponent<Animal_Behaviour>();
            if(animal_behavior != null)
            {
                animal_behavior.SetHpoint(spawnPoint);
            }
            //Debug.Log("H point: " + tempAnimal.GetComponent<Animal_Behaviour>().GetHPoint().transform.position);

            //animalList.Add(tempAnimal);
            gameManager.GetComponent<GameManager>().animalList.Add(tempAnimal); 
            numberSpawned++;
        }
        
    }
}
