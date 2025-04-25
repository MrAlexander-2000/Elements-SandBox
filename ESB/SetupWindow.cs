using Raylib_cs;
using System;

namespace ESB
{
    public class SetupWindow
    {
        const int screenWidth = 800;
        const int screenHeight = 600;

        CellGrid grid = new CellGrid(80, 60);
        Tools tools = new Tools();

        public void CreateWindow()
        {
            Raylib.InitWindow(screenWidth, screenHeight, "ESB - Elements SandBox - Version: " + EntryPoint.versionNum);
            Raylib.SetTargetFPS(60);

            if (Raylib.IsWindowReady())
                Console.WriteLine("SetupWindow: Created game's window successfully!");
            else
                Console.WriteLine("SetupWindow: Failed to create game's window!");

            while (!Raylib.WindowShouldClose())
            {
                HandleInput();
                tools.Update(grid);

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);
                grid.Draw();
                DrawUI();
                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }

        private void HandleInput()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.One)) Tools.CurrentTool = Tools.ToolType.Sand;
            if (Raylib.IsKeyPressed(KeyboardKey.Two)) Tools.CurrentTool = Tools.ToolType.Water;
            if (Raylib.IsKeyPressed(KeyboardKey.Three)) Tools.CurrentTool = Tools.ToolType.Fire;
            if (Raylib.IsKeyPressed(KeyboardKey.Four)) Tools.CurrentTool = Tools.ToolType.Smoke;
            if (Raylib.IsKeyPressed(KeyboardKey.Five)) Tools.CurrentTool = Tools.ToolType.Wall;
            if (Raylib.IsKeyPressed(KeyboardKey.Six)) Tools.CurrentTool = Tools.ToolType.Lava;
            if (Raylib.IsKeyPressed(KeyboardKey.Seven)) Tools.CurrentTool = Tools.ToolType.Ice;
            if (Raylib.IsKeyPressed(KeyboardKey.Eight)) Tools.CurrentTool = Tools.ToolType.Wood;
            if (Raylib.IsKeyPressed(KeyboardKey.Nine)) Tools.CurrentTool = Tools.ToolType.Delete;

            if (Raylib.IsKeyPressed(KeyboardKey.C))
            {
                grid.ClearGrid();
            }

            if (Raylib.IsMouseButtonDown(MouseButton.Left))
            {
                int x = Raylib.GetMouseX() / grid.CellSize;
                int y = Raylib.GetMouseY() / grid.CellSize;

                if (grid.IsInsideGrid(x, y))
                {
                    var type = Tools.CurrentTool switch
                    {
                        Tools.ToolType.Sand => CellType.Sand,
                        Tools.ToolType.Water => CellType.Water,
                        Tools.ToolType.Lava => CellType.Lava,
                        Tools.ToolType.Wall => CellType.Wall,
                        Tools.ToolType.Fire => CellType.Fire,
                        Tools.ToolType.Smoke => CellType.Smoke,
                        Tools.ToolType.Wood => CellType.Wood,
                        Tools.ToolType.Ice => CellType.Ice,
                        Tools.ToolType.Delete => CellType.Empty,
                        _ => CellType.Empty
                    };

                    grid.SetCell(x, y, new Cell(type));
                }
            }
        }

        private void DrawUI()
        {
            Raylib.DrawText($"Tool: {Tools.CurrentTool}", 10, 10, 20, Color.White);
        }
    }
}