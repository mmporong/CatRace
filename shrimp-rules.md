# CatRace Project Development Guidelines

## Project Overview

**Project Type**: Unity 2D Roguelike Cat Racing Game
**Technology Stack**: Unity 2023.x, Universal Render Pipeline (URP), 2D Animation, Input System, Tilemap System
**Target Platform**: Multi-platform 2D game

## Naming Conventions

### Variable Naming
- **Private variables**: Use camelCase
  - ✅ `private float moveSpeed;`
  - ✅ `private int healthPoints;`
  - ❌ `private float MoveSpeed;`
  - ❌ `private int health_points;`

- **Public variables**: Use PascalCase (uppercase start)
  - ✅ `public float MoveSpeed;`
  - ✅ `public int HealthPoints;`
  - ❌ `public float moveSpeed;`
  - ❌ `public int healthPoints;`

- **SerializeField variables**: Use camelCase with [SerializeField] attribute
  - ✅ `[SerializeField] private float moveSpeed;`
  - ✅ `[SerializeField] private GameObject catPrefab;`

### Method and Class Naming
- **Methods**: Use PascalCase
  - ✅ `public void MoveCat()`
  - ✅ `private void OnTriggerEnter2D()`
  - ❌ `public void moveCat()`

- **Classes**: Use PascalCase, match filename
  - ✅ `public class Cat : MonoBehaviour` (in Cat.cs)
  - ✅ `public class RaceManager : MonoBehaviour` (in RaceManager.cs)

- **Interfaces**: Use PascalCase with 'I' prefix
  - ✅ `public interface ICatAbility`
  - ✅ `public interface IRaceable`

### Constants and Enums
- **Constants**: Use PascalCase
  - ✅ `public const float MAX_SPEED = 10f;`
  - ✅ `private const int DEFAULT_HEALTH = 100;`

- **Enums**: Use PascalCase
  - ✅ `public enum CatType { Speedster, Tank, Agile }`
  - ✅ `public enum RaceState { Waiting, Racing, Finished }`

## Unity Development Standards

### MonoBehaviour Patterns
- **Always inherit from MonoBehaviour** for game objects
- **Use Start() and Update()** only when necessary
- **Prefer Awake()** for initialization that doesn't depend on other objects
- **Use OnEnable()/OnDisable()** for event subscription/unsubscription

```csharp
// ✅ Correct MonoBehaviour pattern
public class Cat : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    public float MoveSpeed { get; set; }
    
    private void Awake()
    {
        // Initialize components
    }
    
    private void OnEnable()
    {
        // Subscribe to events
    }
    
    private void OnDisable()
    {
        // Unsubscribe from events
    }
}
```

### Component References
- **Cache component references** in Awake() or Start()
- **Use [SerializeField]** for inspector-assigned references
- **Never use GetComponent() in Update()**

```csharp
// ✅ Correct component caching
private Rigidbody2D rb;
private SpriteRenderer spriteRenderer;

private void Awake()
{
    rb = GetComponent<Rigidbody2D>();
    spriteRenderer = GetComponent<SpriteRenderer>();
}
```

### SerializeField Usage
- **Always use [SerializeField]** for private fields that need inspector access
- **Use [Header("")]** to organize inspector fields
- **Use [Range()]** for numeric sliders

```csharp
[Header("Movement Settings")]
[SerializeField] private float moveSpeed = 5f;
[SerializeField] [Range(0f, 10f)] private float jumpForce = 5f;
[Header("Cat Properties")]
[SerializeField] private CatType catType;
```

## 2D Game Development Rules

