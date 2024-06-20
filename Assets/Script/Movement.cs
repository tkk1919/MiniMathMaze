using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;

public class Movement : MonoBehaviour
{
    private InputWrapper _inputs;
    private Animator _animator;
    private Vector2 _velocity;
    [SerializeField] private float _speed;
    [SerializeField] private float _rotateSmoothing = 10f;
    private Rigidbody _rb;
    private int current_key;

    [SerializeField] private GameObject enigmaUI;
    [SerializeField] private GameObject FinishUI;
    [SerializeField] private GameObject KeyGO;

    [SerializeField] private EnigmaManager enigma_manager;

    [SerializeField] private GameObject TutoUI;
    [SerializeField] private GameObject CurrentLevelUI;

    [SerializeField] private GameObject NextLevelUI;
    [SerializeField] private GameObject OnKeyUI;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _inputs = GetComponent<InputWrapper>();
        _animator = GetComponent<Animator>();


        enigmaUI.SetActive(false);
        FinishUI.SetActive(false);

        NextLevelUI.SetActive(false);
        OnKeyUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        _velocity = _inputs._move * _speed;
        IsMovingAnimation();
        Rotate();
    }

    void FixedUpdate()
    {
        _rb.velocity = new Vector3(_velocity.x, 0, _velocity.y);
    }

    private void IsMovingAnimation()
    {
        if (_inputs._move.x > 0.1 || _inputs._move.x < -0.1 || _inputs._move.y > 0.1 || _inputs._move.y < -0.1)
        {
            _animator.SetBool("IsMoving", true);
        }
        else
        {
            _animator.SetBool("IsMoving", false);
        }
    }

    private void Rotate()
    {
        Vector3 playerDirection = Vector3.right * _inputs._move.x + Vector3.forward * _inputs._move.y;
        if (playerDirection.sqrMagnitude > 0.0f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(playerDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotateSmoothing * Time.deltaTime);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enigma")
        {
            enigmaUI.SetActive(true);
            enigma_manager.current_door = other.gameObject;
        }

        if (other.gameObject.tag == "Key")
        {
            current_key++;

            OnKeyUI.SetActive(true);
            StartCoroutine(enigma_manager.HideFeedbackAfterGivenTime(0.8f, OnKeyUI));
            Destroy(other.gameObject);
        }

        if (other.gameObject.tag == "Door")
        {
            if (current_key > 0)
            {
                current_key--;
                enigma_manager.current_difficulty++;
                NextLevelUI.SetActive(true);
                StartCoroutine(enigma_manager.HideFeedbackAfterGivenTime(0.8f, NextLevelUI));
                Destroy(other.gameObject);
            }
        }

        if (other.gameObject.tag == "EndingDoor")
        {
            if (current_key > 0)
            {
                current_key--;
                enigma_manager.current_difficulty++;
                Destroy(other.gameObject);
            }
        }

        if (other.gameObject.tag == "End")
        {
            FinishUI.SetActive(true);
            GameObject player = FindObjectOfType<Movement>().gameObject;
            player.GetComponent<Movement>().enabled = false;
            _animator.SetBool("IsMoving", false);
        }

        if (other.gameObject.tag == "Lvl1")
        {
            CurrentLevelUI.GetComponent<TextMeshProUGUI>().text = "Level 1";
        }
        if (other.gameObject.tag == "Lvl2")
        {
            TutoUI.SetActive(false);
            CurrentLevelUI.GetComponent<TextMeshProUGUI>().text = "Level 2";
            enigma_manager.current_min_clamp = 0;
            enigma_manager.current_max_clamp = 6;

            enigma_manager.RandomEnigma();
        }
        if (other.gameObject.tag == "Lvl3")
        {
            CurrentLevelUI.GetComponent<TextMeshProUGUI>().text = "Level 3";
            enigma_manager.current_min_clamp = 0;
            enigma_manager.current_max_clamp = 9;
            enigma_manager.RandomEnigma();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enigma")
        {
            enigmaUI.SetActive(false);
        }
        if (other.gameObject.tag == "End")
        {
            FinishUI.SetActive(false);
        }
    }
}
