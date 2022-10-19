using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;
    [SerializeField] private Cell CellPrefab;
    [SerializeField] private Player PlayerPrefab;
    [SerializeField] private Player Player2Prefab;
    [SerializeField] private Player Player3Prefab;
    [SerializeField] private PowerSource PowerSourcePrefab;
    [SerializeField] private Tower TowerPrefab;
    [SerializeField] private Tower Tower2Prefab;
    private Grid grid;
    public Player player;
    [SerializeField]
    private float closestDistance;

    public int budgetPlayer;

    public int budgetTowers;

    private void Awake()
    {
        Instance = this;
    }

    public void SetupBoard()
    {
        grid = new Grid(11, 20, 1, CellPrefab);
        Instantiate(PowerSourcePrefab, new Vector2(5, 19), Quaternion.identity);
        //Needs 3 random to generate:
        // 1. X
        // 2. Y
        // 3. Much
        //--------------------------------------------------------------
        int selected;
        int budget = budgetTowers;
        for (int i = 0; i < Random.Range(1,5); i++)
        {
            selected = Random.Range(1,3);
            if(selected == 1 && budget >= TowerPrefab.Cost)
            {
                Instantiate(TowerPrefab, new Vector2(Random.Range(1,7), Random.Range(10,17)), Quaternion.identity);    
            }
            if(selected == 2 && budget >= Tower2Prefab.Cost)
            {
                Instantiate(Tower2Prefab, new Vector2(Random.Range(1,7), Random.Range(10,17)), Quaternion.identity);    
            }
        }
        PathManager.Instance.powerUnitLocation = new Vector2Int((int)findClosestEnemy().transform.position.x, (int)findClosestEnemy().transform.position.y);

        //Needs 4 random to generate:
        // 1. X
        // 2. Y
        // 3. Much
        // 4. Speed
        //--------------------------------------------------------------
        budget = budgetPlayer;
        for (int i = 0; i < Random.Range(1,5); i++)
        {
            selected = Random.Range(1,3);
            if(selected == 1 && budget >= PlayerPrefab.Cost)
            {
                player = Instantiate(PlayerPrefab, new Vector2(Random.Range(0,5), Random.Range(0,5)), Quaternion.identity);
                player.starMoving(grid, Random.Range(2,6));
                budget = budget -100;
            }
            if(selected == 2 && budget >= Player2Prefab.Cost)
            {
                player = Instantiate(Player2Prefab, new Vector2(Random.Range(0,5), Random.Range(0,5)), Quaternion.identity);
                player.starMoving(grid, Random.Range(2,6));
                budget = budget -500;
            }
            if(selected == 3 && budget >= Player3Prefab.Cost)
            {
                player = Instantiate(Player3Prefab, new Vector2(Random.Range(0,5), Random.Range(0,5)), Quaternion.identity);
                player.starMoving(grid, Random.Range(2,6));
                budget = budget - 800;
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
