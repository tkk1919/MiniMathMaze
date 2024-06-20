using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class EnigmaManager : MonoBehaviour
{
    [SerializeField] PlayerHealth player_health;
    [SerializeField] GameObject good_answer_feedback_text;
    [SerializeField] GameObject bad_answer_feedback_text;
    [SerializeField] Enigma[] all_enigmas_;
    private List<Enigma> current_enigmas_available_;
    [SerializeField] private GameObject statement_go;
    [SerializeField] private List<GameObject> answer_btn;
    [SerializeField] private List<GameObject> btn_position;
    public GameObject current_door;
    public int current_difficulty;

    public int current_min_clamp = 0;
    public int current_max_clamp = 1;

    private bool is_player_enigma_first_error;
    private Enigma current_enigma_;
    [SerializeField] private GameObject[] choice_btn_;

    [System.Serializable]
    struct Enigma
    {
        [SerializeField] public int difficulty;
        [SerializeField] public string statement;
        [SerializeField] public string good_answer;
        [SerializeField] public string false_answer_1;
        [SerializeField] public string false_answer_2;
        [SerializeField] public string false_answer_3;
        [SerializeField] public GameObject hint;
    }

    // Start is called before the first frame update
    void Start()
    {
        current_difficulty = 0;
        current_min_clamp = 0;
        current_max_clamp = 3;
        is_player_enigma_first_error = true;
        good_answer_feedback_text.SetActive(false);
        bad_answer_feedback_text.SetActive(false);
        current_enigmas_available_ = new List<Enigma>();

        if (all_enigmas_ != null)
        {
            foreach (var enigma in all_enigmas_)
            {
                current_enigmas_available_.Add(enigma);
            }
        }

        RandomEnigma();
    }

    public void RandomEnigma()
    {
        List<Enigma> current_difficulty_enigmas = current_enigmas_available_.Where(enigma => enigma.difficulty == current_difficulty).ToList();
        if (current_difficulty_enigmas.Count <= 0)
        {
            int random_enigma = Random.Range(0, current_enigmas_available_.Count);
            current_enigma_ = current_enigmas_available_[random_enigma];
        }
        else
        {
            int random_enigma = Random.Range(0, current_difficulty_enigmas.Count);
            current_enigma_ = current_difficulty_enigmas[random_enigma];
        }

        foreach (var btn in choice_btn_)
        {
            btn.SetActive(true);
        }

        statement_go.GetComponentInChildren<TextMeshProUGUI>().text = current_enigma_.statement;
        answer_btn[0].GetComponentInChildren<TextMeshProUGUI>().text =
            current_enigma_.good_answer;
        answer_btn[1].GetComponentInChildren<TextMeshProUGUI>().text =
            current_enigma_.false_answer_1;
        answer_btn[2].GetComponentInChildren<TextMeshProUGUI>().text =
            current_enigma_.false_answer_2;
        answer_btn[3].GetComponentInChildren<TextMeshProUGUI>().text =
            current_enigma_.false_answer_3;
        RemoveEnigma(current_enigma_);
        current_difficulty_enigmas.Clear();
        SetBtnPos();
    }

    // Update is called once per frame
    void Update()
    {
       Debug.Log(current_difficulty);  
    }


    public void OnCorrectAnswer()
    {
        Destroy(current_door);
        current_difficulty++;
        ClampDifficulty(current_min_clamp, current_max_clamp);
        good_answer_feedback_text.SetActive(true);
        is_player_enigma_first_error = true;
        current_enigma_.hint.SetActive(false);
        StartCoroutine(HideFeedbackAfterGivenTime(0.8f, good_answer_feedback_text));
        RandomEnigma();
    }

    public void OnFalseAnswer(GameObject btnGameObject)
    {
        if (is_player_enigma_first_error)
        {
            current_enigma_.hint.SetActive(true);
            is_player_enigma_first_error = false;
            bad_answer_feedback_text.SetActive(true);
            StartCoroutine(HideFeedbackAfterGivenTime(0.8f, bad_answer_feedback_text));
            btnGameObject.SetActive(false);
        }
        else
        {
            player_health.OnDamage();
            current_difficulty--;
            ClampDifficulty(current_min_clamp, current_max_clamp);
            Destroy(current_door);
            bad_answer_feedback_text.SetActive(true);
            is_player_enigma_first_error = true;
            current_enigma_.hint.SetActive(false);
            StartCoroutine(HideFeedbackAfterGivenTime(0.8f, bad_answer_feedback_text));
            RandomEnigma();
        }

    }

    private void ClampDifficulty()
    {
        if (current_difficulty < 0)
        {
            current_difficulty = 0;
        }

        if (current_difficulty >= 5)
        {
            current_difficulty = 4;
        }
    }

    public void ClampDifficulty(int min, int max)
    {
        if (current_difficulty < min)
        {
            current_difficulty = min;
        }

        if (current_difficulty >= max + 1)
        {
            current_difficulty = max;
        }
    }

    public IEnumerator HideFeedbackAfterGivenTime(float timeBeforeHiding, GameObject objectToDestroy)
    {
        yield return new WaitForSeconds(timeBeforeHiding);
        objectToDestroy.SetActive(false);
    }

    private void RemoveEnigma(Enigma enigma_to_remove)
    {
        current_enigmas_available_.Remove(enigma_to_remove);
    }

    private void ShufflePositions()
    {
        int n = btn_position.Count;
        for (int i = 0; i < n; i++)
        {
            int randomIndex = Random.Range(i, n);
            (btn_position[i], btn_position[randomIndex]) = (btn_position[randomIndex], btn_position[i]);
        }
    }

    private void SetBtnPos()
    {
        ShufflePositions();
        for (int i = 0; i < answer_btn.Count; i++)
        {
            answer_btn[i].GetComponent<RectTransform>().anchoredPosition = btn_position[i].GetComponent<RectTransform>().anchoredPosition;
        }
    }
}
