
using Game;
using Graphics;
using SDL2;
using TinyEngine.Drawing;
using TinyEngine.Ecs;
using TinyEngine.General;
using TinyEngine.Input;

if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
{
    throw new Exception("Failed to start SDL2");
}

var screen = new Screen(640, 720, "Space Game");
var spriteSheet = new SpriteSheet("Assets/SpriteSheet.png", "Assets/SpriteSheet.txt", screen);
var inputState = new InputState();

var world = new World();
world.AddResource(screen);
world.AddResource(spriteSheet);
world.AddResource(new Camera(640,720, 1.0));
world.AddResource(inputState);
var timeDelta = new TimeDelta();
world.AddResource(timeDelta);
world.AddResource(new PlayArea(new( new(0,0), 500, 600)));

world.AddComponent(new Table<WorldPosition>());
world.AddComponent(new Table<SpriteAnimation>());
world.AddComponent(new Table<Kinematics>());
world.AddComponent(new Singleton<Player>());
world.AddComponent(new Table<ConfineToPlayArea>());

var shipSpawner = world.CreateInstance<ShipSpawner>();
shipSpawner.Execute();

var playerInput = world.CreateInstance<PlayerInputSystem>();
var kinematics = world.CreateInstance<UpdateKinematicsSystem>();
var updatePlayerAnimation = world.CreateInstance<UpdatePlayerAnimations>();
var snap = world.CreateInstance<ConfineToPlayAreaSystem>();

var animationAdvance = world.CreateInstance<AnimationAdvanceSystem>();
var renderShip = world.CreateInstance<AnimateSpriteSystem>();

var game = new SpaceShipGameLoop(
    screen, 
    inputState, 
    timeDelta, 
    [
        playerInput,
        kinematics,
        snap,
        updatePlayerAnimation,
        animationAdvance, 
        renderShip
    ]);

game.Run();

screen.Dispose();
SDL.SDL_Quit();
