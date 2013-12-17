using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Puzzle_League_MAYHEM {
	class Endless100Background: Background {
		// This is the Whitney Music Box by Jim Bumgardner adapted to C# and XNA/Monogame. Thanks very much to him for this animation.
		private int stageWidth = 256;
		private int stageHeight = 244;
		private int positionX = 256 / 2;
		private int positionY = 244 / 2;
		private float positionRadius = (float)(256 / 2 *.95);

		private int numberOfPoints = 48;
		private int cycleLength = 3 * 60; // Three minutes.
		private int speedOfSpin;

		private Texture2D whitePixelTexture;
		private Texture2D whiteCircleTexture;

		public Endless100Background() {
			texturePath += "Sprites/PurplePixel";

			for (int i = 0; i < numberOfPoints; ++i) {
				//var r = i / numberofpoints;
				//var len = (i + 1) * positionradius / numberofpoints;
				//var rad = math.max(2, r * 8);
				//mc._x = positionx;
				//mc._y = positiony;
				//mc.len = len;

				//mc.createemptymovieclip("dot", 1);
				//mc.dot._x = len;
				//mc.dot._y = 0;
				//mc.dot.clear();
				//mc.dot.linestyle(rad * 2, getcolor(r), 100);
				//mc.dot.moveto(-.2, -.2);
				//mc.dot.lineto(.2, .2);

				//mc.createemptymovieclip("outline", 2);
				//mc.outline.clear();
				//mc.outline._x = len;
				//mc.outline._y = 0;
				//mc.outline.linestyle(rad * 2 + 2, getcolor(r) | 0xf0f0f0, 100);
				//mc.outline.moveto(-.2, -.2);
				//mc.outline.lineto(.2, .2);
			}
		}

		private int getColor(float ratio) {
			float a = (float)(2 * Math.PI * ratio);
			// trace(a);

			float r = (float)(128 + Math.Cos(a) * 127);
			float g = (float)(128 + Math.Cos(a + 2 * Math.PI / 3) * 127);
			float b = (float)(128 + Math.Cos(a + 4 * Math.PI / 3) * 127);
			return ((int)r << 16) | ((int)g << 8) | (int)b;
		}

		public override void LoadContent(ContentManager theContentManager) {
			base.LoadContent(theContentManager);

			whitePixelTexture = theContentManager.Load<Texture2D>("Graphics/Sprites/WhitePixel");
			whiteCircleTexture = theContentManager.Load<Texture2D>("Graphics/Backgrounds/WhiteCircle");
		}

		public override void Update(GameTime gameTime) {

			base.Update(gameTime);
		}

		public override void Draw(SpriteBatch spriteBatch) {


			base.Draw(spriteBatch);
		}
	}
}
