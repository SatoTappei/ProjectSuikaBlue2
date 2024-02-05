using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    public class InGameState
    {
        public InGameState(PlayerParameterSettings player, DungeonParameterSettings dungeon)
        {
            PlayerSettings = player;
            DungeonSettings = dungeon;
        }

        public PlayerParameterSettings PlayerSettings { get; private set; }
        public DungeonParameterSettings DungeonSettings { get; private set; }
    }
}