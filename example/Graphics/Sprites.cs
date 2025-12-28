using TinyEngine.Drawing;
using TinyEngine.General;
using TinyEngine.SdlAbstractions;

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

    public SpriteAtlas<GameSprite> SpriteAtlas {get;}

    public Animations Animations {get;} = new();
}

public class Animations
{
    public static Sprite<GameSprite> MakeSprite(GameSprite spriteKey)
    {
        return new Sprite<GameSprite>()
        {
            SpriteKey = spriteKey,
            Scale = 2.5
        };
    }

    public Animation<GameSprite> ShipCenter {get;} = new(
        [ new(MakeSprite(GameSprite.ShipCenter1), TimeSpan.FromMilliseconds(500)),
          new(MakeSprite(GameSprite.ShipCenter2), TimeSpan.FromMilliseconds(500))
        ]
    );

    public Animation<GameSprite> ShipLeft {get;} = new([
        new(MakeSprite(GameSprite.ShipLeft1), TimeSpan.FromMilliseconds(500)),
        new(MakeSprite(GameSprite.ShipLeft2), TimeSpan.FromMilliseconds(500)),
    ]);

    public Animation<GameSprite> ShipBankLeft {get;} = new([
        new(MakeSprite(GameSprite.ShipBankLeft1), TimeSpan.FromMilliseconds(500)),
        new(MakeSprite(GameSprite.ShipBankLeft2), TimeSpan.FromMilliseconds(500)),
    ]);

    public Animation<GameSprite> ShipRight {get;} = new([
        new(MakeSprite(GameSprite.ShipRight1), TimeSpan.FromMilliseconds(500)),
        new(MakeSprite(GameSprite.ShipRight2), TimeSpan.FromMilliseconds(500)),
    ]);

    public Animation<GameSprite> ShipBankRight {get;} = new([
        new(MakeSprite(GameSprite.ShipBankRight1), TimeSpan.FromMilliseconds(500)),
        new(MakeSprite(GameSprite.ShipBankRight2), TimeSpan.FromMilliseconds(500)),
    ]);
}

public struct SpriteAnimation(Animation<GameSprite> animation)
{
    public Animation<GameSprite> Animation {get; private set;} = animation;
    public FrameIndex FrameIndex {get; set;}

    public void Update(TimeSpan delta)
    {
        var advance = FrameIndex;
        advance.FrameTimeElapsed += delta;
        
        FrameIndex = Animation.NextIndex(advance);
    }

    public void ChangeAnimation(Animation<GameSprite> newAnimation)
    {
        Animation = newAnimation;
        if (FrameIndex.Index > Animation.Frames.Length)
        {
            FrameIndex = new();
        }
    }
}