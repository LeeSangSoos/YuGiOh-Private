from rlcard.games.custom.card import Monster
from rlcard.games.custom.card import Card
from rlcard.games.custom.utils import cards2list
import random
import numpy as np

class Round:
    page_names = ['Draw', 'Main', 'Battle', 'End']

    def __init__(self, num_players, np_random):
        ''' Initialize the round class

        Args:
            num_players (int): the number of players in game   
        '''
        self.np_random = np_random
        self.current_player_id = random.randint(0,1)
        self.num_players = num_players
        self.is_over = False
        self.winner = None
        
        self.page_index = 1 #시작은 main
        self.page = self.page_names[self.page_index]
        self.current_turn = self.current_player_id
        self.first_turn = True

    def proceed_round(self, players, action):
        action_values = action.split("-")
        player = players[self.current_player_id]
        enemy = players[1 - self.current_player_id]
        if action_values[0] == 'draw':
            self._perform_draw_action(players, self.current_player_id, 1)
        elif action_values[0] == 'endpage':
            self.change_page(1)
            if self.page == 'Draw':
                player = players[self.current_player_id]
                player.turn_draw = 1
                player.turn_summon = 1
                for monster in player.monsterfield:
                    if isinstance(monster, Monster):
                        monster.atk_chance = 1
        elif action_values[0] == 'summon':
            if action_values[1] == '0': #제물없는 소환
                #각 action_values는 소환체, 제물, faceup
                self.normal_summon(0, player, int(action_values[2]), faceup = action_values[3])
            elif action_values[1] == '1':#제물 1개 소환
                self.normal_summon(1, player, int(action_values[2]), int(action_values[3]), faceup = action_values[4])
            elif action_values[1] == '2':#제물 2개 소환
                self.normal_summon(2, player, int(action_values[2]), int(action_values[3]), int(action_values[4]), faceup = action_values[5])
        elif action_values[0] == 'attack':
            if action_values[2] == 'direct':
                self. battle(player, enemy, int(action_values[1]))
            else:
                self. battle(player, enemy, int(action_values[1]), int(action_values[2]))
        elif action_values[0] == 'discard':
            player.hand.pop(int(action_values[1]))
            
    def get_legal_actions(self, players, player_id):
        legal_actions = set()
        opponent = players[1 - player_id]
        player = players[player_id]
        hand = players[player_id].hand
        player_monsterfield = players[player_id].monsterfield
        opponent_monsterfield = opponent.monsterfield

        if self.current_turn == player_id:
            if self.page == "Draw":
                if player.turn_draw >= 1:
                    legal_actions.add("draw")
                else:
                    legal_actions.add("endpage")
            elif self.page == "Main":
                used_places = set()
                for i in range(len(player_monsterfield)):
                    if player_monsterfield[i] is not None:
                        used_places.add(i)
                if player.turn_summon >= 1:
                    for hand_idx in range(len(hand)):
                        card = hand[hand_idx]
                        if isinstance(card, Monster) and card is not None:
                            monster = card
                            if monster.level <= 4 and len(used_places) < 5:
                                legal_actions.add(f"summon-0-{hand_idx}-faceup")
                                legal_actions.add(f"summon-0-{hand_idx}-facedown")
                            elif monster.level <= 6 and len(used_places) >= 1:
                                for sacrifice_idx in used_places:
                                    legal_actions.add(f"summon-1-{hand_idx}-{sacrifice_idx}-faceup")
                                    legal_actions.add(f"summon-1-{hand_idx}-{sacrifice_idx}-facedown")
                            elif monster.level >= 7 and len(used_places) >= 2:
                                for sacrifice_idx1 in used_places:
                                    for sacrifice_idx2 in used_places:
                                        if sacrifice_idx2 != sacrifice_idx1:
                                            legal_actions.add(f"summon-2-{hand_idx}-{sacrifice_idx1}-{sacrifice_idx2}-faceup")
                                            legal_actions.add(f"summon-2-{hand_idx}-{sacrifice_idx1}-{sacrifice_idx2}-facedown")
            elif self.page == "Battle":
                if not self.first_turn:
                    for player_field_idx in range(len(player_monsterfield)):
                        if player_monsterfield[player_field_idx] is not None:
                            card = player_monsterfield[player_field_idx]
                            if isinstance(card, Monster):
                                monster = card
                                if monster.atk_chance >= 1 and monster.faceup:
                                    if all(card is None for card in opponent_monsterfield):
                                        legal_actions.add(f"attack-{player_field_idx}-direct")
                                    else:
                                        for enemy_field_idx in range(len(opponent_monsterfield)):
                                            if opponent_monsterfield[enemy_field_idx] is not None:
                                                enemy = opponent_monsterfield[enemy_field_idx]
                                                if isinstance(enemy, Monster):
                                                    legal_actions.add(f"attack-{player_field_idx}-{enemy_field_idx}")
            elif self.page == "End":
                if len(hand) >= 7:
                    for hand_idx in range(len(hand)):
                        card = hand[hand_idx]
                        if card is not None:
                            legal_actions.add(f"discard-{hand_idx}")
                else:
                    legal_actions.add("endpage")

        if self.page == "Main" or self.page == "Battle":
            legal_actions.add("endpage")
        return list(legal_actions) 

    def get_state(self, players, player_id):
        ''' Get player's state
        Args:
            players (list): The list of Player
            player_id (int): The id of the player
        '''
        state = {}
        player = players[player_id]
        enemy = players[1 - player_id]
        state['hand'] = cards2list(player.hand) ## card id list
        state['deck'] = cards2list(player.deck)
        state['player_monsterfield'] = cards2list(player.monsterfield)
        state['legal_actions'] = self.get_legal_actions(players, player_id)
        state['life'] = player.life
        state['enemy_monsterfield'] = cards2list(enemy.monsterfield)
        state['enemy_life'] = enemy.life
        state['page'] = self.page
        return state

    def _perform_draw_action(self, players, player_id, n):
        # replace deck if there is no card in draw pile
        player = players[player_id]
        for _ in range(n):
            if not player.deck:
                #print('Is Over')
                self.is_over = True
                self.winner = 1-player_id
                break
            else:
                player.hand.append(player.deck.pop())
                player.turn_draw -= 1
    
    def battle(self, player, enemy, attacker_idx, enemy_idx = -1):

        attacker = player.monsterfield[attacker_idx]

        if enemy_idx == -1:
            enemy.life -= attacker.atk
        else:
            defender = enemy.monsterfield[enemy_idx]
            if defender.faceup:
                if attacker.atk > defender.atk:
                    enemy.monsterfield[enemy_idx] = None
                    enemy.life -= (attacker.atk - defender.atk)
                elif attacker.atk == defender.atk:
                    enemy.monsterfield[enemy_idx] = None
                    player.monsterfield[attacker_idx] = None
                elif attacker.atk < defender.atk:
                    player.monsterfield[attacker_idx] = None
                    player.life -= (defender.atk - attacker.atk)
            else:
                if attacker.atk > defender.defense:
                    enemy.monsterfield[enemy_idx] = None
                elif attacker.atk == defender.defense:
                    pass
                elif attacker.atk < defender.defense:
                    player.life -= (defender.defense - attacker.atk)
            
        if enemy.life <= 0:
            #print('Is Over')
            self.is_over = True
            self.winner = player.get_player_id()
        elif player.life <=0:
            #print('Is Over')
            self.is_over = True
            self.winner = enemy.get_player_id()
        attacker.atk_chance -= 1
        '''
        print(player.player_id, " : ", player.life, " ", enemy.player_id, " : ", enemy.life)
        '''

    def change_page(self, increment):
        self.page_index += increment
        if self.page_index >= len(self.page_names):
            self.current_player_id = 1 - self.current_player_id
            self.current_turn = self.current_player_id
            self.page_index = 0
        self.page = self.page_names[self.page_index]

        if self.first_turn == True and self.page == "Draw":
            self.first_turn = False
    
    def normal_summon(self, type, player, hand_idx, sacrifice_idx1=-1, sacrifice_idx2=-1, faceup='faceup'):
        '''
        type == 0 : 제물 없음
        type == 1 : 제물 1개
        type == 2 : 제물 2개
        '''
        if type == 1 or type == 2:
            player.monsterfield[sacrifice_idx1] = None
        if type == 2:
            player.monsterfield[sacrifice_idx2] = None

        # 소환
        for idx_to_summon in range(len(player.monsterfield)):
            if player.monsterfield[idx_to_summon] is None:
                card_to_summon = player.hand[hand_idx]
                if faceup == 'facedown':
                    card_to_summon.faceup = False  # Change faceup to False for facedown summon
                player.monsterfield[idx_to_summon] = card_to_summon
                break

        player.hand.pop(hand_idx)
        player.turn_summon -= 1
    