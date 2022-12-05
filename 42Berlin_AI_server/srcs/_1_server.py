import socket
import json
from utils_exceptions import ServerException, ClientFatalException
from _2_client_session import ClientSession
from utils_constants import HOST, PORT
from utils_debug import *
from utils_colors import Colors
from service_open_weather import ServiceOpenWeather

class Server:
    def __init__(self, host, port):
        self.host = host
        self.port = port
        self.socket = None
        self.client = None

    def init_server_socket(self):
        try:
            self.socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            self.socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
            self.socket.bind((HOST, PORT))
            self.socket.setblocking(1)
            self.socket.listen(5)
            self.socket.settimeout(3.0)
        except Exception as e:    
            raise ServerException(e)

    def accept_new_connection(self):
        connection, address = self.socket.accept()
        self.client = ClientSession(connection, address)
        print_debug(f"[SERVER]: accepted connection from client {self.client.config['address']}", P_SERVER)
        
    #    try:
    #        weather_report = ServiceOpenWeather.get_current_weather_berlin()
    #        response = {
    #            "type": "weather_init",
    #            "author": "AI BEAR",
    #            "string": weather_report
    #        }
    #        message = json.dumps(response)
    #        self.client.send_response(response=message)
    #    except Exception as e:
    #        print_error(f"failed to get weather report for berlin. ({str(e)})")

    def accept_and_handle_connection(self):
        print_debug("[SERVER]: waiting for new connection", P_SERVER)
        while True:
            try:
                self.accept_new_connection()
                self.client.interact()
            except TimeoutError:
                # print_debug("[SERVER]: time out, trying again", P_HIDDEN)
                continue
            except OSError as e:
                print_error(f"client socket error. Trying again. ({str(e)})")
                print_debug("[SERVER]: waiting for new connection", P_SERVER)
                continue
            except ClientFatalException as e:
                print_error(f"client session fatal error. Closing client session. ({str(e)})")
                if self.client is not None:
                    self.client.close_socket()
                print_debug("[SERVER]: waiting for new connection", P_SERVER)
                continue

    def launch(self):
        self.init_server_socket()
        self.accept_and_handle_connection()

        