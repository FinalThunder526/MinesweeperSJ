using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Minesweeper
{
    class Sprite
    {
        public Vector2 Position;
        public string AssetName;
        protected Texture2D Texture;
        public int height;

        public Sprite(Vector2 Position, string theAssetName)
        {
            this.Position = Position;
            this.AssetName = theAssetName;
        }
        public Sprite()
        {
            this.Position = new Vector2(0, 0);
            this.AssetName = "";
        }

        public void changeAsset(string newAsset, ContentManager Content)
        {
            AssetName = newAsset;
            this.LoadContent(Content);
        }

        public void LoadContent(ContentManager theContentManager)
        {
            Texture = theContentManager.Load<Texture2D>(AssetName);
        }
        public bool isOver(Vector2 OtherPosition)
        {
            if (OtherPosition.X >= Position.X && OtherPosition.X <= (Position.X + this.height))
            {
                if (OtherPosition.Y >= Position.Y && OtherPosition.Y <= (Position.Y + this.height))
                {
                    return true;
                }
            }
            return false;
        }
        public void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(Texture, Position,
                 new Rectangle(0, 0, Texture.Width, Texture.Height), Color.White,
                 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }
    }
}
