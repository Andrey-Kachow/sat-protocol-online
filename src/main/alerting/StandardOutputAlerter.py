from main.alerting.BaseAlerter import BaseAlerter

class StandardOutputAlerter(BaseAlerter):
    def alert(self, msg: str, **kwargs):
        print(msg, **kwargs)


class DebugLogAlerter(StandardOutputAlerter):
    def __init__(self, is_debug) -> None:
        self.is_debug = is_debug

    def alert(self, msg: str, **kwargs):
        if self.is_debug:
            super().alert(msg, **kwargs)