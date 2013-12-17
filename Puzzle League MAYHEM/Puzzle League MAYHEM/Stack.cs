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

		public enum Direction {
			None,
			Up,
			Down,
			Left,
			Right
		}

		public enum Characters {
			Netelle, // Cytopia
			Pon, // Puzzle League HQ
			Tilde, // Minneapolis
			Neurotype, // Abandoned Lab
			Scorch, // Dark Alley
			KalTerek, // Windy Roost
			Silvite, // Silent Cave
			Gorrum, // Hazardous Crag
			Poochy, // Vacant Field
			Lip, // Vast Meadow
			Malware, // Central Processor - Michelangelo, Pikachu Virus, Nimda, Melissa
			Thanatos, // Deus Machina
			Corbania, // The Singularity
			Tutorial
		}

		public Cursor PlayerCursor = new Cursor();

		public static bool DarkBlueOn = false;
		public bool CursorCannotBeMoved = false;

        private Frame[] theStack = new Frame[72];
		// [66] [67] [68] [69] [70] [71]
        private Frame[] theRow = new Frame[6];
		// [72] [73] [74] [75] [76] [77]

		public PlayerIndex PlayerNumber = PlayerIndex.One;
		public Characters Character = Characters.Netelle;

		private int initialBlocks;

		private bool stop = false;
		public static float MAX_STOP_TIME = 1.00f;
		private float stopTime = 1.00f; // Length of stop is based on difficulty, but max stopTime should always be 1.0f.

		private bool stackWasJustRaised = false;
		private int raiseAmount = 0;
		private int timeSinceLastBump;
		private int milliPerBump = 50;

		public int GravitySpeed = 12;

		public uint Score;

		public ushort Chain = 0;

		public bool GameOver = false;

		private bool blocksAreFalling = false;
		private bool blocksAreBeingDestroyed = false;
		private bool blocksAreBeingSwitched = false;

		private Texture2D redBlockTexture;
		private Texture2D yellowBlockTexture;
		private Texture2D greenBlockTexture;
		private Texture2D lightBlueBlockTexture;
		private Texture2D purpleBlockTexture;
		private Texture2D darkBlueBlockTexture;
		private Texture2D exclamationBlockTexture;
		private Texture2D errorBlockTexture;

		private Texture2D redPixelTexture;
		private Texture2D yellowPixelTexture;
		private Texture2D greenPixelTexture;
		private Texture2D lightBluePixelTexture;
		private Texture2D purplePixelTexture;
		private Texture2D darkBluePixelTexture;
		private Texture2D exclamationPixelTexture;
		private Texture2D whitePixelTexture;

		private Texture2D stackOverlayTexture;

		private Texture2D minimapTexture;
		private Texture2D borderTexture;
		private Texture2D stopBarTexture;

		private bool minimapOn = true; // Good for more than 2-player multi.
		private int minicursorTimer = 0;

		private int switchTimer = 0;
		private int currentlySwitchedBlock = 0;

		private bool manuallyRaising = false;
		public const int RAISE_COOLDOWN = 3;
		private int raiseCooldownTimer = 0;

		public Vector2 StackLocation = new Vector2(8, 20); // Player 2's position is StackLocation.X + 144.
		public Vector2 MinimapLocation = new Vector2(110, 180);

        public Stack() {
			initialBlocks = 30;
        }

		public Stack(int initBlocks) {
			initialBlocks = initBlocks;
		}

        public void FillStack(int init) { // Stack always starts with 30 blocks in versus. Only one block is ever above the cursor.
			initialBlocks = init;

			for (int x = 0; x < 72; x++) {
				theStack[x].Index = x;
				theStack[x].PositionX = x % 6;
				theStack[x].PositionY = x / 6;
				theStack[x].CannotBeSwitched = false;
			}
			for (int x = 0; x < 6; x++) {
				theRow[x].HeldBlock = GenerateBlock();
				theRow[x].Index = 72 + x;
				theRow[x].PositionX = x % 6;
				theRow[x].PositionY = 12 + x / 6;
				theRow[x].HeldBlock.Location = new Vector2(theStack[x].PositionX, theStack[x].PositionY);
			}

			if (theRow[2].HeldBlock.BlockColor == theRow[1].HeldBlock.BlockColor && // Makes sure you can't get three of the same color in the Row.
				theRow[1].HeldBlock.BlockColor == theRow[0].HeldBlock.BlockColor) {
				theRow[2].HeldBlock.AssignExcept(DarkBlueOn, theRow[2].HeldBlock.BlockColor);
				theRow[2].HeldBlock = GenerateBlock(theRow[2].HeldBlock.BlockColor);
			}

			if (theRow[3].HeldBlock.BlockColor == theRow[2].HeldBlock.BlockColor &&
				theRow[2].HeldBlock.BlockColor == theRow[1].HeldBlock.BlockColor) {
				theRow[3].HeldBlock.AssignExcept(DarkBlueOn, theRow[3].HeldBlock.BlockColor);
				theRow[3].HeldBlock = GenerateBlock(theRow[3].HeldBlock.BlockColor);
			}

			if (theRow[4].HeldBlock.BlockColor == theRow[3].HeldBlock.BlockColor &&
				theRow[3].HeldBlock.BlockColor == theRow[2].HeldBlock.BlockColor) {
				theRow[4].HeldBlock.AssignExcept(DarkBlueOn, theRow[4].HeldBlock.BlockColor);
				theRow[4].HeldBlock = GenerateBlock(theRow[4].HeldBlock.BlockColor);
			}

			if (theRow[5].HeldBlock.BlockColor == theRow[4].HeldBlock.BlockColor &&
				theRow[4].HeldBlock.BlockColor == theRow[3].HeldBlock.BlockColor) {
				theRow[5].HeldBlock.AssignExcept(DarkBlueOn, theRow[5].HeldBlock.BlockColor);
				theRow[5].HeldBlock = GenerateBlock(theRow[5].HeldBlock.BlockColor);
			}

			for (int x = 0; x < 6; x++)
				theRow[x].HeldBlock.AnimationIndex = 5;

			for (int x = 0; x < initialBlocks; x++) { // Actual filling goes here.
				int current = RNG.Next(6);
				
				theStack[current].HeldBlock = GenerateBlock();
				instantGravity(current);
			}

			assignColors(); // After blocks are placed, this makes sure no two of the same color are touching.

			PlayerCursor.Index = 38;
        }

		private void assignColors() {
			for (int x = 70; x >= 0; x--) { // Iterates through the stack backwards starting from the second-to-last block.
				if (theStack[x].HeldBlock != null) {
					if (theStack[x + 1].HeldBlock != null) { // Checks the block to the right.
						if (theStack[x].HeldBlock.BlockColor == theStack[x + 1].HeldBlock.BlockColor) {
							theStack[x].HeldBlock.AssignExcept(DarkBlueOn, theStack[x].HeldBlock.BlockColor);
							theStack[x].HeldBlock = GenerateBlock(theStack[x].HeldBlock.BlockColor);
						}
					}
					if (x < 66 && theStack[x + 6].HeldBlock != null) { // Checks the block below.
						if (theStack[x].HeldBlock.BlockColor == theStack[x + 6].HeldBlock.BlockColor) {
							if (theStack[x].HeldBlock.BlockColor == theStack[x + 6].HeldBlock.BlockColor) {
								theStack[x].HeldBlock.AssignExcept(DarkBlueOn, theStack[x].HeldBlock.BlockColor);
								theStack[x].HeldBlock = GenerateBlock(theStack[x].HeldBlock.BlockColor);
							}
						}
					}
				}
			}
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
                theRow[x].HeldBlock = GenerateBlock();
				theRow[x].HeldBlock.AnimationIndex = 5;
            }

			if (theRow[2].HeldBlock.BlockColor == theRow[1].HeldBlock.BlockColor && // Makes sure you can't get three of the same color in the Row.
				theRow[1].HeldBlock.BlockColor == theRow[0].HeldBlock.BlockColor) {
				theRow[2].HeldBlock.AssignExcept(DarkBlueOn, theRow[2].HeldBlock.BlockColor);
				theRow[2].HeldBlock = GenerateBlock(theRow[2].HeldBlock.BlockColor);
			}

			if (theRow[3].HeldBlock.BlockColor == theRow[2].HeldBlock.BlockColor &&
				theRow[2].HeldBlock.BlockColor == theRow[1].HeldBlock.BlockColor) {
				theRow[3].HeldBlock.AssignExcept(DarkBlueOn, theRow[3].HeldBlock.BlockColor);
				theRow[3].HeldBlock = GenerateBlock(theRow[3].HeldBlock.BlockColor);
			}

			if (theRow[4].HeldBlock.BlockColor == theRow[3].HeldBlock.BlockColor &&
				theRow[3].HeldBlock.BlockColor == theRow[2].HeldBlock.BlockColor) {
				theRow[4].HeldBlock.AssignExcept(DarkBlueOn, theRow[4].HeldBlock.BlockColor);
				theRow[4].HeldBlock = GenerateBlock(theRow[4].HeldBlock.BlockColor);
			}

			if (theRow[5].HeldBlock.BlockColor == theRow[4].HeldBlock.BlockColor &&
				theRow[4].HeldBlock.BlockColor == theRow[3].HeldBlock.BlockColor) {
				theRow[5].HeldBlock.AssignExcept(DarkBlueOn, theRow[5].HeldBlock.BlockColor);
				theRow[5].HeldBlock = GenerateBlock(theRow[5].HeldBlock.BlockColor);
			}
        }

        public Block GenerateBlock() {
			Block returnableBlock = new Block(DarkBlueOn);
			switch (returnableBlock.BlockColor) {
				case Puzzle_League_MAYHEM.Block.BlockColors.Blank:
					returnableBlock.BlockTexture = errorBlockTexture;  returnableBlock.PixelTexture = whitePixelTexture; break;
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
			return returnableBlock;
        }

		public Block GenerateBlock(Block.BlockColors bc) {
			Block returnableBlock = new Block(bc);
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
			return returnableBlock;
		}

		public bool MoveCursor(Direction d) {
			if (!CursorCannotBeMoved) {
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
			else return false;
		}

		public void Switch() {
			if (!theStack[PlayerCursor.Index].CannotBeSwitched && !theStack[PlayerCursor.Index + 1].CannotBeSwitched) {
				if (theStack[PlayerCursor.Index].HeldBlock != null || theStack[PlayerCursor.Index + 1].HeldBlock != null)
					if (switchTimer == 0)
						switchTimer++;
			}
		}

		public void ManualRaise() {
			if (!blocksAreBeingDestroyed && !blocksAreFalling) {
				stopTime = 0.0f;
				manuallyRaising = true;
			}
		}

		private void gravity() { // Blocks fall at 1 block per frame and take 12 frames to start falling.
			for (int x = 65; x >= 0; x--) {
				if (theStack[x].HeldBlock != null) {
					if (theStack[x + 6].HeldBlock == null) {
						theStack[x].CannotBeSwitched = true;
						theStack[x + 6].CannotBeSwitched = true;
						theStack[x].HeldBlock.FallTimer++;
						theStack[x].HeldBlock.IsFalling = true;
					}

					if (theStack[x].HeldBlock.FallTimer > GravitySpeed) {
						theStack[x].CannotBeSwitched = false;
						theStack[x + 6].CannotBeSwitched = false;

						if (!theStack[x].HeldBlock.IsBeingSwitched && !GameOver) {
							if (theStack[x + 6].HeldBlock == null) {
								theStack[x + 6].HeldBlock = theStack[x].HeldBlock;
								theStack[x].HeldBlock = null;

								while (theStack[x - 6].HeldBlock != null) {
									theStack[x].HeldBlock = theStack[x - 6].HeldBlock;
									theStack[x - 6].HeldBlock = null;

									x -= 6;
								}
							}
							else {
								theStack[x].HeldBlock.FallTimer = 0;
								theStack[x].HeldBlock.IsFalling = false;
							}

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
			 * 
			 * x14+ chain - 1800 points
			 * 
			 * Poochy.EXE3 years agoin reply to manaseater 
			 * Any clear made while a chain is active elsewhere nets you a bonus equal
			 * to the value of the most recent hit of the chain. It doesn't add to the
			 * chain; it re-counts the last hit. It's been a game mechanic since the
			 * original Panel de Pon / Tetris Attack, except it was less noticeable
			 * back then because of the bug that made the 14th and subsequent hits in
			 * a chain worth 0 instead of the intended 1800 each.
			*/

		}

		private bool detectBlocks() {
			return false; // Temporary.
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
			errorBlockTexture = theContentManager.Load<Texture2D>("Graphics/Sprites/ErrorBlock");
			whitePixelTexture = theContentManager.Load<Texture2D>("Graphics/Sprites/WhitePixel");

			stackOverlayTexture = theContentManager.Load<Texture2D>("Graphics/Interface/StackOverlay");

			minimapTexture = theContentManager.Load<Texture2D>("Graphics/Interface/MinimapBackdrop");
			borderTexture = theContentManager.Load<Texture2D>("Graphics/Interface/TildeBorder");
			stopBarTexture = theContentManager.Load<Texture2D>("Graphics/Interface/StopBar");

			PlayerCursor.LoadContent(theContentManager);
			PlayerCursor.PositionX = 2;
			PlayerCursor.PositionY = 6;

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
			while (i < 72 && theStack[i].HeldBlock != null && !GameOver) {
				theStack[i].HeldBlock.AnimationIndex = 1;
				i += 6;
			}
		}

        public void Update(GameTime gameTime) {
			blocksAreFalling = false;
			blocksAreBeingSwitched = false;
			blocksAreBeingDestroyed = false;

			stackWasJustRaised = false;

			// STACK RAISING
			timeSinceLastBump += gameTime.ElapsedGameTime.Milliseconds;
			if (timeSinceLastBump > milliPerBump) {
				timeSinceLastBump -= milliPerBump;
				if (!stop && !blocksAreBeingDestroyed && !blocksAreFalling && !blocksAreBeingSwitched && !manuallyRaising && !GameOver) // T
					raiseAmount++;
			}

			if (raiseCooldownTimer > RAISE_COOLDOWN) {
				raiseCooldownTimer = 0;
			}

			if (raiseCooldownTimer > 0) {
				manuallyRaising = false;
				raiseCooldownTimer++;
			}

			if (manuallyRaising)
				raiseAmount++; // Manual raising raises the stack up one pixel per frame.

			if (raiseAmount > 15) {
				if (manuallyRaising) {
					manuallyRaising = false;
					raiseCooldownTimer++;
				}
				stackWasJustRaised = true;
				raiseAmount = 0;
				PlayerCursor.PositionY -= 1; // Cursor has to be forced to move here instead of going through the MoveCursor method.
				if (PlayerCursor.PositionY < 1) {
					PlayerCursor.PositionY = 1;
				}
				else {
					PlayerCursor.Index -= 6;
				}
				NextRow();
			}

			// BLOCK LOCATIONS AND GRAVITY
			gravity();

			for (int x = 0; x < 72; x++) {
				if (x < 6 && theStack[x].HeldBlock == null && theStack[x + 6].HeldBlock == null) { // Unbounces blocks.
					int i = x;
					while (i < 72 && !GameOver) {
						if (theStack[i].HeldBlock != null && theStack[i].HeldBlock.AnimationIndex != 2)
							theStack[i].HeldBlock.AnimationIndex = 0;
						i += 6;
					}
				}

				if (theStack[x].HeldBlock != null) {
					if (x < 12)
						bounceBlocks(x);

					if (theStack[x].HeldBlock.IsFalling)
						blocksAreFalling = true;

					theStack[x].HeldBlock.Location = new Vector2((theStack[x].PositionX * 16) + StackLocation.X, (theStack[x].PositionY * 16) + StackLocation.Y - raiseAmount);

					if (theStack[x].HeldBlock.IsBeingSwitched) {
						theStack[x].HeldBlock.FallTimer = 0;
						theStack[x].HeldBlock.IsFalling = false;
					}
					theStack[x].HeldBlock.Update(gameTime);
				}
			}

			for (int x = 0; x < 6; x++) {
				theRow[x].HeldBlock.Location = new Vector2((theRow[x].PositionX * 16) + StackLocation.X, (theRow[x].PositionY * 16) + StackLocation.Y - raiseAmount);
				theRow[x].HeldBlock.Update(gameTime);
			}

			// BLOCK SWITCHING - There's a problem with switching blocks when a new row is generated. bool stackWasJustRaised is there to maybe be a fix.
			if (switchTimer > 0) {
				switch (switchTimer) {
					case 1:
						if (theStack[PlayerCursor.Index].HeldBlock != null || theStack[PlayerCursor.Index + 1].HeldBlock != null)
							CursorCannotBeMoved = true;
						else
							break;

						blocksAreBeingSwitched = true;
						currentlySwitchedBlock = PlayerCursor.Index;

						if (theStack[currentlySwitchedBlock].HeldBlock != null) {
							theStack[currentlySwitchedBlock].HeldBlock.IsBeingSwitched = true;
							theStack[currentlySwitchedBlock].HeldBlock.FallTimer = 0;
							theStack[currentlySwitchedBlock].HeldBlock.IsFalling = false;
							theStack[currentlySwitchedBlock].HeldBlock.Location = new Vector2(theStack[currentlySwitchedBlock].HeldBlock.Location.X + 4,
								theStack[currentlySwitchedBlock].HeldBlock.Location.Y);
							theStack[currentlySwitchedBlock].HeldBlock.LayerDepth = 0.55f;
						}
						if (theStack[currentlySwitchedBlock + 1].HeldBlock != null) {
							theStack[currentlySwitchedBlock + 1].HeldBlock.IsBeingSwitched = true;
							theStack[currentlySwitchedBlock + 1].HeldBlock.FallTimer = 0;
							theStack[currentlySwitchedBlock + 1].HeldBlock.IsFalling = false;
							theStack[currentlySwitchedBlock + 1].HeldBlock.Location = new Vector2(theStack[currentlySwitchedBlock + 1].HeldBlock.Location.X - 4,
								theStack[currentlySwitchedBlock + 1].HeldBlock.Location.Y);
						}

						switchTimer++;
						break;
					case 2:
						blocksAreBeingSwitched = true;
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
							theStack[currentlySwitchedBlock].HeldBlock.IsBeingSwitched = false;
							theStack[currentlySwitchedBlock].HeldBlock.Location = new Vector2(theStack[currentlySwitchedBlock].HeldBlock.Location.X + 16,
								theStack[currentlySwitchedBlock].HeldBlock.Location.Y);
							theStack[currentlySwitchedBlock].HeldBlock.FallTimer = 0;
							theStack[currentlySwitchedBlock].HeldBlock.IsFalling = false;
						}
						if (theStack[currentlySwitchedBlock + 1].HeldBlock != null) {
							theStack[currentlySwitchedBlock + 1].HeldBlock.IsBeingSwitched = false;
							theStack[currentlySwitchedBlock + 1].HeldBlock.Location = new Vector2(theStack[currentlySwitchedBlock + 1].HeldBlock.Location.X - 16,
								theStack[currentlySwitchedBlock + 1].HeldBlock.Location.Y);
							theStack[currentlySwitchedBlock + 1].HeldBlock.FallTimer = 0;
							theStack[currentlySwitchedBlock + 1].HeldBlock.IsFalling = false;
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

			// STOP BAR
			if (stopTime > MAX_STOP_TIME)
				stopTime = MAX_STOP_TIME;
			if (stopTime > 0.000f) {
				stop = true;
				if (!blocksAreFalling && !blocksAreBeingDestroyed)
					stopTime -= 0.002f; // Speed of stop bar set here.
			}
			else
				stop = false;
			if (stopTime < 0.000f)
				stopTime = 0.000f;

			// CURSOR
			PlayerCursor.Update(gameTime);
			PlayerCursor.Location = new Vector2(PlayerCursor.Location.X + StackLocation.X, PlayerCursor.Location.Y + StackLocation.Y - raiseAmount);

			
			// DEFEAT
			for (int x = 0; x < 6; x++) {
				if (theStack[x].HeldBlock != null)
					GameOver = true;
			}

			if (GameOver) {
				for (int x = 0; x < 72; x++) {
					if (theStack[x].HeldBlock != null)
						theStack[x].HeldBlock.AnimationIndex = 4;

				}
			}
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

			//spriteBatch.Draw(stopBarTexture, new Rectangle( // Stop bar draws at 0.8, this wipes right to left
			//    (int)StackLocation.X - 3, (int)StackLocation.Y - 16, (int)(stopBarTexture.Width * stopTime), stopBarTexture.Height
			//    ), new Rectangle(
			//    0, 0, (int)(stopBarTexture.Width * stopTime), stopBarTexture.Height), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.8f);

			//spriteBatch.Draw(stopBarTexture, new Rectangle( // Stop bar draws at 0.8, this wipes left to right
			//    (int)StackLocation.X - 3 + (int)(stopBarTexture.Width - (stopBarTexture.Width * stopTime)), (int)StackLocation.Y - 16, (int)(stopBarTexture.Width * stopTime), stopBarTexture.Height
			//    ), new Rectangle(
			//    (int)(stopBarTexture.Width - (stopBarTexture.Width * stopTime)), 0, (int)(stopBarTexture.Width * stopTime), stopBarTexture.Height), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.8f);

			spriteBatch.Draw(stopBarTexture, new Rectangle( // Stop bar draws at 0.8, this wipes from the outside in
			    (int)StackLocation.X - 3 + (int)((stopBarTexture.Width / 2) - ((stopBarTexture.Width / 2) * stopTime)), (int)StackLocation.Y - 16, (int)(stopBarTexture.Width * stopTime), stopBarTexture.Height
			    ), new Rectangle(
				(int)((stopBarTexture.Width / 2) - ((stopBarTexture.Width / 2) * stopTime)), 0, (int)(stopBarTexture.Width * stopTime), stopBarTexture.Height), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.8f);

			// MINIMAP
			if (minimapOn) {

				spriteBatch.Draw(minimapTexture, // Minimap box draws at 0.8
					new Rectangle((int)MinimapLocation.X, (int)MinimapLocation.Y, minimapTexture.Width, minimapTexture.Height),
					null, Color. White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.8f);

				int minimapBump = 0;
				if (raiseAmount > 7)
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
			// DEBUG STUFF
			if (PuzzleLeagueGame.DebugModeOn)
				spriteBatch.Draw(stackOverlayTexture, new Rectangle((int)StackLocation.X, (int)StackLocation.Y - raiseAmount, (int)stackOverlayTexture.Width, (int)stackOverlayTexture.Height), Color.White);

        }
    }
}
