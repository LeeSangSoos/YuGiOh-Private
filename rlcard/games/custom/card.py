class Card:
    info = {'type':  ['monster', 'magic', 'trap']
            }

    def __init__(self, card_type, card_id,card_name):
        self.type = card_type
        self.name = card_name
        self.id = card_id
        self.ingame_id = "0"
        self.faceup = True
    
    def set_ingame_id(self, id):
        self.ingame_id = id

    def get_str(self):
        ''' Get the string representation of card

        Return:
            (string): The string of card's
        '''
        return str(self.id) + '-' + str(self.faceup)

    @staticmethod
    def print_cards(cards):
        if isinstance(cards, str):
            cards = [cards]
        for i, card in enumerate(cards):

            print(card.name, card.type, end='')

            if i < len(cards) - 1:
                print(', ', end='')

class Monster(Card):

    def __init__(self, card_id, card_name, atk, defense, level):
        super().__init__('monster', card_id, card_name)
        self.atk = atk
        self.defense = defense
        self.level = level
        self.atk_chance = 1
        self.info = super().info.copy()
        self.info.update({
            'atk': atk,
            'def': defense,
            'level': level,
        })
    
    def get_str(self):
        return str(self.id) + '-' + str(self.faceup) + '-' + str(self.atk) + '-' + str(self.defense)