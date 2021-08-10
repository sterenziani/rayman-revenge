using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float movspeed = 10f;
    private enum Direction {UP, DOWN, LEFT, RIGHT, NONE};
    private Direction direction = Direction.NONE;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("GO!");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            direction = Direction.LEFT;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            direction = Direction.RIGHT;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            direction = Direction.UP;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            direction = Direction.DOWN;

        switch (direction)
        {
            case Direction.DOWN:
                transform.position = new Vector2(transform.position.x, transform.position.y - movspeed * Time.deltaTime);
                break;
            case Direction.UP:
                transform.position = new Vector2(transform.position.x, transform.position.y + movspeed * Time.deltaTime);
                break;
            case Direction.LEFT:
                transform.position = new Vector2(transform.position.x - movspeed * Time.deltaTime, transform.position.y);
                break;
            case Direction.RIGHT:
                transform.position = new Vector2(transform.position.x + movspeed * Time.deltaTime, transform.position.y);
                break;
            default:
                break;
        }
    }
}
