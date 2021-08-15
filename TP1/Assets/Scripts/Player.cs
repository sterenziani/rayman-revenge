using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float movspeed = 1f;
    private Vector2 _vel;
    private Animator _anmCtrl;
    private SpriteRenderer _sprRnd;
    private enum Direction {UP, DOWN, LEFT, RIGHT, NONE};

    private Direction direction = Direction.NONE;
    private Direction bufferedDirection = Direction.NONE;
    private Node currentNode;
    private KeyCode PACMAN_LEFT_KEY = KeyCode.LeftArrow;
    private KeyCode PACMAN_RIGHT_KEY = KeyCode.RightArrow;
    private KeyCode PACMAN_UP_KEY = KeyCode.UpArrow;
    private KeyCode PACMAN_DOWN_KEY = KeyCode.DownArrow;

    // Start is called before the first frame update
    void Start()
    {
        _anmCtrl = GetComponent<Animator>();
        _sprRnd = GetComponent<SpriteRenderer>();
        Node node = getNodeAtPosition(transform.localPosition);
        if(node != null)
            currentNode = node;
        Debug.Log("GO!");
    }

    // Update is called once per frame
    void Update()
    {
        updateInput();
        updateMovement();
    }

    void updateInput()
    {
        if (Input.GetKeyDown(PACMAN_LEFT_KEY)){
            direction = Direction.LEFT;
            moveToNode(Vector2.left);
        }
        else if (Input.GetKeyDown(PACMAN_RIGHT_KEY)){
            direction = Direction.RIGHT;
            moveToNode(Vector2.right);
        }
        else if (Input.GetKeyDown(PACMAN_UP_KEY)){
            direction = Direction.UP;
            moveToNode(Vector2.up);
        }
        else if (Input.GetKeyDown(PACMAN_DOWN_KEY)){
            direction = Direction.DOWN;
            moveToNode(Vector2.down);
        }
    }

    void updateMovement()
    {
        switch(direction)
        {
            case Direction.DOWN:
                _anmCtrl.SetBool("move", true);
                _sprRnd.flipX = true;
                //transform.localRotation = Quaternion.Euler(0,0,0);
                transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
                //transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y - movspeed * Time.deltaTime);
                break;
            case Direction.UP:
                _anmCtrl.SetBool("move", true);
                _sprRnd.flipX = false;
                //transform.localRotation = Quaternion.Euler(0,0,90);
                transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
                //transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y + movspeed * Time.deltaTime);
                break;
            case Direction.LEFT:
                _anmCtrl.SetBool("move", true);
                _sprRnd.flipX = true;
                //transform.localRotation = Quaternion.Euler(0,0,0);
                transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
                //transform.localPosition = new Vector2(transform.localPosition.x - movspeed * Time.deltaTime, transform.localPosition.y);
                break;
            case Direction.RIGHT:
                _anmCtrl.SetBool("move", true);
                _sprRnd.flipX = false;
                //transform.localRotation = Quaternion.Euler(0,0,-90);
                transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
                //transform.localPosition = new Vector2(transform.localPosition.x + movspeed * Time.deltaTime, transform.localPosition.y);
                break;
            default:
                break;
        }
    }

    Node getNodeAtPosition(Vector2 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[(int)pos.x, (int)pos.y];
        if(tile != null)
            return tile.GetComponent<Node>();
        return null;
    }

    Node canMove(Vector2 d)
    {
        Node moveToNode = null;
        for(int i=0; i < currentNode.neighbors.Length; i++)
        {
            if(currentNode.validDirections[i] == d){
                moveToNode = currentNode.neighbors[i];
                break;
            }
        }
        return moveToNode;
    }

    void moveToNode(Vector2 d)
    {
        Node moveToNode = canMove(d);
        if(moveToNode != null){
            transform.localPosition = moveToNode.transform.position;
            currentNode = moveToNode;
        }
    }
}