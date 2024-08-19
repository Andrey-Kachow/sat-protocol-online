import sqlite3

from flask import current_app, g

class UserRegistrationError(Exception):
    pass

class AvatarCreationError(Exception):
    pass


def get_db():
    if 'db' not in g:
        g.db = sqlite3.connect(
            current_app.config['DATABASE'],
            detect_types=sqlite3.PARSE_DECLTYPES
        )
        g.db.row_factory = sqlite3.Row

    return g.db


def close_db(e=None):
    db = g.pop('db', None)
    if db is not None:
        db.close()


def create_user(username, password_hash, player_uid):
    db = get_db()
    try:
        db.execute(
            "INSERT INTO user (username, password, playeruid) VALUES (?, ?, ?)",
            (username, password_hash, player_uid),
        )
        db.commit()
    except db.IntegrityError as e:
        print(e)
        raise UserRegistrationError()


def get_user(username):
    return get_db().execute(
        'SELECT * FROM user WHERE username = ?', (username,)
    ).fetchone()


def get_user_by_id(user_id):
    return get_db().execute(
        'SELECT * FROM user WHERE id = ?', (user_id,)
    ).fetchone()


def get_all_users():
    return get_db().execute('SELECT * FROM user').fetchall()


def create_child_avatar(user_id, avatar_data):
    db = get_db()
    try:
        db.execute(
            "INSERT INTO user (user_id, download_url, avatarsdk_code) VALUES (?, ?, ?)",
            (user_id, avatar_data['download_url'], avatar_data['avatar_code']),
        )
        db.commit()
    except db.IntegrityError as e:
        print(e)
        raise AvatarCreationError()