using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _scoreText;

    [SerializeField]
    private Sprite[] _liveSprites;
    [SerializeField]
    private Image _LivesImg;
    [SerializeField]
    private TMP_Text _gameOverText;



    // Start is called before the first frame update
    void Start()
    {    
        _scoreText.text = "Score: " + 00;
        _gameOverText.gameObject.SetActive(false);
    }

    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }

    public void UpdateLives(int currentLives)
    {
        _LivesImg.sprite = _liveSprites[currentLives];

        if (currentLives == 0)
        {
            _gameOverText.gameObject.SetActive(true);
            StartCoroutine(GameOverFlashRoutine());
        }
    }

    IEnumerator GameOverFlashRoutine()
    {
        while (true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
