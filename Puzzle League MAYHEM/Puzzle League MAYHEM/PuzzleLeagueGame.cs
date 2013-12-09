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
		ContentManager theContentManager;
        ObjectManager objectManager;
		KeyboardState keyState, lastKeyState;
		GamePadState padState1, lastPadState1, padState2, lastPadState2;

        // CONTENT


		// TESTING
		Stack testStack = new Stack();

        public PuzzleLeagueGame() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
			theContentManager = new ContentManager(Content.ServiceProvider, Content.RootDirectory);
        }

        protected override void Initialize() { // Minimum native resolution for SNES is 256 x 224.
			graphics.PreferredBackBufferWidth = 256 * 2;
			graphics.PreferredBackBufferHeight = 224 * 2;

            objectManager = new ObjectManager(this);
            Components.Add(objectManager);

            base.Initialize();
        }

         protected override void LoadContent() {
            // GRAPHICS
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // AUDIO


            // ENGINE


			// TESTING
			testStack.LoadContent(theContentManager);
        }

        protected override void UnloadContent() {
            
        }

        protected override void Update(GameTime gameTime) {
            // KILLSWITCH
			keyState = Keyboard.GetState();
			padState1 = GamePad.GetState(PlayerIndex.One);
			padState2 = GamePad.GetState(PlayerIndex.Two);

			bool blockSwitch = false;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyState.IsKeyDown(Keys.Escape))
                this.Exit();


			if (keyState.IsKeyDown(Keys.Up) && !lastKeyState.IsKeyDown(Keys.Up))
				testStack.MoveCursor(0);
			if (keyState.IsKeyDown(Keys.Right) && !lastKeyState.IsKeyDown(Keys.Right))
				testStack.MoveCursor(1);
			if (keyState.IsKeyDown(Keys.Down) && !lastKeyState.IsKeyDown(Keys.Down))
				testStack.MoveCursor(2);
			if (keyState.IsKeyDown(Keys.Left) && !lastKeyState.IsKeyDown(Keys.Left))
				testStack.MoveCursor(3);

			if (keyState.IsKeyDown(Keys.Z) && !lastKeyState.IsKeyDown(Keys.Z))
				blockSwitch = true;

			if (padState1.IsButtonDown(Buttons.DPadUp) && !padState1.IsButtonDown(Buttons.DPadUp))
				testStack.MoveCursor(0);
			if (padState1.IsButtonDown(Buttons.DPadRight) && !padState1.IsButtonDown(Buttons.DPadRight))
				testStack.MoveCursor(1);
			if (padState1.IsButtonDown(Buttons.DPadDown) && !padState1.IsButtonDown(Buttons.DPadDown))
				testStack.MoveCursor(2);
			if (padState1.IsButtonDown(Buttons.DPadLeft) && !padState1.IsButtonDown(Buttons.DPadLeft))
				testStack.MoveCursor(3);

			if (padState1.IsButtonDown(Buttons.A) && !lastPadState1.IsButtonDown(Buttons.A))
				blockSwitch = true;
			if (padState1.IsButtonDown(Buttons.B) && !lastPadState1.IsButtonDown(Buttons.B))
				blockSwitch = true;
			if (padState1.IsButtonDown(Buttons.X) && !lastPadState1.IsButtonDown(Buttons.X))
				blockSwitch = true;
			if (padState1.IsButtonDown(Buttons.Y) && !lastPadState1.IsButtonDown(Buttons.Y))
				blockSwitch = true;

			if (blockSwitch) {
				testStack.Switch();
				blockSwitch = false;
			}

			testStack.Update(gameTime);

            base.Update(gameTime);

			lastKeyState = keyState;
			lastPadState1 = padState1; lastPadState2 = padState2;
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.DarkRed);

			spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
			GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
			testStack.Draw(spriteBatch);

			spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
