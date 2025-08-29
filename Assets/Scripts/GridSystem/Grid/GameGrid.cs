using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
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
        [Header("Events")]
        [SerializeField] private BuildGridDoneEvent buildGridDoneEvent;
        [SerializeField] private LevelSelectEvent levelSelectEvent;
        [SerializeField] private ResetGridEvent resetGridEvent;
        [SerializeField] private BuildGridEvent buildGridEvent;
        [SerializeField] private LevelDatabase levelDatabase;

        [SerializeField] private NutSetEvent nutSetEvent;
        [SerializeField] private NutRemovedEvent nutRemovedEvent;
        [SerializeField] private GoalSetEvent goalSetEvent;
        [SerializeField] private GoalRemovedEvent goalRemovedEvent;
        [SerializeField] private TileQueryEvent tileQueryEvent;
        [SerializeField] private WallSetEvent wallSetEvent;
        [SerializeField] private RoadSetEvent roadSetEvent;
        #endregion

        #region tile objects
        [Header("Tile objects")]
        // Prefabs
        [SerializeField] private GameObject emptyPrefab;
        [SerializeField] private GameObject roadPrefab;
        [SerializeField] private GameObject roadStonePrefab;
        [SerializeField] private GameObject veverkaPrefab;
        [SerializeField] private GameObject nutPrefab;
        [SerializeField] private GameObject nutStonePrefab;       
        [SerializeField] private GameObject wallPrefab;
        [SerializeField] private GameObject wallStonePrefab;
        [SerializeField] private GameObject holePrefab;
        [SerializeField] private GameObject goalPrefab;

        [Header("Other objects")]
        [SerializeField] private GameObject backgroundPrefab;
        //Adjust screen for each level size
        [SerializeField] private Transform gridRoot;
        #endregion

        #region fields
        // grid variables       
        readonly Vector2Int maxScreenGridSize = new(15, 11);
        private Vector2Int gridSize;     
        private TileType[,] grid;
        private NutType[,] nutGrid;
        private GoalType[,] goalGrid;
        private WallType[,] wallGrid;
        private RoadType[,] roadGrid;
        private readonly List<GameObject> backgroundPool = new();
        // pool for surrounding wall tiles to avoid repeated instantiation
        private readonly List<GameObject> surroundingWalls = new();
        #endregion
           
        #region Awake
        /// <summary>
        /// init singleton and events
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
        }

        private void OnEnable()
        {
            // add listeners for events
            levelSelectEvent.AddListener(OnLevelSelected);
            buildGridEvent.AddListener(InitializeGrid);
            resetGridEvent.AddListener(ResetGrid);           
            tileQueryEvent.AddListener(OnTileQuery);        
            nutSetEvent.AddListener(OnNutSet);
            nutRemovedEvent.AddListener(OnNutRemoved);
            goalSetEvent.AddListener(OnGoalSet);
            goalRemovedEvent.AddListener(OnGoalRemoved);
            wallSetEvent.AddListener(OnWallSet);
            roadSetEvent.AddListener(OnRoadSet);
        }

        /// <summary>
        /// On object disable
        /// </summary>
        private void OnDisable()
        {
            // remove listeners for events
            levelSelectEvent.RemoveListener(OnLevelSelected);
            resetGridEvent.RemoveListener(ResetGrid);        
            buildGridEvent.RemoveListener(InitializeGrid);
            tileQueryEvent.RemoveListener(OnTileQuery);
            nutSetEvent.RemoveListener(OnNutSet);
            nutRemovedEvent.RemoveListener(OnNutRemoved);
            goalSetEvent.RemoveListener(OnGoalSet);
            goalRemovedEvent.RemoveListener(OnGoalRemoved);
            wallSetEvent.RemoveListener(OnWallSet);
            roadSetEvent.RemoveListener(OnRoadSet);
        }

        #endregion

        #region Grid Init
        /// <summary>
        /// Initialize a new grid
        /// </summary>
        /// <param name="grid"></param>
        private void InitializeGrid(TileType[,] grid)
        {
            //get new grid size
            this.grid = grid;
            this.nutGrid = GridUtils.CachedNutGrid;
            this.goalGrid = GridUtils.CachedGoalGrid;
            this.wallGrid= GridUtils.CachedWallGrid;
            this.roadGrid = GridUtils.CachedRoadGrid;
            this.gridSize.x = grid.GetLength(0);
            this.gridSize.y = grid.GetLength(1);
            

            Debug.Log($"Size GameGrid: {gridSize.x}, {gridSize.y}");

            // Spawn objects based on tile types            
            BuildLevelFromGrid();

            // add surrounding walls where grid does not fill the screen
            BuildSurroundings();

            //level database initialization
            levelDatabase.gridSize = gridSize;
            int goalCount = 0;
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    if (grid[x, y] == TileType.Goal && goalGrid[x, y] == GoalType.BasicGoal)
                    {
                        goalCount++;
                    }
                }
            }
            levelDatabase.GoalsCount = goalCount;
            levelDatabase.gridOrigin = transform;

            //raise event
            buildGridDoneEvent.Raise();
        }

        /// <summary>
        /// Ensures that the background pool contains at least the specified number of objects.
        /// </summary>
        /// <remarks>If the current count of objects in the background pool is less than <paramrefname="requiredCount"/>,
        /// additional objects are instantiated using the <c>backgroundPrefab</c> and added to
        /// the pool.  Newly instantiated objects are initialized at the origin, deactivated, and parented to
        /// <c>gridRoot</c>.</remarks>
        /// <param name="requiredCount">The minimum number of objects that the background pool should contain.</param>
        private void EnsureBackgroundPool(int requiredCount)
        {
            for (int i = backgroundPool.Count; i < requiredCount; i++)
            {
                var background = Instantiate(backgroundPrefab, Vector3.zero, Quaternion.identity, gridRoot);
                background.SetActive(false);
                backgroundPool.Add(background);
            }
        }

        /// <summary>
        /// Build a level from a grid
        /// </summary>
        private void BuildLevelFromGrid()
        {
            EnsureBackgroundPool(gridSize.x * gridSize.y);

            int bgIndex = 0;

            // for cycle over grid width
            for (int x = 0; x < gridSize.x; x++)
            {
                // for cycle over grid height
                for (int y = 0; y < gridSize.y; y++)
                {
                    //Prepare tile from the grid
                    Vector2Int tilePos = new(x, y);
                    Vector3 worldTilePos = GridUtils.GridToWorld(tilePos);
                    TileType tileType =  GetTileType(tilePos);

                    var background = backgroundPool[bgIndex++];
                    background.transform.SetParent(gridRoot, false);
                    background.transform.localPosition = worldTilePos;
                    background.SetActive(true);

                    // insert specific tiles
                    switch (tileType)
                    {
                        case TileType.Empty:

                            SetTileType(tilePos, TileType.Empty);
                            break;

                        case TileType.Wall:

                            GameObject tile;
                            if (wallGrid[x, y] == WallType.StoneWall)
                            {
                                tile = Instantiate(wallStonePrefab, Vector3.zero, Quaternion.identity, gridRoot);
                            }
                            else
                            {
                                tile = Instantiate(wallPrefab, Vector3.zero, Quaternion.identity, gridRoot);
                            }
                            tile.transform.SetParent(gridRoot, false);
                            tile.transform.localPosition = worldTilePos;
                            SetTileType(tilePos, TileType.Wall);
                            break;

                        case TileType.Veverka:

                            SetTileType(tilePos, TileType.Empty);

                            // generete veverka
                            CharVeverka veverka = Instantiate(veverkaPrefab, Vector3.zero, Quaternion.identity, gridRoot).GetComponent<CharVeverka>();
                            veverka.transform.SetParent(gridRoot, false);
                            veverka.transform.localPosition = worldTilePos;
                            veverka.Init(tileType, CharacterType.BasicVeverka, tilePos);

                            //Center grid according veverka
                            levelDatabase.gridCenterStartTarget = veverka.transform;
                            break;

                        case TileType.Nut:

                            if (nutGrid[x, y] == NutType.StoneNut)
                            {
                                StoneNutTile stoneNutTile = Instantiate(nutStonePrefab, Vector3.zero, Quaternion.identity, gridRoot).GetComponent<StoneNutTile>();
                                stoneNutTile.transform.SetParent(gridRoot, false);
                                stoneNutTile.transform.localPosition = worldTilePos;
                                stoneNutTile.Init(tileType, tilePos, NutType.StoneNut);
                            }
                            else
                            {
                                BasicNutTile nutTile = Instantiate(nutPrefab, Vector3.zero, Quaternion.identity, gridRoot).GetComponent<BasicNutTile>();
                                nutTile.transform.SetParent(gridRoot, false);
                                nutTile.transform.localPosition = worldTilePos;
                                nutTile.Init(tileType, tilePos, NutType.BasicNut);
                            }
                            break;

                        case TileType.Goal:

                            if (goalGrid[x, y] == GoalType.HoleGoal)
                            {
                                HoleTile holeTile = Instantiate(holePrefab, Vector3.zero, Quaternion.identity, gridRoot).GetComponent<HoleTile>();
                                holeTile.transform.SetParent(gridRoot, false);
                                holeTile.transform.localPosition = worldTilePos;
                                holeTile.Init(tileType, tilePos, GoalType.HoleGoal);
                            }
                            else
                            {
                                GoalTile goalTile = Instantiate(goalPrefab, Vector3.zero, Quaternion.identity, gridRoot).GetComponent<GoalTile>();
                                goalTile.transform.SetParent(gridRoot, false);
                                goalTile.transform.localPosition = worldTilePos;
                                goalTile.Init(tileType, tilePos, GoalType.BasicGoal);
                            }
                            break;

                        default: break;
                    }
                }
            }

            for (int i = bgIndex; i < backgroundPool.Count; i++)
            {
                backgroundPool[i].SetActive(false);
            }
        }

        /// <summary>
        /// Build surround environment for grids smaller than screen
        /// </summary>
        void BuildSurroundings()
        {
            // if grid is larger than max screen size, do not build surroundings
            int diffX = maxScreenGridSize.x - gridSize.x;
            int diffY = maxScreenGridSize.y - gridSize.y;

            if (diffX < 0 || diffY < 0 || (diffX == 0 && diffY == 0))
            {
                return;
            }

            //Offset calculation for surrounding walls
            int offsetLeft = diffX / 2;
            int offsetRight = diffX - offsetLeft;
            int offsetDown = diffY / 2;
            int offsetUp = diffY - offsetDown;

            //number of needed surrounding walls
            int widthWithOffset = gridSize.x + offsetLeft + offsetRight;
            int needed = widthWithOffset * (offsetDown + offsetUp) + gridSize.y * (offsetLeft + offsetRight);

            // if there are not enough walls in pool, instantiate new ones
            for (int i = surroundingWalls.Count; i < needed; i++)
            {
                var wall = Instantiate(wallPrefab, Vector3.zero, Quaternion.identity, gridRoot);
                surroundingWalls.Add(wall);
            }

            int index = 0;

            // Position surrounding walls on bottom
            for (int y = -offsetDown; y < 0; y++)
            {
                for (int x = -offsetLeft; x < gridSize.x + offsetRight; x++)
                {
                    PositionWall(index++, x, y);
                }
            }

            // Position surrounding walls on top
            for (int y = gridSize.y; y < gridSize.y + offsetUp; y++)
            {
                for (int x = -offsetLeft; x < gridSize.x + offsetRight; x++)
                {
                    PositionWall(index++, x, y);
                }
            }

            //position surrounding walls on left
            for (int x = -offsetLeft; x < 0; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    PositionWall(index++, x, y);
                }
            }

            //position surrounding walls on right
            for (int x = gridSize.x; x < gridSize.x + offsetRight; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    PositionWall(index++, x, y);
                }
            }

            // Deactivate remaining walls in pool
            for (; index < surroundingWalls.Count; index++)
            {
                surroundingWalls[index].SetActive(false);
            }
        }

        /// <summary>
        /// Positions a wall at the specified grid coordinates and activates it.
        /// </summary>
        /// <remarks>The method activates the wall at the specified index, sets its parent to the grid
        /// root, and positions it at the corresponding world coordinates. Ensure that the <paramref name="index"/> is
        /// within the bounds of the surrounding walls collection.</remarks>
        /// <param name="index">The index of the wall in the surrounding walls collection. Must be a valid index within the collection.</param>
        /// <param name="x">The x-coordinate of the grid position where the wall should be placed.</param>
        /// <param name="y">The y-coordinate of the grid position where the wall should be placed.</param>
        void PositionWall(int index, int x, int y)
        {
            var wall = surroundingWalls[index];
            wall.SetActive(true);
            Vector3 worldTilePos = GridUtils.GridToWorld(new Vector2Int(x, y));
            wall.transform.SetParent(gridRoot, false);
            wall.transform.localPosition = worldTilePos;
        }

        /// <summary>
        /// Reset grid and raise again to load selected level
        /// </summary>
        /// <param name="payload"> level select payload</param>
        private void OnLevelSelected(LevelSelectPayload payload)
        {
            // Executes only when reset is requested
            if (payload.resetRequested)
            {
                ResetGrid();

                // raise again for other listeners
                levelSelectEvent.Raise(new LevelSelectPayload
                {
                    levelNumber = payload.levelNumber,
                    resetRequested = false
                });

                Debug.Log($"Grid reseted and starting new level {levelDatabase.CurrentLevelIndex}");
                return;
            }
        }

        /// <summary>
        /// Reset grid layout 
        /// </summary>
        /// <param name="noInt">no integer input necessary</param>
        private void ResetGrid()
        {
            //Destroy old grid except pooled backgrounds
           foreach (Transform child in gridRoot.transform)
            {
                // pool of surrounding walls
                if (surroundingWalls.Contains(child.gameObject))
                {
                    child.gameObject.SetActive(false);
                    continue;
                }
                
                // pool of in game backgrounds
                if (!backgroundPool.Contains(child.gameObject))
                {
                    Destroy(child.gameObject);
                }
                
            }              

            foreach (var background in backgroundPool)
            {
                background.SetActive(false);
            }

            // clear grids
            nutGrid = null;
            goalGrid = null;
            wallGrid = null;
            roadGrid = null;

            // clear level database
            if (levelDatabase != null)
            {
                levelDatabase.gridSize = Vector2Int.zero;
                levelDatabase.GoalsCount = 0;
                levelDatabase.gridCenterStartTarget = null;
                levelDatabase.gridOrigin = null;
            }
        }
        #endregion

        #region tile handling
        /// <summary>
        /// On wall set event - instantiate wall prefab and set tile type in grid
        /// </summary>
        /// <param name="payload"></param>
        private void OnWallSet(WallBasicPayload payload)
        {
            // instantiate wall prefab
            if (payload.instantiateTile)
            { 
                GameObject tile;
                if (payload.WallType == WallType.StoneWall)
                {
                    tile = Instantiate(wallStonePrefab, Vector3.zero, Quaternion.identity, gridRoot);
                }
                else
                {
                    tile = Instantiate(wallPrefab, Vector3.zero, Quaternion.identity, gridRoot);
                }

                tile.transform.SetParent(gridRoot, false);
                tile.transform.localPosition = new Vector3(payload.Position.x, payload.Position.y, 0);
            }
            // set tile and wall type in grid
            SetTileType(payload.Position, TileType.Wall);
            wallGrid[payload.Position.x, payload.Position.y] = payload.WallType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        private void OnRoadSet(RoadBasicPayload payload)
        {
            if (payload.instantiateTile) 
            {
                GameObject tile;
                if (payload.RoadType == RoadType.StoneRoad)
                {
                    tile = Instantiate(roadStonePrefab, Vector3.zero, Quaternion.identity, gridRoot);
                }
                else
                {
                    tile = Instantiate(roadPrefab, Vector3.zero, Quaternion.identity, gridRoot);
                }

                tile.transform.SetParent(gridRoot, false);
                tile.transform.localPosition = new Vector3(payload.Position.x, payload.Position.y, 0);
            }

            // set tile type and road type in grid
            SetTileType(payload.Position, TileType.Road);
            roadGrid[payload.Position.x, payload.Position.y] = payload.RoadType;
        }

        /// <summary>
        /// Set nut tile on position and update tile type in the grid
        /// </summary>
        /// <param name="payload"></param>
        private void OnNutSet(NutBasicPayload payload)
        {
            if (!IsInGrid(payload.Position))
            {
                Debug.LogError("Set tile is outside the grid");
                return;
            }
            SetTileType(payload.Position, TileType.Nut);
            nutGrid[payload.Position.x, payload.Position.y] = payload.NutType;
        }

        /// <summary>
        /// Remove nut tile on position and update tile type in the grid
        /// </summary>
        /// <param name="payload"></param>
        private void OnNutRemoved(NutBasicPayload payload)
        {
            if (!IsInGrid(payload.Position))
            {
                Debug.LogError("Remove tile is outside the grid");
                return;
            }
            SetTileType(payload.Position, TileType.Empty);
            nutGrid[payload.Position.x, payload.Position.y] = payload.NutType;
        }

        /// <summary>
        /// Set goal tile on position and update tile type in the grid
        /// </summary>
        /// <param name="payload"></param>
        private void OnGoalSet(GoalBasicPayload payload)
        {
            SetTileType(payload.Position, TileType.Goal);
            goalGrid[payload.Position.x, payload.Position.y] = payload.GoalType;
        }

        /// <summary>
        /// Remove goal tile on position and update tile type in the grid
        /// </summary>
        /// <param name="payload"></param>
        private void OnGoalRemoved(GoalBasicPayload payload)
        {
            if (!IsInGrid(payload.Position))
            {
                Debug.LogError("Remove tile is outside the grid");
                return;
            }
            SetTileType(payload.Position, TileType.Empty);
            goalGrid[payload.Position.x, payload.Position.y] = payload.GoalType;
        }

        /// <summary>
        /// Responds to tile query events and fills the payload with information about the tile.
        /// </summary>
        /// <param name="payload">Query payload containing position to check.</param>
        private void OnTileQuery(TileQueryPayload payload)
        {
            payload.IsInGrid = IsInGrid(payload.Position);
            if (!payload.IsInGrid) return;

            payload.IsWalkable = IsWalkableAt(payload.Position);
            payload.IsPushable = IsPushableAt(payload.Position);
            payload.TileType = grid[payload.Position.x, payload.Position.y];
            payload.GoalType = goalGrid[payload.Position.x, payload.Position.y];
            payload.WallType = wallGrid[payload.Position.x, payload.Position.y];
            payload.NutType = nutGrid[payload.Position.x, payload.Position.y];
            payload.RoadType = roadGrid[payload.Position.x, payload.Position.y];
        }

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
            grid[position.x, position.y] = tileType;
        }

        /// <summary>
        /// Checks whether a tile at position can be walked on by a character.
        /// </summary>
        public bool IsWalkableAt(Vector2Int position)
        {
            var tile = grid[position.x, position.y];
            if (tile == TileType.Goal)
            {
                return goalGrid[position.x, position.y] != GoalType.HoleGoal;
            }
            return tile == TileType.Empty || tile == TileType.Road;
        }

        /// <summary>
        /// Checks whether a tile at position can be entered by a nut.
        /// </summary>
        public bool IsPushableAt(Vector2Int position)
        {
            var tile = grid[position.x, position.y];
            if (tile == TileType.Goal)
            {
                return true;
            }
            return tile == TileType.Empty || tile == TileType.Road;
        }
        
        /// <summary>
        /// Check whether position is in game grid
        /// </summary>
        /// <param name="position">tile position to check (x,y)</param>
        /// <returns></returns>
        public bool IsInGrid(Vector2Int position)
        {
            if (position.x >= 0 && position.x < gridSize.x
                && position.y >= 0 && position.y < gridSize.y)
            {
                return true;
            }
            else
            {              
                return false;
            }                 
        }
        
        #endregion
    }
}

