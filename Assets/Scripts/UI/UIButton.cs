using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using deVoid.Utils;
using UnityEngine.EventSystems;
public class ButtonSignal : ASignal<UIButton> { }

public class UIButton : MonoBehaviour {
  public GameObject shadow;
  public GameObject foreground;
  public Vector3 originalPosition;
  public ButtonSignal buttonSignal = new ButtonSignal();
  // Start is called before the first frame update
  void Start() {
    originalPosition = foreground.transform.position;
  }

  // Update is called once per frame
  void Update() {

  }

  public void OnDown() {
    if (shadow != null) {
      foreground.transform.position = shadow.transform.position;
    }
  }

  public void OnUp() {
    foreground.transform.position = originalPosition;
    buttonSignal.Dispatch(this);
  }
}
