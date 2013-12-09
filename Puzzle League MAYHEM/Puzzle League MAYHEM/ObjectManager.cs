using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Puzzle_League_MAYHEM {
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ObjectManager : Microsoft.Xna.Framework.DrawableGameComponent {
        SpriteBatch spriteBatch;
        List<GameObject> objectList = new List<GameObject>();
        List<GameObject> deadList = new List<GameObject>();

        public ObjectManager(Game game)
            : base(game) {

        }

        public override void Initialize() {

            base.Initialize();
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            foreach (GameObject s in objectList) {
                s.LoadContent(Game.Content);
            }
            base.LoadContent();
        }

        public override void Update(GameTime gameTime) {
            // LEVELS

            // GAMEOBJECTS
            for (int x = 0; x < objectList.Count; x++) {
                GameObject g = objectList[x];
                if (g.Alive) {
                    g.Update(gameTime);
                }
                else
                    deadList.Add(g);
            }

            for (int y = 0; y < deadList.Count; y++)
                objectList.Remove(deadList[y]);

            deadList.Clear();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime) {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            // LEVELS

            // GAMEOBJECTS
            foreach (GameObject s in objectList) {
                s.Draw(spriteBatch);
            }
            // HUD

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
