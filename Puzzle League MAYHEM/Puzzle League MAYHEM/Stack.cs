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

		private int initBlocks;

		public static float MAX_STOP_TIME = 1.00f;
		private float stopTime = 1.00f;

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

		public Vector2 stackLocation = new Vector2(50,30);

        public Stack() {
			
        }

        public void FillStack(int init) { // Stack always starts with 30 blocks in versus.
			initBlocks = init;

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

			for (int x = 0; x < initBlocks; x++) {
				int current = RNG.Next(6);
				theStack[current].HeldBlock = GenerateBlock(theStack[x]);
				gravity(current, theStack[current].HeldBlock);
			}

			PlayerCursor.Index = 32;
        }

        public void NextRow() {
            for (int x = 0; x < 66; x++)
                theStack[x].HeldBlock = theStack[x + 6].HeldBlock;

            theStack[66].HeldBlock = theRow[0].HeldBlock;
            theStack[67].HeldBlock = theRow[1].HeldBlock;
            theStack[68].HeldBlock = theRow[2].HeldBlock;
            theStack[69].HeldBlock = theRow[3].HeldBlock;
            theStack[70].HeldBlock = theRow[4].HeldBlock;
            theStack[71].HeldBlock = theRow[5].HeldBlock;

            GenerateNextRow();
        }

        public void GenerateNextRow() {
            foreach (Frame f in theRow) {
                GenerateBlock(f);
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
			f.HeldBlock = returnableBlock;
			return returnableBlock;
        }

		public void MoveCursor(short direction) {
			switch (direction) { // Clockwise.
				case 0: // Up
					PlayerCursor.PositionY -= 1;
					if (PlayerCursor.PositionY < 0)
						PlayerCursor.PositionY = 0;
					else {
						PlayerCursor.Index -= 6;
					}
					break;
				case 1: // Right
					PlayerCursor.PositionX += 1;
					if (PlayerCursor.PositionX > 4)
						PlayerCursor.PositionX = 4;
					else {
						PlayerCursor.Index += 1;
					}
					break;
				case 2: // Down
					PlayerCursor.PositionY += 1;
					if (PlayerCursor.PositionY > 11)
						PlayerCursor.PositionY = 11;
					else {
						PlayerCursor.Index += 6;
					}
					break;
				case 3: // Left
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

			PlayerCursor.Location = new Vector2(PlayerCursor.Location.X + stackLocation.X, PlayerCursor.Location.Y + stackLocation.Y);

			FillStack(30);

			for (int x = 0; x < 72; x++) {
				if (theStack[x].HeldBlock != null)
					theStack[x].HeldBlock.LoadContent(theContentManager);
			}
			for (int x = 0; x < 6; x++)
				theRow[x].HeldBlock.LoadContent(theContentManager);

		}

        public void Update(GameTime gameTime) {
            PlayerCursor.Update(gameTime);
			
			PlayerCursor.Location = new Vector2(PlayerCursor.Location.X + stackLocation.X, PlayerCursor.Location.Y + stackLocation.Y);

			for (int x = 0; x < 72; x++) {
				if (theStack[x].HeldBlock != null) {
					theStack[x].HeldBlock.Location = new Vector2((theStack[x].PositionX * 16) + stackLocation.X, (theStack[x].PositionY * 16) + stackLocation.Y);
					theStack[x].HeldBlock.Update(gameTime);
					gravity(x, theStack[x].HeldBlock);
				}
			}
			for (int x = 0; x < 6; x++) {
				theRow[x].HeldBlock.Location = new Vector2((theRow[x].PositionX * 16) + stackLocation.X, (theRow[x].PositionY * 16) + stackLocation.Y);
				theRow[x].HeldBlock.Update(gameTime);
			}

			if (stopTime > 0.00f)
				stopTime -= 0.005f;
        }

        public void Draw(SpriteBatch spriteBatch) {
			foreach (Frame f in theStack) {
				if (f.HeldBlock != null) {
					f.HeldBlock.LayerDepth = 0.5f;
					f.HeldBlock.Draw(spriteBatch);
				}
			}
			foreach (Frame f in theRow) {
				f.HeldBlock.LayerDepth = 0.8f;
				f.HeldBlock.Draw(spriteBatch);
			}

			spriteBatch.Draw(borderTexture, new Rectangle( // Border draws at 0.7
				(int)stackLocation.X - 8, (int)stackLocation.Y - 16, borderTexture.Width, borderTexture.Height
				), null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.7f);

			spriteBatch.Draw(stopBarTexture, new Rectangle( // Stop bar draws at 0.8
				(int)stackLocation.X - 3, (int)stackLocation.Y - 16, (int)(stopBarTexture.Width * stopTime), stopBarTexture.Height
				), new Rectangle(
				0, 0, (int)(stopBarTexture.Width * stopTime), stopBarTexture.Height), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.8f);

			PlayerCursor.Draw(spriteBatch);
        }
    }
}
