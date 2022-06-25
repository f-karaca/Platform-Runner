using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    [SerializeField] private int healthAmount;
    [Space(10)]
    [SerializeField] private UnityEvent OnDeadEvent;

    private GameManager _gameManager;

    private int health;

    private void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        health = healthAmount;
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            PlayerController pc = GetComponent<PlayerController>();
            if (pc != null)
                pc.CanMove = true;
            OnDeadEvent.Invoke();
        }
    }


}
