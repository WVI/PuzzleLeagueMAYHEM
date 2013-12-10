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
            public Block HeldBlock;
        }

		public Cursor PlayerCursor = new Cursor();

		public static bool DarkBlueOn = false;
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
		private int milliPerBump = 500;

		public uint Score;

		private bool blocksAreBeingDestroyed = false;

		private Texture2D redBlockTexture;
		private Texture2D yellowBlockTexture;
		private Texture2D greenBlockTexture;
		private Texture2D lightBlueBlockTexture;
		private Texture2D purpleBlockTexture;
		private Texture2D darkBlueBlockTexture;
		private Texture2D exclamationBlockTexture;

		private Texture2D borderTexture;
		private Texture2D stopBarTexture;

		public Vector2 StackLocation = new Vector2(5,20); // Player 2's position is StackLocation.X + 150.

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
				gravity(current, theStack[current].HeldBlock);
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
				case Puzzle_League_MAYHEM.Block.BlockColors.Red: returnableBlock.BlockTexture = redBlockTexture; break;
				case Puzzle_League_MAYHEM.Block.BlockColors.Yellow: returnableBlock.BlockTexture = yellowBlockTexture; break;
				case Puzzle_League_MAYHEM.Block.BlockColors.Green: returnableBlock.BlockTexture = greenBlockTexture; break;
				case Puzzle_League_MAYHEM.Block.BlockColors.LightBlue: returnableBlock.BlockTexture = lightBlueBlockTexture; break;
				case Puzzle_League_MAYHEM.Block.BlockColors.Purple: returnableBlock.BlockTexture = purpleBlockTexture; break;
				case Puzzle_League_MAYHEM.Block.BlockColors.DarkBlue: returnableBlock.BlockTexture = darkBlueBlockTexture; break;
				default: returnableBlock.BlockTexture = exclamationBlockTexture; break;
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

		public void MoveCursor(Direction d) {
			switch (d) { // Clockwise.
				case Direction.Up: // Up
					PlayerCursor.PositionY -= 1;
					if (PlayerCursor.PositionY < 1)
						PlayerCursor.PositionY = 1;
					else {
						PlayerCursor.Index -= 6;
					}
					break;
				case Direction.Right: // Right
					PlayerCursor.PositionX += 1;
					if (PlayerCursor.PositionX > 4)
						PlayerCursor.PositionX = 4;
					else {
						PlayerCursor.Index += 1;
					}
					break;
				case Direction.Down: // Down
					PlayerCursor.PositionY += 1;
					if (PlayerCursor.PositionY > 11)
						PlayerCursor.PositionY = 11;
					else {
						PlayerCursor.Index += 6;
					}
					break;
				case Direction.Left: // Left
					PlayerCursor.PositionX -= 1;
					if (PlayerCursor.PositionX < 0)
						PlayerCursor.PositionX = 0;
					else {
						PlayerCursor.Index -= 1;
					}
					break;
			}
		}

		public void Switch() {
			Block tempBlock;
			tempBlock = theStack[PlayerCursor.Index].HeldBlock;
			theStack[PlayerCursor.Index].HeldBlock = theStack[PlayerCursor.Index + 1].HeldBlock;
			theStack[PlayerCursor.Index + 1].HeldBlock = tempBlock;
		}

		public void ManualRaise() {

		}

		private void gravity(int x, Block b) {
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
			 * Every time a block is destroyed, it gives 10 points.
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
			theContentManager.Load<Texture2D>("Graphics/Sprites/RedPixel");
			yellowBlockTexture = theContentManager.Load<Texture2D>("Graphics/Sprites/YellowBlock");
			theContentManager.Load<Texture2D>("Graphics/Sprites/YellowPixel");
			greenBlockTexture = theContentManager.Load<Texture2D>("Graphics/Sprites/GreenBlock");
			theContentManager.Load<Texture2D>("Graphics/Sprites/GreenPixel");
			lightBlueBlockTexture = theContentManager.Load<Texture2D>("Graphics/Sprites/LightBlueBlock");
			theContentManager.Load<Texture2D>("Graphics/Sprites/LightBluePixel");
			purpleBlockTexture = theContentManager.Load<Texture2D>("Graphics/Sprites/PurpleBlock");
			theContentManager.Load<Texture2D>("Graphics/Sprites/PurplePixel");
			darkBlueBlockTexture = theContentManager.Load<Texture2D>("Graphics/Sprites/DarkBlueBlock");
			theContentManager.Load<Texture2D>("Graphics/Sprites/DarkBluePixel");
			exclamationBlockTexture = theContentManager.Load<Texture2D>("Graphics/Sprites/ExclamationBlock");

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

        public void Update(GameTime gameTime) {
            
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

			timeSinceLastBump += gameTime.ElapsedGameTime.Milliseconds;
					if (timeSinceLastBump > milliPerBump) {
						timeSinceLastBump -= milliPerBump;
						if (!stop)
							raiseAmount++;
					}

			if (raiseAmount > 15) {
				raiseAmount = 0;
				MoveCursor(0);
				NextRow();
			}

			PlayerCursor.Update(gameTime);
			PlayerCursor.Location = new Vector2(PlayerCursor.Location.X + StackLocation.X, PlayerCursor.Location.Y + StackLocation.Y - raiseAmount);
			
			for (int x = 0; x < 72; x++) {
				if (theStack[x].HeldBlock != null) {
					theStack[x].HeldBlock.Location = new Vector2((theStack[x].PositionX * 16) + StackLocation.X, (theStack[x].PositionY * 16) + StackLocation.Y - raiseAmount);
					theStack[x].HeldBlock.Update(gameTime);
					gravity(x, theStack[x].HeldBlock);
				}
			}

			for (int x = 0; x < 6; x++) {
				theRow[x].HeldBlock.Location = new Vector2((theRow[x].PositionX * 16) + StackLocation.X, (theRow[x].PositionY * 16) + StackLocation.Y - raiseAmount);
				theRow[x].HeldBlock.Update(gameTime);
			}
        }

        public void Draw(SpriteBatch spriteBatch) { // Main blocks draw at 0.5
			foreach (Frame f in theStack) {
				if (f.HeldBlock != null) {
					f.HeldBlock.LayerDepth = 0.5f;
					f.HeldBlock.Draw(spriteBatch);
				}
			}
			foreach (Frame f in theRow) { // Row blocks draw at 0.8
				f.HeldBlock.LayerDepth = 0.8f;
				f.HeldBlock.Draw(spriteBatch);
			}

			spriteBatch.Draw(borderTexture, new Rectangle( // Border draws at 0.7
				(int)StackLocation.X - 8, (int)StackLocation.Y - 16, borderTexture.Width, borderTexture.Height
				), null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.7f);

			spriteBatch.Draw(stopBarTexture, new Rectangle( // Stop bar draws at 0.8
				(int)StackLocation.X - 3, (int)StackLocation.Y - 16, (int)(stopBarTexture.Width * stopTime), stopBarTexture.Height
				), new Rectangle(
				0, 0, (int)(stopBarTexture.Width * stopTime), stopBarTexture.Height), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.8f);

			PlayerCursor.Draw(spriteBatch);
        }
    }
}
