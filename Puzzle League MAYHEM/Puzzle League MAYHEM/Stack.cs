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

		private Texture2D redBlockTexture;
		private Texture2D yellowBlockTexture;
		private Texture2D greenBlockTexture;
		private Texture2D lightBlueBlockTexture;
		private Texture2D purpleBlockTexture;
		private Texture2D darkBlueBlockTexture;
		private Texture2D exclamationBlockTexture;

		private Texture2D borderTexture;

		public Vector2 stackLocation = new Vector2(100,35);

        public Stack() {
			
        }

        public void FillStack() { // Stack always starts with 30 blocks.
			for (int x = 0; x < 72; x++) {
				theStack[x].HeldBlock = GenerateBlock(theStack[x]);
				theStack[x].Index = x;
				theStack[x].PositionX = x % 6;
				theStack[x].PositionY = x / 6;
				theStack[x].HeldBlock.Location = new Vector2(theStack[x].PositionX, theStack[x].PositionY);
			}
			for (int x = 0; x < 6; x++) {
				theRow[x].HeldBlock = GenerateBlock(theRow[x]);
				theRow[x].Index = 72 + x;
				theRow[x].PositionX = x % 6;
				theRow[x].PositionY = 12 + x / 6;
				theRow[x].HeldBlock.Location = new Vector2(theStack[x].PositionX, theStack[x].PositionY);
			}

			PlayerCursor.LeftFrame = theStack[32];
			PlayerCursor.RightFrame = theStack[33];
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
						PlayerCursor.LeftFrame = theStack[PlayerCursor.LeftFrame.Index - 6];
						PlayerCursor.RightFrame = theStack[PlayerCursor.RightFrame.Index - 6];
					}
					break;
				case 1: // Right
					PlayerCursor.PositionX += 1;
					if (PlayerCursor.PositionX > 4)
						PlayerCursor.PositionX = 4;
					else {
						PlayerCursor.LeftFrame = theStack[PlayerCursor.LeftFrame.Index + 1];
						PlayerCursor.RightFrame = theStack[PlayerCursor.RightFrame.Index + 1];
					}
					break;
				case 2: // Down
					PlayerCursor.PositionY += 1;
					if (PlayerCursor.PositionY > 11)
						PlayerCursor.PositionY = 11;
					else {
						PlayerCursor.LeftFrame = theStack[PlayerCursor.LeftFrame.Index + 6];
						PlayerCursor.RightFrame = theStack[PlayerCursor.RightFrame.Index + 6];
					}
					break;
				case 3: // Left
					PlayerCursor.PositionX -= 1;
					if (PlayerCursor.PositionX < 0)
						PlayerCursor.PositionX = 0;
					else {
						PlayerCursor.LeftFrame = theStack[PlayerCursor.LeftFrame.Index - 1];
						PlayerCursor.RightFrame = theStack[PlayerCursor.RightFrame.Index - 1];
					}
					break;
			}
		}

		public void Switch() {
			Block tempBlock;
			tempBlock = PlayerCursor.LeftFrame.HeldBlock;
			PlayerCursor.LeftFrame.HeldBlock = PlayerCursor.RightFrame.HeldBlock;
			PlayerCursor.RightFrame.HeldBlock = tempBlock;
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

			borderTexture = theContentManager.Load<Texture2D>("Graphics/Interface/NetelleBorder");

			PlayerCursor.LoadContent(theContentManager);
			PlayerCursor.PositionX = 2;
			PlayerCursor.PositionY = 5;

			PlayerCursor.Location = new Vector2(PlayerCursor.Location.X + stackLocation.X, PlayerCursor.Location.Y + stackLocation.Y);

			FillStack();

			for (int x = 0; x < 72; x++)
				theStack[x].HeldBlock.LoadContent(theContentManager);
			for (int x = 0; x < 6; x++)
				theRow[x].HeldBlock.LoadContent(theContentManager);

		}

        public void Update(GameTime gameTime) {
            PlayerCursor.Update(gameTime);
			
			PlayerCursor.Location = new Vector2(PlayerCursor.Location.X + stackLocation.X, PlayerCursor.Location.Y + stackLocation.Y);

			for (int x = 0; x < 72; x++) {
				theStack[x].HeldBlock.Location = new Vector2((theStack[x].PositionX * 16) + stackLocation.X, (theStack[x].PositionY * 16) + stackLocation.Y);
				theStack[x].HeldBlock.Update(gameTime);
			}
			for (int x = 0; x < 6; x++) {
				theRow[x].HeldBlock.Location = new Vector2((theRow[x].PositionX * 16) + stackLocation.X, (theRow[x].PositionY * 16) + stackLocation.Y);
				theRow[x].HeldBlock.Update(gameTime);
			}
        }

        public void Draw(SpriteBatch spriteBatch) {
			foreach (Frame f in theStack) {
				f.HeldBlock.LayerDepth = 0.5f;
				f.HeldBlock.Draw(spriteBatch);
			}
			foreach (Frame f in theRow) {
				f.HeldBlock.LayerDepth = 0.8f;
				f.HeldBlock.Draw(spriteBatch);
			}

			spriteBatch.Draw(borderTexture, new Rectangle(
				(int)stackLocation.X - 8, (int)stackLocation.Y - 16, borderTexture.Width, borderTexture.Height
				), null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.7f);
			PlayerCursor.Draw(spriteBatch);
        }
    }
}
