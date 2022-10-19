using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Tower : MonoBehaviour
{
    public static Tower Instance;
    public int HP;
    public int Cost;

    private float closestDistance;

    private int count = 1;

    private List<GameObject> towersDestroy = new List<GameObject>();

    private void Awake()
    {
        HP = 100;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Hit");
        if (collision.gameObject.tag == "Bullet")
        {

            HP -= 20;
            //Debug.Log("Hit by a bullet, new HP "+ HP);
            Destroy(collision.gameObject);
            if (HP < 0)
            {
                
                this.towersDestroy.Add(this.gameObject);
                PathManager.Instance.powerUnitLocation = new Vector2Int((int)findClosestEnemy().transform.position.x, (int)findClosestEnemy().transform.position.y);
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                foreach (var player in players)
                {
                    player.GetComponent<Player>().starMoving(2);
                    player.GetComponent<PlayerShooting>().startShotting = false;
                }
                count++;
                Destroy(this.gameObject);
            }
        }
    }

    private GameObject findClosestEnemy()
    {
        GameObject[] objs;

        objs = GameObject.FindGameObjectsWithTag("Tower");
        if(objs == null || objs.Length - count <= 0)
        {
            objs = GameObject.FindGameObjectsWithTag("PowerSource");
        }
        
        GameObject closestEnemy = null;

        bool first = true;

        List<GameObject> towers = new List<GameObject>();
        foreach (var obj in objs)
        {
            towers.Add(obj);
        }

        if (towersDestroy.Count > 0)
        {
            foreach (var obj in objs)
            {
                foreach (var towerDestroy in towersDestroy)
                {
                    if (obj.Equals(towerDestroy))
                    {
                        towers.Remove(obj);
                    }
                }
            }
        }



        foreach (var obj in towers)
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
                towersDestroy.Add(obj);
            }

        }
        return closestEnemy;
    }

}