### Sprite Management
- **Place all sprites in Assets/@Sprites/**
- **Use SpriteRenderer** for 2D rendering
- **Set sprite import settings** to Sprite (2D and UI)
- **Use Sprite Atlas** for performance optimization

### 2D Physics
- **Use Rigidbody2D** for physics-based movement
- **Use Collider2D** for collision detection
- **Prefer BoxCollider2D** for rectangular objects
- **Use CircleCollider2D** for circular objects

```csharp
// ✅ Correct 2D physics setup
[SerializeField] private Rigidbody2D rb;
[SerializeField] private BoxCollider2D collider;

private void Move(Vector2 direction)
{
    rb.velocity = direction * moveSpeed;
}
```

### Animation System
- **Use Animator** for character animations
- **Create Animation Controllers** in Assets/@Scripts/Animation/
- **Use Animator Parameters** for state transitions
- **Name animation states** clearly (e.g., "Idle", "Running", "Jumping")

### Tilemap Usage
- **Use Tilemap** for level generation
- **Create Tile assets** in Assets/@Sprites/Tiles/
- **Use Rule Tiles** for complex tile patterns
- **Organize tilemaps** by layer (Background, Foreground, Collision)

## Roguelike Game Mechanics

### Procedural Generation
- **Create level generators** in Assets/@Scripts/Generation/
- **Use ScriptableObjects** for generation rules
- **Implement seed-based generation** for reproducibility
- **Separate generation logic** from game logic

```csharp
// ✅ Correct procedural generation pattern
[CreateAssetMenu(fileName = "LevelGenerator", menuName = "CatRace/Level Generator")]
public class LevelGenerator : ScriptableObject
{
    [SerializeField] private int seed;
    [SerializeField] private LevelConfig config;
    
    public LevelData GenerateLevel()
    {
        // Generation logic here
    }
}
```

### Game State Management
- **Use singleton pattern** for game managers
- **Implement state machine** for game states
- **Use events** for state transitions
- **Persist important data** using PlayerPrefs or JSON

### Character Progression
- **Create CatStats** ScriptableObject for cat properties
- **Implement upgrade system** with clear progression paths
- **Use events** for stat changes
- **Save progression data** between runs

### Level Generation
- **Create modular level pieces** as prefabs
- **Use weighted random selection** for piece placement
- **Implement difficulty scaling** based on progression
- **Ensure level connectivity** and playability

## File Organization Standards

### Directory Structure
- **Assets/@Scripts/**: All C# scripts
  - **Assets/@Scripts/Managers/**: Game managers and singletons
  - **Assets/@Scripts/Player/**: Player-related scripts
  - **Assets/@Scripts/Enemies/**: Enemy scripts
  - **Assets/@Scripts/UI/**: User interface scripts
  - **Assets/@Scripts/Generation/**: Procedural generation scripts
  - **Assets/@Scripts/Data/**: ScriptableObjects and data classes

- **Assets/@Sprites/**: All sprite assets
  - **Assets/@Sprites/Cats/**: Cat character sprites
  - **Assets/@Sprites/Environment/**: Environment sprites
  - **Assets/@Sprites/UI/**: UI element sprites
  - **Assets/@Sprites/Tiles/**: Tilemap sprites

- **Assets/@Scenes/**: All Unity scenes
  - **Assets/@Scenes/MainMenu.unity**
  - **Assets/@Scenes/Gameplay.unity**
  - **Assets/@Scenes/LevelSelect.unity**

### File Naming
- **Script files**: Match class name exactly
- **Prefab files**: Use descriptive names (e.g., "Cat_Speedster.prefab")
- **Scene files**: Use PascalCase (e.g., "MainMenu.unity")
- **Asset files**: Use descriptive names with underscores (e.g., "cat_idle_animation.anim")

## Code Architecture Rules

### Single Responsibility Principle
- **One class per responsibility**
- **Separate concerns** (movement, combat, UI, etc.)
- **Use composition** over inheritance
- **Create focused, testable classes**

### Component-Based Architecture
- **Use MonoBehaviour components** for game object behavior
- **Create reusable components** for common functionality
- **Use interfaces** for shared behavior
- **Implement dependency injection** where appropriate

### Event-Driven Patterns
- **Use UnityEvents** for inspector-assignable events
- **Use C# events** for code-based event handling
- **Implement observer pattern** for loose coupling
- **Use ScriptableObject events** for global communication

```csharp
// ✅ Correct event pattern
public class GameEvents : ScriptableObject
{
    public UnityEvent<Cat> OnCatFinished;
    public UnityEvent<int> OnScoreChanged;
}
```

### Data Management
- **Use ScriptableObjects** for game data
- **Create data containers** for complex data structures
- **Implement data validation** in ScriptableObjects
- **Use JSON serialization** for save data

## Asset Management Rules

### Sprite Import Settings
- **Set Texture Type** to Sprite (2D and UI)
- **Use appropriate compression** based on usage
- **Set Pixels Per Unit** consistently (recommended: 100)
- **Use Sprite Atlas** for multiple sprites

### Prefab Organization
- **Create prefab variants** for different cat types
- **Use nested prefabs** for complex objects
- **Organize prefabs** by category in folders
- **Version control prefabs** properly

### Resource Loading
- **Use Resources folder** sparingly
- **Prefer direct references** over Resources.Load()
- **Use Addressables** for large asset bundles
- **Implement proper unloading** to prevent memory leaks

## Prohibited Actions

### Code Prohibitions
- ❌ **Never use GetComponent() in Update()**
- ❌ **Never use public fields** without [SerializeField]
- ❌ **Never use FindObjectOfType()** in runtime code
- ❌ **Never use string literals** for tags or layer names
- ❌ **Never use magic numbers** without constants
- ❌ **Never create circular dependencies** between classes

### Script Creation Prohibitions
- ❌ **Never create Tester scripts** unless explicitly requested by user
- ❌ **Never create Guide scripts** unless explicitly requested by user
- ❌ **Never create Setup scripts** unless explicitly requested by user
- ❌ **Focus only on main functionality scripts** when user requests new features

### Asset Prohibitions
- ❌ **Never place scripts** outside Assets/@Scripts/
- ❌ **Never place sprites** outside Assets/@Sprites/
- ❌ **Never place scenes** outside Assets/@Scenes/
- ❌ **Never modify .meta files** manually
- ❌ **Never use duplicate asset names** in the same folder

### Architecture Prohibitions
- ❌ **Never create god classes** that do everything
- ❌ **Never use static classes** for game state
- ❌ **Never hardcode values** that should be configurable
- ❌ **Never ignore null reference checks**
- ❌ **Never use goto statements**

## Communication Standards

### Language Requirements
- **모든 코드 주석은 한국어로 작성**
- **변수명, 메서드명, 클래스명은 영어 사용**
- **AI 에이전트와의 모든 커뮤니케이션은 한국어로 진행**
- **문서화 및 설명은 한국어로 작성**

```csharp
// ✅ 올바른 주석 작성법
public class Cat : MonoBehaviour
{
    [SerializeField] private float moveSpeed; // 고양이 이동 속도
    
    /// <summary>
    /// 고양이를 지정된 방향으로 이동시킵니다
    /// </summary>
    /// <param name="direction">이동할 방향 벡터</param>
    public void MoveCat(Vector2 direction)
    {
        // 이동 로직 구현
    }
}
```

## AI Decision Guidelines

### When Adding New Features
1. **Check existing similar functionality** first
2. **Follow established patterns** in the codebase
3. **Create appropriate ScriptableObjects** for configurable data
4. **Use events** for communication between systems
5. **Place files** in correct directories according to function

### When Modifying Existing Code
1. **Maintain backward compatibility** when possible
2. **Update related files** if changing interfaces
3. **Follow naming conventions** consistently
4. **Add proper documentation** for complex logic
5. **Test changes** in multiple scenarios

### When Handling Ambiguous Requests
1. **Prioritize performance** for gameplay-critical code
2. **Choose maintainability** over cleverness
3. **Follow Unity best practices** for 2D games
4. **Consider roguelike game patterns** for mechanics
5. **Ask for clarification** only when multiple valid approaches exist

### Code Quality Priorities
1. **Readability** - Code should be self-documenting
2. **Performance** - Optimize for 60+ FPS on target platforms
3. **Maintainability** - Easy to modify and extend
4. **Consistency** - Follow established patterns
5. **Testing** - Write testable, modular code

## File Interaction Standards

### When Modifying Cat.cs
- **Check RaceManager.cs** for race-related functionality
- **Update CatStats ScriptableObject** if changing cat properties
- **Modify UI scripts** if changing display properties
- **Update animation controllers** if changing movement patterns

### When Modifying Game Managers
- **Update all dependent scripts** that reference the manager
- **Check event subscriptions** in other classes
- **Update UI elements** that display manager data
- **Modify save/load systems** if changing persistent data

### When Adding New Cat Types
- **Create new ScriptableObject** for cat data
- **Update CatType enum** if using enum-based system
- **Create new prefab variants** for different cat types
- **Update UI selection screens** if applicable
- **Modify generation systems** if cats affect level generation

## Performance Guidelines

### Optimization Rules
- **Use object pooling** for frequently created/destroyed objects
- **Cache component references** to avoid repeated GetComponent() calls
- **Use coroutines** for non-critical updates instead of Update()
- **Implement LOD systems** for complex sprites
- **Use sprite atlases** to reduce draw calls

### Memory Management
- **Unsubscribe from events** in OnDisable()
- **Destroy unused objects** properly
- **Use weak references** where appropriate
- **Implement proper cleanup** in singleton patterns
- **Monitor memory usage** during development

This document serves as the definitive guide for AI agents working on the CatRace project. All code modifications must follow these standards to ensure consistency, maintainability, and optimal performance.
