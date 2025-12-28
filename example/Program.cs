
using Game;
using Graphics;
using SDL2;
using TinyEngine.Drawing;
using TinyEngine.Ecs;
using TinyEngine.General;

if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
{
    throw new Exception("Failed to start SDL2");
}

var screen = new Screen(640, 720, "Space Game");
var spriteSheet = new SpriteSheet("Assets/SpriteSheet.png", "Assets/SpriteSheet.txt", screen);

var world = new World();
world.AddResource(screen);
world.AddResource(spriteSheet);
world.AddResource(new Camera(640,720, 2.5));
var timeDelta = new TimeDelta();
world.AddResource(timeDelta);

world.AddComponent(new Table<Sprite>());
world.AddComponent(new Table<WorldPosition>());
world.AddComponent(new Table<SpriteAnimation>());

var shipSpawner = world.CreateInstance<ShipSpawner>();
shipSpawner.Execute();

var animationAdvance = world.CreateInstance<AnimationAdvanceSystem>();
var renderShip = world.CreateInstance<AnimateSpriteSystem>();

var game = new SpaceShipGameLoop(screen, timeDelta, [animationAdvance, renderShip]);

game.Run();

screen.Dispose();
SDL.SDL_Quit();
