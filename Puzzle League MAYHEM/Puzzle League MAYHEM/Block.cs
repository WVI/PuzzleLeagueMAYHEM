using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Puzzle_League_MAYHEM {
    class Block: GameObject {

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

		/* Animation index 0: Standard
		 * Animation index 1: Bouncing(in top two rows)
		 * Animation index 2: Being destroyed (single frame)
		 * Animation index 3: In the Row
		 * Animation index 4: Game over (stack has reached the top)
		*/

        public BlockColors BlockColor;
		public Texture2D BlockTexture;
		public bool Falling = false;

        public Block() {
            BlockColor = (BlockColors)RNG.Next(1,6);
			autoAnimate = true;
			spriteSize = new Vector2(16, 16);
        }

        public Block(bool darkBlueOn) {
            if (darkBlueOn)
                BlockColor = (BlockColors)RNG.Next(1,7);
            else
                BlockColor = (BlockColors)RNG.Next(1,6);
			autoAnimate = true;
			spriteSize = new Vector2(16, 16);
        }

		public Block(Block b) {
			BlockColor = b.BlockColor;
			BlockTexture = b.BlockTexture;
			spriteSheet = b.spriteSheet;
			spriteSize = new Vector2(16, 16);
			autoAnimate = true;
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
