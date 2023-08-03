using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Factory
{
    public static IWall CreateWall(GameObject gameObject, Vector3 pos, WallRefsSO wallRefsSO)
    {
        return new NewWall(gameObject, pos, wallRefsSO);
    }
    public static IGround CreateGround(GameObject gameObject, Vector3 pos, WallRefsSO wallRefsSO, float rotation)
    {
        return new NewGround(gameObject, pos, wallRefsSO, rotation);
    }
    public static IObstacle CreateObstacle(GameObject gameObject, Vector3 pos, WallRefsSO wallRefsSO, int obstacleType, Vector3 moveDir, float moveDst)
    {
        return new NewObstacle(gameObject, pos, wallRefsSO, obstacleType, moveDir, moveDst);
    }

    public static IMap CreateMap(GameObject gameObject, Vector3 pos, MapRefsSO mapRefsSO)
    {
        return new NewMap(gameObject, pos, mapRefsSO);
    }
    public static IItem CreateItem(GameObject gameObject, Vector3 pos, ItemRefsSO itemRefsSO)
    {
        return new NewItem(gameObject, pos, itemRefsSO);
    }
    
    public static IPopup CreatePopup(GameObject gameObject, Vector3 pos)
    {
        return new NewPopup(gameObject, pos);
    }
}
