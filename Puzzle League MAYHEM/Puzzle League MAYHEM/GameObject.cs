using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Puzzle_League_MAYHEM {
    abstract class GameObject { // Tilde White!
		// Do not generate things like HUD elements and backgrounds as GameObjects.
		// You don't need these things to constantly check each other's collisions.

		// This version of GameObject handles sprite sheets.
		// Sprite sheets should be arranged as a grid with no pixels in-between the sprites.
		// Aseprite can automatically export sheets such as these.

		// MATH
		protected float speedF = 0.0f;
		public float SpeedF {
			get {return speedF;}
			set {speedF = value;}
		}
		protected bool speedIsFloat = false;
		// Float speed. Good for games which use rotation.
		protected Vector2 speedV = Vector2.Zero;
		public Vector2 SpeedV {
			get {return speedV;}
			set {
				speedV.X = value.X;
				speedV.Y = value.Y;
			}
		}
		// Vector speed. For all those simpler times. Used by default unless speedIsFloat is true.
        protected Vector2 location = Vector2.Zero;
        public Vector2 Location {
            get {return location;}
            set {location = new Vector2(value.X, value.Y);}
        }
        private Vector2 center;
		public Vector2 Center {
			get {return center;}
		}
		// An object's center is calculated automatically and is just a reference.
		protected float scale = 1.0f;
		public float Scale {
			get {return scale;}
			set {scale = value;}
		}
		// For games in which sprites should scale. This value does not adjust the spriteSize value.
		protected float rotation = 0.0f;
		public float Rotation {
			get {return rotation;}
			set {
				rotation = value;
				float twoPi = MathHelper.TwoPi;
				if (rotation < -twoPi)
					rotation = twoPi;
				if (rotation > twoPi)
					rotation = -twoPi;
			}
		}
		// For games in which sprites should rotate.

		// GRAPHICS
        protected Texture2D spriteSheet;
		public Texture2D SpriteSheet {
			get {return spriteSheet;}
			set {spriteSheet = value;}
		}
		protected int spriteIndex = 0;
		// Just lets you know what frame of animation the object is on.
		protected int animationIndex = 0;
		public int AnimationIndex {
			get {return animationIndex;}
			set {animationIndex = value;}
		}
		/* The variable animationIndex is for objects which have more than one animation.
		   If autoAnimate is on, it will, by default, run through the first row of sprites.
		   This is because animationIndex is set to 0 by default. Every row should be a new animation.
		   If you're working with a sprite with multiple animations, change the animationIndex to a new row each
		   time you wish to change animations. Don't forget you can also change a given animation's speed with milliPerSprite below.
		*/
        protected string texturePath = "Graphics/";
		/* All GameObjects' graphics should be in a "Graphics" folder or subfolder.
		   In the constructor of a child class, put down: texturePath += "Whatever";
		*/
        protected Vector2 spriteSize;
        public Vector2 SpriteSize {
            get {return spriteSize;}
        }
		// SpriteSize should be set manually to indicate a given sprite's size in the sheet.
        protected Vector2 currentSprite;
		public Vector2 CurrentSprite {
			get {return currentSprite;}
		}
		protected float layerDepth = 0.50f;
		public float LayerDepth {
			get {return layerDepth;}
			set {layerDepth = value;}
		}
		// Relevant if you're drawing using sprite layers. 0 is front and 1 is back. By default, GameObjects draw to center(.5).
		
		// COLLISION
        protected Rectangle BoundingBox;
		/* All objects have a BoundingBox that encapsulates their entire being.
		   Only when two BoundingBoxes intersect will two objects' CollisionBoxes be checked for intersection.
		*/
        protected List<Rectangle> CollisionBox = new List<Rectangle>();
		/* By default, an object only has one CollisionBox, which overlaps with its BoundingBox.
		   Actual CollisionBoxes should be crafted for each type of object.
		*/

		// ANIMATION
		protected bool autoAnimate = false;
        protected int timeSinceLastSprite = 0;
        protected int milliPerSprite = 50;
		public int MilliPerSprite {
			get {return milliPerSprite;}
			set {milliPerSprite = value;}
		}
		/* The value milliPerSprite can change the speedF at which the sprite animates. By default, it's 50.
		 * You can even change milliPerSprite during certain parts of certain animations by keeping an eye on the spriteIndex.
		 */
		protected int frameCounter = 0;
		protected int framesPerSprite = 6;
		protected bool animationByFrame = false;
		protected bool affectedByScrolling = true;

		// OBJECT DEATH ROUTINE
		protected bool deathAnimation = false;
		/* This will be triggered when an object is dying if it has a death animation.
		   Objects which are dying no longer check collision.
		 */
        protected bool alive = true;
		public bool Alive {
			get {return alive;}
		}
		// Alive means the object exists. This bool shouldn't be flipped off until the object is completely out of the picture.
		// If an object has a death phase, Kill() should handle it.
		protected bool hasDeathAnimation = false;
		protected int deathAnimationIndex = 0;
		/* If an object's sprite sheet has a death animation, it should be on the same sprite sheet.
		   Set deathAnimationIndex to the row which contains the death animation. Make this the last row on the sheet.
		*/

        
        public static bool DevToolsOn = false;
		// This turns developer tools, including visible hitboxes, on. Should be toggleable until the final build.


		// CONSTRUCTORS

        public GameObject() { // Default constructor.
            location = Vector2.Zero;
			alive = true;
        }

        public GameObject(Vector2 v) { // Constructor that sets a location for spawn.
            location = v;
			alive = true;
        }

		public GameObject(Vector2 v, float r) { // Constructor that sets a location for spawn and initial rotation.
			location = v;
			rotation = r;
			alive = true;
		}

		public GameObject(Vector2 v, float r, float s) { // Constructor that sets a location for spawn, initial rotation, and float-based speed.
			location = v;
			rotation = r;
			speedF = s;
			alive = true;
		}

		public GameObject(Vector2 v, float r, Vector2 s) { // Constructor that sets a location for spawn, initial rotation, and vector-based speed.
			location = v;
			rotation = r;
			speedV = s;
			alive = true;
		}

		//

        public virtual void LoadContent(ContentManager theContentManager) {
			spriteSheet = theContentManager.Load<Texture2D>(texturePath);
            spriteSize = new Vector2(spriteSheet.Width, spriteSheet.Height);
			/* By default, the spriteSize is the size of the entire sprite sheet. For sheets with only one sprite, this needn't be adjusted.
			   If you do adjust this Vector2's X and Y, make sure to do so only after calling base.LoadContent from the child class.
			*/
            currentSprite = Vector2.Zero;
			UpdateCollisionBoxes();
			CollisionBox.Add(BoundingBox);
        }

        public void UpdateSpriteSheet() {
			currentSprite.X += spriteSize.X;
			currentSprite.Y = 0 + (spriteSize.Y * animationIndex);
			spriteIndex++;

			
            if (currentSprite.X >= spriteSheet.Width) {
				if (deathAnimation)
					this.alive = false;
					// This is where the object ceases to exist if it exceeds the last sprite of its death animation.
                currentSprite.X = 0;
				spriteIndex = 0;
			}

			// Sprite sheets, which are arranged in a grid, should not have any empty spaces.
        }

        public virtual void Update(GameTime gameTime) {
			if (alive) {
				if (autoAnimate || deathAnimation) {
					if (animationByFrame) {
						if (frameCounter >= framesPerSprite) {
							UpdateSpriteSheet();
							frameCounter = 0;
						}
					}
					else {
						timeSinceLastSprite += gameTime.ElapsedGameTime.Milliseconds;
						if (timeSinceLastSprite > milliPerSprite) {
							timeSinceLastSprite -= milliPerSprite;
							UpdateSpriteSheet();
						}
					}
				}

				if (speedIsFloat) {
					location.X += speedF * ((float)Math.Sin(rotation));
					location.Y -= speedF * ((float)Math.Cos(rotation));
				}
					// Movement using a float. Rotation 0.0f will send the object skyward.
				else {
					location.X += speedV.X;
					location.Y += speedV.Y;
				}
					// Movement using a vector.

				center = new Vector2((location.X + (spriteSize.X * scale)) / 2, (location.Y + (spriteSize.Y * scale)) / 2);
				UpdateCollisionBoxes();

				frameCounter++;
			}
        }

        public virtual void Draw(SpriteBatch spriteBatch) {
			if (alive) {
				spriteBatch.Draw(spriteSheet, location, new Rectangle(
					(int)currentSprite.X, (int)currentSprite.Y,
					(int)(spriteSize.X), (int)(spriteSize.Y)
					),
					Color.White, rotation, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
			}
        }

        public virtual void Kill() { // Default kill behavior is to run the death animation once if there is one, then disappear and unload.
			if (hasDeathAnimation)
				animationIndex = deathAnimationIndex;
			else
				this.alive = false;
        }

        protected void UpdateCollisionBoxes() {
            BoundingBox = new Rectangle((int)Location.X, (int)Location.Y,
                (int)(spriteSize.X * scale), (int)(spriteSize.Y * scale));
			// COLLISIONBOX UPDATE INFO GOES HERE
        }

		protected virtual void CreateCollisionBoxes() {
			/* Override this for any GameObjects which have their own set of hitboxes. The code will look like this:
			  
			 *	 CollisionBox.Add(new Rectangle(
			 *		 (int)Location.X + 50,
			 *		 (int)Location.Y + 0,
			 *		 50 * scale,
			 *		 500 * scale
			 *		 ));
			  
			 * Remember to use DevToolsOn to check that your CollisionBox set sits on your sprite nicely.
			 * Additionally, remember to clear the CollisionBox list since it overlaps with the BoundingBox.
			 * 
			 * This method should be called by LoadContent of the subclass.
			 */
		}

		//public virtual bool CheckCollision(GameObject interceptor) {
		//    if (this.BoundingBox.Intersects(interceptor.BoundingBox)) {
		//        if (!deathAnimation && alive) {
		//            for (int x = 0; x < this.CollisionBox.Count; x++) {
		//                for (int y = 0; y < interceptor.CollisionBox.Count; y++) {
		//                    if (this.CollisionBox[x].Intersects(interceptor.CollisionBox[y])) {
		//                        HandleCollision(interceptor);
		//                        return true;
		//                    }
		//                }
		//            }
		//        }
		//    }
		//    return false;
		//}

		//protected abstract bool HandleCollision(GameObject interceptor); // How an object handles collision is up to that object.

    }
}