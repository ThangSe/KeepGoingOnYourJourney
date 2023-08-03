using System.Collections.Generic;
using UnityEngine;

public class MyGrid : MonoBehaviour
{
    public bool displayGridGizmos;
    public LayerMask unWalkableMask;
    public LayerMask walkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;
    float nodeDiameter;
    int gridSizeX, gridSizeY;
    private Vector3 _originPos;
    [SerializeField] private MapRefsSO _mapRefsSO;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        _originPos = Vector3.zero;
        CreateGrid(_originPos);
        SetAllNodeNeighbours();

    }

    private void Start()
    {
        MainManager.Instance.OnMapChanged += MainManager_OnMapChanged;
    }
    private void MainManager_OnMapChanged(object sender, MainManager.OnMapChangedEventArgs e)
    {
        _originPos = e.newOrigin;
        _mapRefsSO = e.mapRefsSO;
        CreateGrid(e.newOrigin);
        SetAllNodeNeighbours();
    }

    public Node[,] NodeGrid()
    {
        return grid;
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }
    void CreateGrid(Vector3 originPos)
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = originPos - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2; 
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                bool walkable = true;
                bool isDead = true;
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                foreach(var ground in _mapRefsSO.groundInfo)
                {
                    if(worldPoint.x == ground.posX + originPos.x &&  worldPoint.y == ground.posY + originPos.y)
                    {
                        isDead = false;
                        break;
                    }
                }
                foreach(var obstracle in _mapRefsSO.obstacleInfo)
                {
                    if(worldPoint.x == obstracle.posX + originPos.x && worldPoint.y == obstracle.posY + originPos.y)
                    {
                        if(!obstracle.walkable)
                        {
                            walkable = false;
                            break;
                        }                      
                    }
                }
                foreach(var wall in _mapRefsSO.wallInfo)
                {
                    if(worldPoint.x == wall.posX + originPos.x && worldPoint.y == wall.posY + originPos.y)
                    {
                        isDead = false;
                        walkable = false;
                    }
                }
                grid[x, y] = new Node(walkable, isDead, worldPoint, x, y);
            }
        }
    }

    void SetAllNodeNeighbours()
    {
        if (grid != null)
        {
            foreach (Node node in grid)
            {
                node.SetNeighbours(GetNeighbours(node));
            }
        }
    }

    public void UpdateGrid()
    {
        if (grid != null)
        {
            foreach (Node node in grid)
            {
                bool walkable = true;
                bool isDead = true;
                foreach (var ground in _mapRefsSO.groundInfo)
                {
                    if (node.GetWorldPos().x == ground.posX + _originPos.x && node.GetWorldPos().y == ground.posY + _originPos.y)
                    {
                        isDead = false;
                        break;
                    }
                }
                foreach (var obstracle in _mapRefsSO.obstacleInfo)
                {
                    if (node.GetWorldPos().x == obstracle.posX + _originPos.x && node.GetWorldPos().y == obstracle.posY + _originPos.y)
                    {
                        if (!obstracle.walkable)
                        {
                            walkable = false;
                            break;
                        }
                    }
                }
                foreach (var wall in _mapRefsSO.wallInfo)
                {
                    if (node.GetWorldPos().x == wall.posX + _originPos.x && node.GetWorldPos().y == wall.posY + _originPos.y)
                    {
                        isDead = false;
                        walkable = false;
                    }
                }
                node.SetWalkable(walkable);
                node.SetIsDead(isDead);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x - _originPos.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y - _originPos.y + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public Vector3 GetOriginPos()
    {
        return _originPos;
    }


    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(_originPos, new Vector3(gridWorldSize.x, gridWorldSize.y));
        if(grid != null && displayGridGizmos)
        {
            foreach (Node node in grid)
            {
                if(node.Iswalkable() && node.IsDead())
                {
                    Gizmos.color = Color.blue;
                } else if(node.Iswalkable() && !node.IsDead())
                {
                    Gizmos.color = Color.white;
                } else if(!node.Iswalkable())
                {
                    Gizmos.color = Color.red;
                }
                //Gizmos.color = (node.walkable) ? Color.white : Color.red;
                Gizmos.DrawCube(node.GetWorldPos(), Vector3.one * (nodeRadius * 2 - .05f));
            }
        }
    }
}
