using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Puzzle_League_MAYHEM {
    class Stack: GameObject {
        struct Frame {
            public int Index;
            public int PositionX;
            public int PositionY;
            public Block HeldBlock;
        }

        struct Cursor {
            public int PositionX;
            public int PositionY;
            public Frame leftFrame;
            public Frame rightFrame;
        }

        bool darkBlueOn = false;
        Frame[] theStack = new Frame[72];
        Frame[] theRow = new Frame[6];
        // [66] [67] [68] [69] [70] [71]
        Cursor playerCursor = new Cursor();

        public Stack() {
            for (int x = 0; x < 72; x++) {
                theStack[x].Index = x;
                theStack[x].PositionX = x % 6;
                theStack[x].PositionY = x / 6;
            }
        }

        public void Fill() {

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

        public void GenerateBlock(Frame f) {
            f.HeldBlock = new Block(darkBlueOn);
        }

        public void UpdateCursor() {
            if (playerCursor.PositionX < 0)
                playerCursor.PositionX = 0;
            if (playerCursor.PositionY < 0)
                playerCursor.PositionY = 0;

            if (playerCursor.PositionX > 5)
                playerCursor.PositionX = 5;
            if (playerCursor.PositionY > 11)
                playerCursor.PositionY = 11;
        }

        public override void Update(GameTime gameTime) {
            UpdateCursor();

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch) {
            base.Draw(spriteBatch);
        }
    }
}
