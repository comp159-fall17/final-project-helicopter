using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameShop : MonoBehaviour {
    public Text shopMoneyText;
    int shopMoney;

    public GameObject InGameShopCanvas;

    //possible items are 4+ pickups, 2+ normal weapon modifiers
    public GameObject[] possibleShopItems;
    public int[] itemCosts;
    
    GameObject[] shopItems = new GameObject[3]; //always have 3

    public static InGameShop Instance;

	void Start () {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }

        NewItems();
    }

    //decide which items to have in the shop
    void NewItems() {
        for (int i = 0; i < possibleShopItems.Length; i++) {
            possibleShopItems[i].transform.position = new Vector3(0f, possibleShopItems[i].transform.position.y);
        }

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

        shopItems[0] = possibleShopItems[item1];
        shopItems[1] = possibleShopItems[item2];
        shopItems[2] = possibleShopItems[item3];

        shopItems[0].transform.position = new Vector3(-350f, shopItems[0].transform.position.y);
        shopItems[2].transform.position = new Vector3(350f, shopItems[2].transform.position.y);

        shopItems[0].GetComponentsInChildren<Text>()[1].text = "Cost: " + itemCosts[item1];
        shopItems[1].GetComponentsInChildren<Text>()[1].text = "Cost: " + itemCosts[item2];
        shopItems[2].GetComponentsInChildren<Text>()[1].text = "Cost: " + itemCosts[item3];

        GameObject button1 = Instantiate(shopItems[0], InGameShopCanvas.transform, false);
        button1.GetComponent<Button>().onClick.AddListener(() => BuyItem(item1, button1));

        GameObject button2 = Instantiate(shopItems[1], InGameShopCanvas.transform, false);
        button2.GetComponent<Button>().onClick.AddListener(() => BuyItem(item2, button2));

        GameObject button3 = Instantiate(shopItems[2], InGameShopCanvas.transform, false);
        button3.GetComponent<Button>().onClick.AddListener(() => BuyItem(item3, button3));


    }

    int RandomNum(int highest) {
        return Random.Range(0, highest);
    }

    void BuyItem(int item, GameObject button) { //6 items for testing
        if (shopMoney >= itemCosts[item]) {
            Debug.Log("Bought item " + (item+1)); //TODO

            shopMoney -= itemCosts[item];

            RemoveButton(button);
        }

        UpdateGameMoney();
    }

    void RemoveButton(GameObject button) { //this item has been bought
        Destroy(button);
    }

    public void UpdateShopMoney() { //called from GameManager
        shopMoney = GameManager.Instance.money;
        shopMoneyText.text = "Money: " + shopMoney;
    }

    void UpdateGameMoney() {
        shopMoneyText.text = "Money: " + shopMoney;
        GameManager.Instance.money = shopMoney;
    }

    void DisplayShop(bool display) {
        InGameShopCanvas.SetActive(display);
    }

    void OnTriggerEnter(Collider other) { //enable shop when player is in range
        if (other.gameObject.CompareTag("Player")) {
            DisplayShop(true);
        }
    }

    void OnTriggerExit(Collider other) { //disable shop when player leaves
        if (other.gameObject.CompareTag("Player")) {
            DisplayShop(false);
        }
    }
}
