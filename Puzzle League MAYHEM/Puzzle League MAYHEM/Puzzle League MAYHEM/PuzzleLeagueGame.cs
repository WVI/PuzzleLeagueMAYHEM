using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Puzzle_League_MAYHEM {
    /// <summary>
    /// Puzzle League MAYHEM! is a collaborative project by LUElinks users
    /// to create a new, personalized entry in the best puzzle game
    /// series ever made.
    /// </summary>
    public class PuzzleLeagueGame : Microsoft.Xna.Framework.Game {
        // VIDEO
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        // AUDIO

        // ENGINE
        ObjectManager objectManager;

        // CONTENT

        public PuzzleLeagueGame() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize() {
            objectManager = new ObjectManager(this);
            Components.Add(objectManager);

            base.Initialize();
        }

         protected override void LoadContent() {
            // GRAPHICS
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // AUDIO


            // ENGINE


        }

        protected override void UnloadContent() {
            
        }

        protected override void Update(GameTime gameTime) {
            // KILLSWITCH
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.DarkRed);

            base.Draw(gameTime);
        }
    }
}
