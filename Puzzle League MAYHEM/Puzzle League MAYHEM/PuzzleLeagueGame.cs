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
		Endless100Background bg = new Endless100Background();
		Texture2D debugTexture;
		public static bool DebugModeOn = false;
		bool MoveDebugTexture = false;

		int multipleMoveCounter = 0;

		Stack.Direction p1CurrentDirection;

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

			// TESTING
			p1CurrentDirection = Stack.Direction.None;

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
			bg.LoadContent(theContentManager);

			 debugTexture = theContentManager.Load<Texture2D>("Graphics/Interface/debug48by12");
        }

        protected override void UnloadContent() {
            Content.Unload();
        }

        protected override void Update(GameTime gameTime) {
            keyState = Keyboard.GetState();
			padState1 = GamePad.GetState(PlayerIndex.One);
			padState2 = GamePad.GetState(PlayerIndex.Two);

			bg.Update(gameTime);

			// TEST INPUT
			
			bool p1BlockSwitch = false;
			bool p1ManualRaise = false;
			bool p2BlockSwitch = false;
			bool p2ManualRaise = false;

            if (keyState.IsKeyDown(Keys.Escape))
                this.Exit();


			// FRAME ADVANCE DEBUG MODE
			if (keyState.IsKeyDown(Keys.NumPad7))
				DebugModeOn = true;

			if (DebugModeOn && keyState.IsKeyDown(Keys.NumPad8) && !lastKeyState.IsKeyDown(Keys.NumPad8))
				goto UpdateCode;

			if (keyState.IsKeyDown(Keys.NumPad9))
				DebugModeOn = false;

			if (keyState.IsKeyDown(Keys.NumPad4) && !lastKeyState.IsKeyDown(Keys.NumPad4))
				MoveDebugTexture = !MoveDebugTexture;

			if (DebugModeOn)
				goto SkipUpdate;

			UpdateCode:

			if (keyState.IsKeyDown(Keys.NumPad1) && !lastKeyState.IsKeyDown(Keys.NumPad1)) {
				testStack = new Stack();
				testStack.LoadContent(theContentManager);
			}
			if (padState1.IsButtonDown(Buttons.Back) && !lastPadState1.IsButtonDown(Buttons.Back)) {
				testStack2 = new Stack();
				testStack2.LoadContent(theContentManager);
				testStack2.StackLocation.X += 144;
				testStack2.MinimapLocation = new Vector2(134, 180);
			}

			// P1
			if (!testStack.CursorCannotBeMoved) {
				if (keyState.IsKeyDown(Keys.Up)) {
					if (!lastKeyState.IsKeyDown(Keys.Up) && testStack.MoveCursor(Stack.Direction.Up)) {
						multipleMoveCounter = 0;
						goto MovementEndsKeyboard;
					}
					else {
						multipleMoveCounter++;
						if (p1CurrentDirection != Stack.Direction.Up)
							multipleMoveCounter = 0;
						p1CurrentDirection = Stack.Direction.Up;
					}		
				}
				else if (keyState.IsKeyDown(Keys.Down)) {
					if (!lastKeyState.IsKeyDown(Keys.Down) && testStack.MoveCursor(Stack.Direction.Down)) {
						multipleMoveCounter = 0;
					}
					else {
						multipleMoveCounter++;
						if (p1CurrentDirection != Stack.Direction.Down)
							multipleMoveCounter = 0;
						p1CurrentDirection = Stack.Direction.Down;
					}
				}
				if (keyState.IsKeyDown(Keys.Right)) {
					if (!lastKeyState.IsKeyDown(Keys.Right) && testStack.MoveCursor(Stack.Direction.Right)) {
						multipleMoveCounter = 0;
						goto MovementEndsKeyboard;
					}
					else {
						multipleMoveCounter++;
						if (p1CurrentDirection != Stack.Direction.Right)
							multipleMoveCounter = 0;
						p1CurrentDirection = Stack.Direction.Right;
					}
				}
				else if (keyState.IsKeyDown(Keys.Left)) {
					if (!lastKeyState.IsKeyDown(Keys.Left) && testStack.MoveCursor(Stack.Direction.Left)) {
						multipleMoveCounter = 0;
					}
					else {
						multipleMoveCounter++;
						if (p1CurrentDirection != Stack.Direction.Left)
							multipleMoveCounter = 0;

						p1CurrentDirection = Stack.Direction.Left;
					}
				}

				if (keyState.IsKeyUp(Keys.Up) && keyState.IsKeyUp(Keys.Down) && keyState.IsKeyUp(Keys.Left) && keyState.IsKeyUp(Keys.Right))
					p1CurrentDirection = Stack.Direction.None;

			MovementEndsKeyboard:

				if (keyState.IsKeyDown(Keys.Z) && !lastKeyState.IsKeyDown(Keys.Z))
					p1BlockSwitch = true;
				else if (keyState.IsKeyDown(Keys.X))
					p1ManualRaise = true;

			}

			if (multipleMoveCounter >= 10) {
				if (!testStack.MoveCursor(p1CurrentDirection))
					multipleMoveCounter = 0;
			}

			if (p1BlockSwitch) {
				testStack.Switch();
				p1BlockSwitch = false; // Resets the condition for the next frame.
			}
			if (p1ManualRaise) {
				testStack.ManualRaise();
				p1ManualRaise = false; // Resets the condition for the next frame.
			}

			// P2
			if (!testStack2.CursorCannotBeMoved) {
				if (padState1.IsButtonDown(Buttons.DPadUp) && !lastPadState1.IsButtonDown(Buttons.DPadUp))
					testStack2.MoveCursor(Stack.Direction.Up);
				else if (padState1.IsButtonDown(Buttons.DPadRight) && !lastPadState1.IsButtonDown(Buttons.DPadRight))
					testStack2.MoveCursor(Stack.Direction.Right);
				else if (padState1.IsButtonDown(Buttons.DPadDown) && !lastPadState1.IsButtonDown(Buttons.DPadDown))
					testStack2.MoveCursor(Stack.Direction.Down);
				else if (padState1.IsButtonDown(Buttons.DPadLeft) && !lastPadState1.IsButtonDown(Buttons.DPadLeft))
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

			SkipUpdate:

			lastKeyState = keyState;
			lastPadState1 = padState1; lastPadState2 = padState2;

			base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
			GraphicsDevice.SetRenderTarget(targetScreen);
            GraphicsDevice.Clear(Color.DarkRed);
			

			spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
			bg.Draw(spriteBatch);

			testStack.Draw(spriteBatch);
			testStack2.Draw(spriteBatch);

			if (DebugModeOn) {
				if (MoveDebugTexture) {
					spriteBatch.Draw(debugTexture, new Rectangle(0, 0, debugTexture.Width, debugTexture.Height), null,
 						Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
				}
				else {
					spriteBatch.Draw(debugTexture, new Rectangle(targetScreen.Width, 0, debugTexture.Width, debugTexture.Height), null,
						Color.White, 0.0f, new Vector2(debugTexture.Width, 0), SpriteEffects.None, 0.0f);
				}
			}

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
