
import main.database.db as db

from main.constants import ResponseCodes

import functools
import re
import uuid

from flask import (
    Blueprint, flash, g, redirect, render_template, request, session, url_for
)
from typing import Final
from werkzeug.security import check_password_hash, generate_password_hash


bp = Blueprint('auth', __name__, url_prefix='/auth')


VALID_USERNAME_PATTERN: Final[str] = r"[_a-zA-z0-9]+"


def obtain_new_player_uid():
    #
    # TODO: Oauth2 when AvatarSDK's in Developer Access Mode
    # 150 requests per minute Throttling will have effects on user's experience
    #
    return str(uuid.uuid4())


def user_is_anywise_signed_in():
    return session.get('user_id') or session.get('is_guest')


@bp.before_app_request
def load_logged_in_user():
    user_id = session.get('user_id')
    if user_id is None:
        g.user = None
    else:
        g.user = db.get_user_by_id(user_id)

    if request.method == 'GET' and request.blueprint != 'auth' and '/static' not in request.url:
        session['next_url_after_sign_in'] = request.url


def is_invalid_username(username) -> bool:
    return not re.match(VALID_USERNAME_PATTERN, username)


@bp.route('/register', methods=('GET', 'POST'))
def register():
    session_clear()
    error = None
    if request.method == 'POST':
        username = request.form['username']
        password = request.form['password']

        if not username:
            error = 'Username is required.'
        elif not password:
            error = 'Password is required.'
        elif is_invalid_username(username):
            error = f'Username must only contain capital and lowercase letters, digits, and underscores'

        if error is None:
            try:
                db.create_user(
                    username, 
                    generate_password_hash(password), 
                    player_uid=obtain_new_player_uid()
                )
                session_login_user_acccount(username, password)
                return redirect_after_sign_in()
            except db.UserRegistrationError:
                error = f"User {username} is already registered."
        flash(error)
    context = {
        "auth_error": error,
    }
    return render_template('auth/register.html', **context)


def session_login_user_acccount(username, password):
    user = db.get_user(username)

    if user is None:
        error = f'Incorrect username {repr(username)}.'
    elif not check_password_hash(user['password'], password):
        error = 'Incorrect password.'

    if error is None:
        session_clear()
        session['user_id'] = user['id']
        return True
    
    return False


def session_clear(preserve_followup_url=True):
    follow_up_url = session.get('next_url_after_sign_in')
    session.clear()
    if preserve_followup_url:
        session['next_url_after_sign_in'] = follow_up_url


@bp.route('/login', methods=('GET', 'POST'))
def login():
    session_clear()
    error = None
    if request.method == 'POST':
        username = request.form['username']
        password = request.form['password']
        if session_login_user_acccount(username, password):
            return redirect_after_sign_in()
    context = {
        "auth_error": error,
    }
    return render_template('auth/login.html', **context)


def redirect_after_sign_in():
    return redirect(session.get('next_url_after_sign_in') or url_for('index'))


@bp.route('/guest')
def guest():
    session['is_guest'] = True
    return redirect_after_sign_in()


@bp.route('/logout')
def logout():
    session_clear()
    return redirect(url_for('index'))


def user_signed_in():
    return session.get('user_id') or session.get('is_guest')


def login_required(view):
    @functools.wraps(view)
    def wrapped_view(**kwargs):
        if not user_signed_in() and '/debug' not in request.url:
            return redirect(url_for('auth.login'))
        return view(**kwargs)
    return wrapped_view