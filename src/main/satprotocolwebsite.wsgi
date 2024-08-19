#!/usr/bin/python
import sys, os
import subprocess

website_folder = os.path.join(
        os.path.dirname(
            os.path.abspath(__file__)
        ),
        '..'
    )

sys.path.append(website_folder)

from main import create_app
application = create_app()
