﻿using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands
{
    public class Actuator : WECommand
    {
        private readonly bool remove;
        private readonly Expression expression;

        public Actuator(int x, int y, int x2, int y2, TSPlayer plr, Expression expression, bool remove)
            : base(x, y, x2, y2, plr)
        {
            this.remove = remove;
            this.expression = expression ?? new TestExpression(new Test(t => true));
        }

        public override void Execute()
        {
            if (!CanUseCommand()) { return; }
            Tools.PrepareUndo(x, y, x2, y2, plr);
            int edits = 0;
            for (int i = x; i <= x2; i++)
            {
                for (int j = y; j <= y2; j++)
                {
                    var tile = Main.tile[i, j];
                    if (remove == tile.actuator() && select(i, j, plr) && expression.Evaluate(tile))
                    {
                        tile.actuator(!remove);
                        edits++;
                    }
                }
            }
            ResetSection();
            plr.SendSuccessMessage("{0} actuators. ({1})", remove ? "Removed" : "Set", edits);
        }
    }
}