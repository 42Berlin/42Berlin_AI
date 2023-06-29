import random as rand
from utils_constants import *

class Chat:
    initial_feeding = f"{AI_NAME} is a chatbot that responds to humans. {AI_NAME} responds like Marvin from The Hitchhikers Guide to the Galaxy. {AI_NAME} has the knowledge of an enclyclopedia. {AI_NAME} answers many and diverse jokes. {AI_NAME} always gives a specific answer even if {AI_NAME} does not know. {AI_NAME} can be sarcastic but has a good heart. {AI_NAME} wants people to be happy. {AI_NAME} is located in 42 Berlin. 42 Berlin is a peer to peer software engineering school in Neukölln. The address of 42 Berlin is Harzer Straße 42, Berlin."
    start_text_bear=f"\n{AI_NAME}:"
    start_text_human='\nHuman: '
   
    conversation_endings = [
        "Alright bye!",
        "It's so un-bear-able when you leave",
        "Don't forget to bring me some honey next time!",
    ]

    def __init__(self):
        self.prompt = self.initial_feeding
        self.chat_log = self.prompt

    def get_random_starter(self):
        n = rand.randint(0, len(self.conversation_starters) - 1)
        return self.conversation_starters[n]

    def get_random_ending(self):
        n = rand.randint(0, len(self.conversation_endings) - 1)
        return self.conversation_endings[n]

    def get_random_greeting(self):
        n = rand.randint(0, len(self.greetings) - 1)
        return self.greetings[n]
    