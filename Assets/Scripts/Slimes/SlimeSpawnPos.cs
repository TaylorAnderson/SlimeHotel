using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSpawnPos : MonoBehaviour {
  public int dir;
  // Start is called before the first frame update
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }
  public void SpawnSlime(int size, SlimeColor color) {
    var slime = Instantiate(SlimeManager.instance.slimeProgression[size]).GetComponent<Slime>();
    slime.transform.position = transform.position;
    slime.transform.localScale = new Vector3(dir, 1, 1);
    slime.ResetDir();
    Debug.Log(color.slimeColor);
    slime.SetColor(color.slimeColor);
    slime.valueMultiplier = color.valueMultiplier;
  }
}
