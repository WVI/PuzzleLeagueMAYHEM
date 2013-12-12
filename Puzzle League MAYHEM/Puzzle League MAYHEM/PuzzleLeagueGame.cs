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

		int multipleMoveCounter = 0;

        public PuzzleLeagueGame() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
			theContentManager = new ContentManager(Content.ServiceProvider, Content.RootDirectory);
        }

        protected override void Initialize() { // Minimum native resolution for SNES is 256 x 224.
			graphics.IsFullScreen = false;
			graphics.PreferredBackBufferWidth = 256 * 2;
			graphics.PreferredBackBufferHeight = 224 * 2;

			this.IsFixedTimeStep = false;
			this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 60.0f);

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
			testStack2.StackLocation.X += 144;
			testStack2.MinimapLocation = new Vector2(134, 180);
        }

        protected override void UnloadContent() {
            
        }

        protected override void Update(GameTime gameTime) {
            keyState = Keyboard.GetState();
			padState1 = GamePad.GetState(PlayerIndex.One);
			padState2 = GamePad.GetState(PlayerIndex.Two);

			// TEST INPUT
			
			bool p1BlockSwitch = false;
			bool p1ManualRaise = false;
			bool p2BlockSwitch = false;
			bool p2ManualRaise = false;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyState.IsKeyDown(Keys.Escape))
                this.Exit();


			// P1
			Stack.Direction p1CurrentDirection = Stack.Direction.Up;

			if (!testStack.CursorCannotBeMoved) {
				if (keyState.IsKeyDown(Keys.Up)) {
					if (!lastKeyState.IsKeyDown(Keys.Up)) {
						multipleMoveCounter = 0;
						testStack.MoveCursor(Stack.Direction.Up);
					}
					else {
						multipleMoveCounter++;
						p1CurrentDirection = Stack.Direction.Up;
					}		
				}
				else if (keyState.IsKeyDown(Keys.Down)) {
					if (!lastKeyState.IsKeyDown(Keys.Down)) {
						multipleMoveCounter = 0;
						testStack.MoveCursor(Stack.Direction.Down);
					}
					else {
						multipleMoveCounter++;
						p1CurrentDirection = Stack.Direction.Down;
					}
				}
				else if (keyState.IsKeyDown(Keys.Right)) {
					if (!lastKeyState.IsKeyDown(Keys.Right)) {
						multipleMoveCounter = 0;
						testStack.MoveCursor(Stack.Direction.Right);
					}
					else {
						multipleMoveCounter++;
						p1CurrentDirection = Stack.Direction.Right;
					}
				}
				else if (keyState.IsKeyDown(Keys.Left)) {
					if (!lastKeyState.IsKeyDown(Keys.Left)) {
						multipleMoveCounter = 0;
						testStack.MoveCursor(Stack.Direction.Left);
					}
					else {
						multipleMoveCounter++;
						p1CurrentDirection = Stack.Direction.Left;
					}
				}
				


				if (keyState.IsKeyDown(Keys.Z) && !lastKeyState.IsKeyDown(Keys.Z))
					p1BlockSwitch = true;
				if (keyState.IsKeyDown(Keys.X))
					p1ManualRaise = true;

			}
			else
				multipleMoveCounter = 0;

			if (multipleMoveCounter >= 10) {
				if (!testStack.MoveCursor(p1CurrentDirection) || p1BlockSwitch) // Needs to be patched up so that directions can't flow into one another
					multipleMoveCounter = 0;
			}

			if (p1BlockSwitch) {
				testStack.Switch();
				p1BlockSwitch = false;
			}
			if (p1ManualRaise) {
				testStack.ManualRaise();
				p1ManualRaise = false;
			}

			// P2
			if (!testStack2.CursorCannotBeMoved) {
				if (padState1.IsButtonDown(Buttons.DPadUp) && !padState1.IsButtonDown(Buttons.DPadUp))
					testStack2.MoveCursor(Stack.Direction.Up);
				else if (padState1.IsButtonDown(Buttons.DPadRight) && !padState1.IsButtonDown(Buttons.DPadRight))
					testStack2.MoveCursor(Stack.Direction.Right);
				else if (padState1.IsButtonDown(Buttons.DPadDown) && !padState1.IsButtonDown(Buttons.DPadDown))
					testStack2.MoveCursor(Stack.Direction.Down);
				else if (padState1.IsButtonDown(Buttons.DPadLeft) && !padState1.IsButtonDown(Buttons.DPadLeft))
					testStack2.MoveCursor(Stack.Direction.Left);

				if (padState1.IsButtonDown(Buttons.A) && !lastPadState1.IsButtonDown(Buttons.A))
					p2BlockSwitch = true;
				if (padState1.IsButtonDown(Buttons.B) && !lastPadState1.IsButtonDown(Buttons.B))
					p2BlockSwitch = true;
				if (padState1.IsButtonDown(Buttons.X) && !lastPadState1.IsButtonDown(Buttons.X))
					p2BlockSwitch = true;
				if (padState1.IsButtonDown(Buttons.Y) && !lastPadState1.IsButtonDown(Buttons.Y))
					p2BlockSwitch = true;

				if (padState1.IsButtonDown(Buttons.LeftShoulder) && !lastPadState1.IsButtonDown(Buttons.LeftShoulder))
					p2ManualRaise = true;
				if (padState1.IsButtonDown(Buttons.RightShoulder) && !lastPadState1.IsButtonDown(Buttons.RightShoulder))
					p2ManualRaise = true;
			}

			// P1
			

			// P2
			if (p2BlockSwitch) {
				testStack2.Switch();
				p2BlockSwitch = false;
			}
			if (p2ManualRaise) {
				testStack2.ManualRaise();
				p2ManualRaise = false;
			}


			testStack.Update(gameTime);
			testStack2.Update(gameTime);

			lastKeyState = keyState;
			lastPadState1 = padState1; lastPadState2 = padState2;

            base.Update(gameTime);
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
