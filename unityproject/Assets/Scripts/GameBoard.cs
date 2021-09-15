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
        foreach (GameObject o in objects)
        {
            Vector2 pos = o.transform.position;
			// Pellet, InvisibleNode, WalkableTile
			/*
			if (o.name != "Nodes" && o.name != "Non-Nodes" && o.name != "Maze" && o.name != "Pellets" && o.name != "PathTiles" && o.name != "PacMan Spawn Node"
				 && o.tag != "Player" && o.tag != "Enemy" && o.tag != "Maze" && o.tag != "MainCamera" && o.tag != "GhostHomeNode" && o.tag != "UI")
			*/
			if(o.tag == "Pellet" || o.tag == "InvisibleNode" || o.tag == "WalkableTile")
			{
				if(o.GetComponent<Tile>() != null)
				{
					if(o.GetComponent<Tile>().isPellet || o.GetComponent<Tile>().isSuperPellet)
						totalPellets++;
				}
				board[(int)pos.x, (int)pos.y] = o;
			}
        }
		pacman = pacman = GameObject.FindGameObjectWithTag("Player");
		ghosts = GameObject.FindGameObjectsWithTag("Enemy");
		audioSource = transform.GetComponent<AudioSource>();
		healthBar.SetupLives(lives-1);
		Debug.Log("There's " +totalPellets +" pellets");
		StartGame();
	}

	void Update()
	{
		UpdateUI();
		CheckPelletsConsumed();
	}

	void CheckPelletsConsumed()
	{
		if(!winning && totalPellets == pelletsConsumed)
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

	void UpdateUI()
	{
		scoreText.text = score.ToString();
	}

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

	public void Restart()
	{
		readyText.GetComponent<SpriteRenderer>().enabled = false;
		dying = false;
		winning = false;
		pacman.transform.GetComponent<Player>().Restart();
		foreach (GameObject g in ghosts)
			g.GetComponent<Ghost>().Restart();
	}

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
		StartCoroutine(ShowObjectsAfter(2.25f));
	}

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

	// Hides all Ghosts and calls ProcessDeathAnimation
	IEnumerator ProcessDeathAfter(float delay)
	{
		yield return new WaitForSeconds(delay);
		foreach (GameObject g in ghosts)
			g.GetComponent<SpriteRenderer>().enabled = false;
		StartCoroutine(ProcessDeathAnimation(2.3f));	// 2.3 = Time that death animation takes
	}

	// Kills PacMan and calls ProcessRestart
	IEnumerator ProcessDeathAnimation(float delay)
	{
		pacman.transform.GetComponent<Animator>().enabled = true;
		pacman.transform.GetComponent<Player>().Die();
		yield return new WaitForSeconds(delay);
		StartCoroutine(ProcessRestart(1));				// Freeze for 2 seconds after death
	}

	// Shows ReadyText and calls RestartShowObjectsAfter
	IEnumerator ProcessRestart(float delay)
	{
		pacman.GetComponent<SpriteRenderer>().enabled = false;
		lives--;
		if(lives == 0)
		{
			audioSource.Stop();
			gameOverText.GetComponent<SpriteRenderer>().enabled = true;
			StartCoroutine(ProcessGameOver(2));
		}
		else
		{
			readyText.GetComponent<SpriteRenderer>().enabled = true;
			yield return new WaitForSeconds(delay);
			StartCoroutine(RestartShowObjectsAfter(1));
			healthBar.RemoveLives(1);
		}
	}

	// Hides ReadyText and resets ghosts and PacMan
	IEnumerator RestartShowObjectsAfter(float delay)
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

	// Shows  static sprites before start of game and calls StartGameAfter
	IEnumerator ShowObjectsAfter(float delay)
	{
		yield return new WaitForSeconds(delay);
		foreach (GameObject g in ghosts)
			g.GetComponent<SpriteRenderer>().enabled = true;
		pacman.transform.GetComponent<SpriteRenderer>().enabled = true;
		pacman.transform.GetComponent<Animator>().enabled = false;
		StartCoroutine(StartGameAfter(2));
	}

	// Allows everyone to move
	IEnumerator StartGameAfter(float delay)
	{
		yield return new WaitForSeconds(delay);
		foreach (GameObject g in ghosts)
			g.GetComponent<Ghost>().canMove = true;
		pacman.transform.GetComponent<Player>().canMove = true;
		pacman.transform.GetComponent<Animator>().enabled = true;
		readyText.transform.GetComponent<SpriteRenderer>().enabled = false;
	}

	// Freezes screen and hides ghost while showing score. Calls ProcessEatenAfter
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
			eatenGhostScoreText.GetComponent<Text>().enabled = true;
			audioSource.PlayOneShot(eatGhostSound);
			StartCoroutine(ProcessEatenAfter(0.75f, ghost));
		}
	}

	// Unfreeze screen after ghost has been eaten
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

	IEnumerator ProcessGameOver(float delay)
	{
		yield return new WaitForSeconds(delay);
		level = 1;
		lives = 3;
		score = 0;
		SceneManager.LoadScene("MainMenu");
	}

	IEnumerator ChangeLevel(float delay)
	{
		int iterations = 6;
		delay = delay / (iterations * 2);
		for (int i=0; i < iterations; i++)
		{
			pacman.transform.GetComponent<Player>().direction = Vector2.right;
			yield return new WaitForSeconds(delay);
			pacman.transform.GetComponent<Player>().direction = Vector2.left;
			yield return new WaitForSeconds(delay);
		}
		SceneManager.LoadScene("Level1");
	}
}
