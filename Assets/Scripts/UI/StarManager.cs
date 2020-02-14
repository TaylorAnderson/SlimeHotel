using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarManager : MonoBehaviour {
  public List<Star> stars;
  // Start is called before the first frame update
  void Start() {
  }

  // Update is called once per frame
  void Update() {
    for (int i = 0; i < stars.Count; i++) {
      stars[i].filled = i < GameManager.instance.stars;
    }
  }
}
