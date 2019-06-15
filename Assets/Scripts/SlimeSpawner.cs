using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSpawner : MonoBehaviour {
  public SlimeSpawnPos[] spawnPositions;
  public float slimeSpawnRate;
  private float slimeSpawnTimer;
  // Start is called before the first frame update
  void Start() {
    slimeSpawnTimer = slimeSpawnRate;
  }

  // Update is called once per frame
  void Update() {
    slimeSpawnTimer += Time.deltaTime;
    if (slimeSpawnTimer > slimeSpawnRate) {
      slimeSpawnTimer = 0;
      spawnPositions[Random.Range(0, spawnPositions.Length)].SpawnSlime(Random.Range(0, 1), SlimeManager.instance.slimeColors[Random.Range(0, SlimeManager.instance.slimeColors.Length)]);
    }
  }
}
