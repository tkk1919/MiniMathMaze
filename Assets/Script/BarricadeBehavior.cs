using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarricadeBehavior : MonoBehaviour
{
    [SerializeField] private GameObject enigmaUI;
    // Start is called before the first frame update
    void Start()
    {
        enigmaUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        enigmaUI.SetActive(false);
    }
}
