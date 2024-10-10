# Auto Yugi
## Yu-Gi-Oh Playing AI
### Graduation Project by Sangsoo Lee from the Department of Mechanical Engineering/Software Engineering, Sungkyunkwan University
This project was created purely for the advancement of technology and artistic expression, with no commercial gain or monetary objectives. Through this project, I aimed to share the joy of creation and technical exploration. Therefore, this project is not intended for commercial sale or profit generation, but solely focuses on delivering technical value and enjoyment.

### Game Components

#### Main Screen
![Main](https://github.com/LeeSangSoos/Yu-Gi-Oh-AI/assets/105085706/1a725d08-79e3-4b70-9c78-a4872402a5b8)

- **Reset Deck**: Resets all decks to facilitate deck editing.
- **12 Deck Edit**: Edits the deck of the 12 o'clock player (AI player here).
- **6 Deck Edit**: Edits the deck of the 6 o'clock player (user player here).
- **Player: User**: Sets the player to user, or to AI if you want AI players to compete against each other.
- **Play**: Starts the game.

#### Deck Editing Screen
![Deck Edit](https://github.com/LeeSangSoos/Yu-Gi-Oh-AI/assets/105085706/d7a31c32-6c60-4e02-ba9a-caca5df5a9f0)

Select cards from the right side or remove cards from the left deck list. By typing characters into the search bar at the top right, only cards containing those characters will be displayed. The save button saves the deck, and "back to menu" returns to the main menu. The number of cards in the deck is displayed numerically.

#### Game Screen
![Play Scene](https://github.com/LeeSangSoos/Yu-Gi-Oh-AI/assets/105085706/b4093210-54cd-4d0f-89f9-ae8041e796b7)

When one player wins, a victory message is displayed, and the game cannot proceed further. The implemented contents of the game are further explained in the table below.

- **Normal/Set/Tribute Summon**: In this game, there are two basic summons: Normal/Set summon, which can be done once per turn during the main phase. A normal summon places a monster in attack position, allowing it to attack once per turn and calculate damage with its attack power. A set summon places a monster in defense position, preventing it from attacking and calculating damage with its defense power when attacked. Tribute summons are an application of these two: level 5-6 monsters require one monster on the field to be sent to the graveyard for summoning, and level 7 or higher monsters require two.
- **Changing Battle Position**: The battle position of a monster can be changed once per turn, except on the turn it was summoned. Positions change as follows:
  - Attack -> Defense
  - Face-down Defense -> Attack
  - Defense -> Attack

- **Attack**: Monsters can attack once per turn during the battle phase. The attack value is subtracted from the opponent's life points, and if monsters battle, the following calculations are executed:

  | Attack vs Defense | Outcome |
  |-------------------|---------|
  | Attack vs Attack  | The weaker monster is destroyed, and the owner loses life points equal to the difference in attack power. If equal, both are destroyed. |
  | Attack vs Defense | The defender's monster is destroyed. If equal, the defender's monster is flipped face-up but not destroyed. |
  | Attack vs Face-down Defense | The face-down defender's monster is flipped face-up, and battle damage is calculated using its defense power. |

- **Spell Activation**: Spells can be activated from the hand or field. When activated from the hand, the spell card moves to the field. When activated from the field, a set spell card is flipped face-up. After targeting and other activation requirements, the effect is applied, and non-continuous spells are sent to the graveyard.

- **Spell/Trap Setting**: Simply placing spells/traps from the hand to the field in a face-down position. Traps must be set to be activated in subsequent turns.

- **Trap Activation**: Handles the activation of traps set on the field for at least one turn. Traps often have specific conditions, so the game checks for activatable traps with each action.

- **Chain**: Chains occur when multiple cards are activated in succession.

  ![Chain Explanation](https://github.com/LeeSangSoos/Yu-Gi-Oh-AI/assets/105085706/df265996-b7cf-45f8-9fdb-db276d7be8c1)

  In Yu-Gi-Oh rules, chains are explained as a stack where effects of activated cards accumulate and are processed. If the player who triggered the chain (Player A) starts the chain, the opponent (Player B) can respond, and they alternate until both pass on activating more cards. If Player A passes but Player B responds, Player A can reactivate cards before the chain ends. Locks using boolean values like ‘isworking’ or ‘chainonprocess’ ensure proper handling of chain processing and player interactions.

### AI

The AI is built using the RLCard platform.

![AI Environment](https://github.com/LeeSangSoos/Yu-Gi-Oh-AI/assets/105085706/9dbdf6bf-f94d-487e-8049-dd3fc38b37e7)

#### RLCard Platform
The RLCard platform is an open-source toolkit for developing reinforcement learning (RL) environments for card games. It provides various environments and tools to facilitate the development and evaluation of RL algorithms. By using RLCard, this project leverages a robust and flexible framework to implement the complex rules and interactions of Yu-Gi-Oh.

Key features of RLCard include:

Modularity: Easily extendable to accommodate different game rules and mechanics.
Standardization: Provides standardized interfaces for defining game states, actions, and rewards, making it easier to implement and compare different RL algorithms.
Pre-built Environments: Includes several pre-built card game environments, which can serve as references or starting points for custom game implementations.
Using RLCard allowed for efficient implementation and testing of the Yu-Gi-Oh AI, ensuring a high level of compatibility with existing RL techniques and tools.

RL card site link : https://rlcard.org/index.html

#### Yu-Gi-Oh Environment Information
- **Number of Actions**: 473
- **Number of Players**: 2
- **State Shape**: [6, 7, 4]
- **Action Types**: There are 473 action types, divided into draw, summon, attack, discard, and end phase. Summon, attack, and discard actions vary based on monster positions and counts, leading to a total of 473 actions.
- **Player Count**: Always 2.
- **State Shape**: [6, 7, 4] 
  - 6 represents ["Hand", "Player Monster Field", "Opponent Monster Field", "Player Life Points", "Opponent Life Points", "Current Phase"].
  - 7 represents the index, based on the maximum of 7 cards in hand.
  - 4 represents the card's state [“Unique Card ID”, “Face-up or Face-down”, “Attack Power”, “Defense Power”].

Action forms are stored in a dictionary format and accessed via key values during gameplay. The extracted state includes additional information such as legal actions, raw observations, and an action recorder.

The AI uses an epsilon-greedy algorithm for action selection, with rewards recorded as 1 for a win and -1 for a loss. The following table shows the hyperparameters used for training the DQN agent.

#### DQN Agent Training Hyperparameters
| Parameter | Value |
|-----------|-------|
| MLP       | [512*512] |
| Epsilon End | 0.1 |
| Learning Rate | 0.00005 |
| Batch Size | 32 |
| Learning Episodes | 1000 |
| Total Memory Size | 20000 |
| Evaluation Episodes | 1000 |
| Weight | Initialized with uniform distribution |
| Evaluation Term | 50 |
| Discount Factor | 0.99 |
| Epsilon Start | 1.0 |

The MLP consists of two hidden layers with 512 neurons each. Epochs depend on the number of learning episodes and training frequency. The ADAM algorithm updates weights.

#### Training Results
![image](https://github.com/LeeSangSoos/Yu-Gi-Oh-AI/assets/105085706/7e2624f0-4e68-43cf-9a38-cdeb89687eeb)

---

This README provides an overview of the Auto Yugi project, detailing its components, gameplay mechanics, and AI training process. The project aims to explore technical advancements and the joy of creation without any commercial intent.
