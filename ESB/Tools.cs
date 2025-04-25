using Raylib_cs;

namespace ESB
{
    public class Tools
    {
        public enum ToolType
        {
            None,
            Sand,
            Water,
            Fire,
            Smoke,
            Wall,
            Lava,
            Ice,
            Wood,
            Delete
        }

        public static ToolType CurrentTool { get; set; } = ToolType.Sand;

        public void Update(CellGrid grid)
        {
            grid.Update();
        }
    }
}
