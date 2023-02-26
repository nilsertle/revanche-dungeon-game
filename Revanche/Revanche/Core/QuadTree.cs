using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Revanche.GameObjects;
using Revanche.GameObjects.Items;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Revanche.Core
{

    public sealed class Rect
    {
        [JsonProperty] internal Vector2 mPos;
        [JsonProperty] internal Vector2 mSize;

        [JsonConstructor]
        public Rect(Vector2 pos, Vector2 size)
        {
            this.mPos = pos;
            this.mSize = size;
        }
        public Rect(Rectangle hitbox)
        {
            this.mPos = new Vector2(hitbox.Left, hitbox.Top);
            this.mSize = new Vector2(hitbox.Width, hitbox.Height);
        }

        internal bool Contains(Rect rect)
        {
            return (rect.mPos.X >= this.mPos.X &&
                    rect.mPos.Y >= this.mPos.Y &&
                    rect.mPos.X + rect.mSize.X <= this.mPos.X + this.mSize.X &&
                    rect.mPos.Y + rect.mSize.Y <= this.mPos.Y + this.mSize.Y);
        }

        internal bool Overlaps(Rect rect)
        {
            return (this.mPos.X < rect.mPos.X + rect.mSize.X &&
                    this.mPos.X + this.mSize.X >= rect.mPos.X &&
                    this.mPos.Y < rect.mPos.Y + rect.mSize.Y &&
                    this.mPos.Y + this.mSize.Y >= rect.mPos.Y);
        }
    }

    public sealed class SafeStaticGameObjectQuadTree
    {
        private const float Divider2 = 2f;
        private const int Quad = 4;

        [JsonProperty] private Rect mArea;
        [JsonProperty] private int mLevel;
        [JsonProperty] private const int MaxLevel = 12; //Can be changed if 12 ist not a good choice NEVER smaller than 1
        [JsonProperty] private List<GameObject> mObjects;
        [JsonProperty] private List<Rect> mChildAreas;
        [JsonProperty] private List<SafeStaticGameObjectQuadTree> mChildren;

        private SafeStaticGameObjectQuadTree()
        {
        }

        public static SafeStaticGameObjectQuadTree CreateQuadTree(Rect rect, int level = 1)
        {
            var quadTree = new SafeStaticGameObjectQuadTree();
            quadTree.mArea = rect;
            quadTree.mLevel = level;
            quadTree.mObjects = new List<GameObject>();
            quadTree.mChildAreas = new List<Rect>
            {
                new Rect(quadTree.mArea.mPos, Vector2.Divide(quadTree.mArea.mSize, Divider2)),
                new Rect(new Vector2(quadTree.mArea.mPos.X + (quadTree.mArea.mSize.X / Divider2), quadTree.mArea.mPos.Y), Vector2.Divide(quadTree.mArea.mSize, Divider2)),
                new Rect(new Vector2(quadTree.mArea.mPos.X, quadTree.mArea.mPos.Y + (quadTree.mArea.mSize.Y / Divider2)), Vector2.Divide(quadTree.mArea.mSize, Divider2)),
                new Rect(new Vector2(quadTree.mArea.mPos.X + (quadTree.mArea.mSize.X / Divider2), quadTree.mArea.mPos.Y + (quadTree.mArea.mSize.Y / Divider2)), Vector2.Divide(quadTree.mArea.mSize, Divider2))
            };
            quadTree.mChildren = new List<SafeStaticGameObjectQuadTree>(Quad){null, null, null, null};
            return quadTree;
        }

        internal void Clear()
        {
            this.mObjects.Clear();
            for (var i = 0; i < Quad; i++)
            {
                if (mChildren[i] != null)
                {
                    mChildren[i].Clear();
                    mChildren[i] = null;
                }
            }
        }

        internal void Insert(GameObject go)
        {
            var objArea = new Rect(go.Hitbox); //16 because a sprite is 16x16 pixels
            this.InsertHelper(go, objArea);
        }

        private void InsertHelper(GameObject go, Rect area)
        {
            if (this.mLevel >= MaxLevel)
            {
                this.mObjects.Add(go);
                return;
            }
            for (var i = 0; i < Quad; i++)
            {
                if (!this.mChildAreas[i].Contains(area))
                {
                    continue;
                }

                this.mChildren[i] ??= SafeStaticGameObjectQuadTree.CreateQuadTree(this.mChildAreas[i], this.mLevel + 1);
                this.mChildren[i].InsertHelper(go, area);
                return;
            }
            this.mObjects.Add(go);
        }

        /// <summary>
        /// Searches for characters within a rectangle
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        internal List<Character> SearchCharacters(Rect area)
        {
            return Search(area).OfType<Character>().ToList();
        }

        /// <summary>
        /// Searches for characters given an arbitrary position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        internal List<Character> PointSearchCharacters(Vector2 position)
        {
            return Search(new Rect(position, new Vector2(1, 1))).OfType<Character>().ToList();
        }

        /// <summary>
        /// Returns a list containing all GameObjects overlapping with the given Rectangle
        /// </summary>
        /// <param name="hitbox"></param>
        /// <returns></returns>
        internal List<GameObject> Search(Rectangle hitbox)
        {
            return this.Search(new Rect(hitbox));
        }

        /// <summary>
        /// Returns a list containing all Characters overlapping with the given Rectangle
        /// </summary>
        /// <param name="hitbox"></param>
        /// <returns></returns>
        public List<Character> SearchCharacters(Rectangle hitbox)
        {
            return Search(new Rect(hitbox)).OfType<Character>().ToList();
        }

        /// <summary>
        /// Returns a list containing all Items overlapping with the given Rectangle
        /// </summary>
        /// <param name="hitbox"></param>
        /// <returns></returns>
        internal List<Item> SearchItems(Rectangle hitbox)
        {
            return Search(new Rect(hitbox)).OfType<Item>().ToList();
        }

        /// <summary>
        /// Searches for GameObjects inside a rectangle
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        private List<GameObject> Search(Rect area) // can be changed to internal if needed
        {
            var outList = new List<GameObject>();
            Search(area, outList);
            return outList;
        }

        private void Search(Rect area, List<GameObject> outList)
        {
            foreach (var obj in this.mObjects)
            {
                if (area.Overlaps(new Rect(new Vector2(obj.Hitbox.X, obj.Hitbox.Y), new Vector2(obj.Hitbox.Width, obj.Hitbox.Height))))
                {
                    outList.Add(obj);
                }
            }

            for (var i = 0; i < Quad; i++)
            {
                if (this.mChildren[i] == null)
                {
                    continue;
                }

                if (area.Contains(this.mChildAreas[i]))
                {
                    this.mChildren[i].FeedItems(outList);
                }
                else if (area.Overlaps(this.mChildAreas[i]))
                {
                    this.mChildren[i].Search(area, outList);
                }
            }
        }

        private void FeedItems(List<GameObject> outList)
        {
            outList.AddRange(this.mObjects);

            for (var i = 0; i < Quad; i++)
            {
                if (this.mChildren[i] == null)
                {
                    continue;
                }

                this.mChildren[i].FeedItems(outList);
            }
        }

        internal void Draw(SpriteBatch spriteBatch, Rectangle visibleArea)
        {
            foreach (var obj in this.Search(new Rect(new Vector2(visibleArea.Left, visibleArea.Top), new Vector2(visibleArea.Width, visibleArea.Height))))
            {
                obj.Draw(spriteBatch);
            }
        }
    }

}