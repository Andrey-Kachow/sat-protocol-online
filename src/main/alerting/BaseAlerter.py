class BaseAlerter:
    def alert(self, _msg: str):
        pass

    def add_context(self, **extra_context):
        pass

    def get_context(self):
        return {}