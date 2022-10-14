﻿using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetris.States;

namespace Tetris
{
    public class Game1 : ExtendedGame
    {
        public string STATE_PLAYINGSCENE = "playing";

        public Game1()
        {
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            // TODO: use this.Content to load your game content here

            GameStateManager.AddGameState(STATE_PLAYINGSCENE, new PlayingState());
            GameStateManager.SwitchTo(STATE_PLAYINGSCENE);
        }

    }
}