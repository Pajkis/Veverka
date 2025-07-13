using System.Collections.Generic;
using UnityEngine;
using Veverka.Characters.Veverka;

namespace Veverka.GridSystem.GameGrid
{ 
    /// <summary>
    /// Defining Game grid and basic grid functions
    /// </summary>
    public class GameGrid : MonoBehaviour
    {
        #region global vars

        // global grid - Singleton 
        public static GameGrid Instance { get; private set; }

        #endregion

        #region events
        [SerializeField]
        private LevelInitEvent levelInitEvent;

        [SerializeField]
        private LevelSelectEvent levelSelectEvent;

        [SerializeField]
        private ResetGridEvent resetGridEvent;

        [SerializeField]
        private LevelDatabase levelDatabase;

        #endregion

        #region tile objects
        // Prefabs
        [SerializeField] private GameObject veverkaPrefab;
        [SerializeField] private GameObject liskacekPrefab;
        [SerializeField] private GameObject emptyPrefab;
        [SerializeField] private GameObject wallPrefab;
        [SerializeField] private GameObject goalPrefab;
        [SerializeField] private GameObject backgroundPrefab;
        //Adjust screen for each level size
        [SerializeField] private Transform gridRoot;
        #endregion


        #region fields
        // grid variables
        private int gridHeight = 10;
        private int gridWidth = 10;
        private float tileSize = 1f;
        private TileType[,] grid;
        private Dictionary<Vector2Int, TileObject> pushables = new();
        private Dictionary<Vector2Int, TileObject> goals = new();
        #endregion

        #region Properties

        // tile size 
        public float TileSize
        {
            get { return tileSize; }
            // short version of exploiting get only property
            //***
            // public float TileSize => tileSize;
            //***
        }

        /// <summary>
        /// get movables position and objects
        /// </summary>
        public Dictionary<Vector2Int, TileObject> Pushables
        {
            get { return pushables; }
        }

        /// <summary>
        /// get goals position and objects
        /// </summary>
        public Dictionary<Vector2Int, TileObject> Goals
        {
            get { return goals; }
        }
    #endregion

    #region Awake
    /// <summary>
    /// Generates grid
    /// </summary>
    void Awake()
        {
            // singleton routine
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            // add listeners for events
            levelSelectEvent.AddListener(OnLevelSelected);
            resetGridEvent.AddListener(ResetGrid);
        }

        #endregion

        #region Grid Init
        /// <summary>
        /// Initialize a new grid
        /// </summary>
        /// <param name="grid"></param>
        public bool InitializeGrid(TileType[,] grid)
        {
            //get new grid size
            this.grid = grid;
            this.gridWidth = grid.GetLength(0);
            this.gridHeight = grid.GetLength(1);

            // Spawn objects based on tile types
            BuildLevelFromGrid();

            //raise event
            levelInitEvent.Raise(new LevelInitPayload
            {
                GoalCount = goals.Count
            });
            Debug.Log($"Goal Count in GameGrid: {goals.Count}");

            OffsetGridToBottomLeft();

            return true;
        }
    
