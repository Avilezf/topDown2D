using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;

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
    private int count = GameManager.Instance.gameCount;

    private void Awake()
    {
        Instance = this;
    }

    public void SetupBoard()
    {
        if (this.towerConfiguration.Equals("0") && this.playerConfiguration.Equals("0"))
        {
            SetupNormal();
        }
        else
        {
            if (this.towerConfiguration.Equals("1") && this.playerConfiguration.Equals("1"))
            {
                setUpAjust();
            }
            else
            {
                SetupPositionNew();
            }

        }

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

    private void setUpAjust()
    {
        Debug.Log(this.count);
        this.count++;
        GameManager.Instance.gameCount = this.count;
        Debug.Log(this.count);
        switch (this.count)
        {
            case 1:
                this.simulation_1();

                break;

            case 2:
                this.simulation_2();
                break;

            case 3:
                this.simulation_3();
                break;

            case 4:
                this.simulation_4();
                break;

            case 5:
                this.simulation_5();
                break;

            case 6:
                this.simulation_6();
                count = 0;
                GameManager.Instance.gameCount = 0;
                break;
        }
    }

    private async void PredictionBoard()
    {
        var url = "http://localhost:8080/";
        var config = new PlayConfiguration(System.Int32.Parse(towerConfiguration), System.Int32.Parse(playerConfiguration));

        var json = JsonConvert.SerializeObject(config);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        using var client = new HttpClient();

        var response = await client.PostAsync(url, data);

        var result = await response.Content.ReadAsStringAsync();
        float x = System.Convert.ToSingle(result.ToString().Substring(1,result.Length -2));
        int y = (int)(x * 100);
        Debug.Log("Prediction of players winning: "+y+"%");

    }

    private void SetupPositionNew()
    {

        PredictionBoard();

        grid = new Grid(11, 20, 1, CellPrefab);
        Instantiate(PowerSourcePrefab, new Vector2(5, 19), Quaternion.identity);
        //Needs 3 random to generate:
        // 1. X
        // 2. Y
        // 3. Much
        //--------------------------------------------------------------
        //save position de las towers
        List<Vector2> towers1Array = new List<Vector2>();
        List<Vector2> towers2Array = new List<Vector2>();
        int auxIntTower = 0;

        int towerLength = this.towerConfiguration.Length;
        int Length_ = towerLength / 2;
        int i = 0;

        List<Vector2> towerPosition = new List<Vector2>();
        towerPosition.Add(new Vector2(1, 15));
        towerPosition.Add(new Vector2(7, 17));

        while (i < Length_)
        {
            string towerSubstr = towerConfiguration.Substring(auxIntTower, 2);
            if (towerSubstr.Substring(0, 1).Equals("1"))
            {
                tower = Instantiate(TowerPrefab, towerPosition[i], Quaternion.identity);
                towers1Array.Add(towerPosition[i]);
            }
            else
            {
                if (towerSubstr.Substring(0, 1).Equals("2"))
                {
                    tower = Instantiate(Tower2Prefab, towerPosition[i], Quaternion.identity);
                    towers2Array.Add(towerPosition[i]);
                }
            }
            auxIntTower = auxIntTower + 2;
            i++;
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
        List<Vector2> players1Array = new List<Vector2>();
        List<Vector2> players2Array = new List<Vector2>();
        List<Vector2> players3Array = new List<Vector2>();
        int auxIntPlayer = 0;

        int playerLength = this.playerConfiguration.Length;
        i = 0;

        List<Vector2> playerPosition = new List<Vector2>();
        playerPosition.Add(new Vector2(1, 1));
        playerPosition.Add(new Vector2(5, 1));

        while (i < playerLength / 2)
        {
            i++;

            string playerSubstr = playerConfiguration.Substring(auxIntPlayer, 2);
            auxIntPlayer = auxIntPlayer + 2;

            if (playerSubstr.Substring(0, 1).Equals("1"))
            {
                player = Instantiate(PlayerPrefab, playerPosition[i - 1], Quaternion.identity);
                players1Array.Add(playerPosition[i - 1]);
                player.starMoving(grid, PlayerPrefab.moveSpeed);
            }
            else
            {
                if (playerSubstr.Substring(0, 1).Equals("2"))
                {
                    player = Instantiate(Player2Prefab, playerPosition[i - 1], Quaternion.identity);
                    players2Array.Add(playerPosition[i - 1]);
                    player.starMoving(grid, Player2Prefab.moveSpeed);
                }
                else
                {
                    if (playerSubstr.Substring(0, 1).Equals("3"))
                    {
                        player = Instantiate(Player3Prefab, playerPosition[i - 1], Quaternion.identity);
                        players3Array.Add(playerPosition[i - 1]);
                        player.starMoving(grid, Player3Prefab.moveSpeed);
                    }
                }
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

        //Debug.Log(txt);
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

        //Debug.Log(txt);
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

    private void simulation_1()
    {
        //PLAYER 1 VS TOWER1
        grid = new Grid(11, 20, 1, CellPrefab);
        Instantiate(PowerSourcePrefab, new Vector2(5, 19), Quaternion.identity);

        List<Vector2> towers1Array = new List<Vector2>();
        List<Vector2> towers2Array = new List<Vector2>();
        Vector2 towerPosition;

        //TOWER 1
        towerPosition = new Vector2(1, 15);
        tower = Instantiate(TowerPrefab, towerPosition, Quaternion.identity);
        towers1Array.Add(towerPosition);

        towerPosition = new Vector2(7, 17);
        tower = Instantiate(TowerPrefab, towerPosition, Quaternion.identity);
        towers1Array.Add(towerPosition);


        try
        {
            PathManager.Instance.powerUnitLocation = new Vector2Int((int)findClosestEnemy().transform.position.x, (int)findClosestEnemy().transform.position.y);
        }
        catch (System.Exception)
        {
            Debug.Log("Se desoriento!");
        }

        List<Vector2> players1Array = new List<Vector2>();
        List<Vector2> players2Array = new List<Vector2>();
        List<Vector2> players3Array = new List<Vector2>();
        Vector2 playerPosition;

        //PLAYER 1
        playerPosition = new Vector2(1, 1);
        player = Instantiate(PlayerPrefab, playerPosition, Quaternion.identity);
        players1Array.Add(playerPosition);
        player.starMoving(grid, PlayerPrefab.moveSpeed);

        playerPosition = new Vector2(5, 1);
        player = Instantiate(PlayerPrefab, playerPosition, Quaternion.identity);
        players1Array.Add(playerPosition);
        player.starMoving(grid, PlayerPrefab.moveSpeed);

        createTxtPositions(towers1Array, towers2Array, players1Array, players2Array, players3Array);
    }

    private void simulation_2()
    {
        //PLAYER 1 VS TOWER 2
        grid = new Grid(11, 20, 1, CellPrefab);
        Instantiate(PowerSourcePrefab, new Vector2(5, 19), Quaternion.identity);


        List<Vector2> towers1Array = new List<Vector2>();
        List<Vector2> towers2Array = new List<Vector2>();
        Vector2 towerPosition;

        //TOWER 2 -------------------------------
        towerPosition = new Vector2(1, 15);
        tower = Instantiate(Tower2Prefab, towerPosition, Quaternion.identity);
        towers2Array.Add(towerPosition);

        towerPosition = new Vector2(7, 17);
        tower = Instantiate(Tower2Prefab, towerPosition, Quaternion.identity);
        towers2Array.Add(towerPosition);


        try
        {
            PathManager.Instance.powerUnitLocation = new Vector2Int((int)findClosestEnemy().transform.position.x, (int)findClosestEnemy().transform.position.y);
        }
        catch (System.Exception)
        {
            Debug.Log("Se desoriento!");
        }

        List<Vector2> players1Array = new List<Vector2>();
        List<Vector2> players2Array = new List<Vector2>();
        List<Vector2> players3Array = new List<Vector2>();
        Vector2 playerPosition;

        //PLAYER 1
        playerPosition = new Vector2(1, 1);
        player = Instantiate(PlayerPrefab, playerPosition, Quaternion.identity);
        players1Array.Add(playerPosition);
        player.starMoving(grid, PlayerPrefab.moveSpeed);

        playerPosition = new Vector2(5, 1);
        player = Instantiate(PlayerPrefab, playerPosition, Quaternion.identity);
        players1Array.Add(playerPosition);
        player.starMoving(grid, PlayerPrefab.moveSpeed);

        createTxtPositions(towers1Array, towers2Array, players1Array, players2Array, players3Array);
    }

    private void simulation_3()
    {
        //PLAYER 2 VS TOWER 1
        grid = new Grid(11, 20, 1, CellPrefab);
        Instantiate(PowerSourcePrefab, new Vector2(5, 19), Quaternion.identity);

        List<Vector2> towers1Array = new List<Vector2>();
        List<Vector2> towers2Array = new List<Vector2>();
        Vector2 towerPosition;

        //TOWER 1
        towerPosition = new Vector2(1, 15);
        tower = Instantiate(TowerPrefab, towerPosition, Quaternion.identity);
        towers1Array.Add(towerPosition);

        towerPosition = new Vector2(7, 17);
        tower = Instantiate(TowerPrefab, towerPosition, Quaternion.identity);
        towers1Array.Add(towerPosition);


        try
        {
            PathManager.Instance.powerUnitLocation = new Vector2Int((int)findClosestEnemy().transform.position.x, (int)findClosestEnemy().transform.position.y);
        }
        catch (System.Exception)
        {
            Debug.Log("Se desoriento!");
        }

        List<Vector2> players1Array = new List<Vector2>();
        List<Vector2> players2Array = new List<Vector2>();
        List<Vector2> players3Array = new List<Vector2>();
        Vector2 playerPosition;

        //PLAYER 2
        playerPosition = new Vector2(1, 1);
        player = Instantiate(Player2Prefab, playerPosition, Quaternion.identity);
        players2Array.Add(playerPosition);
        player.starMoving(grid, Player2Prefab.moveSpeed);

        playerPosition = new Vector2(5, 1);
        player = Instantiate(Player2Prefab, playerPosition, Quaternion.identity);
        players2Array.Add(playerPosition);
        player.starMoving(grid, Player2Prefab.moveSpeed);

        createTxtPositions(towers1Array, towers2Array, players1Array, players2Array, players3Array);
    }

    private void simulation_4()
    {
        //PLAYER 2 VS TOWER 2
        grid = new Grid(11, 20, 1, CellPrefab);
        Instantiate(PowerSourcePrefab, new Vector2(5, 19), Quaternion.identity);

        List<Vector2> towers1Array = new List<Vector2>();
        List<Vector2> towers2Array = new List<Vector2>();
        Vector2 towerPosition;

        //TOWER 2
        towerPosition = new Vector2(1, 15);
        tower = Instantiate(Tower2Prefab, towerPosition, Quaternion.identity);
        towers2Array.Add(towerPosition);

        towerPosition = new Vector2(7, 17);
        tower = Instantiate(Tower2Prefab, towerPosition, Quaternion.identity);
        towers2Array.Add(towerPosition);


        try
        {
            PathManager.Instance.powerUnitLocation = new Vector2Int((int)findClosestEnemy().transform.position.x, (int)findClosestEnemy().transform.position.y);
        }
        catch (System.Exception)
        {
            Debug.Log("Se desoriento!");
        }

        List<Vector2> players1Array = new List<Vector2>();
        List<Vector2> players2Array = new List<Vector2>();
        List<Vector2> players3Array = new List<Vector2>();
        Vector2 playerPosition;

        //PLAYER 2
        playerPosition = new Vector2(1, 1);
        player = Instantiate(Player2Prefab, playerPosition, Quaternion.identity);
        players2Array.Add(playerPosition);
        player.starMoving(grid, Player2Prefab.moveSpeed);

        playerPosition = new Vector2(5, 1);
        player = Instantiate(Player2Prefab, playerPosition, Quaternion.identity);
        players2Array.Add(playerPosition);
        player.starMoving(grid, Player2Prefab.moveSpeed);

        createTxtPositions(towers1Array, towers2Array, players1Array, players2Array, players3Array);
    }

    private void simulation_5()
    {
        //PLAYER 3 VS TOWER 1
        grid = new Grid(11, 20, 1, CellPrefab);
        Instantiate(PowerSourcePrefab, new Vector2(5, 19), Quaternion.identity);

        List<Vector2> towers1Array = new List<Vector2>();
        List<Vector2> towers2Array = new List<Vector2>();
        Vector2 towerPosition;

        //TOWER 1
        towerPosition = new Vector2(1, 15);
        tower = Instantiate(TowerPrefab, towerPosition, Quaternion.identity);
        towers1Array.Add(towerPosition);

        towerPosition = new Vector2(7, 17);
        tower = Instantiate(TowerPrefab, towerPosition, Quaternion.identity);
        towers1Array.Add(towerPosition);


        try
        {
            PathManager.Instance.powerUnitLocation = new Vector2Int((int)findClosestEnemy().transform.position.x, (int)findClosestEnemy().transform.position.y);
        }
        catch (System.Exception)
        {
            Debug.Log("Se desoriento!");
        }

        List<Vector2> players1Array = new List<Vector2>();
        List<Vector2> players2Array = new List<Vector2>();
        List<Vector2> players3Array = new List<Vector2>();
        Vector2 playerPosition;

        //PLAYER 3
        playerPosition = new Vector2(1, 1);
        player = Instantiate(Player3Prefab, playerPosition, Quaternion.identity);
        players3Array.Add(playerPosition);
        player.starMoving(grid, Player3Prefab.moveSpeed);

        playerPosition = new Vector2(5, 1);
        player = Instantiate(Player3Prefab, playerPosition, Quaternion.identity);
        players3Array.Add(playerPosition);
        player.starMoving(grid, Player3Prefab.moveSpeed);

        createTxtPositions(towers1Array, towers2Array, players1Array, players2Array, players3Array);
    }

    private void simulation_6()
    {
        //PLAYER 3 VS TOWER 2
        grid = new Grid(11, 20, 1, CellPrefab);
        Instantiate(PowerSourcePrefab, new Vector2(5, 19), Quaternion.identity);

        List<Vector2> towers1Array = new List<Vector2>();
        List<Vector2> towers2Array = new List<Vector2>();
        Vector2 towerPosition;

        //TOWER 2
        towerPosition = new Vector2(1, 15);
        tower = Instantiate(Tower2Prefab, towerPosition, Quaternion.identity);
        towers2Array.Add(towerPosition);

        towerPosition = new Vector2(7, 17);
        tower = Instantiate(Tower2Prefab, towerPosition, Quaternion.identity);
        towers2Array.Add(towerPosition);


        try
        {
            PathManager.Instance.powerUnitLocation = new Vector2Int((int)findClosestEnemy().transform.position.x, (int)findClosestEnemy().transform.position.y);
        }
        catch (System.Exception)
        {
            Debug.Log("Se desoriento!");
        }


        List<Vector2> players1Array = new List<Vector2>();
        List<Vector2> players2Array = new List<Vector2>();
        List<Vector2> players3Array = new List<Vector2>();
        Vector2 playerPosition;

        //PLAYER 3
        playerPosition = new Vector2(1, 1);
        player = Instantiate(Player3Prefab, playerPosition, Quaternion.identity);
        players3Array.Add(playerPosition);
        player.starMoving(grid, Player3Prefab.moveSpeed);

        playerPosition = new Vector2(5, 1);
        player = Instantiate(Player3Prefab, playerPosition, Quaternion.identity);
        players3Array.Add(playerPosition);
        player.starMoving(grid, Player3Prefab.moveSpeed);

        createTxtPositions(towers1Array, towers2Array, players1Array, players2Array, players3Array);
    }

    private void SetupPosition()
    {
        //PLAYER 3 VS TOWER 2
        grid = new Grid(11, 20, 1, CellPrefab);
        Instantiate(PowerSourcePrefab, new Vector2(5, 19), Quaternion.identity);

        List<Vector2> towers1Array = new List<Vector2>();
        List<Vector2> towers2Array = new List<Vector2>();
        Vector2 towerPosition;

        //TOWER 2
        towerPosition = new Vector2(1, 15);
        tower = Instantiate(Tower2Prefab, towerPosition, Quaternion.identity);
        towers2Array.Add(towerPosition);

        towerPosition = new Vector2(7, 17);
        tower = Instantiate(Tower2Prefab, towerPosition, Quaternion.identity);
        towers2Array.Add(towerPosition);


        try
        {
            PathManager.Instance.powerUnitLocation = new Vector2Int((int)findClosestEnemy().transform.position.x, (int)findClosestEnemy().transform.position.y);
        }
        catch (System.Exception)
        {
            Debug.Log("Se desoriento!");
        }


        List<Vector2> players1Array = new List<Vector2>();
        List<Vector2> players2Array = new List<Vector2>();
        List<Vector2> players3Array = new List<Vector2>();
        Vector2 playerPosition;

        //PLAYER 3
        playerPosition = new Vector2(1, 1);
        player = Instantiate(Player3Prefab, playerPosition, Quaternion.identity);
        players3Array.Add(playerPosition);
        player.starMoving(grid, Player3Prefab.moveSpeed);

        playerPosition = new Vector2(5, 1);
        player = Instantiate(Player3Prefab, playerPosition, Quaternion.identity);
        players3Array.Add(playerPosition);
        player.starMoving(grid, Player3Prefab.moveSpeed);

        createTxtPositions(towers1Array, towers2Array, players1Array, players2Array, players3Array);
    }


}
