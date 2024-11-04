using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class MazeGenerator : MonoBehaviour
{
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    public Tilemap doorTilemap;
    public Tilemap roomFloorTilemap;
    public Tilemap floorLadderTilemap;
   // public Tilemap spawnEnemyTilemap;
    public Tile floorTile;
    public Tile wallTile;
    public Tile doorTile;
    public Tile roomFloorTile;
    public Tile floorLadderTile;
  //  public Tile spawnEnemyTile;
    public int gridSize = 5;
    public int roomSize = 10;
    public int roomMax = 6;
    public int roomMin = 3;
    public int roomCheckDistance = 2;
    public int ladderDistance = 3;
    public int maximumRooms = 9;
    public int ladderMaximumRandom;
    public int algorithmMethod = 0;
    public int randomWalkSteps = 300;
    public int randomWalkMazeSteps = 500;
    public int generateRooms = 0;


    //public int minShapeSize = 5;
    //public int maxShapeSize = 15;

    public PlayerMovement player;
    public GameObject enemyPrefab;
    public GameObject chestPrefab;
    //public GameObject[] spawnPointsEnemy;
    public List<Transform> spawnPointsSpawn;
    //public EnemyWalker EnemyWalkerPrefab;
    //public int minEnemies = 2;
    //public int maxEnemies = 10;

    private Vector2Int ladderRoomIndex;

    //private bool isGenerating = false;
    //private const float generationCooldown = 5f;
    //private Coroutine generationCoroutine;
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private List<GameObject> spawnedChests = new List<GameObject>();

    public event Action OnMazeGenerated;
    private Stopwatch stopwatch;

    private int[,] maze;

    private void Start()
    {
        ladderMaximumRandom = maximumRooms - 1;
        TriggerGeneration();
    }

    private void GenerateMaze()
    {

        maze = new int[gridSize * roomSize, gridSize * roomSize];
        // maze = new int[gridSize * gridSize, gridSize * gridSize];
        for (int i = 0; i < maze.GetLength(0); i++)
        {
            for (int j = 0; j < maze.GetLength(1); j++)
            {
                if (i == 0 || i == maze.GetLength(0) - 1 || j == 0 || j == maze.GetLength(1) - 1)
                {
                    maze[i, j] = 0; // border walls
                }
                else
                {
                    maze[i, j] = 0; // everything wall  
                }

            }

        }
        stopwatch = Stopwatch.StartNew();
        if (algorithmMethod == 0)
        {
            RecursiveBacktracking(1, 1);// This is the starting position of the generation
            if (generateRooms == 0)
            {
                GenerateRooms();
                SpawnEnemiesInRooms();
                SpawnChestsInRooms();
                CheckAndAdjustChestPositions();
            }
            UnityEngine.Debug.LogError("Algorithm Recursive Backtracking");
        }
        if (algorithmMethod == 1)
        {
            BinaryTreeAlgorithm();
            if (generateRooms == 0)
            {
                GenerateRooms();
                SpawnEnemiesInRooms();
                SpawnChestsInRooms();
                CheckAndAdjustChestPositions();
            }
            UnityEngine.Debug.LogError("Algorithm Binary Tree");
        }
        if (algorithmMethod == 2)
        {
            RandomWalk(gridSize * roomSize / 2, gridSize * roomSize / 2, randomWalkSteps); // This is the middle of the maze technically 
                                                                                           // RandomWalk(1,1, 1000);
            if (generateRooms == 0)
            {
                GenerateRooms();
                SpawnEnemiesInRooms();
                SpawnChestsInRooms();
                CheckAndAdjustChestPositions();
            }
            UnityEngine.Debug.LogError("Algorithm Random Walk");
        }
        if(algorithmMethod == 3)
        {
            RecursiveDivision(0, 0, maze.GetLength(0), maze.GetLength(1));
            if (generateRooms == 0)
            {
                GenerateRooms();
                SpawnEnemiesInRooms();
                SpawnChestsInRooms();
                CheckAndAdjustChestPositions();
            }
            UnityEngine.Debug.LogError("Algorithm Recursive Division");
        }
        else if (algorithmMethod >= 4)
        { UnityEngine.Debug.LogError("Algorithm out of range"); }
        stopwatch.Stop();
        UnityEngine.Debug.Log("Maze generation time: " + stopwatch.ElapsedMilliseconds + " milliseconds");
    }

   

    private void GenerateRooms()
    {
        System.Random rand = new System.Random();
        bool ladderPlaced = false;
        int numRooms = rand.Next(1, maximumRooms + 1);
        int numSpawnPoints = UnityEngine.Random.Range(0, 3);
        DeleteAllEnemies();
        DeleteAllChests();
        CheckAndAdjustChestPositions();

        for (int i = 1; i < gridSize * roomSize - 1; i += roomSize)
        {
            for (int j = 1; j < gridSize * roomSize - 1; j += roomSize)
            {
                if ((i != 1 || j != 1) && (i != 1 || j != gridSize * roomSize - roomSize - 1) &&
                    (i != gridSize * roomSize - roomSize - 1 || j != 1) &&
                    (i != gridSize * roomSize - roomSize - 1 || j != gridSize * roomSize - roomSize - 1))
                {
                    int roomWidth = rand.Next(roomMin, roomMax);
                    int roomHeight = rand.Next(roomMin, roomMax);

                    for (int x = i - 1; x <= i + roomWidth; x++)
                    {
                        for (int y = j - 1; y <= j + roomHeight; y++)
                        {
                            if (x >= 0 && x < maze.GetLength(0) && y >= 0 && y < maze.GetLength(1))
                            {
                                if ((maze[x, y] == 1 || maze[x, y] == 3) && !(x == i - 1 && y == j - 1) &&
                                    !(x == i - 1 && y == j + roomHeight) && !(x == i + roomWidth && y == j - 1) &&
                                    !(x == i + roomWidth && y == j + roomHeight))
                                {
                                    // Place doors only if the adjacent area is a walkable space and not on corners
                                    maze[x, y] = 2;
                                }
                            }
                        }
                    }
                    for (int x = i; x < i + roomWidth; x++)
                    {
                        for (int y = j; y < j + roomHeight; y++)
                        {
                            if (x < maze.GetLength(0) - 1 && y < maze.GetLength(1) - 1)
                            {
                                if (x == i || x == i + roomWidth - 1 || y == j || y == j + roomHeight - 1)
                                {
                                    maze[x, y] = 1; // this is the border next to the wall not the actual wall
                                }
                                else
                                {
                                    if (!ladderPlaced && rand.Next(ladderMaximumRandom) == 0) // 1 in 10 chance to place a ladder
                                    {
                                        maze[x, y] = 4; // = 4 means to place ladder
                                        ladderPlaced = true; 
                                        ladderRoomIndex = new Vector2Int(i / roomSize, j / roomSize); // Store the ladder room index
                                    }
                                    else
                                    {
                                        maze[x, y] = 3; // place room floors

                                        //if (rand.Next(10) == 0) // 1 in 10 chance to spawn a chest
                                        //{
                                        //    Vector3 spawnPosition = new Vector3(x : 2f + 1.5f, y  : 2f + 1.5f, 0); // Center of the tile
                                        //    GameObject newChest = Instantiate(chestPrefab, spawnPosition, Quaternion.identity);
                                        //    spawnedChests.Add(newChest);
                                        //}
                                    }
                                }
                            }
                        }
                    }
                   
                   
                }
            }
        }

    }


    private void SpawnEnemiesInRooms()
    {
        // Iterate through each room in the maze
        for (int x = 0; x < maze.GetLength(0); x++)
        {
            for (int y = 0; y < maze.GetLength(1); y++)
            {
                if (maze[x, y] == 3) // Check if the tile represents a room
                {
                    // Calculate the world position for spawning the enemy
                    float randomXOffset = UnityEngine.Random.Range(0, 2) == 0 ? -2f : 2f;
                    float randomYOffset = UnityEngine.Random.Range(0, 2) == 0 ? -2f : 2f;
                    Vector3 spawnPosition = new Vector3(x / randomXOffset, y / randomYOffset, 0);

                    // Spawn the enemy
                    GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

                    // Add reference to list
                    spawnedEnemies.Add(newEnemy);

                    break;
                }
            }
        }
    }

    private void SpawnChestsInRooms()
    {
        // Iterate through each room in the maze
        for (int x = 0; x < maze.GetLength(0); x++)
        {
            for (int y = 0; y < maze.GetLength(1); y++)
            {
                if (maze[x, y] == 3) // Check if the tile represents a room
                {
                    // Calculate the world position for spawning the chest
                    float randomXOffset = UnityEngine.Random.Range(0, 2) == 0 ? -2f : 2f;
                    float randomYOffset = UnityEngine.Random.Range(0, 2) == 0 ? -2f : 2f;
                    float spawnX = x / randomXOffset + 1.5f;
                    float spawnY = y / randomYOffset + 1.5f;
                    spawnX = Mathf.Floor(spawnX) + 0.75f; 
                    spawnY = Mathf.Floor(spawnY) + 0.75f; 
                    Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0);

                    GameObject newChest = Instantiate(chestPrefab, spawnPosition, Quaternion.identity);

                    spawnedChests.Add(newChest);
                    break;
                }
            }
        }
    }
    private void CheckAndAdjustChestPositions()
    {
        foreach (GameObject chest in spawnedChests)
        {
            // Get the position of the chest
            Vector3Int tilePosition = floorTilemap.WorldToCell(chest.transform.position);

            // Check if the tile position is a wall tile
            if (wallTilemap.HasTile(tilePosition))
            {
                // Move the chest up a unit
                Vector3 newPosition = chest.transform.position + new Vector3(0f, 1f, 0f);

                chest.transform.position = newPosition;
            }
        }
    }

    private void DeleteAllChests()
    {
        foreach (GameObject chest in spawnedChests)
        {
            Destroy(chest);
        }
        spawnedChests.Clear(); 
    }

    private void DeleteAllEnemies()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            Destroy(enemy);
        }
        spawnedEnemies.Clear();
    }

    private void RecursiveBacktracking(int v1, int v2)
    {
        maze[v1, v2] = 1;

        int[] cardinalDirections = { 1, 2, 3, 4, };
        ShuffleArray(cardinalDirections);

        foreach (var dir in cardinalDirections)
        {
            int dx = 0, dy = 0;
            switch (dir)
            {
                case 1: dy = 2; break; // Up
                case 2: dy = -2; break; // Down
                case 3: dx = 2; break; // Right
                case 4: dx = -2; break; // Left
            }

            //if(v1 + dx > 0 && v1 + dx < maze.GetLength(0) - 1 && v2 + dy > 0 && v2 + dy < maze.GetLength(1) - 1 && maze[v1 + dx, v2 + dy] == 0)
            // if (v1 + dx > 0 && v1 + dx < maze.GetLength(0) && v2 + dy > 0 && v2 + dy < maze.GetLength(1) && maze[v1 + dx, v2 + dy] == 1)
            if (v1 + dx > 0 && v1 + dx < maze.GetLength(0) && v2 + dy > 0 && v2 + dy < maze.GetLength(1) && maze[v1 + dx, v2 + dy] == 0)
            {
                maze[v1 + dx / 2, v2 + dy / 2] = 1;
                RecursiveBacktracking(v1 + dx, v2 + dy);
            }
        }
    }


    private void DrawMaze()
    {
        for (int i = 0; i < maze.GetLength(0); i++)
        {
            for (int j = 0; j < maze.GetLength(1); j++)
            {

                Vector3Int tilePos = new Vector3Int(i - gridSize * roomSize / 2, j - gridSize * roomSize / 2, 0);


                switch (maze[i, j])
                {
                    case 1:
                        wallTilemap.SetTile(tilePos, wallTile);
                        break;
                    case 2:
                        doorTilemap.SetTile(tilePos, doorTile);
                        break;
                    case 3:
                        roomFloorTilemap.SetTile(tilePos, roomFloorTile);
                        break;
                    case 4:
                        floorLadderTilemap.SetTile(tilePos, floorLadderTile);
                        break;
                    default:
                        floorTilemap.SetTile(tilePos, floorTile);
                        break;
                }
            }
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        System.Random rand = new System.Random();
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = rand.Next(i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }




    private void ShuffleArray(int[] cardinalDirections)
    {
        System.Random rand = new System.Random();
        for (int i = cardinalDirections.Length - 1; i > 0; i--)
        {
            int j = rand.Next(i + 1);
            int temp = cardinalDirections[i];
            cardinalDirections[i] = cardinalDirections[j];
            cardinalDirections[j] = temp;
        }
    }

    public void ResetMaze()
    {
        if (wallTilemap != null)
        {
            wallTilemap.ClearAllTiles();
        }
        if (floorTilemap != null)
        {
            floorTilemap.ClearAllTiles();
        }
        if (doorTilemap != null)
        {
            doorTilemap.ClearAllTiles();
        }
        if (roomFloorTilemap != null)
        {
            roomFloorTilemap.ClearAllTiles();
        }
        if (floorLadderTilemap != null)
        {
            floorLadderTilemap.ClearAllTiles();
        }
    }

    public void TriggerGeneration()
    {
        ResetMaze(); // This function deletes all wall and floor tiles therefore reseting the maze
        GenerateMaze(); // This generates the actual maze
        DrawMaze(); // this draws out the maze 
      //  DeleteAllEnemies();
        RespawnPlayer();
        if (player != null)
        {
            RespawnPlayer();
            UnityEngine.Debug.Log("Player respawned.");
        }
        OnMazeGenerated?.Invoke();

    }

    

    private void RespawnPlayer()
    {
        Vector2Int playerRoomIndex;
        do
        {
            int x = Random.Range(1, gridSize - 1);
            int y = Random.Range(1, gridSize - 1);
            playerRoomIndex = new Vector2Int(x, y);
        } while (playerRoomIndex == ladderRoomIndex);

        //Calculate respawn position
        float xPos = (playerRoomIndex.x * roomSize + roomSize / 2) - gridSize * roomSize / 2;
        float yPos = (playerRoomIndex.y * roomSize + roomSize / 2) - gridSize * roomSize / 2;
        Vector3 respawnPosition = new Vector3(xPos, yPos, 0);
        player.transform.position = respawnPosition;

        //Reset health
        player.currentHealth = player.maxHealth;

        //Re-enable camera
        Camera playerCamera = player.GetComponentInChildren<Camera>();
        if (playerCamera != null)
        {
            playerCamera.enabled = true;


        }
    }

    private void BinaryTreeAlgorithm()
    {
        System.Random rand = new System.Random();

        
        maze = new int[gridSize * roomSize, gridSize * roomSize];
        for (int i = 0; i < maze.GetLength(0); i++)
        {
            for (int j = 0; j < maze.GetLength(1); j++)
            {
                maze[i, j] = (i == 0 || i == maze.GetLength(0) - 1 || j == 0 || j == maze.GetLength(1) - 1) ? 0 : 1;
            }
        }

       
        for (int i = 1; i < maze.GetLength(0) - 1; i += 2)
        {
            for (int j = 1; j < maze.GetLength(1) - 1; j += 2)
            {
                maze[i, j] = 0;
                // maze[] = 0 sets the current position to a floor tilemap
                if (i == 1)
                {
                    maze[i, j + 1] = 0; //east
                }
                else if (j == maze.GetLength(1) - 2)
                {
                    maze[i - 1, j] = 0; // north
                }
                else if (rand.Next(2) == 0)
                {
                    maze[i - 1, j] = 0; // north
                }
                else
                {
                    maze[i, j + 1] = 0; // east
                }
            }
        }
    }

    private void RandomWalk(int startX, int startY, int steps)
    {
        System.Random rand = new System.Random();
        int x = startX;
        int y = startY;

        //Perform random steps
        for (int i = 0; i < steps; i++)
        {
            //Generate direction (up, down, left, or right)
            int direction = rand.Next(4);

            //Move in direction
            switch (direction)
            {
                case 0: // Up
                    if (y - 1 >= 0)
                        y--;
                    break;
                case 1: // Down
                    if (y + 1 < maze.GetLength(1))
                        y++;
                    break;
                case 2: // Left
                    if (x - 1 >= 0)
                        x--;
                    break;
                case 3: // Right
                    if (x + 1 < maze.GetLength(0))
                        x++;
                    break;
            }

            maze[x, y] = 1;
        }
    }
    private void RecursiveDivision(int startX, int startY, int width,int height)
    {
        //width or height of the current area is too small, stop dividing
        if (width < 4 || height < 4)
        {
            return;
        }

        //divide horizontally or vertically
        bool divideHorizontally = Random.Range(0, 2) == 0;

        //create a wall along a random line
        int wallX = startX + (divideHorizontally ? 0 : Random.Range(1, width / 2) * 2);
        int wallY = startY + (divideHorizontally ? Random.Range(1, height / 2) * 2 : 0);

        // Create wall
        for (int i = 0; i < (divideHorizontally ? width : height); i++)
        {
            if (divideHorizontally && (startX + i) != wallX)
            {
                maze[startX + i, wallY] = 1;
            }
            else if (!divideHorizontally && (startY + i) != wallY)
            {
                maze[wallX, startY + i] = 1;
            }
        }

        //Recursively divide the areas created by the wall
        if (divideHorizontally)
        {
            RecursiveDivision(startX, startY, width, wallY - startY);
            RecursiveDivision(startX, wallY + 1, width, startY + height - wallY - 1);
        }
        else
        {
            RecursiveDivision(startX, startY, wallX - startX, height);
            RecursiveDivision(wallX + 1, startY, startX + width - wallX - 1, height);
        }
    }
}

