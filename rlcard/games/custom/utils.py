import os
import json
import numpy as np
from collections import OrderedDict

import rlcard

from rlcard.games.custom.card import Card, Monster

#set path to rlcard
ROOT_PATH = rlcard.__path__[0]

# a map of abstract action to its index and a list of abstract action
with open(os.path.join(ROOT_PATH, 'games/custom/jsondata/action_space.json'), 'r') as file:
    ACTION_SPACE = json.load(file, object_pairs_hook=OrderedDict)
    ACTION_LIST = list(ACTION_SPACE.keys())

#create deck from card data json files
#get cards from json and add them to deck by *3
def init_deck():
    with open(os.path.join(ROOT_PATH, 'games/custom/jsondata/monster_cards.json'), 'r') as file:
        Mcard_data = json.load(file)

    deck = []
    for card_data in Mcard_data:
        for i in range(3):
            card = Monster(card_data['id'], card_data['name'], card_data['attack'], card_data['defense'], card_data['level'])
            ingame_id = f"{card_data['id']}_{i}" #initialize card with new ingame id
            card.set_ingame_id(ingame_id)
            deck.append(card)

    return deck

def cards2list(cards):
    ''' Get the corresponding string representation of cards

    Args:
        cards (list): list of CustomCard objects

    Returns:
        (string): string representation of cards
    '''
    cards_list = []
    for card in cards:
        if card is not None:
            cards_list.append(card.get_str())
    return cards_list

def card2dict(cards):
    ''' Get the corresponding dict representation of cards

    Args:
        cards (list): list of string of cards's card

    Returns:
        (dict): dictionary of cards
    '''
    card_dict = {} # count numbers of each card in hand
    for card in cards:
        if card not in card_dict:
            card_dict[card] = 1
        else:
            card_dict[card] += 1
    return card_dict

def draw_card(player, num=1):
    for _ in range(num):
        if player.deck:  
            card = player.deck.pop(0)  
            player.hand.append(card)  
        else:
            print("The deck is empty. No cards to draw.")
            # need loose condition
            break

def encode_hand(plane, hand):
    ''' Encode hand and represerve it into plane

    Args:
        plane (array): 7*4 numpy array
        hand (list): list of string of hand's card

    Returns:
        (array): 7*4 numpy array
    '''
    #hand = card2dict(hand)
    
    for hand_idx in range(len(hand)):
        card = hand[hand_idx]
        card_info = card.split('-')
        card_id = int(card_info[0])
        faceup = bool(card_info[1])
        atk = int(card_info[2])
        defence = int(card_info[3])
        plane[hand_idx][0] = card_id
        plane[hand_idx][1] = faceup
        plane[hand_idx][2] = atk
        plane[hand_idx][3] = defence
    return plane

def encode_field(plane, field):
    for field_idx in range(len(field)):
        card = field[field_idx]
        card_info = card.split('-')
        card_id = int(card_info[0])
        faceup = bool(card_info[1])
        atk = int(card_info[2])
        defence = int(card_info[3])
        plane[field_idx][0] = card_id
        plane[field_idx][1] = faceup
        plane[field_idx][2] = atk
        plane[field_idx][3] = defence
    return plane

def encode_life(plane, life):
    plane[0][0] = life
    return plane

def encode_page(plane, page):
    if page == 'Draw':
        plane[0][0] = 0
    elif page == 'Main':
        plane[0][0] = 1
    elif page == 'Battle':
        plane[0][0] = 2
    elif page == 'End':
        plane[0][0] = 3
    return plane