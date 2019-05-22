using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtil {

  public static float Map(float x, float fromMin, float fromMax, float toMin, float toMax) {
    return toMin + ((x - fromMin) / (fromMax - fromMin)) * (toMax - toMin);
  }

  public static float Vector2Angle(Vector2 v) {
    return Mathf.Atan2(v.y, v.x);
  }
}
