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
    private TMP_Text _ammoCountText;

    [SerializeField]
    private Sprite[] _liveSprites;
    [SerializeField]
    private Image _livesImg;

    [SerializeField]
    private Slider _thrusterBar;

    [SerializeField]
    private TMP_Text _gameOverText;
    [SerializeField]
    private TMP_Text _restartText;


    private GameManager _gameManager;



    // Start is called before the first frame update
    void Start()
    {
        _ammoCountText.text = "Ammo: " + 15.ToString();
        _scoreText.text = "Score: " + 00;

        _gameOverText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        if (_gameManager == null)
        {
            Debug.LogError("GameManager in NULL");
        }
    }

    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }

    public void UpdateLives(int currentLives)
    {
        _livesImg.sprite = _liveSprites[currentLives];

        if (currentLives == 0)
        {
            GameOverSequence();
        }
    }

    public void UpdateAmmoCount(int playerAmmo)
    {
        _ammoCountText.text = "Ammo: " + playerAmmo.ToString();
    }

    public void UpdateThrusterBar(float thrusterEnergy)
    {
        _thrusterBar.value = thrusterEnergy;
    }

    void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlashRoutine());
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
