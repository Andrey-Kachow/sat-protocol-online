#!/bin/bash

rm -rf venv

if [ -f envvars ]; then
    echo "Sourcing environment variables from envvars file"
    source envvars
else
    echo "envvars file not found. Please create it with the required environment variables."
    exit 1
fi

echo "Creating virtual environment..."
python3.10 -m venv venv

echo "Activating virtual environment..."
source venv/bin/activate

echo "Installing requirements..."
pip install -r requirements.txt

echo "Running Flask application..."
flask run
