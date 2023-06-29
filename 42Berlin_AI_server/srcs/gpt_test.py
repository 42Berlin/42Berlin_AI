from utils_debug import *
from utils_constants import *
from utils_chat import Chat
import json
from service_open_ai import ServiceOpenAI
from google.protobuf.json_format import MessageToDict
import os
import shutil


############################### GENERATE ANSWER ##########################################
config = {
    "chat": Chat(),
    "chat_history": None,
    "request_intent": None,
    "intent": None,
}
config['chat_history'] = config['chat'].initial_feeding
#config['chat_history'] = config['chat'].initial_feeding
#print(config['chat_history'])
input = [{"role": "system", "content": "You are a helpful assistant."}, {"role": "user", "content": "Hello!"}]
#print(ServiceOpenAI().get_gpt3_answer(config['chat_history']))
print(ServiceOpenAI().get_gpt3_answer((config["chat_history"])))