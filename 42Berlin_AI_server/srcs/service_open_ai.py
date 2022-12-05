import os
from dotenv import load_dotenv
import openai
from utils_debug import *
from utils_colors import Colors

load_dotenv()
openai.api_key = os.getenv('OPENAI_API_KEY')
print_debug(f"OPENAI_API_KEY: {openai.api_key}", P_CONFIG)

class ServiceOpenAI:
    def __init__(self):
        pass

    # API reference of call https://beta.openai.com/docs/api-reference/completions/create 
    def call_gpt3(self, prompt, engine='text-davinci-002', response_length=1024,
         temperature=1, top_p=1, frequency_penalty=2, presence_penalty=1,
         start_text='', restart_text='', stop_seq=[]):
        
        response = openai.Completion.create(
            prompt=prompt + start_text,
            engine=engine,
            max_tokens=response_length,
            temperature=temperature,
            top_p=top_p,
            frequency_penalty=frequency_penalty,
            presence_penalty=presence_penalty,
            stop=stop_seq,
        )
        answer = response.choices[0]['text']
        return answer

    def call_gpt3_wrapper(self, chat_history):
        return self.call_gpt3(chat_history,
                            start_text=f"\n{AI_NAME}:",
                            restart_text='\nHuman: ',
                            stop_seq=['\nHuman:', '\n'])

    def clean_chat_history(self, chat_history):
        chat_list = chat_history.splitlines()
        cleaned_chat_history = ""
        last_talker = ""
        first_ending_line = True
        for (i, line) in enumerate(chat_list):
            if (i <= CHAT_BEGINNING):
                # print_color(f"{i} : {line}", Colors.BLUE)
                if (i == CHAT_BEGINNING):
                    last_talker = line.split(":")[0]
                cleaned_chat_history += line + "\n"
            elif (i >= (len(chat_list) - CHAT_END)):
                if (first_ending_line == True):
                    first_ending_line = False
                    if (line.split(":")[0] == last_talker):
                        # print_color(f"{i} : {line}", Colors.GREEN)
                        continue
                # print_color(f"{i} : {line}", Colors.ORANGE)
                cleaned_chat_history += line + "\n"
            # else:
                # print_color(f"{i} : {line}", Colors.PINK)
        return cleaned_chat_history

    def get_gpt3_answer(self, chat_history):
        try:
            cleaned_chat_history = self.clean_chat_history(chat_history)
            gpt_answer = self.call_gpt3_wrapper(cleaned_chat_history)
            while gpt_answer == '' or gpt_answer == ' ' :
                gpt_answer = self.call_gpt3_wrapper(cleaned_chat_history)
            return gpt_answer
        except Exception as e:
            print_error("Error in OpenAI gpt3 call: %s. Generating default response." % e)
            return "I didn't understand."