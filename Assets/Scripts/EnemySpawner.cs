using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

  public GameObject adventurer;
  private float spawnTimer = 0;
  private float timeToSpawn = 0;//20;

  private float minTimeToSpawn = 7;
  private float maxTimeToSpawn = 16;


  // Start is called before the first frame update
  void Start() {

  }

  // Update is called once per frame
  void Update() {
    spawnTimer += Time.deltaTime;
    if (spawnTimer > timeToSpawn) {
      spawnTimer = 0;
      timeToSpawn = Random.Range(minTimeToSpawn, maxTimeToSpawn);
      var newAdventurer = Instantiate(adventurer).GetComponent<Adventurer>();
      newAdventurer.transform.position = transform.position;
      newAdventurer.dir = Random.value > 0.5f ? -1 : 1;
    }
  }
}
