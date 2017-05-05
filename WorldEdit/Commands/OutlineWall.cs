﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands
{
    public class OutlineWall : WECommand
    {
        private Expression expression;
        private int wallType;
        private int color;

        public OutlineWall(int x, int y, int x2, int y2, TSPlayer plr, int wallType, int color, Expression expression)
            : base(x, y, x2, y2, plr)
        {
            this.wallType = wallType;
            this.color = color;
            this.expression = expression ?? new TestExpression(new Test(t => true));
        }

        public override void Execute()
        {
            if (x < 2) x = 2;
            else if (x > (Main.maxTilesX - 3)) x = (Main.maxTilesX - 3);
            if (y < 2) y = 2;
            else if (y > (Main.maxTilesY - 3)) y = (Main.maxTilesY - 3);
            if (x2 < 2) x2 = 2;
            else if (x2 > (Main.maxTilesX - 3)) x2 = (Main.maxTilesX - 3);
            if (y2 < 2) y2 = 2;
            else if (y2 > (Main.maxTilesY - 3)) y2 = (Main.maxTilesY - 3);

            Tools.PrepareUndo(x - 1, y - 1, x2 + 1, y2 + 1, plr);
            int edits = 0;
            List<Point> walls = new List<Point>();

            for (int i = x; i <= x2; i++)
            {
                for (int j = y; j <= y2; j++)
                {
                    var tile = Main.tile[i, j];
                    bool XY = tile.wall != 0;

                    bool mXmY = Main.tile[i - 1, j - 1].wall == 0;
                    bool mXpY = Main.tile[i - 1, j + 1].wall == 0;
                    bool mXY = Main.tile[i - 1, j].wall == 0;
                    bool pXmY = Main.tile[i + 1, j - 1].wall == 0;
                    bool pXpY = Main.tile[i + 1, j + 1].wall == 0;
                    bool pXY = Main.tile[i + 1, j].wall == 0;
                    bool XmY = Main.tile[i, j - 1].wall == 0;
                    bool XpY = Main.tile[i, j + 1].wall == 0;

                    if (XY && expression.Evaluate((Tile)Main.tile[i, j]))
                    {
                        if (mXmY)
                        {
                            walls.Add(new Point(i - 1, j - 1));
                            edits++;
                        }
                        if (XmY)
                        {
                            walls.Add(new Point(i, j - 1));
                            edits++;
                        }
                        if (pXmY)
                        {
                            walls.Add(new Point(i + 1, j - 1));
                            edits++;
                        }
                        if (mXY)
                        {
                            walls.Add(new Point(i - 1, j));
                            edits++;
                        }
                        if (pXY)
                        {
                            walls.Add(new Point(i + 1, j));
                            edits++;
                        }
                        if (mXpY)
                        {
                            walls.Add(new Point(i - 1, j + 1));
                            edits++;
                        }
                        if (XpY)
                        {
                            walls.Add(new Point(i, j + 1));
                            edits++;
                        }
                        if (pXpY)
                        {
                            walls.Add(new Point(i + 1, j + 1));
                            edits++;
                        }
                    }
                }
            }

            foreach (Point p in walls)
            {
                var tile = Main.tile[p.X, p.Y];
                tile.wallColor((byte)color);
                tile.wall = (byte)wallType;
            }

            ResetSection();
            plr.SendSuccessMessage("Set wall outline. ({0})", edits);
        }
    }
}