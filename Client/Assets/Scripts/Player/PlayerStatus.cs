using UnityEngine;
using System.Collections;

public class PlayerStatus {
	private float currentHP = 0f;
	private float maxHP = 0f;
    public PlayerStatus(float currentHP, float maxHP) {
        this.currentHP = currentHP;
        this.maxHP = maxHP;
    }
    public void SetHealth(float currentHP, float maxHP) {
        this.currentHP = currentHP;
        this.maxHP = maxHP;
	}
}
