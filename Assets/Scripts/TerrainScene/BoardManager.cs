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

    public Tower tower;
    [SerializeField]
    private float closestDistance;

    public int budgetPlayer;

    public int budgetTowers;

    public string towerConfiguration;

    public string playerConfiguration;

    private void Awake()
    {
        Instance = this;
    }

    public void SetupBoard()
    {
        if (this.towerConfiguration.Equals("0") || this.playerConfiguration.Equals("0"))
        {
            SetupNormal();
        }
        else
        {
            SetupPosition();
        }

    }
    private void SetupPosition()
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
        //save position de las towers
        List<Vector2> towers1Array = new List<Vector2>();
        List<Vector2> towers2Array = new List<Vector2>();
        int auxIntTower = 0;

        int towerLength = this.towerConfiguration.Length;
        for (int i = 0; i < (towerLength / 2); i++)
        {
            string towerSubstr = towerConfiguration.Substring(auxIntTower, 2);

            if (towerSubstr.Substring(auxIntTower, 1).Equals("1"))
            {
                Vector2 towerPosition = new Vector2(Random.Range(1, int.Parse(towerSubstr.Substring(auxIntTower + 1, 1))), Random.Range(10, 17));
                tower = Instantiate(TowerPrefab, towerPosition, Quaternion.identity);
                towers1Array.Add(towerPosition);
            }
            else
            {
                if (towerSubstr.Substring(auxIntTower, 1).Equals("2"))
                {
                    Vector2 towerPosition = new Vector2(Random.Range(1, int.Parse(towerSubstr.Substring(auxIntTower + 1, 1))), Random.Range(10, 17));
                    tower = Instantiate(Tower2Prefab, towerPosition, Quaternion.identity);
                    towers2Array.Add(towerPosition);
                }
            }
            auxIntTower = auxIntTower + 2;
        }

        try
        {
            PathManager.Instance.powerUnitLocation = new Vector2Int((int)findClosestEnemy().transform.position.x, (int)findClosestEnemy().transform.position.y);
        }
        catch (System.Exception)
        {
            Debug.Log("Se desoriento!");
        }


        //Needs 4 random to generate:
        // 1. X
        // 2. Y
        // 3. Much
        // 4. Speed
        //--------------------------------------------------------------
        budget = budgetPlayer;
        List<Vector2> players1Array = new List<Vector2>();
        List<Vector2> players2Array = new List<Vector2>();
        List<Vector2> players3Array = new List<Vector2>();
        Vector2 playerPosition;
        int auxIntPlayer = 0;

        int playerLength = this.playerConfiguration.Length;
        Debug.Log(playerLength / 2);

        for (int i = 0; i < (playerLength / 2); i++)
        {
            string playerSubstr = playerConfiguration.Substring(auxIntPlayer, 2);

            if (playerSubstr.Substring(auxIntPlayer, 1).Equals("1"))
            {
                playerPosition = new Vector2(Random.Range(0, int.Parse(playerSubstr.Substring(auxIntPlayer + 1, 1))), Random.Range(0, 5));
                player = Instantiate(PlayerPrefab, playerPosition, Quaternion.identity);
                players1Array.Add(playerPosition);
                player.starMoving(grid, Random.Range(2, 6));
            }
            else
            {
                if (playerSubstr.Substring(auxIntPlayer, 1).Equals("2"))
                {
                    playerPosition = new Vector2(Random.Range(0, int.Parse(playerSubstr.Substring(auxIntPlayer + 1, 1))), Random.Range(0, 5));
                    player = Instantiate(Player2Prefab, playerPosition, Quaternion.identity);
                    players2Array.Add(playerPosition);
                    player.starMoving(grid, Random.Range(2, 6));
                }
                else
                {
                    if (playerSubstr.Substring(auxIntPlayer, 1).Equals("3"))
                    {
                        playerPosition = new Vector2(Random.Range(0, int.Parse(playerSubstr.Substring(auxIntPlayer + 1, 1))), Random.Range(0, 5));
                        player = Instantiate(Player3Prefab, playerPosition, Quaternion.identity);
                        players3Array.Add(playerPosition);
                        player.starMoving(grid, Random.Range(2, 6));
                    }
                }
            }
            auxIntPlayer = auxIntPlayer + 2;
        }

        createTxtPositions(towers1Array, towers2Array, players1Array, players2Array, players3Array);
    }

    private void SetupNormal()
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
        //save position de las towers
        List<Vector2> towers1Array = new List<Vector2>();
        List<Vector2> towers2Array = new List<Vector2>();
        for (int i = 0; i < Random.Range(1, 5); i++)
        {
            selected = Random.Range(1, 3);
            if (selected == 1 && budget >= TowerPrefab.Cost)
            {
                Vector2 towerPosition = new Vector2(Random.Range(1, 7), Random.Range(10, 17));
                tower = Instantiate(TowerPrefab, towerPosition, Quaternion.identity);
                towers1Array.Add(towerPosition);
                budget = budget - TowerPrefab.Cost;
            }
            if (selected == 2 && budget >= Tower2Prefab.Cost)
            {
                Vector2 towerPosition = new Vector2(Random.Range(1, 7), Random.Range(10, 17));
                tower = Instantiate(Tower2Prefab, towerPosition, Quaternion.identity);
                towers2Array.Add(towerPosition);
                budget = budget - Tower2Prefab.Cost;
            }

        }

        try
        {
            PathManager.Instance.powerUnitLocation = new Vector2Int((int)findClosestEnemy().transform.position.x, (int)findClosestEnemy().transform.position.y);
        }
        catch (System.Exception)
        {
            Debug.Log("Se desoriento!");
        }


        //Needs 4 random to generate:
        // 1. X
        // 2. Y
        // 3. Much
        // 4. Speed
        //--------------------------------------------------------------
        budget = budgetPlayer;
        List<Vector2> players1Array = new List<Vector2>();
        List<Vector2> players2Array = new List<Vector2>();
        List<Vector2> players3Array = new List<Vector2>();
        Vector2 playerPosition;
        for (int i = 0; i < Random.Range(1, 5); i++)
        {
            selected = Random.Range(1, 3);
            if (selected == 1 && budget >= PlayerPrefab.Cost)
            {
                playerPosition = new Vector2(Random.Range(0, 5), Random.Range(0, 5));
                player = Instantiate(PlayerPrefab, playerPosition, Quaternion.identity);
                players1Array.Add(playerPosition);
                player.starMoving(grid, Random.Range(2, 6));
                budget = budget - 100;
            }
            if (selected == 2 && budget >= Player2Prefab.Cost)
            {
                playerPosition = new Vector2(Random.Range(0, 5), Random.Range(0, 5));
                player = Instantiate(Player2Prefab, playerPosition, Quaternion.identity);
                players2Array.Add(playerPosition);
                player.starMoving(grid, Random.Range(2, 6));
                budget = budget - 500;
            }
            if (selected == 3 && budget >= Player3Prefab.Cost)
            {
                playerPosition = new Vector2(Random.Range(0, 5), Random.Range(0, 5));
                player = Instantiate(Player3Prefab, playerPosition, Quaternion.identity);
                players3Array.Add(playerPosition);
                player.starMoving(grid, Random.Range(2, 6));
                budget = budget - 800;
            }
        }
        createTxtPositions(towers1Array, towers2Array, players1Array, players2Array, players3Array);
    }

    private GameObject findClosestEnemy()
    {
        GameObject[] objs;
        objs = GameObject.FindGameObjectsWithTag("Tower");



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

    private void createTxt(List<Vector2> towers1Array, List<Vector2> towers2Array, List<Vector2> players1Array, List<Vector2> players2Array, List<Vector2> players3Array)
    {
        string txt = "<row>";
        //Tower 1
        for (int i = 0; i < towers1Array.Count; i++)
        {
            if (i == 0)
            {
                txt = txt + "<Tower1>" + towers1Array[i].ToString();
                if (i == towers1Array.Count - 1)
                {
                    txt = txt + "</Tower1>";
                }
            }
            else
            {
                if (i == towers1Array.Count - 1)
                {
                    txt = txt + "," + towers1Array[i].ToString() + "</Tower1>";
                }
                else
                {
                    txt = txt + "," + towers1Array[i].ToString();
                }
            }
        }

        //Tower 2
        for (int i = 0; i < towers2Array.Count; i++)
        {
            if (i == 0)
            {
                txt = txt + "<Tower2>" + towers2Array[i].ToString();
                if (i == towers2Array.Count - 1)
                {
                    txt = txt + "</Tower2>";
                }
            }
            else
            {
                if (i == towers2Array.Count - 1)
                {
                    txt = txt + "," + towers2Array[i].ToString() + "</Tower2>";
                }
                else
                {
                    txt = txt + "," + towers2Array[i].ToString();
                }
            }
        }


        //Player 1
        for (int i = 0; i < players1Array.Count; i++)
        {
            if (i == 0)
            {
                txt = txt + "<Player1>" + players1Array[i].ToString();
                if (i == players1Array.Count - 1)
                {
                    txt = txt + "</Player1>";
                }
            }
            else
            {
                if (i == players1Array.Count - 1)
                {
                    txt = txt + "," + players1Array[i].ToString() + "</Player1>";
                }
                else
                {
                    txt = txt + "," + players1Array[i].ToString();
                }
            }
        }

        //Player 2
        for (int i = 0; i < players2Array.Count; i++)
        {
            if (i == 0)
            {
                txt = txt + "<Player2>" + players2Array[i].ToString();
                if (i == players2Array.Count - 1)
                {
                    txt = txt + "</Player2>";
                }
            }
            else
            {
                if (i == players2Array.Count - 1)
                {
                    txt = txt + "," + players2Array[i].ToString() + "</Player2>";
                }
                else
                {
                    txt = txt + "," + players2Array[i].ToString();
                }
            }
        }

        //Player 3
        for (int i = 0; i < players3Array.Count; i++)
        {
            if (i == 0)
            {
                txt = txt + "<Player3>" + players3Array[i].ToString();
                if (i == players3Array.Count - 1)
                {
                    txt = txt + "</Player3>";
                }
            }
            else
            {
                if (i == players3Array.Count - 1)
                {
                    txt = txt + "," + players3Array[i].ToString() + "</Player3>";
                }
                else
                {
                    txt = txt + "," + players3Array[i].ToString();
                }
            }
        }

        Debug.Log(txt);
        this.sendToGenerateFile(txt);
    }

    private void sendToGenerateFile(string board)
    {
        GenerateFile.Instance.generateFile(board);
    }

    private void createTxtPositions(List<Vector2> towers1Array, List<Vector2> towers2Array, List<Vector2> players1Array, List<Vector2> players2Array, List<Vector2> players3Array)
    {
        string towers = "";
        string players = "";
        //Tower 1
        for (int i = 0; i < towers1Array.Count; i++)
        {
            towers = towers + "1" + setPosition(towers1Array[i]);
        }

        //Tower 2
        for (int i = 0; i < towers2Array.Count; i++)
        {
            towers = towers + "2" + setPosition(towers2Array[i]);
        }


        //Player 1
        for (int i = 0; i < players1Array.Count; i++)
        {
            players = players + "1" + setPosition(players1Array[i]);
        }

        //Player 2
        for (int i = 0; i < players2Array.Count; i++)
        {
            players = players + "2" + setPosition(players2Array[i]);
        }

        //Player 3
        for (int i = 0; i < players3Array.Count; i++)
        {
            players = players + "3" + setPosition(players3Array[i]);
        }

        createTxt2(players, towers);
    }


    private void createTxt2(string players, string towers)
    {
        string txt = "<row>";
        //Towers
        txt = txt + "<towers>" + towers + "</towers>";

        //Players
        txt = txt + "<players>" + players + "</players>";

        Debug.Log(txt);
        this.sendToGenerateFile(txt);
    }



    private string setPosition(Vector2 position)
    {
        if (position.x < 3)
        {
            return "1";
        }

        if (position.x < 5)
        {
            return "2";
        }

        return "3";
    }
}
