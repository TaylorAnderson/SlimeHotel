using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Star : MonoBehaviour {

  public Sprite filledSprite;
  public Sprite emptySprite;
  public bool filled = true;
  [Autohook]
  public Image image;
  // Start is called before the first frame update
  void Start() {

  }

  // Update is called once per frame
  void Update() {
    image.sprite = filled ? filledSprite : emptySprite;
  }
}
