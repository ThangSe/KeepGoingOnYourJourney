using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObstacle: IWall
{
    void Act();
    void Activate(Vector3 pos, WallRefsSO wallRefsSO, Transform parent, int obstacleType, Vector3 moveDir, float moveDst);
}
