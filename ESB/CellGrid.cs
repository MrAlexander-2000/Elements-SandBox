using Raylib_cs;
using System;

namespace ESB
{
    public enum CellType
    {
        Empty,
        Sand,
        Water,
        Lava,
        Fire,
        Smoke,
        Wall,
        Metal,
        Wood,
        Ice,
        Rock,
        Steam
    }

    public class Cell
    {
        public CellType Type;
        public int Lifetime;

        public Cell(CellType type = CellType.Empty)
        {
            Type = type;
            Lifetime = type switch
            {
                CellType.Fire => 60,
                CellType.Smoke => 100,
                _ => -1
            };
        }

        public Color GetColor()
        {
            return Type switch
            {
                CellType.Sand => Color.Yellow,
                CellType.Water => Color.Blue,
                CellType.Lava => Color.Orange,
                CellType.Fire => Color.Red,
                CellType.Smoke => Color.LightGray,
                CellType.Wall => Color.Gray,
                CellType.Metal => Color.DarkGray,
                CellType.Wood => Color.Brown,
                CellType.Ice => Color.SkyBlue,
                CellType.Rock => Color.DarkBrown,
                _ => Color.Blank
            };
        }

        public bool IsSolid() => Type == CellType.Wall || Type == CellType.Metal || Type == CellType.Wood || Type == CellType.Rock;
        public bool IsEmpty() => Type == CellType.Empty;
        public bool IsLiquid() => Type == CellType.Water || Type == CellType.Lava;
        public bool IsMovable() => Type == CellType.Sand || IsLiquid() || Type == CellType.Smoke || Type == CellType.Fire;
    }

    public class CellGrid
    {
        private Cell[,] grid;
        public int Width { get; }
        public int Height { get; }
        public int CellSize { get; } = 10;

        public CellGrid(int width, int height)
        {
            Width = width;
            Height = height;
            grid = new Cell[width, height];
            Clear();
        }

        public void Clear()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    grid[x, y] = new Cell();
        }

        public bool IsInsideGrid(int x, int y) => x >= 0 && y >= 0 && x < Width && y < Height;

        public void SetCell(int x, int y, Cell cell)
        {
            if (IsInsideGrid(x, y))
                grid[x, y] = cell;
        }

