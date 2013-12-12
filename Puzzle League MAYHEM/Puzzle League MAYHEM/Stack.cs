using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Puzzle_League_MAYHEM {
    class Stack {
        public struct Frame {
            public int Index;
            public int PositionX;
            public int PositionY;
			public bool CannotBeSwitched;
            public Block HeldBlock;
        }

		public Cursor PlayerCursor = new Cursor();

		public static bool DarkBlueOn = false;
		public bool CursorCannotBeMoved = false;

        private Frame[] theStack = new Frame[72];
		// [66] [67] [68] [69] [70] [71]
        private Frame[] theRow = new Frame[6];
		// [72] [73] [74] [75] [76] [77]

		private int initialBlocks;

		private bool stop = false;
		public static float MAX_STOP_TIME = 1.00f;
		private float stopTime = 1.00f;

		private int raiseAmount = 0;
		private int timeSinceLastBump;
		private int milliPerBump = 800;

		public uint Score;

		private bool blocksAreFalling = false;
		private bool blocksAreBeingDestroyed = false;

		private Texture2D redBlockTexture;
		private Texture2D yellowBlockTexture;
		private Texture2D greenBlockTexture;
		private Texture2D lightBlueBlockTexture;
		private Texture2D purpleBlockTexture;
		private Texture2D darkBlueBlockTexture;
		private Texture2D exclamationBlockTexture;

		private Texture2D redPixelTexture;
		private Texture2D yellowPixelTexture;
		private Texture2D greenPixelTexture;
		private Texture2D lightBluePixelTexture;
		private Texture2D purplePixelTexture;
		private Texture2D darkBluePixelTexture;
		private Texture2D exclamationPixelTexture;
		private Texture2D whitePixelTexture;

		private Texture2D minimapTexture;
		private Texture2D borderTexture;
		private Texture2D stopBarTexture;

		private bool minimapOn = true; // Good for more than 2-player multi.
		private int minicursorTimer = 0;

		private int switchTimer = 0;
		private int currentlySwitchedBlock = 0;

		private bool manuallyRaising = false;

		public Vector2 StackLocation = new Vector2(8, 20); // Player 2's position is StackLocation.X + 144.
		public Vector2 MinimapLocation = new Vector2(110, 180);

        public Stack() {
			initialBlocks = 30;
        }

		public Stack(int initBlocks) {
			initialBlocks = initBlocks;
		}

        public void FillStack(int init) { // Stack always starts with 30 blocks in versus.
			initialBlocks = init;

			for (int x = 0; x < 72; x++) {
				theStack[x].Index = x;
				theStack[x].PositionX = x % 6;
				theStack[x].PositionY = x / 6;
				theStack[x].CannotBeSwitched = false;
			}
			for (int x = 0; x < 6; x++) {
				theRow[x].HeldBlock = GenerateBlock(theRow[x]);
				theRow[x].Index = 72 + x;
				theRow[x].PositionX = x % 6;
				theRow[x].PositionY = 12 + x / 6;
				theRow[x].HeldBlock.Location = new Vector2(theStack[x].PositionX, theStack[x].PositionY);
			}

			for (int x = 0; x < initialBlocks; x++) {
				int current = RNG.Next(6);
				theStack[current].HeldBlock = GenerateBlock(theStack[x]);
				instantGravity(current);
			}

			PlayerCursor.Index = 32;
        }

        public void NextRow() {
            for (int x = 0; x < 66; x++)
                theStack[x].HeldBlock = theStack[x + 6].HeldBlock;

			theStack[66].HeldBlock = new Block(theRow[0].HeldBlock);
			theStack[67].HeldBlock = new Block(theRow[1].HeldBlock);
			theStack[68].HeldBlock = new Block(theRow[2].HeldBlock);
			theStack[69].HeldBlock = new Block(theRow[3].HeldBlock);
			theStack[70].HeldBlock = new Block(theRow[4].HeldBlock);
			theStack[71].HeldBlock = new Block(theRow[5].HeldBlock);

            GenerateNextRow();
        }

        public void GenerateNextRow() {
            for (int x = 0; x < 6; x++){
                theRow[x].HeldBlock = GenerateBlock(new Frame());
            }
        }

        public Block GenerateBlock(Frame f) {
			Block returnableBlock = new Block(DarkBlueOn);
			switch (returnableBlock.BlockColor) {
				case Puzzle_League_MAYHEM.Block.BlockColors.Red:
					returnableBlock.BlockTexture = redBlockTexture; returnableBlock.PixelTexture = redPixelTexture; break;
				case Puzzle_League_MAYHEM.Block.BlockColors.Yellow:
					returnableBlock.BlockTexture = yellowBlockTexture; returnableBlock.PixelTexture = yellowPixelTexture; break;
				case Puzzle_League_MAYHEM.Block.BlockColors.Green:
					returnableBlock.BlockTexture = greenBlockTexture; returnableBlock.PixelTexture = greenPixelTexture; break;
				case Puzzle_League_MAYHEM.Block.BlockColors.LightBlue:
					returnableBlock.BlockTexture = lightBlueBlockTexture; returnableBlock.PixelTexture = lightBluePixelTexture; break;
				case Puzzle_League_MAYHEM.Block.BlockColors.Purple:
					returnableBlock.BlockTexture = purpleBlockTexture; returnableBlock.PixelTexture = purplePixelTexture; break;
				case Puzzle_League_MAYHEM.Block.BlockColors.DarkBlue:
					returnableBlock.BlockTexture = darkBlueBlockTexture; returnableBlock.PixelTexture = darkBluePixelTexture; break;
				default: returnableBlock.BlockTexture = exclamationBlockTexture; returnableBlock.PixelTexture = exclamationPixelTexture; break;
			}
			returnableBlock.SpriteSheet = returnableBlock.BlockTexture;
			f.HeldBlock = returnableBlock;
			return returnableBlock;
        }

		public enum Direction {
			Up,
			Down,
			Left,
			Right
		}

		public bool MoveCursor(Direction d) {
			switch (d) { // Clockwise.
				case Direction.Up: // Up
					PlayerCursor.PositionY -= 1;
					if (PlayerCursor.PositionY < 1) {
						PlayerCursor.PositionY = 1;
						return false;
					}
					else {
						PlayerCursor.Index -= 6;
						return true;
					}
				case Direction.Right: // Right
					PlayerCursor.PositionX += 1;
					if (PlayerCursor.PositionX > 4) {
						PlayerCursor.PositionX = 4;
						return false;
					}
					else {
						PlayerCursor.Index += 1;
						return true;
					}
				case Direction.Down: // Down
					PlayerCursor.PositionY += 1;
					if (PlayerCursor.PositionY > 11) {
						PlayerCursor.PositionY = 11;
						return false;
					}
					else {
						PlayerCursor.Index += 6;
						return true;
					}
				case Direction.Left: // Left
					PlayerCursor.PositionX -= 1;
					if (PlayerCursor.PositionX < 0) {
						PlayerCursor.PositionX = 0;
						return false;
					}
					else {
						PlayerCursor.Index -= 1;
						return true;
					}
				default: return false;
			}
		}

		public void Switch() {
			if ((theStack[PlayerCursor.Index].HeldBlock != null && !theStack[PlayerCursor.Index].CannotBeSwitched) ||
				(theStack[PlayerCursor.Index + 1].HeldBlock != null  && !theStack[PlayerCursor.Index + 1].CannotBeSwitched)) {
				if (switchTimer == 0)
					switchTimer++;
			}
		}

		public void ManualRaise() {
			if (!blocksAreBeingDestroyed && !blocksAreFalling) {
				CursorCannotBeMoved = true;
				stopTime = 0.0f;
				manuallyRaising = true;
			}
		}

		private void gravity(int x) { // Blocks fall at 1 block per frame and take 12 frames to start falling.
			if (x < 66) {
				if (theStack[x + 6].HeldBlock == null) {
					theStack[x].HeldBlock.FallTimer++;
					theStack[x].HeldBlock.Falling = true;
					theStack[x].CannotBeSwitched = true;
					theStack[x + 6].CannotBeSwitched = true;
				}

				if (theStack[x].HeldBlock.FallTimer >= 12) {
					if (theStack[x + 6].HeldBlock == null) {
						theStack[x + 6].HeldBlock = theStack[x].HeldBlock;
						theStack[x].HeldBlock = null;

						while (x - 6 >= 0 && theStack[x - 6].HeldBlock != null) {
							if (theStack[x - 6].HeldBlock.FallTimer == 0) {
								theStack[x].HeldBlock = theStack[x - 6].HeldBlock;
								theStack[x - 6].HeldBlock = null;
							}

							x -= 6;
						}

						theStack[x].CannotBeSwitched = false;
						theStack[x + 6].CannotBeSwitched = false;
					}
					else {
						theStack[x].HeldBlock.FallTimer = 0;
						theStack[x].HeldBlock.Falling = false;

						while (x - 6 >= 0 && theStack[x - 6].HeldBlock != null) {
							theStack[x].HeldBlock.FallTimer = 0;
							theStack[x].HeldBlock.Falling = false;
							// theStack[x].HeldBlock.AnimationIndex = 2;
							x -= 6;
						}
					}
				}

			}
		}

		private void instantGravity(int x) {
			if (x < 66) {
				if (theStack[x + 6].HeldBlock == null) {
					while (x < 66 && theStack[(x + 6)].HeldBlock == null) {
						theStack[x + 6].HeldBlock = theStack[x].HeldBlock;
						theStack[x].HeldBlock = null;
						x += 6;
					}
				}
			}
		}

		private void addScore(uint scoreToAdd) {
			Score += scoreToAdd;

			/* Score table
			 *
			 * Every time a block is destroyed, regardless of combo or chain, it gives 10 points.
			 * You inexplicably gain 1 point for raising the stack.
			 * 4 combo - 20 points
			 * 5 combo - 30 points
			 * 6 combo - 50 points
			 * 7 combo - 60 points
			 * 8 combo - 70 points
			 * x2 chain - 50 points
			 * x3 chain - 80 points
			*/

		}

		public void LoadContent(ContentManager theContentManager) {
			redBlockTexture= theContentManager.Load<Texture2D>("Graphics/Sprites/RedBlock");
			redPixelTexture = theContentManager.Load<Texture2D>("Graphics/Sprites/RedPixel");
			yellowBlockTexture = theContentManager.Load<Texture2D>("Graphics/Sprites/YellowBlock");
			yellowPixelTexture = theContentManager.Load<Texture2D>("Graphics/Sprites/YellowPixel");
			greenBlockTexture = theContentManager.Load<Texture2D>("Graphics/Sprites/GreenBlock");
			greenPixelTexture = theContentManager.Load<Texture2D>("Graphics/Sprites/GreenPixel");
			lightBlueBlockTexture = theContentManager.Load<Texture2D>("Graphics/Sprites/LightBlueBlock");
			lightBluePixelTexture = theContentManager.Load<Texture2D>("Graphics/Sprites/LightBluePixel");
			purpleBlockTexture = theContentManager.Load<Texture2D>("Graphics/Sprites/PurpleBlock");
			purplePixelTexture = theContentManager.Load<Texture2D>("Graphics/Sprites/PurplePixel");
			darkBlueBlockTexture = theContentManager.Load<Texture2D>("Graphics/Sprites/DarkBlueBlock");
			darkBluePixelTexture = theContentManager.Load<Texture2D>("Graphics/Sprites/DarkBluePixel");
			exclamationBlockTexture = theContentManager.Load<Texture2D>("Graphics/Sprites/ExclamationBlock");
			exclamationPixelTexture = theContentManager.Load<Texture2D>("Graphics/Sprites/GrayPixel");
			whitePixelTexture = theContentManager.Load<Texture2D>("Graphics/Sprites/WhitePixel");

			minimapTexture = theContentManager.Load<Texture2D>("Graphics/Interface/MinimapBackdrop");
			borderTexture = theContentManager.Load<Texture2D>("Graphics/Interface/TildeBorder");
			stopBarTexture = theContentManager.Load<Texture2D>("Graphics/Interface/StopBar");

			PlayerCursor.LoadContent(theContentManager);
			PlayerCursor.PositionX = 2;
			PlayerCursor.PositionY = 5;

			PlayerCursor.Location = new Vector2(PlayerCursor.Location.X + StackLocation.X, PlayerCursor.Location.Y + StackLocation.Y);

			FillStack(initialBlocks);

			for (int x = 0; x < 72; x++) {
				if (theStack[x].HeldBlock != null)
					theStack[x].HeldBlock.LoadContent(theContentManager);
			}
			for (int x = 0; x < 6; x++)
				theRow[x].HeldBlock.LoadContent(theContentManager);

		}

		private void bounceBlocks(int i) {
			for (int x = 0; x < 6; x++) {
				if (x % i == 0) {
					for (int y = 0; y < 72; y +=6) {
						theStack[x].HeldBlock.AnimationIndex = 1;
					}
				}
			}
		}

        public void Update(GameTime gameTime) {
			// STOP BAR
			if (stopTime > MAX_STOP_TIME)
				stopTime = MAX_STOP_TIME;
			if (stopTime > 0.000f) {
				stop = true;
				stopTime -= 0.002f;
			}
			else
				stop = false;
			if (stopTime < 0.000f)
				stopTime = 0.000f;

			// BLOCK LOCATIONS AND GRAVITY
			for (int x = 0; x < 72; x++) {
				if (theStack[x].HeldBlock != null) {
					theStack[x].HeldBlock.Location = new Vector2((theStack[x].PositionX * 16) + StackLocation.X, (theStack[x].PositionY * 16) + StackLocation.Y - raiseAmount);
					if (theStack[x].HeldBlock.Falling == true)
						blocksAreFalling = true;
					theStack[x].HeldBlock.Update(gameTime);
					gravity(x);
				}
			}

			for (int x = 0; x < 6; x++) {
				theRow[x].HeldBlock.Location = new Vector2((theRow[x].PositionX * 16) + StackLocation.X, (theRow[x].PositionY * 16) + StackLocation.Y - raiseAmount);
				theRow[x].HeldBlock.Update(gameTime);
			}

			// BLOCK SWITCHING
			if (switchTimer > 0) {
				switch (switchTimer) {
					case 1:
						CursorCannotBeMoved = true;

						if (theStack[PlayerCursor.Index].HeldBlock != null) {
							theStack[PlayerCursor.Index].HeldBlock.Location = new Vector2(theStack[PlayerCursor.Index].HeldBlock.Location.X + 4,
								theStack[PlayerCursor.Index].HeldBlock.Location.Y);
							theStack[PlayerCursor.Index].HeldBlock.LayerDepth = 0.55f;
						}
						if (theStack[PlayerCursor.Index + 1].HeldBlock != null) {
							theStack[PlayerCursor.Index + 1].HeldBlock.Location = new Vector2(theStack[PlayerCursor.Index + 1].HeldBlock.Location.X - 4,
								theStack[PlayerCursor.Index + 1].HeldBlock.Location.Y);
						}
						currentlySwitchedBlock = PlayerCursor.Index;
						switchTimer++;
						break;
					case 2:
						CursorCannotBeMoved = false;
						if (theStack[currentlySwitchedBlock].HeldBlock != null) {
							theStack[currentlySwitchedBlock].HeldBlock.Location = new Vector2(theStack[currentlySwitchedBlock].HeldBlock.Location.X + 8,
							theStack[currentlySwitchedBlock].HeldBlock.Location.Y);
						}
						if (theStack[currentlySwitchedBlock + 1].HeldBlock != null) {
							theStack[currentlySwitchedBlock + 1].HeldBlock.Location = new Vector2(theStack[currentlySwitchedBlock + 1].HeldBlock.Location.X - 8,
								theStack[currentlySwitchedBlock + 1].HeldBlock.Location.Y);
						}
						switchTimer++;
						break;
					case 3:
						if (theStack[currentlySwitchedBlock].HeldBlock != null) {
							theStack[currentlySwitchedBlock].HeldBlock.Location = new Vector2(theStack[currentlySwitchedBlock].HeldBlock.Location.X + 12,
								theStack[currentlySwitchedBlock].HeldBlock.Location.Y);
						}
						if (theStack[currentlySwitchedBlock + 1].HeldBlock != null) {
							theStack[currentlySwitchedBlock + 1].HeldBlock.Location = new Vector2(theStack[currentlySwitchedBlock + 1].HeldBlock.Location.X - 12,
								theStack[currentlySwitchedBlock + 1].HeldBlock.Location.Y);
						}
						switchTimer++;
						break;
					case 4:
						if (theStack[currentlySwitchedBlock].HeldBlock != null) {
							theStack[currentlySwitchedBlock].HeldBlock.Location = new Vector2(theStack[currentlySwitchedBlock].HeldBlock.Location.X + 16,
								theStack[currentlySwitchedBlock].HeldBlock.Location.Y);
						}
						if (theStack[currentlySwitchedBlock + 1].HeldBlock != null) {
							theStack[currentlySwitchedBlock + 1].HeldBlock.Location = new Vector2(theStack[currentlySwitchedBlock + 1].HeldBlock.Location.X - 16,
								theStack[currentlySwitchedBlock + 1].HeldBlock.Location.Y);
						}

						if (theStack[currentlySwitchedBlock].HeldBlock != null)
							theStack[currentlySwitchedBlock].HeldBlock.LayerDepth = 0.50f;
						if (theStack[currentlySwitchedBlock + 1].HeldBlock != null)
							theStack[currentlySwitchedBlock + 1].HeldBlock.LayerDepth = 0.50f;

						Block tempBlock;
						tempBlock = theStack[currentlySwitchedBlock].HeldBlock;
						theStack[currentlySwitchedBlock].HeldBlock = theStack[currentlySwitchedBlock + 1].HeldBlock;
						theStack[currentlySwitchedBlock + 1].HeldBlock = tempBlock;
						switchTimer = 0;
						break;
					default:
						break;
				}
			}

			// STACK RAISING
			timeSinceLastBump += gameTime.ElapsedGameTime.Milliseconds;
					if (timeSinceLastBump > milliPerBump) {
						timeSinceLastBump -= milliPerBump;
						if (!stop && !blocksAreBeingDestroyed && !blocksAreFalling && !manuallyRaising)
							raiseAmount++;
					}

			if (manuallyRaising)
				raiseAmount++; // Manual raising raises the stack up one pixel per frame.

			if (raiseAmount > 15) {
				if (manuallyRaising) {
					manuallyRaising = false;
					CursorCannotBeMoved = false;
				}
				raiseAmount = 0;
				MoveCursor(Direction.Up);
				NextRow();
			}

			// ANIMATION
			for (int x = 0; x < 12; x++) {
				if (theStack[x].HeldBlock != null)
					theStack[x].HeldBlock.AnimationIndex = 0; // Change to 1
			}


			// CURSOR
			PlayerCursor.Update(gameTime);
			PlayerCursor.Location = new Vector2(PlayerCursor.Location.X + StackLocation.X, PlayerCursor.Location.Y + StackLocation.Y - raiseAmount);

			
				
        }

        public void Draw(SpriteBatch spriteBatch) { // Main blocks draw at 0.5
			foreach (Frame f in theStack) {
				if (f.HeldBlock != null) {
					f.HeldBlock.Draw(spriteBatch);
				}
			}
			foreach (Frame f in theRow) { // Row blocks draw at 0.8
				f.HeldBlock.LayerDepth = 0.8f;
				f.HeldBlock.Draw(spriteBatch);
			}

			PlayerCursor.Draw(spriteBatch);

			spriteBatch.Draw(borderTexture, new Rectangle( // Border draws at 0.7
				(int)StackLocation.X - 8, (int)StackLocation.Y - 16, borderTexture.Width, borderTexture.Height
				), null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.7f);

			spriteBatch.Draw(stopBarTexture, new Rectangle( // Stop bar draws at 0.8
				(int)(StackLocation.X - 3), (int)StackLocation.Y - 16, (int)(stopBarTexture.Width * stopTime), stopBarTexture.Height
				), new Rectangle(
				0, 0, (int)(stopBarTexture.Width * stopTime), stopBarTexture.Height), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.8f);

			// MINIMAP
			if (minimapOn) {

				spriteBatch.Draw(minimapTexture, // Minimap box draws at 0.8
					new Rectangle((int)MinimapLocation.X, (int)MinimapLocation.Y, minimapTexture.Width, minimapTexture.Height),
					null, Color. White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.8f);

				int minimapBump = 0;
				if (raiseAmount > 5)
					minimapBump = 1;

				foreach (Frame f in theStack) { // Minimap pixels draw at 0.5
					if (f.HeldBlock != null) {
						spriteBatch.Draw(f.HeldBlock.PixelTexture,
							new Rectangle((int)MinimapLocation.X + (f.PositionX * 2), (int)MinimapLocation.Y + (f.PositionY * 2) - minimapBump, 2, 2),
							null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.5f);
					}
				}

				bool drawMinicursor = false;

				minicursorTimer++;
				if (minicursorTimer > 15) {
					drawMinicursor = true;
				}
				if (minicursorTimer > 30) {
					drawMinicursor = false;
					minicursorTimer = 0;
				}

				if (drawMinicursor) {
					spriteBatch.Draw(whitePixelTexture, // Minimap cursor draws at 0.3
						new Rectangle((int)MinimapLocation.X + (PlayerCursor.PositionX * 2), (int)MinimapLocation.Y + (PlayerCursor.PositionY * 2) - minimapBump, 4, 2),
						null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.3f);
				}

			}

        }
    }
}
