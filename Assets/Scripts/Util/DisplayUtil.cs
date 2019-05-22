using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DisplayUtil {
  public static void FlashWhite(MonoBehaviour script, SpriteRenderer sprite, float timeInSeconds) {
    script.StartCoroutine(FlashWhiteForSeconds(sprite, timeInSeconds));
  }
  public static void FlashOutlineWhite(MonoBehaviour script, SpriteOutline outline, float timeInSeconds) {

    script.StartCoroutine(FlashOutlineWhiteForSeconds(outline, timeInSeconds));
  }
  public static IEnumerator FlashWhiteForSeconds(SpriteRenderer sprite, float seconds) {
    Shader shaderGUItext = Shader.Find("GUI/Text Shader");
    Shader normalShader = Shader.Find("Sprites/Default");
    sprite.material.shader = shaderGUItext;
    sprite.color = Color.white;
    yield return new WaitForSeconds(seconds);
    sprite.material.shader = normalShader;
    sprite.color = Color.white;

  }

  public static IEnumerator FlashOutlineWhiteForSeconds(SpriteOutline outline, float seconds) {
    var outlineColor = outline.color;

    outline.color = Color.white;
    outline.Regenerate();
    yield return new WaitForSeconds(seconds);
    outline.color = outlineColor;
    outline.Regenerate();

  }
}
