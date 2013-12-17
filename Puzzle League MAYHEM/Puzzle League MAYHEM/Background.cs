using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Puzzle_League_MAYHEM {
	abstract class Background: GameObject {

		public Background() {
			spriteSize = new Vector2(256, 244);
			autoAnimate = true;
			layerDepth = 1.0f; // The very back.
		}
	}
}
