using System.Drawing;
using Graphics;
using TinyEngine.Drawing;
using TinyEngine.Ecs;

namespace Game;

public class DrawUISystem(
    Screen screen,
    Camera camera,
    GameState state,
    SpriteSheet spriteSheet,
    PlayArea area) : GameSystem
{
    public override void Execute()
    {
        screen.DrawBorder(area.Area, Color.Blue);

        DrawLives();
        DrawSidebar();

        if (state.Lives < 0)
        {
            screen.DrawTextCentered(new("Game Over", 64, Color.White), area.Area.Center);
        }

        screen.SetDrawColor(Color.Black);
    }

    private void DrawLives()
    {
        var diffY = screen.Window.Height - area.Area.BottomLeft.Y;
        var middleY = area.Area.BottomLeft.Y + (diffY / 2);
        var offsetX = 20;

        var textPosition = screen.DrawText(new("Lives: ", 24, Color.White), new(offsetX,middleY));

        var bounds = spriteSheet.GetBounds(GameSprite.ShipCenter1);

        var xStart = textPosition.TopRight.X;
        var yStart = middleY - (bounds.Height / 4);
        for (var i = 0; i < state.Lives; i++)
        {
            var position = bounds.WithTopLeft( new(xStart, yStart));
            spriteSheet.SpriteAtlas.DrawSprite(new(GameSprite.ShipCenter1)
            {
                Transform = position
            }, screen, camera);

            xStart += bounds.Width + 2.0;
        }
    }

    private void DrawSidebar()
    {
        var xStart = area.Area.TopRight.X + 5.0;
        var yStart = 50.0;


        screen.DrawText(new($"Wave {state.WaveNumber}", 24, Color.White), new(xStart, yStart));

        yStart += 100.0;

        var size = screen.DrawText(new("Score", 24, Color.White), new(xStart,yStart));

        yStart += size.Height + 2.0;
        screen.DrawText(new($"{state.Score:N0}", 24, Color.White), new(xStart, yStart));
    }
}