﻿using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetris.LevelObjects;
using Tetris.States;

namespace Tetris
{
    public class Game1 : ExtendedGame
    {
        public static string STATE_PLAYINGSCENE = "playing";

        public Game1()
        {
            IsMouseVisible = true;
            windowSize = new Point(620, 740);
            worldSize = new Point(620, 740);
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            // TODO: use this.Content to load your game content here

            GameStateManager.AddGameState(STATE_PLAYINGSCENE, new Level());
            GameStateManager.SwitchTo(STATE_PLAYINGSCENE);
        }

    }
}