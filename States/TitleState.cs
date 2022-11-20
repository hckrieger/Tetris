using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.States
{
    internal class TitleState : GameState
    {

        TextGameObject title = new TextGameObject("Fonts/ScreenTitle", 1f, Color.DeepSkyBlue, TextGameObject.Alignment.Center);
        TextGameObject instructions = new TextGameObject("Fonts/Instructions", 1f, Color.DeepSkyBlue, TextGameObject.Alignment.Center);
        TextGameObject credit = new TextGameObject("Fonts/DebugFont", 1f, Color.DeepSkyBlue, TextGameObject.Alignment.Center);

        public TitleState()
        {
            title.Text = "Tetris";
            title.LocalPosition = new Vector2(310, 100);
            gameObjects.AddChild(title);

            instructions.Text = "        Instructions:\n\n" +
                                "Left Arrow - Move Left \n\n" +
                                "Right Arrow - Move Right\n\n" +
                                "Up Arrow - Rotate Shape\n\n" +
                                "Down Arrow - Soft Drop\n\n" +
                                "Space - Hard Drop \n\n\n" +
                                "Press Enter to Play";
            instructions.LocalPosition = new Vector2(310, 250);
            gameObjects.AddChild(instructions);

            credit.Text = "   Programmed by  Hunter Krieger";
            credit.LocalPosition = new Vector2(310, 680);
            gameObjects.AddChild(credit);


        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);

            if (inputHelper.KeyPressed(Keys.Enter))
            {
                ExtendedGame.GameStateManager.SwitchTo(Game1.STATE_PLAYINGSCENE);
            }
        }
    }
}
