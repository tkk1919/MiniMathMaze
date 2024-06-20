using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 3;
    [SerializeField] private GameObject[] hearts;

    [SerializeField] private GameObject GameOverUI;

    // Start is called before the first frame update
    void Start()
    {
        GameOverUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            GameOverUI.SetActive(true);
            GameObject player =  FindObjectOfType<Movement>().gameObject;
            player.GetComponent<Movement>().enabled = false;
        }
    }


    public void OnDamage()
    {
        health--;
        Debug.Log(health);
        foreach (var heart in hearts)
        {
            heart.SetActive(false);
        }
        for (int i = 0; i < health; i++)
        {
            hearts[i].SetActive(true);
        }
    }


}
