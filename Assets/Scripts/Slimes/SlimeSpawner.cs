using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class SlimeSpawner : Pausable {
  public SlimeSpawnPos[] spawnPositions;
  public float slimeSpawnRate;
  private float slimeSpawnTimer;
  // Start is called before the first frame update
  override public void Start() {
    base.Start();
    slimeSpawnTimer = slimeSpawnRate * 3;
  }

  // Update is called once per frame
  override public void PausableUpdate() {
    slimeSpawnTimer += Time.deltaTime;
    if (slimeSpawnTimer > slimeSpawnRate) {
      slimeSpawnTimer -= slimeSpawnRate;
      var randomSpawnPos = UnityEngine.Random.Range(0, spawnPositions.Length);

      var randomInt = UnityEngine.Random.value;
      var tupleList = new List<Tuple<SlimeColor, float>>();

      for (int i = 0; i < SlimeManager.instance.slimeColors.Length; i++) {
        var value = SlimeManager.instance.slimeColors[i];
        tupleList.Add(Tuple.Create(value, value.probability));
      }
      var color = ListUtil.WeightedPick(tupleList);
      Debug.Log(color.valueMultiplier);


      spawnPositions[randomSpawnPos].SpawnSlime(UnityEngine.Random.Range(0, 1), color);

    }
  }
}
