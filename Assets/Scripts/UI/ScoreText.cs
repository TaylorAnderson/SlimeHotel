﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreText : MonoBehaviour {

  [Autohook]
  public SuperTextMesh text;
  // Start is called before the first frame update
  void Start() {

  }

  // Update is called once per frame
  void Update() {
    text.text = "$" + DisplayUtil.InsertCommas(GameManager.instance.income);
  }
}
