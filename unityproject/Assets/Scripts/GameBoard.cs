using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour
{
	public int boardWidth = 30;
	public int boardHeight = 33;
	private int totalPellets = 0;
	public int lives = 3;
	public int score = 0;
	public GameObject[,] board;
	private AudioSource audioSource;
	public AudioClip backgroundAudioNormal;
	public AudioClip backgroundAudioScared;
	public AudioClip startupSound;
	private GameObject pacman;
	private GameObject[] ghosts;

	private bool dying = false;
	public GameObject readyText;

	void Start()
    {
		totalPellets = 0;
		board = new GameObject[boardWidth, boardHeight];
		// Load all interactive tiles into the array and place them on the board
		Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));
        foreach (GameObject o in objects)
        {
            Vector2 pos = o.transform.position;
			if (o.name != "Nodes" && o.name != "Non-Nodes" && o.name != "Maze" && o.name != "Pellets" && o.name != "PathTiles" && o.name != "PacMan Spawn Node"
				 && o.tag != "Player" && o.tag != "Enemy" && o.tag != "Maze" && o.tag != "MainCamera" && o.tag != "GhostHomeNode" && o.tag != "UI")
			{
				if(o.GetComponent<Tile>() != null)
				{
					if(o.GetComponent<Tile>().isPellet || o.GetComponent<Tile>().isSuperPellet)
						totalPellets++;
				}
				if ((int)pos.x == 14 && (int)pos.y == 7)
					Debug.Log("Meto " +o);
				board[(int)pos.x, (int)pos.y] = o;
			}
			if(o.name != "Pellets")
			{
				totalPellets++;
			}
        }
		pacman = pacman = GameObject.FindGameObjectWithTag("Player");
		ghosts = GameObject.FindGameObjectsWithTag("Enemy");
		audioSource = transform.GetComponent<AudioSource>();
		StartGame();
	}


	public void Restart()
	{
		readyText.GetComponent<SpriteRenderer>().enabled = false;
		lives--;
		dying = false;
		pacman.transform.GetComponent<Player>().Restart();
		foreach (GameObject g in ghosts)
			g.GetComponent<Ghost>().Restart();
	}

	public void StartGame()
	{
		audioSource.PlayOneShot(startupSound);
		// Hide ghosts and PAcman, make them unable to move
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

	IEnumerator ProcessDeathAfter(float delay)
	{
		yield return new WaitForSeconds(delay);
		foreach (GameObject g in ghosts)
			g.GetComponent<SpriteRenderer>().enabled = false;
		StartCoroutine(ProcessDeathAnimation(2.3f));	// 2.3 = Time that death animation takes
	}

	IEnumerator ProcessDeathAnimation(float delay)
	{
		pacman.transform.GetComponent<Animator>().enabled = true;
		pacman.transform.GetComponent<Player>().Die();
		yield return new WaitForSeconds(delay);
		StartCoroutine(ProcessRestart(1));				// Freeze for 2 seconds after death
	}
	
	IEnumerator ProcessRestart(float delay)
	{
		readyText.GetComponent<SpriteRenderer>().enabled = true;
		pacman.GetComponent<SpriteRenderer>().enabled = false;
		yield return new WaitForSeconds(delay);
		StartCoroutine(RestartShowObjectsAfter(1));
	}

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

	IEnumerator ShowObjectsAfter(float delay)
	{
		yield return new WaitForSeconds(delay);
		foreach (GameObject g in ghosts)
			g.GetComponent<SpriteRenderer>().enabled = true;
		pacman.transform.GetComponent<SpriteRenderer>().enabled = true;
		pacman.transform.GetComponent<Animator>().enabled = false;
		StartCoroutine(StartGameAfter(2));
	}

	IEnumerator StartGameAfter(float delay)
	{
		yield return new WaitForSeconds(delay);
		foreach (GameObject g in ghosts)
			g.GetComponent<Ghost>().canMove = true;
		pacman.transform.GetComponent<Player>().canMove = true;
		pacman.transform.GetComponent<Animator>().enabled = true;
		readyText.transform.GetComponent<SpriteRenderer>().enabled = false;
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
}
