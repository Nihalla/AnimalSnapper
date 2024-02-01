using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularSpawning : MonoBehaviour
{
    GameObject spawnPoint;
    GameObject gameManager; 

    //public List<GameObject> animalList;

    public GameObject player;
    public GameObject prefab;

    public bool canSpawn = false;

    public float firstSpawn;
    [Range(1, 10)] public float spawnCD;


    public int numberToSpawn = 0;
    private int numberSpawned = 0;


    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        //animalList = new List<GameObject>();
    }


    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = this.transform.Find("SpawnPoint").gameObject;
        InvokeRepeating("CreateAnimal", firstSpawn, spawnCD);
    }

    public void CreateAnimal()
    {
        /*Debug.Log("size of animal list: " + animalList.Count + " CD " + spawnCD + 
            " firstSpawn " + firstSpawn);*/
        if (canSpawn && numberSpawned < numberToSpawn &&
            gameManager.GetComponent<GameManager>().animalList.Count <
            gameManager.GetComponent<GameManager>().capacity)
        {
            GameObject tempAnimal;

            tempAnimal = Instantiate(prefab, spawnPoint.transform.position, spawnPoint.transform.rotation);

            var animal_behavior = tempAnimal.gameObject.GetComponent<Animal_Behaviour>();
            if (animal_behavior != null)
            {
                animal_behavior.SetHpoint(spawnPoint);
            }
            gameManager.GetComponent<GameManager>().animalList.Add(tempAnimal);
            numberSpawned++;
        }
    }
}
