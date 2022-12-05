from _1_server import Server
from utils_exceptions import ServerException
from utils_constants import HOST, PORT
from utils_debug import *
from utils_colors import Colors

def execute_main():
    try:
        server = Server(HOST, PORT)
        server.launch()
    except KeyboardInterrupt:
        print_warning(f"keyboard interruption. Exiting server.")
        if server.client is not None:
            server.client.close_socket()
        exit(1)
    except ServerException as e:
        print_error(f"socket error. Exiting server. ({str(e)})")
        exit(1)

if __name__ == "__main__":
    execute_main()