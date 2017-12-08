using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestItem : MonoBehaviour {
    public GameObject chestCanvas;
    GameObject chestScreen = null;

    //possible items are the 3 special weapons
    public GameObject[] possibleChestItems;

    PlayerControls player; //player's script

    AudioSource getItemSound;

	void Start () {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControls>();

        SpawnItem();
    }

    void SpawnItem() {
        getItemSound = GameObject.FindGameObjectWithTag("UIBuy").GetComponent<AudioSource>();

        chestScreen = Instantiate(chestCanvas);

        int item = Random.Range(0, possibleChestItems.Length);

        GameObject button = Instantiate(possibleChestItems[item], chestScreen.transform, false);
        button.GetComponentsInChildren<Text>()[1].text = "";
        button.GetComponent<Button>().onClick.AddListener(() => GetItem(item, button));
    }

    void GetItem(int item, GameObject button, bool newItem = true) {
        bool swap = false;
        int oldSpecial = -1;

        switch (item) {
        case 0: //grenade launcher
        case 1: //shotgun
        case 2: //ring cannon
            if (player.specialType != 3) { //swap specials
                swap = true;
                oldSpecial = player.specialType;
            }

            player.CollectSpecial(item + 1);

            if (swap && newItem) {
                player.ResetAmmo(item);
            }

            break;
        }

        getItemSound.PlayOneShot(getItemSound.clip);

        if (!swap || oldSpecial == item) {
            RemoveButton(button);
        } else {
            SwapButton(button, oldSpecial);
        }
    }

    void RemoveButton(GameObject button) { //this item has been obtained
        Destroy(button);
    }

    void SwapButton(GameObject button, int specialType) {
        GameObject newButton = Instantiate(possibleChestItems[specialType], chestScreen.transform, false);
        newButton.GetComponentsInChildren<Text>()[1].text = "";
        newButton.GetComponent<Button>().onClick.AddListener(() => GetItem(specialType, newButton, false));

        RemoveButton(button); //old button
    }

    void DisplayUI(bool display) {
        chestScreen.SetActive(display);
    }

    void OnTriggerEnter(Collider other) { //enable shop when player is in range
        if (other.gameObject.CompareTag("Player")) {
            DisplayUI(true);
        }
    }

    void OnTriggerExit(Collider other) { //disable shop when player leaves
        if (other.gameObject.CompareTag("Player")) {
            DisplayUI(false);
        }
    }

    void OnDestroy() { //destroy all instantiated objects
        if (chestScreen != null) {
            foreach (Transform obj in chestScreen.transform.GetComponentsInChildren<Transform>()) {
                if (obj.gameObject.name.Contains("ShopItem")) {
                    Destroy(obj.gameObject);
                }
            }

            Destroy(chestScreen);
        }
    }
}
