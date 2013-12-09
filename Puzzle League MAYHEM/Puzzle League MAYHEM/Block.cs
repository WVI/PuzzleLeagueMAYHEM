using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Puzzle_League_MAYHEM {
    class Block: GameObject {
        Random rand = new Random();

        public enum BlockColors {
            Blank,      // 0
            Red,        // 1
            Yellow,     // 2
            Purple,     // 3
            Green,      // 4
            LightBlue,  // 5
            DarkBlue,   // 6
            Exclamation,// 7
            Garbage     // 8
        }
        public BlockColors BlockColor;
		public Texture2D BlockTexture;

        public Block() {
            BlockColor = (BlockColors)rand.Next(1,6);
        }

        public Block(bool darkBlueOn) {
            if (darkBlueOn)
                BlockColor = (BlockColors)rand.Next(1,7);
            else
                BlockColor = (BlockColors)rand.Next(1,6);
        }

		public override void LoadContent(ContentManager theContentManager) {
			texturePath += "Sprites/WhitePixel";

			base.LoadContent(theContentManager);

			spriteSize = new Vector2(16, 16);
			autoAnimate = true;
		}

		public override void Update(GameTime gameTime) {
			if (BlockTexture != spriteSheet)
				spriteSheet = BlockTexture;

			base.Update(gameTime);
		}

	}
}
