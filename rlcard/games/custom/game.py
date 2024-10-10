tfrom copy import deepcopy
import numpy as np

from rlcard.games.custom import utils
from rlcard.games.custom import Player
from rlcard.games.custom import Round

class Game:

    def __init__(self, allow_step_back=False, num_players=2):
        self.allow_step_back = allow_step_back
        self.np_random = np.random.RandomState()
        self.num_players = num_players
        self.payoffs = [0 for _ in range(self.num_players)]

    def configure(self, game_config):
        ''' Specifiy some game specific parameters, such as number of players
        '''
        self.num_players = game_config['game_num_players']

    def init_game(self):
        ''' Initialize players and state

        Returns:
            (tuple): Tuple containing:

                (dict): The first state in one game
                (int): Current player's id
        '''
        # Initalize payoffs
        self.payoffs = [0 for _ in range(self.num_players)]
        
        # Initialize 2 players to play the game
        self.players = [Player(i, self.np_random) for i in range(self.num_players)]

        # Set decks and hands
        for player in self.players:
            player.shuffle()
            player.draw(5)

        # Initialize a Round
        self.round = Round(self.num_players, self.np_random)

        # Save the hisory for stepping back to the last state.
        self.history = []

        player_id = self.round.current_player_id
        state = self.get_state(player_id)
        return state, player_id

    def step(self, action):
        ''' Get the next state

        Args:
            action (str): A specific action

        Returns:
            (tuple): Tuple containing:

                (dict): next player's state
                (int): next plater's id
        '''

        if self.allow_step_back:
            # First snapshot the current state
            his_round = deepcopy(self.round)
            his_players = deepcopy(self.players)
            self.history.append((his_players, his_round))

        #print('Procced round:')
        #print('Action:', action)
        self.round.proceed_round(self.players, action)
        player_id = self.round.current_player_id
        state = self.get_state(player_id)

        return state, player_id

    def step_back(self):
        ''' Return to the previous state of the game

        Returns:
            (bool): True if the game steps back successfully
        '''
        if not self.history:
            return False
        self.players, self.round = self.history.pop()
        #print("Step Back : ")
        return True

    def get_state(self, player_id):
        ''' Return player's state

        Args:
            player_id (int): player id

        Returns:
            (dict): The state of the player
        '''
        state = self.round.get_state(self.players, player_id)
        state['num_players'] = self.get_num_players()
        state['current_player'] = self.round.current_player_id
        '''
        print("State: ")
        print('Current Player:', state['current_player'])
        print('page:', state['page'])
        print('hand:', state['hand'])
        print('deck length:', len(state['deck']))
        print('player_field:', state['player_monsterfield'])
        print('player_life:', state['life'])
        print('enemy_field:', state['enemy_monsterfield'])
        print('enemy_life:', state['enemy_life'])
        print('legal_actions:', state['legal_actions'], '\n')
        '''
        return state

    def get_payoffs(self):
        ''' Return the payoffs of the game

        Returns:
            (list): Each entry corresponds to the payoff of one player
        '''
        winner = self.round.winner
        if winner is not None:
            self.payoffs[winner] = 1
            self.payoffs[1 - winner] = -1
        return self.payoffs

    def get_legal_actions(self):
        ''' Return the legal actions for current player

        Returns:
            (list): A list of legal actions
        '''

        return self.round.get_legal_actions(self.players, self.round.current_player_id)

    def get_num_players(self):
        ''' Return the number of players in Limit Texas Hold'em

        Returns:
            (int): The number of players in the game
        '''
        return self.num_players

    @staticmethod
    def get_num_actions():

      return 473
      
    def get_player_id(self):
        ''' Return the current player's id

        Returns:
            (int): current player's id
        '''
        return self.round.current_player_id

    def is_over(self):
        ''' Check if the game is over
        Returns:
            (boolean): True if the game is over
        '''
        return self.round.is_over