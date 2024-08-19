

import main.admin as admin
import main.alerting.alerting as alerting
import main.api as api
import main.auth as auth
import main.database.commands as db_cmd

from main.constants import *

import errno
import os
import sys

from flask import (
    Flask,
    redirect,
    render_template,
    url_for,
    send_file,
)
from datetime import timedelta


def create_unless_exists(path):
    try:
        os.makedirs(path)
    except FileExistsError as that_is_desired:
        return that_is_desired
    return None
        

def init_instance_dir() -> bool:
    try:
        return all([
            create_unless_exists(path) for path in [
                Paths.INSTANCE_FOLDER,
                AvatarStorage.AVATARS_ROOT,
                AvatarStorage.ACCOUNT_HOLDERS,
                AvatarStorage.GUESTS,
                Paths.WEBGL_BUILDS_FOLDER
            ]
        ])
    except OSError as bad_error:
        print(bad_error)
    return False


def try_send_file(path):
    try:
        return send_file(path)
    except IsADirectoryError: 
        return "Page not Found. Try different Link (Code: 500)"
    except FileNotFoundError:
        return ResponseCodes.FILE_NOT_FOUND
    except:
        return ResponseCodes.SOMETHING_WENT_WRONG

def init_instance_stash():
    return True
    # for folder_name in os.listdir(Paths.INSTANCE_STASH_FOLDER):
    #     folder_path = os.path.join(Paths.INSTANCE_STASH_FOLDER, folder_name)
    #     subprocess.run(f"""

    #         zip -r ss <directory name>

    #     """.strip().split())

def generate_secret_key():
    key = os.environ.get('FLASK_APP_SECRET_KEY')
    if not key:
        if not ErrorHandling.DEBUG_MODE_ACTIVE:
            assert False
        key = ErrorHandling.DEFAULT_SECRET_KEY
    return key

def create_app():
    app = Flask(__name__, instance_relative_config=True)
    app.config.from_mapping(
        SECRET_KEY=generate_secret_key(),
        DATABASE=os.path.join(Paths.INSTANCE_FOLDER, 'satprotocol.sqlite'),
        PERMANENT_SESSION_LIFETIME=timedelta(days=7)
    )

    init_instance_dir()
    if not init_instance_stash():
        sys.exit(errno.ECANCELED)

    db_cmd.init_app(app) 
    admin.setup_admin_credentials()
    alerting.create_alerter()

    app.register_blueprint(admin.bp)
    app.register_blueprint(auth.bp)
    app.register_blueprint(api.bp)

    @app.after_request
    def after_request_func(response):
        response.headers.add('Access-Control-Allow-Origin', '*')
        response.headers.add('Access-Control-Allow-Headers', 'Content-Type, Authorization')
        response.headers.add('Access-Control-Allow-Methods', 'GET, POST, PATCH, DELETE, OPTIONS')
        return response
    
    @app.context_processor
    def inject_top_level_context():
        """Adds global dictionary to the Jinja2 context as a subset of app context for JavaScript"""
        return{
            'global_dict': GLOBAL_DICT
        }

    @app.errorhandler(500)
    def internal_error(error):
        return "Code: 500. <br> If you are a Good Samaritan, contact admin via email andrey.popov20@imperial.ac.uk"

    @app.route('/')
    def index():
        return render_template('index.html')

    @app.route('/about')
    def about():
        return render_template('about.html')

    @app.route('/exercises')
    @auth.login_required
    def exercises():
        return render_template('exercises.html')

    @app.route("/profile/<string:username>")
    @auth.login_required
    def profile(username):
        return render_template('profile.html')
    
    @app.route('/avatar')
    @auth.login_required
    def avatar_editor():
        return render_template('avatar_editor.html')

    @app.route('/game')
    @auth.login_required
    def game():
        fresh_build_path = os.path.join(Paths.MAIN_FOLDER, 'static', 'webglbuilds', 'FreshBuild', 'index.html')
        if os.path.exists(fresh_build_path):
            return redirect(url_for('fresh', path='index.html'))
        return redirect(url_for('instance', path=Videogame.MOST_RELEVANT_BUILD_ROUTE))


    @app.route('/static/webglbuilds/FreshBuild/<path:path>')
    @auth.login_required
    def fresh(path):
        return try_send_file(os.path.join(Paths.MAIN_FOLDER, 'static', 'webglbuilds', 'FreshBuild', path))


    @app.route('/instance/<path:path>')
    @auth.login_required
    def instance(path):
        return try_send_file(os.path.join(Paths.INSTANCE_FOLDER, path))

    return app
