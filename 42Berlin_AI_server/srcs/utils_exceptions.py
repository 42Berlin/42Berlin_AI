class ServerException(Exception):
    pass

class ClientFatalException(Exception):
    pass

class ClientException(Exception):
    pass

class RequestException(Exception):
    pass


class TimerException(Exception):
    """A custom exception used to report errors in use of Timer class"""