        public void Update()
        {
            for (int y = Height - 2; y >= 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    Cell cell = grid[x, y];
                    if (!cell.IsMovable()) continue;

                    switch (cell.Type)
                    {
                        case CellType.Sand: TryFall(x, y, heavy: true); break;
                        case CellType.Water: TryFlow(x, y); break;
                        case CellType.Lava: TryFlowLava(x, y); TryLavaBurn(x, y); TryCoolLava(x, y); break;
                        case CellType.Fire: TryFireFade(x, y); TryIgniteNearby(x, y); break;
                        case CellType.Smoke: TryRise(x, y); TrySmokeFade(x, y); break;
                        case CellType.Wood: TryBurn(x, y); break;
                        case CellType.Ice: TryMeltIce(x, y); break;
                        case CellType.Metal: TryHeatTransfer(x, y); break;
                    }
                }
            }
        }

        private void TryFall(int x, int y, bool heavy = false)
        {
            if (!TryMove(x, y, x, y + 1))
            {
                int dir = Raylib.GetRandomValue(0, 1) == 0 ? -1 : 1;
                if (!TryMove(x, y, x + dir, y + 1))
                    TryMove(x, y, x - dir, y + 1);
            }
        }

        private void TryFlow(int x, int y)
        {
            if (!TryMove(x, y, x, y + 1))
            {
                int dir = Raylib.GetRandomValue(0, 1) == 0 ? -1 : 1;
                if (!TryMove(x, y, x + dir, y))
                    TryMove(x, y, x - dir, y);
            }
        }

        private void TryFlowLava(int x, int y)
        {
            if (!TryMove(x, y, x, y + 1))
            {
                int dir = Raylib.GetRandomValue(0, 1) == 0 ? -1 : 1;
                if (!TryMove(x, y, x + dir, y))
                    TryMove(x, y, x - dir, y);
            }
        }

        private void TryFireFade(int x, int y)
        {
            if (grid[x, y].Lifetime > 0)
            {
                grid[x, y].Lifetime--;
                if (grid[x, y].Lifetime <= 0)
                    grid[x, y] = new Cell(CellType.Smoke);
            }
        }

        private void TrySmokeFade(int x, int y)
        {
            if (grid[x, y].Lifetime > 0)
            {
                grid[x, y].Lifetime--;
                if (grid[x, y].Lifetime <= 0)
                    grid[x, y] = new Cell();
            }
        }

        private void TryRise(int x, int y)
        {
            if (!TryMove(x, y, x, y - 1))
            {
                int dir = Raylib.GetRandomValue(0, 1) == 0 ? -1 : 1;
                if (!TryMove(x, y, x + dir, y))
                    TryMove(x, y, x - dir, y);
            }
        }

        private void TryIgniteNearby(int x, int y)
        {
            TryIgnite(x - 1, y);
            TryIgnite(x + 1, y);
            TryIgnite(x, y - 1);
            TryIgnite(x, y + 1);
        }

        private void TryIgnite(int x, int y)
        {
            if (!IsInsideGrid(x, y)) return;

            if (grid[x, y].Type == CellType.Wood)
                grid[x, y] = new Cell(CellType.Fire);
            else if (grid[x, y].Type == CellType.Water)
                grid[x, y] = new Cell(CellType.Steam);
        }

        private void TryBurn(int x, int y)
        {
            if (grid[x, y].Type == CellType.Wood && IsAdjacentToFire(x, y))
                grid[x, y] = new Cell(CellType.Fire);
        }

        private void TryLavaBurn(int x, int y)
        {
            foreach (var (dx, dy) in new[] { (-1, 0), (1, 0), (0, -1), (0, 1) })
            {
                int nx = x + dx, ny = y + dy;
                if (IsInsideGrid(nx, ny) && grid[nx, ny].Type == CellType.Wood)
                    grid[nx, ny] = new Cell(CellType.Fire);
            }
        }

        private void TryCoolLava(int x, int y)
        {
            foreach (var (dx, dy) in new[] { (-1, 0), (1, 0), (0, -1), (0, 1) })
            {
                int nx = x + dx, ny = y + dy;
                if (IsInsideGrid(nx, ny) &&
                    (grid[nx, ny].Type == CellType.Ice || grid[nx, ny].Type == CellType.Water))
                {
                    grid[x, y] = new Cell(CellType.Rock);
                }
            }
        }

        private void TryMeltIce(int x, int y)
        {
            if (IsAdjacentToFire(x, y))
                grid[x, y] = new Cell(CellType.Water);
        }

        private void TryHeatTransfer(int x, int y)
        {
            if (IsAdjacentToFire(x, y) || IsAdjacentToLava(x, y))
            {
                Raylib.DrawRectangleLines(x * CellSize, y * CellSize, CellSize, CellSize, Color.Orange);
            }
        }

        public void ClearGrid()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    SetCell(x, y, new Cell(CellType.Empty));
                }
            }
        }


        private bool IsAdjacentToFire(int x, int y)
        {
            return IsAdjacentToType(x, y, CellType.Fire);
        }

        private bool IsAdjacentToLava(int x, int y)
        {
            return IsAdjacentToType(x, y, CellType.Lava);
        }

        //private bool IsAdjacentToIce(int x, int y)
        //{
        //    return IsAdjacentToType(x, y, CellType.Ice);
        //}

        private bool IsAdjacentToType(int x, int y, CellType type)
        {
            return (IsInsideGrid(x - 1, y) && grid[x - 1, y].Type == type)
                || (IsInsideGrid(x + 1, y) && grid[x + 1, y].Type == type)
                || (IsInsideGrid(x, y - 1) && grid[x, y - 1].Type == type)
                || (IsInsideGrid(x, y + 1) && grid[x, y + 1].Type == type);
        }

        public bool TryMove(int x1, int y1, int x2, int y2)
        {
            if (IsInsideGrid(x2, y2) && grid[x2, y2].IsEmpty())
            {
                grid[x2, y2] = grid[x1, y1];
                grid[x1, y1] = new Cell();
                return true;
            }
            return false;
        }

        public void Draw()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    Raylib.DrawRectangle(x * CellSize, y * CellSize, CellSize, CellSize, grid[x, y].GetColor());
        }
    }
}
