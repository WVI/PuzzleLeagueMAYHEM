using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Puzzle_League_MAYHEM {
    abstract class GameObject {
        protected Vector2 location = Vector2.Zero;
        public Vector2 Location {
            get { return location; }
            set { location = new Vector2(value.X, value.Y); }
        }
        public Vector2 Center;
        protected Vector2 speed = Vector2.Zero;
        protected Texture2D spriteSheet;
        protected string texturePath = "Graphics/";
        protected Vector2 spriteSize;
        public Vector2 SpriteSize {
            get { return spriteSize; }
            set { spriteSize = value; }
        }
        protected Vector2 currentSprite;
        protected int timeSinceLastFrame = 0;
        protected int milliPerFrame = 50;
        protected int layerDepth = 1;
        public bool Alive = true;
        protected bool affectedByScrolling = false;
        protected bool autoAnimate = false;
        protected bool customAnimationSpeed = false;

        public GameObject() {
            location = Vector2.Zero;
        }

        public virtual void LoadContent(ContentManager theContentManager) {
            spriteSheet = theContentManager.Load<Texture2D>(texturePath);
            spriteSize = new Vector2(spriteSheet.Width, spriteSheet.Height);
            currentSprite = Vector2.Zero;
        }

        public void UpdateSpriteSheet() {
            currentSprite.X += spriteSize.X;
            if (currentSprite.X >= spriteSheet.Width) {
                currentSprite.X = 0;
                currentSprite.Y += spriteSize.Y;
                if (currentSprite.Y >= spriteSheet.Height) {
                    currentSprite.Y = 0;
                }
            }
        }

        public virtual void Update(GameTime gameTime) {
            if (customAnimationSpeed) {
                timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
                if (timeSinceLastFrame > milliPerFrame) {
                    timeSinceLastFrame -= milliPerFrame;

                    if (autoAnimate)
                        UpdateSpriteSheet();
                }
            }
            else
                if (autoAnimate)
                    UpdateSpriteSheet();
            location.X += speed.X;
            location.Y += speed.Y;
            Center = new Vector2((location.X + spriteSize.X) / 2, (location.Y + spriteSize.Y) / 2);
        }

        public virtual void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(spriteSheet, location, new Rectangle((int)currentSprite.X, (int)currentSprite.Y,
                (int)spriteSize.X, (int)spriteSize.Y), Color.White);
        }
    }
}