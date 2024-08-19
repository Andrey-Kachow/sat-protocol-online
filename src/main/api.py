import main.database.db as db
import main.auth as auth

from main.alerting.alerting import get_alerter
from main.constants import ErrorHandling, ResponseCodes, AvatarStorage, Telegram
from main.models import User

import json
import os
import requests

from flask import (
    Blueprint, flash, g, redirect, render_template, request, session, url_for, jsonify
)

bp = Blueprint('api', __name__, url_prefix='/api')

GUEST_ID_SUBSTITUTE: int = -1


@bp.route('/dummy/reference')
def get_dummy_reference():
    return ErrorHandling.DUMMY_STRING


@bp.route('/telegram/context', methods=['GET', 'PUT'])
def telegram_context():
    print("eee")
    if request.method == 'GET':
        deliverable_context = {
            Telegram.CHAT_ID_CONTEXT_KEY:
                get_alerter().get_context().get(Telegram.CHAT_ID_CONTEXT_KEY, ErrorHandling.DUMMY_STRING),
            Telegram.TOKEN_CONTEXT_KEY: 
                get_alerter().get_context().get(Telegram.CHAT_ID_CONTEXT_KEY, ErrorHandling.DUMMY_STRING),
            }
        return json.dumps(deliverable_context), ResponseCodes.SUCCESS_200, { 'ContentType':'application/json' }
    get_alerter().add_context(request.json)


@bp.route("/unity/sync")
def sync_unity():
    pass


@bp.route("/unity/session", methods=['POST'])
def send_session_to_unity():
    """Serializes relevant session attributes as CSV text with two line breaks custom delimiter"""
    return "\n\n".join(map(str, [
        session.get('user_id') or GUEST_ID_SUBSTITUTE,
        session.get('avatar_data', {}).get('avatar_code') or 'null',
        session.get('avatar_data', {}).get('download_url') or 'null',
    ]))

@bp.route("/avatarsdk/clientcreds", methods=["POST"])
def get_avatarsdk_client_credentials():
    if auth.user_is_anywise_signed_in():
        response_dict = {
            "client_id": os.environ.get("AVATAR_SDK_DEV_CLIENT_ID"),
            "client_secret": os.environ.get("AVATAR_SDK_DEV_SECRET_KEY")
        }
        print(response_dict)
        return jsonify(response_dict)
    return ResponseCodes.THIS_IS_NOT_WELCOMED


@bp.route("/avatarsdk/isready", methods=["POST"])
def record_avatar_export_data():
    session['avatar_data'] = request.get_json()
    user_id = session.get('user_id')
    if user_id:
        try:
            db.update_user_avatar_data(user_id, session['avatar_data'])
        except db.AvatarCreationError as err:
            get_alerter().alert(err)
            return ResponseCodes.AVATAR_CREATION_FAIL
    return ResponseCodes.SUCCESS_200


@bp.route("/avatarsdk/download", methods=["POST"])
def start_download_glb_now():
    """Currently reserved as plan B method, as AvatarSDK is providing storage for us"""
    avatar_data = request.get_json()
    destination_filename = os.path.join(AvatarStorage.GUESTS, avatar_data['avatar_code'] + '.glb')
    try:
        with requests.get(avatar_data['download_url'], stream=True) as response_stream:
            response_stream.raise_for_status()
            with open(destination_filename, 'wb') as f_glb:
                for chunk in response_stream.iter_content(chunk_size=AvatarStorage.DOWNLOAD_CHUNK_SIZE): 
                    f_glb.write(chunk)
                #
            ##
        ###
    except requests.HTTPError as err:
        get_alerter().alert(err)
        return ResponseCodes.GLB_DOWNLOAD_FAIL
    
    get_alerter().alert("Successful Download")
    return ResponseCodes.SUCCESS_200
















                                                                                                                                                                                                                                                 


























';';';';';'  # Hi there! Have a good way :D ;;;;;;;;;;;;;;;;;;;;;;;
