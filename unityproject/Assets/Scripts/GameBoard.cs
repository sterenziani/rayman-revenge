using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameBoard : MonoBehaviour
{
	// These are shared between stages
	public HealthBar healthBar;
	public static int level = 1;
	public static int lives = 3;
	public static int score = 0;
	public static int ghostEatenScore;
	public static int boardWidth = 30;
	public static int boardHeight = 33;

	private int totalPellets;
	public int pelletsConsumed;
	public GameObject[,] board;
	private GameObject pacman;
	private GameObject[] ghosts;

	private AudioSource audioSource;
	public AudioClip backgroundAudioNormal;
	public AudioClip backgroundAudioScared;
	public AudioClip startupSound;
	public AudioClip eatGhostSound;
	public AudioClip victorySound;

	private bool dying = false;
	private bool eatingGhost = false;
	private bool winning = false;
	public GameObject readyText;
	public GameObject gameOverText;
	public Text scoreText;
	public Text levelText;
	public Text eatenGhostScoreText;

	void Start()
    {
		totalPellets = 0;
		pelletsConsumed = 0;
		board = new GameObject[boardWidth, boardHeight];
		// Load all interactive tiles into the array and place them on the board
		Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));
		pacman = pacman = GameObject.FindGameObjectWithTag("Player");
		ghosts = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (GameObject o in objects)
        {
            Vector2 pos = o.transform.position;
			if(o.tag == "Pellet" || o.tag == "WalkableTile" || (o.tag == "InvisibleNode" && o.GetComponent<Node>() != pacman.GetComponent<Player>().startingPosition))
			{
				if(o.GetComponent<Tile>() != null)
				{
					if(o.GetComponent<Tile>().isPellet || o.GetComponent<Tile>().isSuperPellet)
						totalPellets++;
				}
				board[(int)pos.x, (int)pos.y] = o;
			}
        }
		audioSource = transform.GetComponent<AudioSource>();
		healthBar.SetupLives(lives-1);
		StartGame();
	}

	///// STARTUP METHODS /////
	public void StartGame()
	{
		levelText.text = level.ToString();
		audioSource.PlayOneShot(startupSound);
		// Hide ghosts and Pacman, make them unable to move
		foreach (GameObject g in ghosts)
		{
			g.GetComponent<Ghost>().canMove = false;
			g.GetComponent<SpriteRenderer>().enabled = false;
		}
		pacman.transform.GetComponent<Player>().canMove = false;
		pacman.transform.GetComponent<SpriteRenderer>().enabled = false;
		StartCoroutine(ShowCharactersUponStartupAfter(2.25f));
	}

	// Shows static sprites for a bit and starts game
	IEnumerator ShowCharactersUponStartupAfter(float delay)
	{
		yield return new WaitForSeconds(delay);
		foreach (GameObject g in ghosts)
			g.GetComponent<SpriteRenderer>().enabled = true;
		pacman.transform.GetComponent<SpriteRenderer>().enabled = true;
		pacman.transform.GetComponent<Animator>().enabled = false;
		// Pause for 2 seconds and allow everyone to move
		yield return new WaitForSeconds(2);
		foreach (GameObject g in ghosts)
			g.GetComponent<Ghost>().canMove = true;
		pacman.transform.GetComponent<Player>().canMove = true;
		pacman.transform.GetComponent<Animator>().enabled = true;
		readyText.transform.GetComponent<SpriteRenderer>().enabled = false;
	}

	void Update()
	{
		UpdateUI();
		CheckPelletsConsumed();
	}

	void UpdateUI()
	{
		scoreText.text = score.ToString();
	}

	void CheckPelletsConsumed()
	{
		if (!winning && totalPellets == pelletsConsumed)
		{
			winning = true;
			PlayerWin();
		}
	}

	void PlayerWin()
	{
		level++;
		audioSource.Stop();
		audioSource.PlayOneShot(victorySound);
		foreach (GameObject g in ghosts)
		{
			g.GetComponent<Ghost>().canMove = false;
			g.GetComponent<Animator>().enabled = false;
		}
		pacman.transform.GetComponent<Player>().Celebrate();
		pacman.transform.GetComponent<Player>().canMove = false;
		StartCoroutine(ChangeLevel(6));
	}

	// Makes Pacman dance for {delay} seconds and loads next scene
	IEnumerator ChangeLevel(float delay)
	{
		int iterations = 6;
		delay = delay / (iterations * 2);
		for (int i = 0; i < iterations; i++)
		{
			pacman.transform.GetComponent<Player>().direction = Vector2.right;
			yield return new WaitForSeconds(delay);
			pacman.transform.GetComponent<Player>().direction = Vector2.left;
			yield return new WaitForSeconds(delay);
		}
		int nextLevel = 1 + ((level+1) % 2);
		SceneManager.LoadScene("Level" +nextLevel.ToString());
	}



	///// DEATH METHODS /////
	public void StartDeath()
	{
		if(!dying)
		{
			dying = true;
			pacman.transform.GetComponent<Player>().canMove = false;
			foreach (GameObject g in ghosts)
				g.GetComponent<Ghost>().canMove = false;
			pacman.transform.GetComponent<Animator>().enabled = false;
			audioSource.Stop();
			StartCoroutine(ProcessDeathAfter(2));		// Freeze for 2 seconds after touched
		}
	}

	// Pauses for {delay} seconds, hides all ghosts and calls ProcessRestartAfter
	IEnumerator ProcessDeathAfter(float delay)
	{
		yield return new WaitForSeconds(delay);
		foreach (GameObject g in ghosts)
			g.GetComponent<SpriteRenderer>().enabled = false;
		pacman.transform.GetComponent<Animator>().enabled = true;
		pacman.transform.GetComponent<Player>().Die();
		StartCoroutine(ProcessRestartAfter(2.3f));	// 2.3 = Time that death animation takes
	}

	// Pauses for {delay} seconds and either restarts or quits. Calls ProcessGameOver and ShowCharactersUponRestartAfter
	IEnumerator ProcessRestartAfter(float delay)
	{
		yield return new WaitForSeconds(delay);
		pacman.GetComponent<SpriteRenderer>().enabled = false;
		lives--;
		if (lives == 0)
		{
			audioSource.Stop();
			gameOverText.GetComponent<SpriteRenderer>().enabled = true;
			StartCoroutine(ProcessGameOver(5));
		}
		else
		{
			readyText.GetComponent<SpriteRenderer>().enabled = true;
			yield return new WaitForSeconds(1);
			StartCoroutine(ShowCharactersUponRestartAfter(1));
			healthBar.RemoveLives(1);
		}
	}

	// Hides ReadyText and resets ghosts and PacMan
	IEnumerator ShowCharactersUponRestartAfter(float delay)
	{
		readyText.GetComponent<SpriteRenderer>().enabled = false;
		foreach (GameObject g in ghosts)
		{
			g.GetComponent<Ghost>().moveToSpawn();
			g.GetComponent<SpriteRenderer>().enabled = true;
		}
		pacman.transform.GetComponent<Player>().moveToSpawn();
		pacman.GetComponent<SpriteRenderer>().enabled = true;
		pacman.GetComponent<Animator>().enabled = false;
		yield return new WaitForSeconds(delay);
		Restart();
	}

	// Pauses for {delay} seconds, resets all variables and returns to main menu
	IEnumerator ProcessGameOver(float delay)
	{
		yield return new WaitForSeconds(delay);
		level = 1;
		lives = 3;
		score = 0;
		SceneManager.LoadScene("MainMenu");
	}

	public void Restart()
	{
		readyText.GetComponent<SpriteRenderer>().enabled = false;
		dying = false;
		winning = false;
		pacman.transform.GetComponent<Player>().Restart();
		foreach (GameObject g in ghosts)
			g.GetComponent<Ghost>().Restart();
	}



	///// EATING METHODS /////
	// Freezes screen and hides {ghost} while showing score. Calls ProcessEatenAfter
	public void StartEaten(Ghost ghost)
	{
		if(!eatingGhost)
		{
			eatingGhost = true;
			foreach (GameObject g in ghosts)
				g.GetComponent<Ghost>().canMove = false;
			pacman.transform.GetComponent<Player>().canMove = false;
			ghost.transform.GetComponent<SpriteRenderer>().enabled = false;
			audioSource.Stop();
			
			Vector2 pos = ghost.transform.position;
			Vector2 viewPortPoint = Camera.main.WorldToViewportPoint(pos);
			eatenGhostScoreText.GetComponent<RectTransform>().anchorMin = viewPortPoint;
			eatenGhostScoreText.GetComponent<RectTransform>().anchorMax = viewPortPoint;
			eatenGhostScoreText.GetComponent<Text>().text = ghostEatenScore.ToString();
			eatenGhostScoreText.GetComponent<Text>().enabled = true;
			audioSource.PlayOneShot(eatGhostSound);
			StartCoroutine(ProcessEatenAfter(0.75f, ghost));
		}
	}

	// Pause for {delay} seconds and unfreeze screen after {ghost} has been eaten
	IEnumerator ProcessEatenAfter(float delay, Ghost ghost)
	{
		yield return new WaitForSeconds(delay);
		foreach (GameObject g in ghosts)
			g.GetComponent<Ghost>().canMove = true;
		pacman.transform.GetComponent<Player>().canMove = true;
		pacman.transform.GetComponent<SpriteRenderer>().enabled = true;
		ghost.transform.GetComponent<SpriteRenderer>().enabled = true;
		audioSource.Play();
		eatenGhostScoreText.GetComponent<Text>().enabled = false;
		eatingGhost = false;
	}



	///// MUSIC METHODS /////
	public void PlayNormalAlarm()
	{
		audioSource.clip = backgroundAudioNormal;
		audioSource.Play();
	}

	public void PlayScaredAlarm()
	{
		audioSource.clip = backgroundAudioScared;
		audioSource.Play();
	}
}
