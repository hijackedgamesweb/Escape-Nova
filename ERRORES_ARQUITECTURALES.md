# Análisis de Errores Arquitecturales - Escape Nova

## Índice
1. [Introducción](#introducción)
2. [Errores Arquitecturales Identificados](#errores-arquitecturales-identificados)
3. [Soluciones Propuestas](#soluciones-propuestas)
4. [Priorización de Mejoras](#priorización-de-mejoras)
5. [Plan de Implementación](#plan-de-implementación)

---

## Introducción

Este documento analiza los principales problemas arquitecturales identificados en el proyecto **Escape Nova**, un juego de gestión de recursos espaciales desarrollado en Unity. El análisis se centra en aspectos estructurales del código que pueden dificultar el mantenimiento, la escalabilidad y la testabilidad del proyecto.

### Contexto del Proyecto
- **Motor**: Unity (C#)
- **Patrón Principal**: MVC modificado con elementos de ECS
- **Tamaño**: ~252 archivos de código C#
- **Estado**: En desarrollo (fase Alpha/Beta)

---

## Errores Arquitecturales Identificados

### 1. **Uso Excesivo del Patrón Singleton**

#### Descripción del Problema
El proyecto implementa múltiples managers como Singletons:
- `SaveManager`
- `AudioManager`
- `NotificationManager`
- `WorldManager`
- `UIManager`
- `AstrariumManager`
- `SceneTransition`

**Ubicación**: `Assets/Code/Scripts/Patterns/Singleton/`

#### ¿Por qué es un problema?

1. **Acoplamiento fuerte**: Los Singletons crean dependencias ocultas y globales que dificultan el testing y la reutilización de código.

2. **Estado global mutable**: El estado compartido entre diferentes partes del sistema puede llevar a bugs difíciles de rastrear.

3. **Dificultad para testing**: Los Singletons son difíciles de mockear y pueden causar problemas en tests unitarios al compartir estado entre pruebas.

4. **Violación del principio de responsabilidad única**: Los Singletons tienden a acumular responsabilidades adicionales con el tiempo.

5. **Problemas con la carga de escenas**: En Unity, los Singletons con `DontDestroyOnLoad` pueden causar duplicados y problemas de sincronización.

#### Ejemplo del Código Actual
```csharp
// Assets/Code/Scripts/Patterns/Singleton/Singleton.cs
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    
    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
}
```

**Impacto**: Alto - Afecta a toda la arquitectura del proyecto

---

### 2. **Uso Mixto de Service Locator y Singleton**

#### Descripción del Problema
El proyecto utiliza simultáneamente dos patrones para gestión de dependencias:
- **Service Locator** (`Assets/Code/Scripts/Patterns/ServiceLocator/`)
- **Singleton Pattern**

Esto genera confusión sobre cuál patrón usar en cada situación y duplica la funcionalidad.

#### ¿Por qué es un problema?

1. **Inconsistencia**: No hay claridad sobre cuándo usar uno u otro patrón.

2. **Mantenimiento complejo**: Los desarrolladores deben conocer y entender ambos patrones.

3. **Anti-patrones**: Tanto Singleton como Service Locator son considerados anti-patrones en arquitecturas modernas, ya que ocultan dependencias.

#### Ejemplo del Código Actual
```csharp
// Uso de Service Locator
ServiceLocator.RegisterService<StorageSystem>(playerStorage);
var storage = ServiceLocator.GetService<StorageSystem>();

// Uso de Singleton
var saveManager = SaveManager.Instance;
```

**Estadísticas**:
- 66 usos de `ServiceLocator.GetService` en el código
- 8+ clases implementando Singleton

**Impacto**: Alto - Afecta la consistencia del proyecto

---

### 3. **Eventos Estáticos Globales**

#### Descripción del Problema
El proyecto usa eventos estáticos para la comunicación entre sistemas:
- `SystemEvents`
- `GameEvents`
- `ConstructionEvents`
- `DiplomacyEvents`
- `ResearchEvents`
- `UIEvents`
- `MissionEvents`
- `CraftingEvents`

**Ubicación**: `Assets/Code/Scripts/Core/Events/`

#### ¿Por qué es un problema?

1. **Dificultad para rastrear flujo de datos**: Los eventos estáticos hacen difícil saber qué componentes están escuchando y respondiendo a eventos.

2. **Memory leaks**: Las suscripciones a eventos estáticos no se limpian automáticamente, causando referencias que previenen la recolección de basura.

3. **Orden de ejecución impredecible**: No hay garantía del orden en que se ejecutan los listeners.

4. **Testing complejo**: Los eventos estáticos mantienen estado entre tests.

5. **Estado global**: Similar a los Singletons, crean dependencias ocultas globales.

#### Ejemplo del Código Actual
```csharp
// Assets/Code/Scripts/Core/Events/SystemEvents.cs
public static class SystemEvents
{
    public static event Action OnResearchUnlocked;
    public static event Action OnInventoryUnlocked;
    public static event Action OnConstellationsUnlocked;
    public static event Action OnDiplomacyUnlocked;
    // ... más eventos estáticos
}
```

**Impacto**: Medio-Alto - Dificulta el debugging y mantenimiento

---

### 4. **Separación Poco Clara entre Lógica de Negocio y Presentación**

#### Descripción del Problema
Los managers y sistemas mezclan lógica de negocio con código de UI y presentación.

#### ¿Por qué es un problema?

1. **Violación de SRP**: Una clase tiene múltiples razones para cambiar.

2. **Dificultad para reutilizar lógica**: La lógica de negocio está acoplada a la implementación de UI específica.

3. **Testing complicado**: Requiere un entorno Unity completo para testear lógica de negocio.

4. **Mantenimiento difícil**: Cambios en UI pueden afectar lógica de negocio y viceversa.

#### Ejemplo
```csharp
// WorldManager mezcla lógica de juego con manejo de UI
public class WorldManager : InGameSingleton<WorldManager>, ISaveable
{
    // Lógica de negocio
    [SerializeField] List<ResourceData> _worldResources = new();
    [SerializeField] private InventoryData _startingInventory;
    
    // Referencias a UI/Managers
    [SerializeField] private CivilizationManager _civilizationManager;
    
    // Mezcla de responsabilidades
}
```

**Impacto**: Medio - Afecta mantenibilidad y testabilidad

---

### 5. **Falta de Interfaces y Abstracción**

#### Descripción del Problema
Aunque existen algunas interfaces (`ISaveable`, `ICommand`, `IState`), muchas clases dependen directamente de implementaciones concretas en lugar de abstracciones.

#### ¿Por qué es un problema?

1. **Acoplamiento fuerte**: Las clases están fuertemente acopladas a implementaciones específicas.

2. **Dificultad para testing**: No se pueden crear mocks o stubs fácilmente.

3. **Inflexibilidad**: Cambiar implementaciones requiere modificar múltiples clases.

4. **Violación del principio de inversión de dependencias**: Las clases de alto nivel dependen de clases de bajo nivel.

#### Ejemplos de Mejora Necesaria
- Los managers podrían depender de interfaces en lugar de clases concretas
- Los sistemas deberían comunicarse a través de interfaces bien definidas
- Las dependencias deberían inyectarse en lugar de ser buscadas (Service Locator/Singleton)

**Impacto**: Medio - Afecta extensibilidad y testing

---

### 6. **Gestión Manual de Dependencias**

#### Descripción del Problema
Las dependencias se gestionan manualmente mediante:
- Service Locator registration en `Awake()`
- Referencias `[SerializeField]` en el Inspector
- Acceso directo a Singletons con `.Instance`

#### ¿Por qué es un problema?

1. **Dependencias ocultas**: No es claro qué depende de qué sin revisar el código.

2. **Errores en tiempo de ejecución**: Las dependencias faltantes solo se descubren al ejecutar el juego.

3. **Difícil de refactorizar**: Cambiar la estructura de dependencias es propenso a errores.

4. **Testing complejo**: Configurar el estado para tests requiere mucho setup manual.

#### Ejemplo del Problema
```csharp
protected override void Awake()
{
    base.Awake();
    _invoker = new CommandInvoker();
    
    // Registro manual en Service Locator
    StorageSystem playerStorage = new StorageSystem(_worldResources, _startingInventory);
    ServiceLocator.RegisterService<StorageSystem>(playerStorage);
    
    _player = new Entity.Player.Player(_invoker, _playerData, playerStorage);
}

private async void Start()
{
    // Obtención manual del Service Locator
    _gameTime = ServiceLocator.GetService<IGameTime>();
    
    if (_gameTime != null)
    {
        _gameTime.OnCycleCompleted += UpdateWorld;
    }
}
```

**Impacto**: Alto - Afecta robustez y mantenibilidad

---

### 7. **Inicialización Compleja y Orden de Dependencias**

#### Descripción del Problema
La inicialización está repartida entre `Awake()`, `Start()`, y ocasionalmente métodos `async Start()`, lo que puede causar problemas de orden de ejecución.

#### ¿Por qué es un problema?

1. **Race conditions**: Dependencias pueden no estar listas cuando se necesitan.

2. **Orden de inicialización implícito**: Unity ejecuta `Awake()` y `Start()` en orden de carga de escena, que puede variar.

3. **Difícil de debuggear**: Los errores de inicialización solo aparecen en ciertos escenarios.

4. **Código frágil**: Añadir nuevas dependencias puede romper la inicialización existente.

#### Ejemplo del Problema
```csharp
private async void Start()
{
    _gameTime = ServiceLocator.GetService<IGameTime>();
    // ¿Qué pasa si IGameTime no está registrado todavía?
    
    if (_gameTime != null)
    {
        _gameTime.OnCycleCompleted += UpdateWorld;
    }
    
    // Carga asíncrona puede completarse antes o después que otras inicializaciones
    if (SaveManager.Instance != null && SaveManager.Instance.SlotExists())
    {
        await SaveManager.Instance.LoadSlotAsync();
    }
}
```

**Impacto**: Medio - Puede causar bugs intermitentes

---

### 8. **Uso Excesivo de SerializeField para Dependencias**

#### Descripción del Problema
Muchas dependencias se configuran mediante `[SerializeField]` en el Inspector de Unity en lugar de usar inyección de dependencias programática.

#### ¿Por qué es un problema?

1. **Error humano**: Fácil olvidar asignar referencias en el Inspector.

2. **Errores silenciosos**: Referencias no asignadas solo se descubren en tiempo de ejecución.

3. **Refactoring difícil**: Cambiar estructura de objetos requiere actualizar muchas escenas/prefabs.

4. **No escalable**: Con muchos objetos, mantener referencias se vuelve inmanejable.

5. **Dificulta testing**: Los tests necesitan simular el entorno de Unity completo.

#### Ejemplo
```csharp
public class WorldManager : InGameSingleton<WorldManager>, ISaveable
{
    [SerializeField] List<ResourceData> _worldResources = new();
    [SerializeField] private InventoryData _startingInventory;
    [SerializeField] PlayerSO _playerData;
    [SerializeField] List<CivilizationSO> _civilizationSOs = new();
    [SerializeField] private CivilizationManager _civilizationManager;
    // Muchas dependencias configuradas manualmente
}
```

**Impacto**: Medio - Afecta mantenibilidad y robustez

---

### 9. **Falta de Separación por Capas**

#### Descripción del Problema
No hay una clara separación entre capas arquitecturales:
- Capa de Presentación (UI)
- Capa de Lógica de Negocio (Game Logic)
- Capa de Datos (Persistence)
- Capa de Infraestructura (Unity-specific)

#### ¿Por qué es un problema?

1. **Acoplamiento alto**: Cambios en una parte del sistema afectan otras partes.

2. **Dificultad para testear**: No se puede testear lógica de negocio independientemente.

3. **Difícil de entender**: No está claro dónde va cada tipo de código.

4. **Portabilidad limitada**: La lógica de negocio está acoplada a Unity.

**Impacto**: Alto - Afecta toda la estructura del proyecto

---

### 10. **Estado Mutable Compartido**

#### Descripción del Problema
Múltiples sistemas comparten y modifican estado mutable directamente, especialmente a través de eventos estáticos y Singletons.

#### ¿Por qué es un problema?

1. **Race conditions**: Modificaciones concurrentes pueden causar estados inconsistentes.

2. **Difícil de debuggear**: No está claro qué sistema modificó el estado y cuándo.

3. **Testing complejo**: El estado compartido entre tests puede causar fallos intermitentes.

4. **Bugs sutiles**: Efectos secundarios no intencionados son difíciles de detectar.

#### Ejemplo
```csharp
// SystemEvents mantiene estado global mutable
public static class SystemEvents
{
    public static bool IsResearchUnlocked { get; private set; }
    public static bool IsInventoryUnlocked { get; private set; }
    public static bool IsConstellationsUnlocked { get; private set; }
    public static bool IsDiplomacyUnlocked { get; set; } // ¡Incluso modificable públicamente!
    
    // Estado compartido globalmente
}
```

**Impacto**: Medio - Puede causar bugs difíciles de rastrear

---

## Soluciones Propuestas

### Solución 1: Reemplazar Singletons con Inyección de Dependencias

#### Implementación
1. **Crear un sistema de Dependency Injection Container**:
```csharp
public class GameContainer
{
    private Dictionary<Type, object> _services = new();
    
    public void Register<T>(T service)
    {
        _services[typeof(T)] = service;
    }
    
    public T Resolve<T>()
    {
        if (_services.TryGetValue(typeof(T), out var service))
        {
            return (T)service;
        }
        throw new Exception($"Service {typeof(T)} not registered");
    }
}
```

2. **Modificar managers para aceptar dependencias**:
```csharp
// ANTES
public class SaveManager : Singleton<SaveManager>
{
    public void Save() { ... }
}

// DESPUÉS
public class SaveManager : ISaveManager
{
    private readonly IFileSystem _fileSystem;
    
    public SaveManager(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }
    
    public void Save() { ... }
}
```

3. **Inicializar en un punto único**:
```csharp
public class GameBootstrapper : MonoBehaviour
{
    private void Awake()
    {
        var container = new GameContainer();
        
        // Registrar servicios en orden correcto
        var fileSystem = new UnityFileSystem();
        container.Register<IFileSystem>(fileSystem);
        
        var saveManager = new SaveManager(fileSystem);
        container.Register<ISaveManager>(saveManager);
        
        // ... más registros
    }
}
```

#### Beneficios
- Dependencias explícitas y claras
- Fácil de testear con mocks
- Orden de inicialización controlado
- Sin estado global oculto

---

### Solución 2: Implementar Event Bus en lugar de Eventos Estáticos

#### Implementación
1. **Crear un Event Bus centralizado**:
```csharp
public interface IEvent { }

public class EventBus : IEventBus
{
    private Dictionary<Type, List<object>> _subscribers = new();
    
    public void Subscribe<T>(Action<T> handler) where T : IEvent
    {
        var type = typeof(T);
        if (!_subscribers.ContainsKey(type))
        {
            _subscribers[type] = new List<object>();
        }
        _subscribers[type].Add(handler);
    }
    
    public void Unsubscribe<T>(Action<T> handler) where T : IEvent
    {
        var type = typeof(T);
        if (_subscribers.ContainsKey(type))
        {
            _subscribers[type].Remove(handler);
        }
    }
    
    public void Publish<T>(T @event) where T : IEvent
    {
        var type = typeof(T);
        if (_subscribers.ContainsKey(type))
        {
            foreach (var handler in _subscribers[type])
            {
                ((Action<T>)handler)(@event);
            }
        }
    }
}
```

2. **Definir eventos como clases**:
```csharp
// ANTES
public static class SystemEvents
{
    public static event Action OnResearchUnlocked;
    
    public static void UnlockResearch()
    {
        OnResearchUnlocked?.Invoke();
    }
}

// DESPUÉS
public class ResearchUnlockedEvent : IEvent
{
    public string ResearchId { get; }
    
    public ResearchUnlockedEvent(string researchId)
    {
        ResearchId = researchId;
    }
}

// Uso
public class ResearchSystem
{
    private readonly IEventBus _eventBus;
    
    public ResearchSystem(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }
    
    public void UnlockResearch(string researchId)
    {
        // lógica...
        _eventBus.Publish(new ResearchUnlockedEvent(researchId));
    }
}
```

#### Beneficios
- Control de ciclo de vida de suscripciones
- Eventos con datos tipados
- Fácil de testear
- No hay leaks de memoria
- Permite logging y debugging centralizado

---

### Solución 3: Implementar Arquitectura por Capas

#### Estructura Propuesta
```
Assets/Code/Scripts/
├── Core/
│   ├── Domain/               # Lógica de negocio pura
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── Services/
│   │   └── Interfaces/
│   ├── Application/          # Casos de uso
│   │   ├── UseCases/
│   │   └── DTOs/
│   ├── Infrastructure/       # Implementaciones técnicas
│   │   ├── Persistence/
│   │   ├── Unity/
│   │   └── External/
│   └── Presentation/         # UI y presentación
│       ├── ViewModels/
│       ├── Views/
│       └── Controllers/
```

#### Ejemplo de Implementación
```csharp
// DOMAIN LAYER - Lógica de negocio pura
namespace Core.Domain.Services
{
    public interface IResearchService
    {
        bool CanUnlockResearch(string researchId, Player player);
        void UnlockResearch(string researchId, Player player);
    }
    
    public class ResearchService : IResearchService
    {
        // Sin dependencias de Unity
        // Solo lógica de negocio
    }
}

// APPLICATION LAYER - Casos de uso
namespace Core.Application.UseCases
{
    public class UnlockResearchUseCase
    {
        private readonly IResearchService _researchService;
        private readonly IPlayerRepository _playerRepository;
        private readonly IEventBus _eventBus;
        
        public UnlockResearchUseCase(
            IResearchService researchService,
            IPlayerRepository playerRepository,
            IEventBus eventBus)
        {
            _researchService = researchService;
            _playerRepository = playerRepository;
            _eventBus = eventBus;
        }
        
        public async Task<bool> Execute(string researchId)
        {
            var player = await _playerRepository.GetCurrentPlayer();
            
            if (!_researchService.CanUnlockResearch(researchId, player))
                return false;
            
            _researchService.UnlockResearch(researchId, player);
            await _playerRepository.SavePlayer(player);
            
            _eventBus.Publish(new ResearchUnlockedEvent(researchId));
            return true;
        }
    }
}

// PRESENTATION LAYER - UI Controller
namespace Core.Presentation.Controllers
{
    public class ResearchPanelController : MonoBehaviour
    {
        private UnlockResearchUseCase _unlockResearchUseCase;
        
        // Inyectado desde el container
        public void Initialize(UnlockResearchUseCase useCase)
        {
            _unlockResearchUseCase = useCase;
        }
        
        public async void OnUnlockButtonClick(string researchId)
        {
            var success = await _unlockResearchUseCase.Execute(researchId);
            // Actualizar UI
        }
    }
}
```

#### Beneficios
- Separación clara de responsabilidades
- Lógica de negocio testeable sin Unity
- Cambios en UI no afectan lógica de negocio
- Más fácil de entender y mantener

---

### Solución 4: Implementar Repository Pattern para Persistencia

#### Implementación
```csharp
// Interface para el repositorio
public interface IPlayerRepository
{
    Task<Player> GetCurrentPlayer();
    Task SavePlayer(Player player);
    Task<Player> LoadPlayer(string playerId);
}

// Implementación con Unity PlayerPrefs
public class UnityPlayerRepository : IPlayerRepository
{
    private readonly IFileSystem _fileSystem;
    
    public UnityPlayerRepository(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }
    
    public async Task<Player> GetCurrentPlayer()
    {
        // Implementación específica de Unity
    }
    
    public async Task SavePlayer(Player player)
    {
        // Implementación específica de Unity
    }
}

// Implementación para testing
public class InMemoryPlayerRepository : IPlayerRepository
{
    private Dictionary<string, Player> _players = new();
    
    public Task<Player> GetCurrentPlayer()
    {
        // Implementación en memoria para tests
    }
}
```

#### Beneficios
- Abstracción de la persistencia
- Fácil cambiar sistema de guardado
- Testeable con implementaciones en memoria
- Separación de concerns

---

### Solución 5: Aplicar SOLID Principles

#### Single Responsibility Principle (SRP)
**Problema Actual**: `WorldManager` hace demasiado
```csharp
public class WorldManager : InGameSingleton<WorldManager>
{
    // Gestión de recursos
    [SerializeField] List<ResourceData> _worldResources;
    
    // Gestión de jugador
    [SerializeField] PlayerSO _playerData;
    
    // Gestión de civilizaciones
    [SerializeField] List<CivilizationSO> _civilizationSOs;
    
    // Gestión de comandos
    private CommandInvoker _invoker;
    
    // ¡Demasiadas responsabilidades!
}
```

**Solución**: Dividir en servicios especializados
```csharp
public class ResourceManager
{
    // Solo gestiona recursos
}

public class PlayerManager
{
    // Solo gestiona jugador
}

public class CivilizationManager
{
    // Solo gestiona civilizaciones
}

public class WorldCoordinator
{
    private readonly ResourceManager _resourceManager;
    private readonly PlayerManager _playerManager;
    private readonly CivilizationManager _civilizationManager;
    
    // Coordina entre los managers
}
```

#### Dependency Inversion Principle (DIP)
**Problema Actual**: Dependencia de implementaciones concretas
```csharp
public class ResearchSystem
{
    private SaveManager _saveManager; // Dependencia concreta
    
    public void UnlockResearch()
    {
        // usa _saveManager directamente
    }
}
```

**Solución**: Depender de abstracciones
```csharp
public interface ISaveManager
{
    Task SaveGame();
    Task<GameState> LoadGame();
}

public class ResearchSystem
{
    private readonly ISaveManager _saveManager; // Dependencia abstracta
    
    public ResearchSystem(ISaveManager saveManager)
    {
        _saveManager = saveManager;
    }
}
```

---

### Solución 6: Implementar Command Pattern Mejorado

El proyecto ya usa Command Pattern, pero puede mejorarse:

```csharp
// Comando con undo/redo
public interface ICommand
{
    void Execute();
    void Undo();
    bool CanExecute();
}

// Comando específico
public class ConstructPlanetCommand : ICommand
{
    private readonly Planet _planet;
    private readonly SolarSystem _solarSystem;
    private readonly IResourceService _resourceService;
    
    public ConstructPlanetCommand(
        Planet planet, 
        SolarSystem solarSystem,
        IResourceService resourceService)
    {
        _planet = planet;
        _solarSystem = solarSystem;
        _resourceService = resourceService;
    }
    
    public bool CanExecute()
    {
        return _resourceService.HasResources(_planet.Cost);
    }
    
    public void Execute()
    {
        _resourceService.ConsumeResources(_planet.Cost);
        _solarSystem.AddPlanet(_planet);
    }
    
    public void Undo()
    {
        _solarSystem.RemovePlanet(_planet);
        _resourceService.AddResources(_planet.Cost);
    }
}

// Command Invoker mejorado
public class CommandInvoker
{
    private Stack<ICommand> _executedCommands = new();
    private Stack<ICommand> _undoneCommands = new();
    
    public bool Execute(ICommand command)
    {
        if (!command.CanExecute())
            return false;
        
        command.Execute();
        _executedCommands.Push(command);
        _undoneCommands.Clear(); // Clear redo stack
        return true;
    }
    
    public void Undo()
    {
        if (_executedCommands.Count == 0)
            return;
        
        var command = _executedCommands.Pop();
        command.Undo();
        _undoneCommands.Push(command);
    }
    
    public void Redo()
    {
        if (_undoneCommands.Count == 0)
            return;
        
        var command = _undoneCommands.Pop();
        command.Execute();
        _executedCommands.Push(command);
    }
}
```

#### Beneficios
- Sistema de undo/redo robusto
- Validación antes de ejecutar
- Historia de comandos para debugging
- Mejor testabilidad

---

## Priorización de Mejoras

### Prioridad Alta (Crítico - Implementar Primero)

1. **Reemplazar Singleton/Service Locator con DI Container**
   - **Tiempo estimado**: 2-3 semanas
   - **Impacto**: Muy alto
   - **Razón**: Base para todas las demás mejoras

2. **Implementar Event Bus**
   - **Tiempo estimado**: 1 semana
   - **Impacto**: Alto
   - **Razón**: Reduce acoplamiento inmediatamente

### Prioridad Media (Importante - Implementar Después)

3. **Separar en Capas Arquitecturales**
   - **Tiempo estimado**: 3-4 semanas
   - **Impacto**: Muy alto
   - **Razón**: Mejora estructura general pero requiere refactoring extenso

4. **Aplicar Single Responsibility Principle a Managers**
   - **Tiempo estimado**: 2 semanas
   - **Impacto**: Medio-Alto
   - **Razón**: Mejora mantenibilidad significativamente

5. **Implementar Repository Pattern**
   - **Tiempo estimado**: 1-2 semanas
   - **Impacto**: Medio
   - **Razón**: Facilita testing y cambiode sistemas de persistencia

### Prioridad Baja (Mejoras Opcionales)

6. **Mejorar Command Pattern con Undo/Redo**
   - **Tiempo estimado**: 1 semana
   - **Impacto**: Bajo-Medio
   - **Razón**: Nice-to-have, no crítico

7. **Crear Abstracciones Adicionales**
   - **Tiempo estimado**: Continuo
   - **Impacto**: Medio
   - **Razón**: Se puede hacer gradualmente

---

## Plan de Implementación

### Fase 1: Fundamentos (Semanas 1-4)

#### Semana 1: Preparación
- [ ] Crear rama de refactoring
- [ ] Documentar dependencias actuales
- [ ] Crear tests para funcionalidad existente (regression tests)
- [ ] Preparar métricas de código

#### Semana 2-3: Dependency Injection
- [ ] Implementar DI Container básico
- [ ] Crear interfaces para servicios principales
- [ ] Migrar SaveManager a DI
- [ ] Migrar AudioManager a DI
- [ ] Migrar WorldManager a DI
- [ ] Testing exhaustivo

#### Semana 4: Event Bus
- [ ] Implementar Event Bus
- [ ] Migrar eventos de SystemEvents
- [ ] Migrar eventos de ConstructionEvents
- [ ] Actualizar suscriptores
- [ ] Testing

### Fase 2: Estructura (Semanas 5-8)

#### Semana 5-6: Separación por Capas
- [ ] Crear estructura de carpetas
- [ ] Mover lógica de negocio a Domain layer
- [ ] Crear Application layer con Use Cases
- [ ] Refactorizar Presentation layer

#### Semana 7-8: Repository Pattern
- [ ] Definir interfaces de repositorios
- [ ] Implementar repositorios concretos
- [ ] Migrar SaveManager a repositorios
- [ ] Crear repositorios in-memory para tests

### Fase 3: Refinamiento (Semanas 9-12)

#### Semana 9-10: SOLID Principles
- [ ] Refactorizar managers grandes
- [ ] Aplicar SRP a cada clase
- [ ] Aplicar DIP donde sea necesario
- [ ] Revisar y refactorizar

#### Semana 11-12: Testing y Documentación
- [ ] Crear suite de tests unitarios
- [ ] Crear tests de integración
- [ ] Documentar arquitectura nueva
- [ ] Crear guías para desarrolladores

### Fase 4: Optimización (Opcional)

#### Mejoras Adicionales
- [ ] Implementar Object Pooling para entidades frecuentes
- [ ] Optimizar Event Bus con prioridades
- [ ] Implementar Command Queue para acciones async
- [ ] Mejorar sistema de logging

---

## Métricas de Éxito

### Antes del Refactoring
- **Acoplamiento**: Alto (66+ usos de Service Locator, 8+ Singletons)
- **Testabilidad**: Baja (difícil crear tests unitarios)
- **Mantenibilidad**: Media-Baja (difícil entender flujo de datos)
- **Bugs por dependencias**: Frecuentes (orden de inicialización)

### Después del Refactoring (Objetivos)
- **Acoplamiento**: Bajo (dependencias explícitas vía DI)
- **Testabilidad**: Alta (80%+ cobertura de tests unitarios)
- **Mantenibilidad**: Alta (arquitectura clara y documentada)
- **Bugs por dependencias**: Raros (orden de inicialización controlado)

### KPIs Específicos
- Reducir usos de Service Locator de 66 a 0
- Eliminar todos los Singletons (8+)
- Alcanzar 80% cobertura de tests en lógica de negocio
- Reducir tiempo de setup para tests de minutos a segundos
- Documentar 100% de las interfaces públicas

---

## Consideraciones Importantes

### Compatibilidad con Unity
- Unity no tiene DI nativo, pero podemos usar VContainer o Zenject
- Alternativamente, implementar DI simple como mostrado
- Los MonoBehaviours requieren tratamiento especial

### Impacto en el Equipo
- Requiere training en nuevos patrones
- Período de adaptación de 1-2 semanas
- Documentación clara es esencial

### Riesgos
- **Tiempo de desarrollo**: El refactoring tomará tiempo
- **Bugs temporales**: Posibles bugs durante la transición
- **Resistencia al cambio**: El equipo puede preferir lo conocido

### Mitigación de Riesgos
- Hacer cambios incrementales
- Mantener tests de regresión
- Documentar cada cambio
- Code reviews exhaustivos
- Mantener rama estable mientras se refactoriza

---

## Recursos Adicionales

### Libros Recomendados
- "Clean Architecture" - Robert C. Martin
- "Design Patterns" - Gang of Four
- "Dependency Injection in .NET" - Mark Seemann

### Herramientas
- **VContainer**: DI Container para Unity (recomendado)
- **Zenject**: Alternativa DI Container
- **UniRx**: Reactive Extensions para Unity
- **NUnit**: Framework de testing

### Artículos
- Unity Dependency Injection Best Practices
- SOLID Principles in Unity
- Event-Driven Architecture in Game Development

---

## Conclusión

El proyecto **Escape Nova** tiene una base sólida pero sufre de problemas arquitecturales comunes en proyectos Unity:
- Uso excesivo de Singletons
- Eventos estáticos globales
- Acoplamiento alto
- Falta de separación de concerns

Las soluciones propuestas son:
1. Dependency Injection para gestión de dependencias
2. Event Bus para comunicación desacoplada
3. Arquitectura por capas para separación clara
4. Repository Pattern para abstracción de persistencia
5. Aplicación de principios SOLID

Implementar estas mejoras llevará aproximadamente **12 semanas** pero resultará en un código:
- Más mantenible
- Más testeable
- Más escalable
- Menos propenso a bugs

El esfuerzo vale la pena para un proyecto de esta escala, especialmente si planean continuar el desarrollo a largo plazo.

---

## Apéndice: Ejemplos de Código Completos

### Ejemplo A: Sistema de DI Completo

```csharp
// Container
public class GameContainer
{
    private Dictionary<Type, object> _services = new();
    private Dictionary<Type, Func<object>> _factories = new();
    
    public void RegisterSingleton<T>(T instance)
    {
        _services[typeof(T)] = instance;
    }
    
    public void RegisterFactory<T>(Func<T> factory)
    {
        _factories[typeof(T)] = () => factory();
    }
    
    public T Resolve<T>()
    {
        var type = typeof(T);
        
        if (_services.ContainsKey(type))
            return (T)_services[type];
        
        if (_factories.ContainsKey(type))
        {
            var instance = _factories[type]();
            return (T)instance;
        }
        
        throw new Exception($"No registration for {type}");
    }
}

// Bootstrapper
public class GameBootstrapper : MonoBehaviour
{
    private void Awake()
    {
        var container = new GameContainer();
        
        // Core services
        var eventBus = new EventBus();
        container.RegisterSingleton<IEventBus>(eventBus);
        
        var fileSystem = new UnityFileSystem();
        container.RegisterSingleton<IFileSystem>(fileSystem);
        
        // Repositories
        var playerRepo = new UnityPlayerRepository(fileSystem);
        container.RegisterSingleton<IPlayerRepository>(playerRepo);
        
        // Services
        var researchService = new ResearchService();
        container.RegisterSingleton<IResearchService>(researchService);
        
        // Use Cases
        container.RegisterFactory<UnlockResearchUseCase>(() =>
            new UnlockResearchUseCase(
                container.Resolve<IResearchService>(),
                container.Resolve<IPlayerRepository>(),
                container.Resolve<IEventBus>()
            )
        );
        
        // Make container available
        ServiceRegistry.SetContainer(container);
    }
}

// Helper para acceso al container
public static class ServiceRegistry
{
    private static GameContainer _container;
    
    public static void SetContainer(GameContainer container)
    {
        _container = container;
    }
    
    public static T Get<T>()
    {
        return _container.Resolve<T>();
    }
}
```

### Ejemplo B: Event Bus Completo

```csharp
// Interfaces
public interface IEvent { }
public interface IEventBus
{
    void Subscribe<T>(Action<T> handler) where T : IEvent;
    void Unsubscribe<T>(Action<T> handler) where T : IEvent;
    void Publish<T>(T @event) where T : IEvent;
}

// Implementación
public class EventBus : IEventBus
{
    private Dictionary<Type, List<Delegate>> _subscribers = new();
    private Queue<IEvent> _eventQueue = new();
    private bool _isProcessing = false;
    
    public void Subscribe<T>(Action<T> handler) where T : IEvent
    {
        var type = typeof(T);
        if (!_subscribers.ContainsKey(type))
        {
            _subscribers[type] = new List<Delegate>();
        }
        _subscribers[type].Add(handler);
    }
    
    public void Unsubscribe<T>(Action<T> handler) where T : IEvent
    {
        var type = typeof(T);
        if (_subscribers.ContainsKey(type))
        {
            _subscribers[type].Remove(handler);
        }
    }
    
    public void Publish<T>(T @event) where T : IEvent
    {
        var type = typeof(T);
        
        if (_isProcessing)
        {
            // Evitar problemas si un handler publica otro evento
            _eventQueue.Enqueue(@event);
            return;
        }
        
        _isProcessing = true;
        
        try
        {
            if (_subscribers.ContainsKey(type))
            {
                foreach (var handler in _subscribers[type].ToList())
                {
                    ((Action<T>)handler)(@event);
                }
            }
            
            // Procesar eventos en cola
            while (_eventQueue.Count > 0)
            {
                var queuedEvent = _eventQueue.Dequeue();
                PublishInternal(queuedEvent);
            }
        }
        finally
        {
            _isProcessing = false;
        }
    }
    
    private void PublishInternal(IEvent @event)
    {
        var type = @event.GetType();
        if (_subscribers.ContainsKey(type))
        {
            foreach (var handler in _subscribers[type].ToList())
            {
                handler.DynamicInvoke(@event);
            }
        }
    }
}

// Ejemplos de eventos
public class ResearchUnlockedEvent : IEvent
{
    public string ResearchId { get; }
    public DateTime UnlockedAt { get; }
    
    public ResearchUnlockedEvent(string researchId)
    {
        ResearchId = researchId;
        UnlockedAt = DateTime.Now;
    }
}

public class PlanetConstructedEvent : IEvent
{
    public string PlanetId { get; }
    public int OrbitIndex { get; }
    
    public PlanetConstructedEvent(string planetId, int orbitIndex)
    {
        PlanetId = planetId;
        OrbitIndex = orbitIndex;
    }
}

// Uso
public class ResearchUIController : MonoBehaviour
{
    private IEventBus _eventBus;
    
    public void Initialize(IEventBus eventBus)
    {
        _eventBus = eventBus;
        _eventBus.Subscribe<ResearchUnlockedEvent>(OnResearchUnlocked);
    }
    
    private void OnDestroy()
    {
        _eventBus.Unsubscribe<ResearchUnlockedEvent>(OnResearchUnlocked);
    }
    
    private void OnResearchUnlocked(ResearchUnlockedEvent @event)
    {
        Debug.Log($"Research unlocked: {@event.ResearchId}");
        // Actualizar UI
    }
}
```

---

**Documento creado**: Diciembre 2024  
**Versión**: 1.0  
**Autor**: Análisis Arquitectural - Escape Nova
