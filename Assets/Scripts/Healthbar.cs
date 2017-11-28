using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour {
    public float maxPoints = 100;
    public float maxPointsLimit = 150;
    public float maxHealthIncrease = 25;
    public Image healthbar;

    public float Points { get { return hp; } }

    float hp;

    void Start() {
        Reset();
    }

    void Update() {
        UpdateHealthbar();
    }

    public void Hit(float speed) {
        hp -= Damage(speed);
    }

    public void Heal(float amount) {
        if ((hp + amount) <= maxPoints) {
            hp += (int) amount;
        } else if (hp < maxPoints) {
            hp = maxPoints;
        } else { //hp == maxPoints
            //increase max health
            if (maxPoints < maxPointsLimit) {
                maxPoints += (int)maxHealthIncrease;
                hp = maxPoints;
            }
        }
    }

    public void Reset() {
        hp = maxPoints;
    }

    /// <summary>
    /// Damage based on speed (scaled by 1/1000 from m/s).
    /// </summary>
    /// <returns>Damage amount by HP.</returns>
    /// <param name="speed">Speed in m/s (default unity).</param>
    float Damage(float speed) {
        if (gameObject.tag == "Enemy")
            return GameManager.Instance.playerBulletDamage;

        return speed / 1000;
    }

    void UpdateHealthbar() {
        healthbar.fillAmount = BarWidth();
    }

    float BarWidth() {
        return hp / maxPoints;
    }
}
