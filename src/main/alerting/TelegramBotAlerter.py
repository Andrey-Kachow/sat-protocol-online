import requests

from main.alerting.BaseAlerter import BaseAlerter

from typing import Final, List


CONSERVATIVE_MESSAGE_SIZE_LIMIT: Final[int] = 2048


class TelegramBotAlerter(BaseAlerter):
    def __init__(self):
        self.context = {}

    def __internal_split_messages(self, msg: str) -> List[str]:
        lines = msg.split('\n')
        chunks = []
        current_chunk = []
        current_chunk_size = 0
        for line in lines:
            line_size = len(line)
            if current_chunk_size + line_size > CONSERVATIVE_MESSAGE_SIZE_LIMIT:
                chunks.append(current_chunk)
                current_chunk = []
                current_chunk_size = 0
            current_chunk.append(line)
            current_chunk_size += line_size
        return ['\n'.join(chunk) for chunk in chunks]

    def alert(self, msg: str):
        if not ('chat_id' in self.context and 'telegram_bot_token' in self.context):
            return

        message_parts = self.__internal_split_messages(msg)
        
        for message_part in message_parts:
            requests.get(
                f'https://api.telegram.org/bot{self.context["telegram_bot_token"]}/sendMessage',
                json={
                    'chat_id': self.context['chat_id'],
                    'text': f'```\n{message_part}\n```'
                }
            )

    def add_context(self, **extra_context):
        self.context = {**self.context, **extra_context}
        
    def get_context(self):
        return self.context