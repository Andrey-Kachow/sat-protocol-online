import main.database.db as db
from main.constants import Paths, ErrorHandling
from main.models import User

import functools
import os

from flask import (
    Blueprint, flash, g, redirect, render_template, request, session, url_for
)
from werkzeug.security import check_password_hash, generate_password_hash


ADMIN_CREDS = {}

def setup_admin_credentials():
    ADMIN_CREDS["admin_username_hash"] = ErrorHandling.DUMMY_STRING
    ADMIN_CREDS["admin_password_hash"] = ErrorHandling.DUMMY_STRING
    admin_username, admin_password = (
        os.environ.get('SAT_PROTOCOL_ONLINE_ADMIN_USERNAME'),
        os.environ.get('SAT_PROTOCOL_ONLINE_ADMIN_PASSWORD')
    )
    if not (admin_username or admin_password):
        #
        #  TODO: Alert admin IRL via some monitoring service...
        #
        return
    ADMIN_CREDS["admin_username_hash"] = str(generate_password_hash(admin_username))
    ADMIN_CREDS["admin_password_hash"] = str(generate_password_hash(admin_password))

bp = Blueprint('admin', __name__, url_prefix='/admin')


def is_admin():
    return session.get('is_admin')
        

def admin_permission_required(view):
    @functools.wraps(view)
    def wrapped_view(**kwargs):
        if not is_admin():
            return redirect(url_for('admin.login'))
        return view(**kwargs)
    return wrapped_view


@bp.route('/login', methods=('GET', 'POST'))
def login():
    error = None
    if request.method == 'POST':
        username = request.form['username']
        password = request.form['password']
        
        print(username, password, ADMIN_CREDS['admin_username_hash'], ADMIN_CREDS['admin_password_hash'])
        print(os.environ['SAT_PROTOCOL_ONLINE_ADMIN_USERNAME'], os.environ['SAT_PROTOCOL_ONLINE_ADMIN_PASSWORD'])

        if not all([
            check_password_hash(ADMIN_CREDS['admin_username_hash'], username),
            check_password_hash(ADMIN_CREDS['admin_password_hash'], password)
        ]):
            error = 'Access denied. Try again'

        if error is None:
            session.clear()
            session['is_admin'] = True
            return redirect(url_for('admin.dashboard'))
        flash(error)
    context = {
        "auth_error": error,
    }
    session.clear()
    return render_template('admin/login.html', **context)


@bp.route('/dashboard', methods=['GET', 'POST'])
@admin_permission_required
def dashboard():
    error = None
    users = list(map(User.from_row, db.get_all_users()))
    context = {
        "fetch_error": error,
        "users": users,
    }
    return render_template('admin/dashboard.html', **context)


def tail_logs(filepath, number_lines: int):
    queue = [None for _ in range(number_lines)]
    queue_ptr = 0
    with open(filepath) as log_file:
        for line in log_file:
            queue[queue_ptr] = line
            queue_ptr = (queue_ptr + 1) % number_lines
    return [line for line in queue[:queue_ptr] + queue[queue_ptr:]]


@bp.route('/logs/<string:logtype>', methods=['GET', 'POST'])
@admin_permission_required
def logs(logtype=''):
    if logtype not in Paths.log_files.keys():
        return redirect(url_for('admin.logs', logtype='access'))
    num_latest_logs = 25
    context = {
        'logs': tail_logs(Paths.log_files[logtype], num_latest_logs)
    }
    return render_template('admin/logs.html', **context)


@bp.route('/exit')
def exit():
    session.clear()
    return redirect(url_for('index'))