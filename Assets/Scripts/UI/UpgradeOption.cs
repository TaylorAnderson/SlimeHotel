using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeOption : UIButton {
  public Upgrade upgrade;

  public ShopWindow shopWindow;

  public SuperTextMesh upgradeText;
  public SuperTextMesh priceText;
  public Image bg;
  // Start is called before the first frame update
  void Start() {

  }

  public void Init() {
    originalPosition = transform.position;
    var data = shopWindow.upgradeData[this.upgrade];
    priceText.text = "<c=money>$</c>" + DisplayUtil.InsertCommas(data.price);
    upgradeText.text = data.name;
  }

  // Update is called once per frame
  void Update() {

  }
  public void Deactivate() {
    var semi = new Color(1, 1, 1, 0.1f);
    this.priceText.color.a = 0x33;
    this.upgradeText.color.a = 0x33;
    this.priceText.Rebuild();
    this.upgradeText.Rebuild();
    this.bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, 0.5f);

  }
}
