@echo off

if exist venv (
    echo Deleting existing virtual environment...
    rmdir /s /q venv
)

if exist envvars (
    echo Sourcing environment variables from envvars file
    for /f "tokens=*" %%i in (envvarswin) do set %%i
) else (
    echo envvars file not found. Please create it with the required environment variables.
    exit /b 1
)

echo Creating virtual environment...
python -m venv venv

echo Activating virtual environment...
call venv\Scripts\activate

echo Installing requirements...
pip install -r requirements.txt

echo Running Flask application...
flask run
