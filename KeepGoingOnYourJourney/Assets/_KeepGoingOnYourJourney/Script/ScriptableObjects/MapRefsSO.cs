using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Map Settings SO", menuName = "ScriptableObjects/MapRefsSO")]
public class MapRefsSO : ScriptableObject
{
    [System.Serializable]
    public struct WallInfo
    {
        public enum SpecialWall
        {
            Normal = 0,
            Fake = 1,
            Finish = 2,
            Environment = 3,
        }
        public enum WallVisual
        {
            House = 0,
            Store = 1,
            Tree = 2,
            WindTurbines = 3,   
        }
        public SpecialWall specialWall;
        public WallVisual wallVisual;
        public WallRefsSO wallRefsSO;
        public float posX, posY;
    }
    [System.Serializable]
    public struct ObstacleInfo
    {
        public enum ObstacleType
        {
            Static = 0,
            Dynamic = 1,
        }
        public enum ObstacleVisual
        {
            Virus = 0,
            Thorn = 1,
        }
        public ObstacleType obstacleType;
        public ObstacleVisual obstacleVisual;
        public bool walkable;
        public Vector3 moveDir;
        public WallRefsSO wallRefsSO;
        public FloatReference moveDistance;
        public float posX, posY;
    }
    [System.Serializable]
    public struct ItemInfo
    {
        public enum ItemVisual
        {
            Diamond = 0,
            Cup = 1,
            Coin = 2,
        }
        public ItemVisual itemVisual;
        public ItemRefsSO itemRefsSO;
        public float posX, posY;
    }

    [System.Serializable]
    public struct GroundInfo
    {
        public enum GroundPos
        {
            Normal = 0,
            Top = 1,
            Left = 2,
            Right = 3,
            Bottom = 4,
            TopLeft = 5,
            TopRight = 6,
            BottomLeft = 7,
            BottomRight = 8,
        }
        public enum GroundVisual
        {
            NoBorder = 0,
            OneBorder = 1,
            TwoAdjacentBorder = 2,
            TwoParallelBorder = 3,
            ThereBorder = 4
        }
        public GroundPos groundPos;
        public GroundVisual groundVisual;
        public WallRefsSO wallRefsSO;
        public float posX, posY;
    }

    public WallInfo[] wallInfo;
    public ObstacleInfo[] obstacleInfo;
    public ItemInfo[] itemInfo;
    public GroundInfo[] groundInfo;
}
