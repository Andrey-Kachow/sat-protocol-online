import os
import json

from typing import Final

class Videogame:
    """
    Currently requires manual hardcoding, this class contains constants that point to
    the most relevant game builds and resources.
    """
    # # #
    #   # # http://satprotocol.online/instance/webglbuilds/b_june_anims_and_Ggrl/index.html # #
    #                                                                                         #
    MOST_RELEVANT_BUILD_ROUTE: Final[str] = 'webglbuilds/b_june_anims_and_Ggrl/index.html'# # #


class DatabaseManagement:
    TEMP_DUMMY_AVATAR_SDK_PLAYER_UID: Final[str] = 'at_least_something'


class Paths:
    MAIN_FOLDER:           Final[os.PathLike] = os.path.dirname(os.path.abspath(__file__))
    DATABASE_SCRIPTS:      Final[os.PathLike] = os.path.join(MAIN_FOLDER, 'database')
    BASE_WEBSITE_FOLDER:   Final[os.PathLike] = os.path.join(MAIN_FOLDER, '..')
    INSTANCE_STASH_FOLDER: Final[os.PathLike] = os.path.join(MAIN_FOLDER, 'instance_stash')
    INSTANCE_FOLDER:       Final[os.PathLike] = os.path.join(BASE_WEBSITE_FOLDER, 'instance')
    WEBGL_BUILDS_FOLDER:   Final[os.PathLike] = os.path.join(INSTANCE_FOLDER, 'webglbuilds')
    LOGS_FOLDER:           Final[os.PathLike] = os.path.join(BASE_WEBSITE_FOLDER, 'logs')
    ACCESS_LOG_FILE:       Final[os.PathLike] = os.path.join(LOGS_FOLDER, 'access.log')
    ERROR_LOG_FILE:        Final[os.PathLike] = os.path.join(LOGS_FOLDER, 'error.log')

    log_files = {
        'access': ACCESS_LOG_FILE,
        'error': ERROR_LOG_FILE 
    }


class AvatarStorage:
    AVATARS_ROOT:        Final[os.PathLike] = os.path.join(Paths.INSTANCE_FOLDER, 'avatars')
    GUESTS:              Final[os.PathLike] = os.path.join(AVATARS_ROOT, 'guests')
    ACCOUNT_HOLDERS:     Final[os.PathLike] = os.path.join(AVATARS_ROOT, 'accounts')
    DOWNLOAD_CHUNK_SIZE: Final[int]         = 8192


class Telegram:
    TOKEN_CONTEXT_KEY:   Final[str] = 'telegram_bot_token'
    CHAT_ID_CONTEXT_KEY: Final[str] = 'chat_id'


class ErrorHandling:
    DEBUG_MODE_ACTIVE = os.environ.get('FLASK_ENV') == 'development'
    DEFAULT_SECRET_KEY = 'YEK_TERCES_TLUAFED'
    DUMMY_STRING: Final[str] = "1_22_333_4444_55555_666666"


class ResponseCodes:
    """
    The specialized HTTP response codes, including internally used and interpreted.
    """
    SUCCESS_200:          tuple[str, int] = "", 200
    FILE_NOT_FOUND:       tuple[str, int] = "", 404
    SOMETHING_WENT_WRONG: tuple[str, int] = "", 418
    GLB_DOWNLOAD_FAIL:    tuple[str, int] = "", 418
    AVATAR_CREATION_FAIL: tuple[str, int] = "", 418
    THIS_IS_NOT_WELCOMED: tuple[str, int] = "", 420


def load_dictionary_constant_from_json(path: str) -> dict:
    with open(path) as json_file:
        return json.load(json_file)


GLOBAL_DICT: Final[str] = json.dumps({
    'DEBUG_MODE_ON': ErrorHandling.DEBUG_MODE_ACTIVE, 
})
