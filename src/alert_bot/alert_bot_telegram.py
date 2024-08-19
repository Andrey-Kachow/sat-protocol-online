#!/usr/bin/env python

import json
import html
import httpx
import logging
import os
import requests

from datetime import datetime
from telegram import Update
from telegram.constants import ParseMode
from telegram.ext import (
    Application,
    CommandHandler, 
    MessageHandler,
    ContextTypes, 
    filters
)
from typing import Final

MESSAGE_SIZE_LIMIT: Final[int] = 4096
CONSERVATIVE_SIZE_LIMIT: Final[int] = MESSAGE_SIZE_LIMIT - 300
TOKEN: Final[str] = os.environ.get('SAT_PROTOCOL_ONLINE_ALERT_BOT_TOKEN')

context_host = 'http://satprotocol.online' 
CONTEXT_ENTRY_POINT: Final[str] = context_host + '/api/telegram/context'
DUMMINESS_ENTRY_POINT: Final[str] = context_host + '/api/dummy/reference'

satprotocol_online_monitoring = {}
response = requests.get(CONTEXT_ENTRY_POINT)
print(f'\n\n\n\nresponse is {response} ---- ({datetime.now()})\n\n\n')
recipient = response.json()
DUMMY_ID: Final[str] = requests.get(DUMMINESS_ENTRY_POINT).text

# Enable logging
logging.basicConfig(
    format="%(asctime)s - %(name)s - %(levelname)s - %(message)s", level=logging.INFO
)
# set higher logging level for httpx to avoid all GET and POST requests being logged
logging.getLogger("httpx").setLevel(logging.WARNING)

logger = logging.getLogger(__name__)


# bot = telegram.Bot(token=TOKEN)

async def start_command(update: Update, context: ContextTypes.DEFAULT_TYPE):
    if recipient.get('chat_id') != DUMMY_ID:
        await update.message.reply_text("There may be only once recipient")
        return
    print(f'\n\n{update}\n#\n{context}\n\n')
    chat_id = update.message.chat_id
    recipient['chat_id'] = chat_id
    requests.put(CONTEXT_ENTRY_POINT, json=recipient)

    await update.message.reply_text("You are signed up for updates from satprotocol.online")

import traceback
async def error_handler(update: object, context: ContextTypes.DEFAULT_TYPE) -> None:
    logger.error(msg="Exception while handling an update:", exc_info=context.error)
    tb_list = traceback.format_exception(None, context.error, context.error.__traceback__)
    tb_string = "".join(tb_list)

    # Build the message with some markup and additional information about what happened.
    # You might need to add some logic to deal with messages longer than the 4096 character limit.
    update_str = update.to_dict() if isinstance(update, Update) else str(update)
    messages = [
        f"An exception was raised while handling an update\n",
        f"update = {json.dumps(update_str, indent=2, ensure_ascii=False)}",
        f"context.chat_data = {str(context.chat_data)}\n\n",
        f"context.user_data = {str(context.user_data)}\n\n",
        tb_string,
    ]

    def html_pre(txt_message):
        return f"<pre>{html.escape(txt_message)}</pre>" 

    #
    #   Naive attemt to handle large messages...
    #
    for message in messages:
        while message:
            await context.bot.send_message(
                chat_id=recipient['chat_id'], text=html_pre(message[:CONSERVATIVE_SIZE_LIMIT]), parse_mode=ParseMode.HTML
            )
            message = message[CONSERVATIVE_SIZE_LIMIT:]


    # Finally, send the message
    await context.bot.send_message(
        chat_id=recipient['chat_id'], text=message[:MESSAGE_SIZE_LIMIT], parse_mode=ParseMode.HTML
    )

async def help_command(update: Update, context: ContextTypes.DEFAULT_TYPE) -> None:
    await update.message.reply_text("Help!")


async def start_monitoring(update: Update, context: ContextTypes.DEFAULT_TYPE) -> None:
    satprotocol_online_monitoring[recipient['chat_id']] = True
    await update.message.reply_text("Started Monitoring satprotocol.online")


async def stop_monitoring(update: Update, context: ContextTypes.DEFAULT_TYPE) -> None:
    satprotocol_online_monitoring[recipient['chat_id']] = False
    await update.message.reply_text("Stopped monitoring satprotocol.online")


def main():
    application = Application.builder().token(TOKEN).build()
    application.add_handler(CommandHandler("start", start_command))
    application.add_handler(CommandHandler("help", help_command))
    application.add_handler(MessageHandler(filters.Regex('Begin Monitoring satprotocol.online'), start_monitoring))
    application.add_handler(MessageHandler(filters.Regex('Stop Monitoring satprotocol.online'), stop_monitoring))
    
    application.add_error_handler(error_handler)

    application.run_polling()
    print("lol")

def Main():
    try:
        main()
    except (httpx.ConnectError, httpx.NetworkError) as err:  #
        print(err)

if __name__ == "__main__":
    Main()