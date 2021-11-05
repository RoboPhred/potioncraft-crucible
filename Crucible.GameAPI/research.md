### Ingredient item stacks

Stack game object
has components:

- ObjectBased.Stack.Stack
- StackHighlight
- StackVisualEffects
- StackTooltipContentProvider
- StackSortingOrderSetter
- Rigidbody2D

Has child game objects

- "Tangleweeds 1-03"
  - components
    - IngredientFromStack
    - SortingGroup
    - StackItemSortingOrderSetterHelper
    - Rigidbody2D
  - Game Objects
    - ColliderInner
      - components
        - PolygonCollider2D
    - ColliderOuter
      - components
        - PolygonCollider2D
        - GraphicStateMachine
    - Sprite
      - components
        - SpriteRenderer
- "GrindedGrass InGame"
  - _only when ground_
  - Auto created when ground from Stack.Ingredient.grindedSubstance.inGamePrefab
  - components
    - GrindedSubstanceInPlay
    - SortingGroup
    - Rigidbody2D
  - Game Objects
    - ColliderInner
      - PolygonCollider2D
    - ColliderOuter
      - components
        - PolygonCollider2D
        - GraphicsStateMachine
    - Sprite
      - components
        - SpriteRenderer
      - GameObjects
        - SpriteFG
          - components
            - SpriteRenderer
        - SpriteScratches
          - components
            - SpriteRenderer
- "GrindedGrass InMortar"
  - Auto created when ground from Stack.Ingredient.grindedSubstance.inMortarPrefab
  - components
    - GrindedSubstanceInMortar
    - Rigidbody2D
    - SortingGroup
  - Game Objects
    - Sprite
      - game objects
        - SpriteFG
        - SpriteScratches
    - Collider
      - components
        - GrindedSubstanceInMortarCollider
        - StackItemViscousMedium (**note**: This might change between item types?)
        - PolygonCollider2D

### Initialization

See ObjectBased.Stack.Stack.SpawnNewItemStack

Sets:

- Stack.inventoryItem
- Stack.SoundController
- Stack.itemsPanel
- Stack.visualEffectsScript
- Stack.visualEffectsScript.stackScript
- Calls Initialize on all direct child IngredientFromStack items
  - Adds self to Stack.itemsFromThisStack
  - Note: Does NOT call Initialize on all StackItem components. Other sublcasses
  - seem to call it at other times
- appearState

#### Stack

Stack properties not hidden from inspector

- isAssembled
  - Do not set. Read and set by game logic.
- isAssembling
  - Do not set. Read and set by game logic.
- assemblingProgress
  - Do not set. Read and set by game logic.
- assemblingSpeed
  - Set this on init. Only ever read by game logic.
- assemblingWhenCatchingInAir
  - Do not set?. Set by StateMachine when changing to the InHand state. Never set to false, only true?
- itemsFromThisStack
  - Do not set. Defaults to an empty list and IngredientFromStack adds itself to it
- visualEffectsScript
- stateMachine
  - Created on awaken. Do not set
- substanceGrinding
  - Created by constructor. Do not set.

Hidden properties, probably automatically taken care of

- grindedSubstanceInPlay
  - Auto set from gameObject component GrindedSubstanceInPlay
    - Cloned from prefab: Stack.Ingredient.grindedSubstance.inGamePrefab
- grindedSubstanceInMortar
  - Auto set from gameObject component GrindedSubstanceInMortar
    - Cloned from prefab: Stack.Ingredient.grindedSubstance.inMortarPrefab
