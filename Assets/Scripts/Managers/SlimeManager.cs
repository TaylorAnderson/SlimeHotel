using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlimeColor {
  public Color slimeColor;
  public float probability;
  public float valueMultiplier = 1;
}
public class SlimeManager : MonoBehaviour {
  public static SlimeManager instance;
  public GameObject[] slimeProgression;
  public SlimeColor[] slimeColors;
  // Start is called before the first frame update
  void Start() {
    SlimeManager.instance = this;
  }
  void Awake() {
    Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
  }

  // Update is called once per frame
  void Update() {

  }
}
