using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ListUtil {

  public static void Shuffle<T>(List<T> ts) {
    var count = ts.Count;
    var last = count - 1;
    for (var i = 0; i < last; ++i) {
      var r = UnityEngine.Random.Range(i, count);
      var tmp = ts[i];
      ts[i] = ts[r];
      ts[r] = tmp;
    }
  }

  public static T WeightedPick<T>(List<Tuple<T, float>> choices) {
    float totalWeight = 0;
    for (int i = 0; i < choices.Count; i++) {
      totalWeight += choices[i].Item2;
    }
    // The weight we are after...
    float itemWeightIndex = UnityEngine.Random.value * totalWeight;
    float currentWeightIndex = 0;

    for (int j = 0; j < choices.Count; j++) {
      currentWeightIndex += choices[j].Item2;

      // If we've hit or passed the weight we are after for this item then it's the one we want....
      if (currentWeightIndex >= itemWeightIndex)
        return choices[j].Item1;
    }

    return default(T);
  }
}
