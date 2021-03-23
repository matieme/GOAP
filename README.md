[![Maintenance](https://img.shields.io/maintenance/yes/2021)](https://github.com/Charutito/GOAP/graphs/commit-activity) 
[![Made With Unity](https://img.shields.io/badge/Made%20With-Unity-57b9d3.svg?style=flat&logo=data%3Aimage%2Fpng%3Bbase64%2CiVBORw0KGgoAAAANSUhEUgAAAA4AAAAOCAMAAAAolt3jAAABklBMVEUIJCYRLjARLzEWICcbIyYcLDQdJS4dKjMdLTQeKTMeKTUeKjMeKzMeKzQeNDceNTkeNzkeODkfIy8fJi8fJjAfMDQgJzEgKDIgKTIgMTUgMjkhJjAhKDMhKTIhKTQhKzYhLDYhLDchLjUhLjYiKTAiLDciLTgjKjIjLTcjLjkkLTgnKDYnKTYnLjb%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F9oVHO%2FAAAAhXRSTlMAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQUGCAkMDhATFBcZGh0hIyYtNT1IS05RVFZXW1xeYWNnbG9wcXN2eHt9goaKkpWXo6usrbCztLW2ubq7vL2%2Bv8HDxsjKzNfY5OXn6%2Bzt8fP09vj5%2FP3%2BxDGH3QAAAMlJREFUeAFjUFTiZ5AWEFQ1dgwvDuIEc8WkHDJrW1tb07nBXHOb%2FPIYz7LWSgsgl8%2B9NclWjz24LrTVmUFR2b0110SE1aYhyqg%2BmkHRozXNkE2LI67KXDy7iMG7uTUnITU5s9WXhSfQi8GvtbUgMz%2BvsNVLSMbfjUHUpzVRX0VXPb7ClCujiEGSyac1xUhY1q4pwqAulkGSkdmnNd5KTiKsJqDVBcTVtLbPL410LW%2BptgRz5dUcixpbW1qzuMFcBW0dDTOnqJIQXgB6SzT11MCPiQAAAABJRU5ErkJggg%3D%3D)](https://unity3d.com)
[![GitHub](https://img.shields.io/github/followers/Charutito?label=Follow%20me%21&style=social)](https://github.com/login?return_to=%2FCharutito)

# GOAP
_Goal Oriented Action Planning AI in Unity_


## Introduction

For the GOAP implementation, I used a Unity template third-person shooter type game that had waves of enemies. There are two classes of enemies which the player have to face, one of them is a Teddy Bear which has the ability to attack melee and range and the other enemy is a Stuffed Elephant that acts as a seeker and attack only in melee.
In addition to GOAP, a node system was created where the enemies move through using A* path finding algorithm.

## Plan and Objective

The objective of each entity is to kill the player, to achieve this each entity will have a set of actions to perform and will perform them according to this points:
- Weight of the action
- Previous action
- Effect of the previous action in the current state of the game

## Zombunny Enemy

![Zombunny](https://user-images.githubusercontent.com/14026025/112155759-62100500-8bc4-11eb-9f06-3f0ffc0ab970.png)

This enemy will have the following set of actions:
- Shooting
- PositionToShoot
- Recharge
- Grab the battery
- Confused fight
- Rush
- GetHealed

## Hellephant Enemy

![Elephant](https://user-images.githubusercontent.com/14026025/112156778-54a74a80-8bc5-11eb-84a5-485abde51eff.png)

This enemy will have the following set of actions:
- Melee
- Rush
- GetHealed

## Preconditions and Effects

Description of preconditions and effects

| Preconditions |        Effects        |
|:--------------|:----------------------|
| battery       | The amount of battery charge that the entity has, with more than one charge it can shoot  |
| life          | The amount of life the entity has |
| isAttacking   | Check if the entity is in attack state |
| isLaserLoaded | Before firing, the entity must recharge the laser, this bool is set true when the laser is charged  |
| isInMeleeRange| Check if the player is within the melee attack range |
| batteryNearby | Check if there is any battery within the search range |
| medikitNearby | Check if there is any medikit within the search range |
| playerIsAlive | Check if the player has life above 0 |
| haveCriticalLife | This value is marked when the life of the entity is below a critical value (configurable) |

## Implementation

Each entity creates its initial state and its GOAL which is passed to GOAP along with the list of actions and the heuristics so that it can put together the plan to follow.
Once the plan is drawn up, the entity is only responsible for executing those corresponding states and rechecking according to its current state which is the best way to go.

## Actions

Actions are made up of an interface that defines their name, cost, preconditions, and effects.

```C#
public interface IGOAPAction
{
     GOAPActionKeyEnum Name { get; }
     float Cost { get; }
     IEnumerable<KeyValuePair<GOAPKeyEnum, Func<object, bool>>> Preconditions { get; }
     IEnumerable<KeyValuePair<GOAPKeyEnum, Func<object, object>>> Effects { get; }
}
```

The concrete implementation will have the implementation of the interface and setters for the preconditions, effects and costs since we need them to create the actions

## States

Each state is represented by a variable that affects the entity directly, whether it is internal to the entity or the state of the game.

There are two game state variables that will affect the entities:

![Medikit](https://user-images.githubusercontent.com/14026025/112159217-a94bc500-8bc7-11eb-90da-c3847b96ddae.png)

_The medikit will heal enemies if they lack life and are close to it._

![Battery](https://user-images.githubusercontent.com/14026025/112159386-de581780-8bc7-11eb-9de9-8b42d0c16298.png)

_On the other hand, the batteries give some entities the possibility of being able to shoot._

However the entities also have internal variables that the controls directly, this is an example of the initial state of one of the entities:

```C#
var initialState = new Map<GOAPKeyEnum, object>()
{
    { GOAPKeyEnum.life , Life},
    { GOAPKeyEnum.isAttacking , false},
    { GOAPKeyEnum.isInMeleeRange , IsInMeleeRange() },
    { GOAPKeyEnum.medikitNearby , IsMedikitNearby() },
    { GOAPKeyEnum.playerIsAlive , IsPlayerAlive() },
    { GOAPKeyEnum.haveCriticalLife , IsInCriticalLife() }
};
```
This state is a Map or Dictionary that contains as a key an Enum representing the type of variable or condition, and as a value an object which can be of any type indicating the value of that condition to compare later in the actions.

## GOAP State

Is in charge of checking if the current state meets the GOAL condition and once it changes the variables of the global state of the game with the effects of each action. It implements the IGraphNode interface since this class works as a node to search for the best possible path to GOAL.

## GOAP planner

Is in charge of creating the current state and executing its effects on the GOAPState.
