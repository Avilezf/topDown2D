using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ref: https://drive.google.com/file/d/1WiF2LwM-6WvEnas9vw32YrYPly9K0Qrv/view

public class Player : MonoBehaviour
{
    List<Cell> path;
    [SerializeField]
    private float moveSpeed = 2f;
    public Vector2 GetPosition => transform.position;
    private bool startMoving = false;
    private Grid grid;
    private bool changedCells = false;
    private Rigidbody2D rb;
    public int HP;
    public int Cost;

    private float closestDistance;

    // Index of current waypoint from which Enemy walks
    // to the next one
    private int waypointIndex = 0;

    private List<GameObject> playersDestroy = new List<GameObject>();



    void FixedUpdate()
    {
        if (startMoving)
            Move();
    }


    public void starMoving(Grid grid, float speed)
    {

        this.grid = grid;
        calculatePath();
        startMoving = true;
        moveSpeed = speed;
    }

    public void starMoving(float speed)
    {

        calculatePath();
        startMoving = true;
        moveSpeed = speed;
    }

    private void calculatePath()
    {
        waypointIndex = 0;
        path = PathManager.Instance.FindPath(grid, (int)GetPosition.x, (int)GetPosition.y);
    }

    public void ResetPosition()
    {
        transform.position = new Vector2(0, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PowerSource")
        {
            Debug.Log("Made it");
            path = null;
        }

        if (collision.gameObject.tag == "EBullet")
        {

            HP -= 20;
            //Debug.Log("Hit by a bullet, new HP "+ HP);
            Destroy(collision.gameObject);
            if (HP < 0)
            {
                GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
                foreach (var tower in towers)
                {
                    tower.GetComponent<TowerShooting>().startShotting = false;
                }
                playersDestroy.Add(this.gameObject);
                if (playersDestroy.Count >= GameObject.FindGameObjectsWithTag("Player").GetLength(0))
                {
                    GameManager.Instance.UpdateGameState(GameManager.GameStateEnum.end);
                }
                else
                {
                    PathManager.Instance.powerUnitLocation = new Vector2Int((int)findClosestEnemy().transform.position.x, (int)findClosestEnemy().transform.position.y);
                    BoardManager.Instance.player.starMoving(3);
                }
                Destroy(this.gameObject);
            }
        }
    }

    private void Move()
    {
        // If player didn't reach last waypoint it can move
        // If player reached last waypoint then it stops
        if (path == null)
            return;

        if (waypointIndex <= path.Count - 1)
        {
            //Debug.Log("Moving to " + path[waypointIndex].transform.position.x.ToString() + " "
            //    + path[waypointIndex].transform.position.y.ToString());

            if (changedCells)
            {
                changedCells = false;
                if (!grid.isWalkable((int)path[waypointIndex].transform.position.x, (int)path[waypointIndex].transform.position.y))
                {
                    //Debug.Log("not walkable");
                    path = null;
                    calculatePath();
                    return;
                }
                else
                {
                    grid.setBusyCell((int)path[waypointIndex - 1].transform.position.x,
                        (int)path[waypointIndex - 1].transform.position.y,
                        (int)path[waypointIndex].transform.position.x,
                        (int)path[waypointIndex].transform.position.y);
                }

            }
            // Move player from current waypoint to the next one
            // using MoveTowards method
            transform.position = Vector2.MoveTowards(transform.position,
               path[waypointIndex].transform.position,
               moveSpeed * Time.deltaTime);

            // If player reaches position of waypoint he walked towards
            // then waypointIndex is increased by 1
            // and player starts to walk to the next waypoint
            if (transform.position == path[waypointIndex].transform.position)
            {
                waypointIndex += 1;
                changedCells = true;
            }
        }
    }

    private GameObject findClosestEnemy()
    {
        GameObject[] objs;

        if (gameObject.tag == "Tower")
        {
            objs = GameObject.FindGameObjectsWithTag("Player");
        }
        else
        {
            objs = GameObject.FindGameObjectsWithTag("Tower");
        }


        GameObject closestEnemy = null;

        bool first = true;

        foreach (var obj in objs)
        {
            float distance = Vector3.Distance(obj.transform.position, transform.position);
            if (first)
            {
                closestDistance = distance;
                closestEnemy = obj;
                first = false;
            }
            else if (distance < closestDistance)
            {
                closestEnemy = obj;
                closestDistance = distance;
            }

        }
        return closestEnemy;
    }

}
