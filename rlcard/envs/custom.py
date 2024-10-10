import numpy as np
from collections import OrderedDict

from rlcard.envs import Env
from rlcard.games.custom import Game
from rlcard.games.custom.utils import encode_hand, encode_field, encode_life, encode_page
from rlcard.games.custom.utils import ACTION_SPACE, ACTION_LIST

DEFAULT_GAME_CONFIG = {
        'game_num_players': 2,
        }

class CustomEnv(Env):
    ''' Yugioh game Environment
    '''

    def __init__(self, config):
        ''' Initialize the environment
        '''
        self.name = 'custom'
        self.default_game_config = DEFAULT_GAME_CONFIG
        self.game = Game()
        super().__init__(config)
        # (hand 0, field 1, enemy_field 2, life 3, enemy_life 4, page 5)*(7 cards at most)*(id 0, faceup 1, atk 2, def 3)
        # life = 8000~0, page = 0~3
        self.state_shape = [[6, 7, 4] for _ in range(self.num_players)]
        self.action_shape = [None for _ in range(self.num_players)]

    def _extract_state(self, state):
        obs = np.zeros((6, 7, 4), dtype=int)

        encode_hand(obs[0], state['hand'])
        encode_field(obs[1], state['player_monsterfield'])
        encode_field(obs[2], state['enemy_monsterfield'])
        encode_life(obs[3], state['life'])
        encode_life(obs[4], state['enemy_life'])
        encode_page(obs[5], state['page'])
        
        legal_action_id = self._get_legal_actions()

        extracted_state = {'obs': obs, 'legal_actions': legal_action_id}
        extracted_state['raw_obs'] = state
        
        extracted_state['raw_legal_actions'] = [a for a in state['legal_actions']]
        extracted_state['action_record'] = self.action_recorder
        return extracted_state

    def get_payoffs(self):
        ''' Get the payoff of a game

        Returns:
           payoffs (list): list of payoffs
        '''
        return np.array(self.game.get_payoffs())

    def _decode_action(self, action_id):
        legal_ids = self._get_legal_actions()
        if action_id in legal_ids:
            return ACTION_LIST[action_id]
            
        return ACTION_LIST[np.random.choice(legal_ids)]

    def _get_legal_actions(self):
        ''' Get all leagal actions
        Returns:
            OrderedDict (list): return encoded legal action list
        '''
        legal_actions = self.game.get_legal_actions()
        legal_ids = {ACTION_SPACE[action]: None for action in legal_actions}
        return OrderedDict(legal_ids)