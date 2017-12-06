using UnityEngine;

public class DisableNode : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.Equals(GameManager.Instance.Player)) {
            gameObject.SetActive(false);
            transform.parent.GetComponent<EnemyRoom>().Spawn();
        }
    }
}
