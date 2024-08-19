
from main.constants import Paths
from main.database.db import get_db, close_db

import click
import os

from flask import current_app


def init_app(app):
    """Register the CLI of the Flask application"""
    app.teardown_appcontext(close_db)
    app.cli.add_command(init_db_command)
    app.cli.add_command(drop_db_command)


def execute_sql_script(script_name: str):
    script_path = os.path.join(Paths.DATABASE_SCRIPTS, script_name)
    with current_app.open_resource(script_path) as f:
        get_db().executescript(f.read().decode('utf8'))


@click.command('init-db')
def init_db_command():
    """Create missing tables using the schema"""
    execute_sql_script('schema.sql')
    click.echo('Initialized the database.')


@click.command('drop-db')
def drop_db_command():
    """Drop the database if required"""
    execute_sql_script('drop.sql')
    click.echo('The database is dismantled piece by piece.')