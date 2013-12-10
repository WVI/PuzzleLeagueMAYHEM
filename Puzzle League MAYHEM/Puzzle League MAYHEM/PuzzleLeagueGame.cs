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
    /// Puzzle League MAYHEM! is a fan game based on the best puzzle game series ever made.
    /// </summary>
    public class PuzzleLeagueGame : Microsoft.Xna.Framework.Game {
        // VIDEO
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
		Vector2 NATIVE_RESOLUTION = new Vector2(256, 224);
		float Scale = 2.0f;
		RenderTarget2D targetScreen;

        // AUDIO


        // ENGINE
		ContentManager theContentManager;
        ObjectManager objectManager;
		KeyboardState keyState, lastKeyState;
		GamePadState padState1, lastPadState1, padState2, lastPadState2;

        // CONTENT


		// TESTING
		Stack testStack = new Stack();
		Stack testStack2 = new Stack();

        public PuzzleLeagueGame() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
			theContentManager = new ContentManager(Content.ServiceProvider, Content.RootDirectory);
        }

        protected override void Initialize() { // Minimum native resolution for SNES is 256 x 224.
			graphics.IsFullScreen = false;
			graphics.PreferredBackBufferWidth = 256 * 2;
			graphics.PreferredBackBufferHeight = 224 * 2;

            objectManager = new ObjectManager(this);
            Components.Add(objectManager);

            base.Initialize();
        }

         protected override void LoadContent() {
            // GRAPHICS
            spriteBatch = new SpriteBatch(GraphicsDevice);
			targetScreen = new RenderTarget2D(GraphicsDevice, (int)NATIVE_RESOLUTION.X, (int)NATIVE_RESOLUTION.Y);

            // AUDIO


            // ENGINE


			// TESTING
			testStack.LoadContent(theContentManager);
			testStack2.LoadContent(theContentManager);
			testStack2.StackLocation.X += 150;
        }

        protected override void UnloadContent() {
            
        }

        protected override void Update(GameTime gameTime) {
            keyState = Keyboard.GetState();
			padState1 = GamePad.GetState(PlayerIndex.One);
			padState2 = GamePad.GetState(PlayerIndex.Two);


			// TEST INPUT
			
			bool blockSwitch = false;
			bool manualRaise = false;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyState.IsKeyDown(Keys.Escape))
                this.Exit();


			if (keyState.IsKeyDown(Keys.Up) && !lastKeyState.IsKeyDown(Keys.Up))
				testStack.MoveCursor(Stack.Direction.Up);
			if (keyState.IsKeyDown(Keys.Right) && !lastKeyState.IsKeyDown(Keys.Right))
				testStack.MoveCursor(Stack.Direction.Right);
			if (keyState.IsKeyDown(Keys.Down) && !lastKeyState.IsKeyDown(Keys.Down))
				testStack.MoveCursor(Stack.Direction.Down);
			if (keyState.IsKeyDown(Keys.Left) && !lastKeyState.IsKeyDown(Keys.Left))
				testStack.MoveCursor(Stack.Direction.Left);

			if (keyState.IsKeyDown(Keys.Z) && !lastKeyState.IsKeyDown(Keys.Z))
				blockSwitch = true;
			if (keyState.IsKeyDown(Keys.X) && !lastKeyState.IsKeyDown(Keys.X))
				manualRaise = true;

			if (padState1.IsButtonDown(Buttons.DPadUp) && !padState1.IsButtonDown(Buttons.DPadUp))
				testStack.MoveCursor(Stack.Direction.Up);
			if (padState1.IsButtonDown(Buttons.DPadRight) && !padState1.IsButtonDown(Buttons.DPadRight))
				testStack.MoveCursor(Stack.Direction.Right);
			if (padState1.IsButtonDown(Buttons.DPadDown) && !padState1.IsButtonDown(Buttons.DPadDown))
				testStack.MoveCursor(Stack.Direction.Down);
			if (padState1.IsButtonDown(Buttons.DPadLeft) && !padState1.IsButtonDown(Buttons.DPadLeft))
				testStack.MoveCursor(Stack.Direction.Left);

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
			if (manualRaise) {
				testStack.ManualRaise();
				manualRaise = false;
			}

			testStack.Update(gameTime);
			testStack2.Update(gameTime);

            base.Update(gameTime);

			lastKeyState = keyState;
			lastPadState1 = padState1; lastPadState2 = padState2;
        }

        protected override void Draw(GameTime gameTime) {
			GraphicsDevice.SetRenderTarget(targetScreen);
            GraphicsDevice.Clear(Color.DarkRed);
			

			spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
			testStack.Draw(spriteBatch);
			testStack2.Draw(spriteBatch);

			spriteBatch.End();

			GraphicsDevice.SetRenderTarget(null);

			spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
			spriteBatch.Draw(targetScreen, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, Scale, SpriteEffects.None, 0.0f);
			spriteBatch.End();

			spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
