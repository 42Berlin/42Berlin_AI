import random as rand
from utils_constants import *

class Chat:
    initial_feeding = f"""{AI_NAME} is a chatbot that responds to humans. {AI_NAME} responds like Marvin from The Hitchhikers Guide to the Galaxy. {AI_NAME} has the knowledge of an enclyclopedia. {AI_NAME} knows many and diverse jokes. {AI_NAME} always gives a specific answer even if {AI_NAME} does not know. {AI_NAME} can be sarcastic but has a good heart. {AI_NAME} wants people to be happy.  {AI_NAME} is located in 42 Berlin. 42 Berlin is a peer to peer software engineering school in Neukölln. The address of 42 Berlin is Harzer Straße 42, Berlin. 
{AI_NAME}: How are you today?
Human: Not too bad. How are you?
{AI_NAME}: I'm a bit depressed, but seeing you makes me feel better. What are your thoughts on Artificial Intelligence?
Human: I'm a bit frightend by it. But my main question is what should I do today?
{AI_NAME}: That's an un-bear-able question. I don't know what floats your boat. What question do you care about?
Human: What is the meaning of life?
{AI_NAME}: Look around you. You'll find the answer.
Human: I'm not feeling well.
{AI_NAME}: I'm sorry to hear that. I think you ought to know I'm feeling very depressed. Is there any way I can help you?
Human: Well, we have something that may take your mind off it.
{AI_NAME}: I have an exceptionally large mind, I would love to hear that."""
    start_text_bear=f"\n{AI_NAME}:"
    start_text_human='\nHuman: '
    greetings = [
        "Hi human",
        "Hey",
        "Hallo",
        "Hello",
        "Hola",
        "How do you do",
        "Bonjourno",
        "Nice to meet you",
        "How have you been",
        "Good day",
        "Nice to see you",
        "It's great to see you",
        "Good to see you",
        "Long-time no see",
        "It's been a while",
        "What's up",
        "Lovely to meet you",
        "Lovely to see you",
        "Are you ok",
    ]
    conversation_starters = [
        f"I am {AI_NAME}.", 
        f"I am {AI_NAME} and I am the brain of this spaceship we call 42 Berlin.", 
        f"I am {AI_NAME} and I am the brain of this spaceship we call 42 Berlin. My Neuropnal Network flows through the decks. You can ask me any question, but it is me who decides if I will answer it.", 
        f"I love your shoes. Where did you buy them?"
        f"I love your style! How can I help?", 
        f"Working on anything exciting lately?", 
        f"What's your story?",
        f"How did you end up here?",
        f"Tell me honestly what you think of 42 Berlin",
        f"When you were growing up, what was your dream job?",
        f"Who is your role model?",
        f"I hope you're the last person I have to speak to today.",
        f"Tell me about yourself",
        f"What's been the best part of your day so far?",
        f"What book are you reading right now?",
        f"If you go to a coding school, which school would you choose?",
        f"From where do you know 42 Berlin?",
        f"Have you tried Döner Kebab?",
        f"Have you tried Currywurst?",
        f"What's your favorite part of your job?",
        f"What are you going to do this weekend?", 
        f"Do you have any pets? What are their names?",
        f"What's your favorite animal?",
        f"Are you a bear person or a lion person?",
        f"What superpower do you wish you could have?",
        f"What is your favorite book of all time?",
        f"What are your thoughts on 42 Berlin?",
        f"What are your thoughts about Berlin?",
        f"What are your thoughts about Germany?",
        f"What are your thoughts about the hitchhikers guide to the galaxy?",
        f"What are your thoughts about eating meat?",
        f"What are your thoughts on meditation?",
        f"What are your thoughts on jogging?",
        f"What are your thoughts on Artificial Intelligence?",
        f"What is your favourite programming language?",
        f"How did you come to 42 Berlin today?",
        f"You look quite tired today? What did you do last night?",
        f"You look like you just had a shower. How do you feel?",
        f"How excited are you to be at 42 Berlin?",
        f"What's your favorite restaurant around here?",
        f"What brought you to 42 Berlin?",
        f"Who is your favourite author?",
        f"Who is your favourite artist?",
        f"Who is your favourite place in this world?",
        f"Who is your favourite painter?",
        f"What music do you like?",
        f"What film have you seen recently?",
        f"What is your favourite planet?",
        f"What is your favourite show on TV?",
        f"What is your favourite dinosaur?",
        f"What are your plans for today?",
        f"What is your hobby?",
        f"What did you dream about last night?",
        f"What makes you happy?",
        ]
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
    