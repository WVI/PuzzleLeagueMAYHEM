using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Puzzle_League_MAYHEM {
	class Cursor: GameObject {
		public int PositionX;
		public int PositionY;
		public Stack.Frame LeftFrame;
		public Stack.Frame RightFrame;

		public Cursor() {
			texturePath += "Sprites/Cursor";
			layerDepth = 0.2f;
		}

		public override void LoadContent(ContentManager theContentManager) {
			base.LoadContent(theContentManager);
			spriteSize = new Vector2(38, 22);
			autoAnimate = true;
			milliPerSprite = 200;
		}

		public override void Update(GameTime gameTime) {
			location = new Vector2((PositionX * 16) - 3, (PositionY * 16) - 3);
			base.Update(gameTime);
		}
	}
}