        /// <summary>
        /// Build a level from a grid
        /// </summary>
        void BuildLevelFromGrid()
        {      
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Vector2Int tilePos = new(x, y);
                    Vector3 worldTilePos = GridUtils.GridToWorld(tilePos);
                    TileType tileType =  GetTileType(tilePos);

                    // create background for each tile
                    var background =  Instantiate(backgroundPrefab, Vector3.zero, Quaternion.identity, gridRoot);
                    background.transform.SetParent(gridRoot, false);
                    background.transform.localPosition = worldTilePos;

                    // insert specific tiles
                    switch (tileType)
                    { 
                    case TileType.Empty:

                            //var tile = Instantiate(emptyPrefab, Vector3.zero, Quaternion.identity, gridRoot);
                            //tile.transform.SetParent(gridRoot, false);
                            //tile.transform.localPosition = worldTilePos;
                            SetTileType(tilePos, TileType.Empty);
                            break;

                    case TileType.Wall:
                            var tile = Instantiate(wallPrefab, Vector3.zero, Quaternion.identity, gridRoot);
                            tile.transform.SetParent(gridRoot, false);
                            tile.transform.localPosition = worldTilePos;
                            SetTileType(tilePos, TileType.Wall);
                            break;

                    case TileType.Veverka:
                            // Generate empty tile under veverka
                            //tile = Instantiate(emptyPrefab, Vector3.zero, Quaternion.identity, gridRoot);
                            //tile.transform.SetParent(gridRoot, false);
                            //tile.transform.localPosition = worldTilePos;
                            SetTileType(tilePos, TileType.Empty); //veverka will have no tile for now

                            // generete veverka
                            CharVeverka veverka = Instantiate(veverkaPrefab, Vector3.zero, Quaternion.identity, gridRoot).GetComponent<CharVeverka>();
                            veverka.transform.SetParent(gridRoot, false);
                            veverka.transform.localPosition = worldTilePos;
                            veverka.Init(tileType, CharacterType.BasicVeverka, tilePos);
                            
                            break;

                    case TileType.Nut:
                            //generate free tile under nut
                            //tile = Instantiate(emptyPrefab, Vector3.zero, Quaternion.identity, gridRoot);
                            //tile.transform.SetParent(gridRoot, false);
                            //tile.transform.localPosition= worldTilePos;

                            // generate nut
                            NutTile nutTile = Instantiate(liskacekPrefab, Vector3.zero, Quaternion.identity, gridRoot).GetComponent<NutTile>();
                            nutTile.transform.SetParent(gridRoot, false);
                            nutTile.transform.localPosition = worldTilePos;
                            nutTile.Init(tileType, tilePos);
                            pushables.Add(tilePos, nutTile);
                            break;

                    case TileType.Goal:
                            // generate empty prefab under goal
                            //tile = Instantiate(emptyPrefab, Vector3.zero, Quaternion.identity, gridRoot);
                            //tile.transform.SetParent(gridRoot, false);
                            //tile.transform.localPosition = worldTilePos;
                        
                            // generate goal 
                            GoalTile goalTile = Instantiate(goalPrefab, Vector3.zero, Quaternion.identity, gridRoot).GetComponent<GoalTile>();
                            goalTile.transform.SetParent(gridRoot, false);
                            goalTile.transform.localPosition = worldTilePos;
                            goalTile.Init(tileType, tilePos);
                            goals.Add(tilePos, goalTile);
                            break;

                    default: break;
                    }                                          
                }
            }
        }

        /// <summary>
        /// Set grid to appear in bottom left corner
        /// </summary>
       void OffsetGridToBottomLeft()
        {
            int gridWidth;
            int gridHeight;

            // Grid widht based on number of tiles
            if ((this.gridWidth % 2) == 0)
            {
                gridWidth = this.gridWidth / 2;
            }
            else
            {
                gridWidth = (this.gridWidth - 1) / 2;
            }
            // hrid height based on number of tiles
            if ((this.gridHeight % 2) == 0)
            {
                gridHeight = this.gridHeight / 2;
            }
            else
            {
                gridHeight = (this.gridHeight - 1) / 2;
            }

            Vector3 offset = -GridUtils.GridToWorld(new Vector2Int(gridWidth, gridHeight));
            gridRoot.position = offset;
        }

        private void OnLevelSelected(LevelSelectPayload payload)
        {
            if (payload.resetRequested)
            {
                ResetGrid();

                // raise again for other listeners
                levelSelectEvent.Raise(new LevelSelectPayload
                {
                    levelNumber = payload.levelNumber,
                    resetRequested = false
                });
                return;
            }

            // běžná logika – např. GenerateGrid()
        }

        /// <summary>
        /// Reset grid layout 
        /// </summary>
        /// <param name="noInt">no integer input necessary</param>
        void ResetGrid()
        {
            //Destroy old grid
            foreach (Transform child in gridRoot.transform)
            {
                Destroy(child.gameObject);
            }

            // clear dictionaries
            if (pushables != null) pushables.Clear();
            if (goals != null) goals.Clear();
        }
        #endregion

        #region tile handling
        /// <summary>
        /// get tile type on input position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public TileType GetTileType(Vector2Int position)
        {
            return grid[position.x, position.y];    
        }

        /// <summary>
        /// Set tile type on position
        /// </summary>
        /// <param name="position"> position of set tile</param>
        /// <param name="tileType">tile type to be set </param>
        public void SetTileType(Vector2Int position, TileType tileType)
        {
            grid[position.x, position.y] = tileType;        }


        /// <summary>
        /// Checks whether tile at position is obstacle
        /// </summary>
        /// <param name="position"> checked position for obstacle</param>
        /// <returns></returns>
        public bool IsObstacleAt(Vector2Int position)
        {
            return TileTypeExtensions.IsObstacle(grid[position.x, position.y]);
        }
    
        /// <summary>
        /// Checks whether tile at position is movable obstacle
        /// </summary>
        /// <param name="position"> checked position for movable obstacle</param>
        /// <returns></returns>
        public bool IsMovableAt(Vector2Int position)
        {
            return TileTypeExtensions.IsMovable(grid[position.x, position.y]);
        }
     
        /// <summary>
        /// Checkes whether tile at position can be walked on
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool IsWalkableAt(Vector2Int position)
        {
            return TileTypeExtensions.IsWalkable(grid[position.x, position.y]);
        }

        public bool IsGoalAt(Vector2Int position)
        {
          //  Debug.Log($"isGoalat reached at position: {position}");
            return TileTypeExtensions.IsGoal(grid[position.x, position.y]);
        }

        /// <summary>
        /// Remove pushable in dictionary on pasiton
        /// </summary>
        /// <param name="position">position to remove pushable</param>    
        public void RemoveGoalAt(Vector2Int position)
        {
            if (!IsInGrid(position))
            {
                Debug.LogError("Remove tile is outside the grid");
                return;
            }
            goals.Remove(position);
        }

        /// <summary>
        /// Check whether position is in game grid
        /// </summary>
        /// <param name="pos">tile position to check (x,y)</param>
        /// <returns></returns>
        public bool IsInGrid(Vector2Int position)
        { 
            return position.x >= 0 && position.x < gridWidth 
                && position.y >= 0 && position.y < gridHeight;
        }
        
        /// <summary>
        /// Get movable object at a position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public TileObject GetPushableAt(Vector2Int position)
        {
            return pushables.GetValueOrDefault(position);
        }


        /// <summary>
        /// Remove pushable in dictionary on pasiton
        /// </summary>
        /// <param name="position">position to remove pushable</param>    
        public void RemovePushableAt(Vector2Int position)
        {
            if (!IsInGrid(position))
            {
                Debug.LogError("Remove tile is outside the grid");
                return;
            }
            pushables.Remove(position);
        }

        /// <summary>
        /// Sets Pushable in dictionary on position
        /// </summary>
        /// <param name="position"> position to add pushable</param>
        /// <param name="tileObject">pushable tile object</param>
        public void SetPushableAt(Vector2Int position, PushableTile tileObject)
        { 
           if (!IsInGrid(position))
             {
                Debug.LogError("Set tile is outside the grid");
                return;
             }   
           pushables[position] = tileObject;
        }


        /// <summary>
        /// Get positon log of movable objects position
        /// </summary>
        public void LogMovablePositions()
        {       
           Debug.Log("Pushable object count: " + pushables.Count);     
        }

        #endregion
    }
}

