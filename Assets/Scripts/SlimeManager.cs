using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeManager : MonoBehaviour {
  public static SlimeManager instance;
  public GameObject[] slimeProgression;
  // Start is called before the first frame update
  void Start() {
    SlimeManager.instance = this;
  }

  // Update is called once per frame
  void Update() {

  }
}
