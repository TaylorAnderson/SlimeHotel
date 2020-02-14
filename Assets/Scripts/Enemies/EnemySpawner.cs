using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Pausable {

  public Enemy adventurer;
  private float spawnTimer = 0;
  public float timeToSpawn = 20;

  public float minTimeToSpawn = 7;
  public float maxTimeToSpawn = 16;



  // Update is called once per frame
  override public void PausableUpdate() {
    spawnTimer += Time.deltaTime;
    if (spawnTimer > timeToSpawn) {
      spawnTimer = 0;
      timeToSpawn = Random.Range(minTimeToSpawn, maxTimeToSpawn);
      var newAdventurer = Instantiate(adventurer, transform.position, transform.rotation).GetComponent<Enemy>();
      newAdventurer.dir = Random.value > 0.5f ? -1 : 1;
    }
  }
}
