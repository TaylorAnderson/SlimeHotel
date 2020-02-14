using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using deVoid.Utils;

//the bool is whether the item was bought or not
public class ItemWindowClosedSignal : ASignal<bool> { }
public class ItemWindow : MonoBehaviour {

  public UIButton buyButton;
  public UIButton backButton;
  public SuperTextMesh description;
  public SuperTextMesh headerText;
  public SuperTextMesh priceText;

  public Upgrade upgrade;
  public ItemWindowClosedSignal itemWindowClosedSignal = new ItemWindowClosedSignal();
  public ShopWindow shopWindow;

  private UpgradeData upgradeData;
  // Start is called before the first frame update
  void Start() {

  }

  public void Init() {
    upgradeData = shopWindow.upgradeData[upgrade];
    buyButton.buttonSignal.AddListener(OnBuyPressed);
    backButton.buttonSignal.AddListener(OnBackPressed);
    headerText.text = "SHOP > " + upgradeData.name;
    description.text = upgradeData.description;
    priceText.text = "<c=money>$</c>" + DisplayUtil.InsertCommas(upgradeData.price);
  }

  // Update is called once per frame
  void Update() {

  }
  void OnBuyPressed(UIButton button) {
    if (GameManager.instance.income > upgradeData.price) {
      GameManager.instance.upgrades.Add(upgrade);
      this.itemWindowClosedSignal.Dispatch(true);
      Destroy(this.gameObject);
    }
  }
  void OnBackPressed(UIButton button) {
    itemWindowClosedSignal.Dispatch(false);
    Destroy(this.gameObject);
  }
}
