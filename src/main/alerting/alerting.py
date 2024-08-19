from main.alerting.TelegramBotAlerter import TelegramBotAlerter
from main.alerting.StandardOutputAlerter import DebugLogAlerter
from main.constants import ErrorHandling


class Alerting:
    alerter = None


def create_alerter(use_telegram_anyway=False):
    Alerting.alerter = DebugLogAlerter(ErrorHandling.DEBUG_MODE_ACTIVE)
    if use_telegram_anyway:
        Alerting.alerter = TelegramBotAlerter()


def get_alerter():
    return Alerting.alerter