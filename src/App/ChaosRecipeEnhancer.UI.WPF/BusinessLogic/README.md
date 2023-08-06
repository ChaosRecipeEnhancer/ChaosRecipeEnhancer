# `EnhancePoEApp/ChaosRecipeEnhancer.UI/BusinessLogic`

If a class or method directly coincides with a piece of functionality available to the user (at a high level, not the external dependencies to achieve a functionality), it should go in here.

If the use of an external interface is required to achieve the underlying task, specifically an interface we do not maintain ourselves (i.e. underlying Windows APIs to handle Windows, Mouse or Keyboard events, etc.), consider moving them to `Extensions`.