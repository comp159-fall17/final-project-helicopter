using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour {
    public float maxPoints = 100;
    public Image healthbar;

    public float Points { get { return hp; } }

    float hp;

    void Start() {
        Reset();
    }

    void Update() {
        UpdateHealthbar();
    }

    public void Hit(BulletController other) {

        hp -= Damage(other.Speed);
    }

    public void Heal(float amount) {
        if ((hp + amount) <= maxPoints) {
            hp += (int) amount;
        } else {
            hp = maxPoints;
        }
    }

    public void Reset() {
        hp = maxPoints;
    }

    float Damage(float speed) {
        // TODO: add bullet speed scaling

        if (gameObject.tag == "Enemy")
            return GameManager.Instance.playerBulletDamage;

        return 1;
    }

    void UpdateHealthbar() {
        healthbar.fillAmount = BarWidth();
    }

    float BarWidth() {
        return hp / maxPoints;
    }
}
