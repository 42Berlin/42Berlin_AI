from utils_constants import HOST, PORT
from utils_debug import *
from datetime import datetime
from utils_colors import Colors

class Timer:
    def __init__(self):
        self.ideal_EOS = None
        self.final_EOS = None
        self.intent_start = None
        self.intent_end = None
        self.gpt3_start = None
        self.gpt3_end = None

    def set_ideal_EOS(self, ideal_EOS):
        self.ideal_EOS = ideal_EOS
        
    def set_final_EOS(self, final_EOS):
        self.final_EOS = final_EOS
        
    def set_intent_start(self, intent_start):
        self.intent_start = intent_start
        
    def set_intent_end(self, intent_end):
        self.intent_end = intent_end
        
    def set_gpt3_start(self, gpt3_start):
        self.gpt3_start = gpt3_start
        
    def set_gpt3_start(self, gpt3_start):
        self.gpt3_start = gpt3_start
        
    def convert_str_to_datetime(self, string_format): # '%d/%m/%Y %H:%M:%S.%f' example: 17/10/2022 18:32:59.481939
        return datetime.strptime(string_format, '%d/%m/%Y %H:%M:%S.%f')