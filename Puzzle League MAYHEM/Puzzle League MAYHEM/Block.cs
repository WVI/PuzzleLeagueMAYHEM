using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Puzzle_League_MAYHEM {
    class Block: GameObject {

        public enum BlockColors { // Blocks are generally referenced by their color rather than their symbol.
            Blank,      // 0 - Shows the "Error Block" texture if it makes it to gameplay.
            Red,        // 1
            Yellow,     // 2
            Purple,     // 3
            Green,      // 4
            LightBlue,  // 5
            DarkBlue,   // 6
            Exclamation,// 7
            Garbage,    // 8
			Final,		// 9
			Destroyed,	// 10
        }

		/* Animation index 0: Standard
		 * Animation index 1: Bouncing(in top two rows)
		 * Animation index 2: Landing after a fall
		 * Animation index 3: Flashing before destruction
		 * Animation index 4: Being destroyed (single sprite)
		 * Animation index 5: In the Row
		*/

        public BlockColors BlockColor;
		public Texture2D BlockTexture;
		public Texture2D PixelTexture;
		public bool IsFalling = false;
		public bool IsBeingSwitched;
		public bool CannotBeMoved = false;
		public int FallTimer = 0;

        public Block() {
            BlockColor = (BlockColors)RNG.Next(1,6);
			autoAnimate = true;
			spriteSize = new Vector2(16, 16);
			layerDepth = 0.5f;
			animationByFrame = true;
			framesPerSprite = 1;
        }

        public Block(bool darkBlueOn) {
            if (darkBlueOn)
                BlockColor = (BlockColors)RNG.Next(1,7);
            else
                BlockColor = (BlockColors)RNG.Next(1,6);
			autoAnimate = true;
			spriteSize = new Vector2(16, 16);
			layerDepth = 0.5f;
			animationByFrame = true;
			framesPerSprite = 1;
        }

		public Block(BlockColors bc) {
			BlockColor = bc;
			spriteSize = new Vector2(16, 16);
			layerDepth = 0.5f;
			animationByFrame = true;
			framesPerSprite = 1;
		}

		public Block(Block b) {
			BlockColor = b.BlockColor;
			BlockTexture = b.BlockTexture;
			PixelTexture = b.PixelTexture;
			spriteSheet = b.spriteSheet;
			spriteSize = new Vector2(16, 16);
			autoAnimate = true;
			layerDepth = 0.5f;
			animationByFrame = true;
			framesPerSprite = 1;
		}

		public void AssignExcept(bool darkBlue, BlockColors bc) { // Changes block color to a random value with special exceptions.
			List<BlockColors> possibleColors = new List<BlockColors> ();
			possibleColors.Add(BlockColors.Red);
			possibleColors.Add(BlockColors.Yellow);
			possibleColors.Add(BlockColors.Purple);
			possibleColors.Add(BlockColors.Green);
			possibleColors.Add(BlockColors.LightBlue);
			
			if (darkBlue)
				possibleColors.Add(BlockColors.DarkBlue);

			possibleColors.Remove(bc); // Removes from the list the index specified.

			this.BlockColor = possibleColors[RNG.Next(possibleColors.Count)];
		}

		public override void LoadContent(ContentManager theContentManager) {
			texturePath += "Sprites/WhitePixel"; // This gets replaced immediately, it's just there to keep the GameObject system working.

			base.LoadContent(theContentManager);

			spriteSize = new Vector2(16, 16);
			autoAnimate = true;
			layerDepth = 0.5f;
			animationByFrame = true;
			framesPerSprite = 1;
		}

		public override void Update(GameTime gameTime) {
			if (BlockTexture != spriteSheet) {
				spriteSheet = BlockTexture;
			}

			base.Update(gameTime);
		}

	}
}
