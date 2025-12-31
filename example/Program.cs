
using Game;
using Graphics;
using SDL2;
using TinyEngine.Drawing;
using TinyEngine.Ecs;
using TinyEngine.Input;
using TinyEngine.SdlAbstractions;

if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
{
    throw new Exception($"Failed to start SDL2: {SDL.SDL_GetError()}");
}

var screen = new Screen(640, 720, "Space Game");
Font.UseFonts();
screen.AddFont(Font.LoadFont("Assets/FreeMono.ttf"));
var spriteSheet = new SpriteSheet("Assets/SpriteSheet.png", "Assets/SpriteSheet.txt", screen);
var inputState = new InputState();

var world = new World();
world.AddResource(new Random());
world.AddResource(screen);
world.AddResource(spriteSheet);
world.AddResource(new Camera(640,720, 1.0));
world.AddResource(inputState);
var timeDelta = new TimeDelta();
world.AddResource(timeDelta);
world.AddResource(new GameState());
world.AddResource(new Queue<SpawnShipMessage>());

var playArea = new PlayArea(new( new(0,0), 500, 600));
world.AddResource(playArea);
world.AddResource(new Queue<LaserSpawnContext>());

var q = new Queue<EnemySpawnContext>();
world.AddResource(q);
world.AddResource(new Queue<DamagedMessage>());

world.AddComponent(new Table<WorldPosition>());
world.AddComponent(new Table<SpriteAnimation>());
world.AddComponent(new Table<Sprite<GameSprite>>());
world.AddComponent(new Table<Kinematics>());
world.AddComponent(new Singleton<Player>());
world.AddComponent(new Table<ConfineToPlayArea>());
world.AddComponent(new Table<DeleteOnExitPlayArea>());
world.AddComponent(new Table<Damage>());
world.AddComponent(new Table<Health>());
world.AddComponent(new Table<MovementPlan>());
world.AddComponent(new Table<MovementIndex>());
world.AddComponent(new Table<Enemy>());
world.AddComponent(new Table<DestroyOnAnimationEnd>());
world.AddComponent(new Table<ShootRandomly>());
world.AddComponent(new Table<FlashState>());
world.AddComponent(new Table<Score>());

var enemySpawner = world.CreateInstance<EnemySpawner>();
q.Enqueue(new(EnemyType.Small, MovementPlan.Diamond(
    150, 
    playArea.Area.Center,
    TimeSpan.FromSeconds(1.0))));
enemySpawner.Execute();

var shipSpawner = world.CreateInstance<ShipSpawner>();
var playerInput = world.CreateInstance<PlayerInputSystem>();
var kinematics = world.CreateInstance<UpdateKinematicsSystem>();
var updatePlayerAnimation = world.CreateInstance<UpdatePlayerAnimations>();
var snap = world.CreateInstance<ConfineToPlayAreaSystem>();
var cleanupExit = world.CreateInstance<DeleteOnExitPlayAreaSystem>();
var cleanupAnimation = world.CreateInstance<DeleteOnAnimationEndSystem>();
var explodeOnDeath = world.CreateInstance<ExplodeOnDeathSystem>();
var flashSystem = world.CreateInstance<FlashOnDamagedSystem>();
var shootRandomly = world.CreateInstance<ShootRandomlySystem>();
var shoot = world.CreateInstance<LaserSpawner>();
var trajectoryMove = world.CreateInstance<TrajectoryMovementSystem>();
var impact = world.CreateInstance<DamageSystem>();
var endGame = world.CreateInstance<GameEndSystem>();

var drawUI = world.CreateInstance<DrawUISystem>();
var animationAdvance = world.CreateInstance<AnimationAdvanceSystem>();
var transformSprites = world.CreateInstance<TransformSpriteSystem>();
var renderSprites = world.CreateInstance<RenderSpriteSystem>();

var game = new SpaceShipGameLoop(
    screen, 
    inputState, 
    timeDelta, 
    [
        shipSpawner,
        playerInput,
        kinematics,
        snap,
        trajectoryMove,
        shootRandomly,
        shoot,
        impact,
        explodeOnDeath,
        cleanupExit,
        cleanupAnimation,
        endGame,
        
        updatePlayerAnimation,
        animationAdvance,
        drawUI,
        flashSystem,
        transformSprites,
        renderSprites
    ]);

game.Run();

screen.Dispose();
SDL.SDL_Quit();
