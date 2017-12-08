using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-179)]
public class InGameShop : MonoBehaviour {
    Text shopMoneyText;
    int shopMoney;

    public GameObject InGameShopCanvas;
    GameObject shopCanvas = null;

    //possible items are 4+ pickups, 2+ normal weapon modifiers and the 3 special weapons
    public GameObject[] possibleShopItems;
    public int[] itemCosts;

    public float playerLuckIncrease;

    PlayerControls player; //player's script

    AudioSource buyItemSound;

    public static InGameShop Instance;

    void Start () {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControls>();
        UpdateShopMoney();

        NewItems();
    }

    //decide which items to have in the shop
    void NewItems() {
        buyItemSound = GameObject.FindGameObjectWithTag("UIBuy").GetComponent<AudioSource>();

        shopCanvas = Instantiate(InGameShopCanvas);

        int numItems = possibleShopItems.Length;
        int item1, item2, item3;

        item1 = RandomNum(numItems);

        while (true) {
            item2 = RandomNum(numItems);

            if (item1 != item2) {
                while (true) {
                    item3 = RandomNum(numItems);

                    if (item1 != item3 && item2 != item3) {
                        break;
                    }
                }

                break;
            }
        }

        //create the buttons for the in-game shop and place them accordingly
        GameObject button1 = Instantiate(possibleShopItems[item1], shopCanvas.transform, false);
        button1.transform.position = new Vector3(-350f, button1.transform.position.y);
        button1.GetComponentsInChildren<Text>()[1].text = "Cost: " + itemCosts[item1];
        button1.GetComponent<Button>().onClick.AddListener(() => BuyItem(item1, button1));

        GameObject button2 = Instantiate(possibleShopItems[item2], shopCanvas.transform, false);
        button2.GetComponentsInChildren<Text>()[1].text = "Cost: " + itemCosts[item2];
        button2.GetComponent<Button>().onClick.AddListener(() => BuyItem(item2, button2));

        GameObject button3 = Instantiate(possibleShopItems[item3], shopCanvas.transform, false);
        button3.transform.position = new Vector3(350f, button3.transform.position.y);
        button3.GetComponentsInChildren<Text>()[1].text = "Cost: " + itemCosts[item3];
        button3.GetComponent<Button>().onClick.AddListener(() => BuyItem(item3, button3));
    }

    int RandomNum(int highest) {
        return Random.Range(0, highest);
    }

    void BuyItem(int item, GameObject button, bool free = false) {
        bool swap = false;
        int oldSpecial = -1;

        if (shopMoney >= itemCosts[item] || free) {
            switch (item) {
            case 0: //health pickup
            case 1: //shield pickup
            case 2: //ammo pickup
                player.CollectPickup(item);
                break;
            case 3: //increase player luck
                GameManager.Instance.playerLuck += playerLuckIncrease;
                break;
            case 4: //grenade launcher
            case 5: //shotgun
            case 6: //ring cannon
                if (player.specialType != 3) { //swap specials
                    swap = true;
                    oldSpecial = player.specialType;
                }

                player.CollectSpecial(item - 3);

                if (swap && !free) {
                    player.ResetAmmo(item - 4);
                }

                break;
            case 7: //double shot
            case 8: //triple shot
            case 9: //rapid fire
                player.CollectModifier(item - 6);
                break;
            }

            if (!free) {
                shopMoney -= itemCosts[item];
            }

            if (!swap || oldSpecial == item - 4) {
                buyItemSound.PlayOneShot(buyItemSound.clip);
                RemoveButton(button);
            } else {
                SwapButton(button, oldSpecial);
            }
        }

        UpdateGameMoney();
    }

    void RemoveButton(GameObject button) { //this item has been bought
        Destroy(button);
    }

    void SwapButton(GameObject button, int specialType) { //only for special weapons
        GameObject newButton = Instantiate(possibleShopItems[specialType+4], shopCanvas.transform, false);
        newButton.transform.position = button.transform.position;
        newButton.GetComponentsInChildren<Text>()[1].text = "Cost: 0";
        newButton.GetComponent<Button>().onClick.AddListener(() => BuyItem(specialType+4, newButton, true));

        RemoveButton(button); //old button
    }

    public void UpdateShopMoney() { //called from GameManager
        shopMoney = GameManager.Instance.money;
        GameManager.Instance.UpdateMoneyText();
    }

    void UpdateGameMoney() {
        GameManager.Instance.money = shopMoney;
        GameManager.Instance.UpdateMoneyText();
    }

    void DisplayShop(bool display) {
        shopCanvas.SetActive(display);
    }

    void OnTriggerEnter(Collider other) { //enable shop when player is in range
        if (other.gameObject.CompareTag("Player")) {
            UpdateShopMoney();
            DisplayShop(true);
        }
    }

    void OnTriggerExit(Collider other) { //disable shop when player leaves
        if (other.gameObject.CompareTag("Player")) {
            DisplayShop(false);
        }
    }

    void OnDestroy() { //destroy all instantiated objects
        if (shopCanvas != null) {
            foreach (Transform obj in shopCanvas.transform.GetComponentsInChildren<Transform>()) {
                if (obj.gameObject.name.Contains("ShopItem")) {
                    Destroy(obj.gameObject);
                }
            }

            Destroy(shopCanvas);
        }
    }
}
