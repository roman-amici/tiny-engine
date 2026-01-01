using TinyEngine.Drawing;
using TinyEngine.General;

namespace Graphics;

public enum GameSprite
{
    ShipBankLeft1,
    ShipBankLeft2,
    ShipLeft1,
    ShipLeft2,
    ShipCenter1,
    ShipCenter2,
    ShipRight1,
    ShipRight2,
    ShipBankRight1,
    ShipBankRight2,
    EnemySmall1,
    EnemySmall2,
    EnemyMedium1,
    EnemyMedium2,
    EnemyLarge1,
    EnemyLarge2,
    Explosion1,
    Explosion2,
    Explosion3,
    Explosion4,
    Explosion5,
    LaserRound1,
    LaserRound2,
    LaserFlat1,
    LaserFlat2,
    PowerUpOrange1,
    PowerUpOrange2,
    PowerUpBlue1,
    PowerUpBlue2,
}
public class SpriteSheet
{
    public SpriteSheet(string imageName, string descriptionName, Screen screen)
    {
        var texture = screen.Renderer.LoadTexture(imageName);
        SpriteAtlas = new SpriteAtlas<GameSprite>(texture);

        foreach(var line in File.ReadLines(descriptionName))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }
    
            var splits = line.Split(",");
            var name = Enum.Parse<GameSprite>(splits[0]);
            var x = int.Parse(splits[1]);
            var y = int.Parse(splits[2]);

            var width = uint.Parse(splits[3]);
            var height = uint.Parse(splits[4]);

            SpriteAtlas.AddSprite(name,x,y,width,height);
        }
    }

    public Rect2D GetBounds(GameSprite spriteKey)
    {
        var dimensions = SpriteAtlas.GetSpriteDimensions(spriteKey);
        return dimensions.Scaled(2.5); // Uniform scaling for now
    }

    public SpriteAtlas<GameSprite> SpriteAtlas {get;}

    public Animations Animations {get;} = new();
}

public class Animations
{
    public Animation<GameSprite> ShipCenter {get;} = new(
        [ new(GameSprite.ShipCenter1, TimeSpan.FromMilliseconds(500)),
          new(GameSprite.ShipCenter2, TimeSpan.FromMilliseconds(500))
        ]
    );

    public Animation<GameSprite> ShipLeft {get;} = new([
        new(GameSprite.ShipLeft1, TimeSpan.FromMilliseconds(500)),
        new(GameSprite.ShipLeft2, TimeSpan.FromMilliseconds(500)),
    ]);

    public Animation<GameSprite> ShipBankLeft {get;} = new([
        new(GameSprite.ShipBankLeft1, TimeSpan.FromMilliseconds(500)),
        new(GameSprite.ShipBankLeft2, TimeSpan.FromMilliseconds(500)),
    ]);

    public Animation<GameSprite> ShipRight {get;} = new([
        new(GameSprite.ShipRight1, TimeSpan.FromMilliseconds(500)),
        new(GameSprite.ShipRight2, TimeSpan.FromMilliseconds(500)),
    ]);

    public Animation<GameSprite> ShipBankRight {get;} = new([
        new(GameSprite.ShipBankRight1, TimeSpan.FromMilliseconds(500)),
        new(GameSprite.ShipBankRight2, TimeSpan.FromMilliseconds(500)),
    ]);

    public Animation<GameSprite> LaserFlat {get;} = new([
       new(GameSprite.LaserFlat1, TimeSpan.FromMilliseconds(250)),
       new(GameSprite.LaserFlat2, TimeSpan.FromMilliseconds(250))
    ]);

    public Animation<GameSprite> LaserRound {get;} = new([
       new(GameSprite.LaserRound1, TimeSpan.FromMilliseconds(250)),
       new(GameSprite.LaserRound2, TimeSpan.FromMilliseconds(250))
    ]);

    public Animation<GameSprite> Explosion {get;} = new([
        new(GameSprite.Explosion1, TimeSpan.FromMilliseconds(250)),
        new(GameSprite.Explosion2, TimeSpan.FromMilliseconds(250)),
        new(GameSprite.Explosion3, TimeSpan.FromMilliseconds(250)),
        new(GameSprite.Explosion4, TimeSpan.FromMilliseconds(250)),
    ], false);

    public Animation<GameSprite> EnemySmall {get;} = new([
        new(GameSprite.EnemySmall1, TimeSpan.FromMilliseconds(500)),
        new(GameSprite.EnemySmall2, TimeSpan.FromMilliseconds(500)),
    ]);

    public Animation<GameSprite> EnemyMedium {get;} = new([
        new(GameSprite.EnemyMedium1, TimeSpan.FromMilliseconds(500)),
        new(GameSprite.EnemyMedium2, TimeSpan.FromMilliseconds(500)),
    ]);
    public Animation<GameSprite> EnemyLarge {get;} = new([
        new(GameSprite.EnemyLarge2, TimeSpan.FromMilliseconds(500)),
        new(GameSprite.EnemyLarge2, TimeSpan.FromMilliseconds(500)),
    ]);
}

public struct SpriteAnimation(Animation<GameSprite> animation)
{
    public Animation<GameSprite> Animation {get; private set;} = animation;
    public TimerIndex FrameIndex {get; set;}

    public void Update(TimeSpan delta)
    {
        var advance = FrameIndex;
        advance.TimeInState += delta;
        
        FrameIndex = Animation.NextIndex(advance);
    }

    public void ChangeAnimation(Animation<GameSprite> newAnimation)
    {
        Animation = newAnimation;
        if (FrameIndex.Index > Animation.Sequence.Length)
        {
            FrameIndex = new();
        }
    }

    public bool IsAtEnd()
    {
        return Animation.IsComplete(FrameIndex);
    }
}