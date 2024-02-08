using System;
using UnityEngine;

namespace Edgar.Unity.Examples.Gungeon
{
    public class GungeonEnemy : MonoBehaviour
    {
        public GungeonRoomManager RoomManager;

        public void returnDead()
        {
            RoomManager.OnEnemyKilled(this);
        }
    }
}