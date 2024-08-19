# SatProtocol.online - A Web Platform for the Self-Attachment Therapy

Manual for running and accessing. Use Windows or Linux. MacOs may adjust the Linux setup.

## Essentials
The application has two components:
1) A website: Python Flask Web app.
    * The code is under `src`
2) Unity WebGL videogame.
    * The Unity Project Archive is under `unity_archive`


The WebGL build folder sits in server and Flask web app serves it when user is redirected to relevant route. The following sections uncover more requirements depending on the depth desired.

## Ways to Access

There are few ways to try out the software:
1) Go to the **satprotocol.online** live website
2) Run the webserver locally
    * With the game build provided
    * With the game built locally (need good machine)

The state of **satprotocol.online** will not be modified until the end of marking.
Any updates to the app will be performed to some other host for the sake of marking.
Playing the app online is a way that requires minimum setup.

During navigating the app, refer to **Usage Notes** section at the bottom.

## Playing the Game Online

Follow the link bellow:

http://satprotocol.online/

Recommended browser: Google Chrome. Definitely not Safari. If the server is dead (should not happen *before* the assessment), refer to **Running the Server Locally** section.

## Running the Server Locally

Clone the repository ans let `$REPO_DIR` be the path where `.git` directory sits. Notice that the directory with the path:
```
$REPO_DIR/src/main/static/webglbuilds/FreshBuild
```
...already contains the built WebGL video game build. Unless one desires to build the game themselves, this build can be used to get the game ready once the server is set up.

## Requirements for Flask Server

The repository contains `setup_and_run.*` twins scripts for Windows and Linux machines. They are designed to do the most work of running, but not all of it. The Windows setup may be more relevant as Unity Project assessment may be more convinient on Windows.

The project requires Python 3.1x to be successfully run locally.
This guide relies on python `venv` for less hassle during reproduction. Linux users may refer to the section called **Sad reality of Linux troubleshooting**.

### Python Pip Requirements. (no action needed)
`requirements.txt` contains the pip packages with the necessary dependencies for the app. Not all are required, but `pip freeze` makes replicating more convenient with `venv`.


### Required Environment Variables. (action needed)
The files `envvars` for Linux and `envvarswin` for Windows contain the list of environment variables required for running the app.

#### AvatarSDK API keys
The AvatarSDK API credentials which the app relies on during Child Avatar creation are stored in the environment variables. Professor Abbas Edalat was granted access to the keys. Andrey Popov is happy be in touch via `andr10xp@gmail.com` for sharing these strings too.

Replace the placeholder strings in the `envvars[win]` file, depending on the platform, with API keys provided:
```
# A pair of API credentials for Metaperson Creator Authorisation
export AVATAR_SDK_DEV_CLIENT_ID='ask Andrey Popov at andr10xp@gmail.com or professor Abbas Edalat for this informaiton'
export AVATAR_SDK_DEV_SECRET_KEY='ask Andrey Popov at andr10xp@gmail.com or professor Abbas Edalat for this informaiton'
```
#### FLASK_APP variable
The Flask application in this approach is specified using the environment variable that specifies directory where `__init__.py` file sits. i.e the "src/main" directory. Replace the contents of the `envvars` or `envvarswin` file with correct path `$REPO_DIR`.
```
export FLASK_APP='</path/to/the/repository>/src/main'
```
One may use `.` if terminal opened in the repository directory.
```
SET FLASK_APP='./src/main'
```

Any other environment variables can remain unchanged.
Admin username and password are used in sign in to admin panel.
The latter relies on existence of logs folder adjacent to main folder. But the admin panel is not doing much in local server anyway.

### Running the Server
If the previous steps were performed, then depending on the OS one may call either
```
.\setup_and_run.bat
```
or
```
./setup_and_run.sh
```
And wait for around 30 seconds until something like this shows up in the terminal output:
```
Running Flask application...
 * Serving Flask app 'src/main'
 * Debug mode: off
WARNING: This is a development server. Do not use it in a production deployment. Use a production WSGI server instead.
 * Running on http://127.0.0.1:5000
Press CTRL+C to quit
```
After that the URL `http://127.0.0.1:5000` can be opened in a web browser.

#### Sad reality of Linux troubleshooting
This project uses some modern Python features such as type hints that are available in 3.10+.
Linux have a certain system-wide python version that might be too old.
For example, Ubuntu 20.04 has Python 3.8. Therefore please use python3.10+.

Additional installation commands that may require:
```
sudo add-apt-repository ppa:deadsnakes/ppa
sudo apt update
sudo apt install python3.10 python3.10-venv python3.10-dev
```
And maybe
```
sudo apt install python3-flask
```

If some *c’est la vie* happened, an ad-hoc attempts to `pip install x` or `sudo apt install x` can be performed, or just 0 marks.

### Building the Game (optional)

The submitted deliverable was designed to have a ready WebGL build to be enjoyed when running server.
The existing game build should alredy be stored under
```
src/main/static/webglbuilds/FreshBuild/Build
```
with some `.js` and `.gz``= files in it.
If they are misteriously abscent or building the game from stratch feels necessary, please refer to the following sections.

#### Blue Screen Of Death problem when Running WebGL Builds.
If someone wants, they can build the game in Unity. In that case the recommended step would be to run the python script `help_scripts\throttle_pids.py`. It sets the affinity of some WebGL building processes in Unity to not use all CPU cores. The build can take 150 seconds even on a powerful machine and maybe this time can be enjoyed more if other applicaions get resources too. Otherwise everything can freeze and seem like not working.
Change the line:
```
ALLOWED_NUM_CORES_FOR_WASM = 10
```
to assign the numeric value that is less than the number of cores.
For example if your computer has 16 cores, setting the constant to 10 should work well, because 6 cores will be availablee for other applications, that shall not freeze when build process is running.
The README author had BSOD errors before employing the script that throttles the affinity.

#### Step-by-step Build Steps

0) (optional)
    * Clean the laptop from dust
    * Put the laptop on elevation
    * Or in the fridge

1) Start the python script and keep it in the background.

In Unity Editor do the following steps:

2) Add the following scenes to build:
    * StartupScene__Main
    * MainMenuScene
    * QuickStartScene
3) In build settings folder select choose the empty folder named `FreshBuild` and store it in `$REPO_DIR/src/main/static/webglbuilds`. The server is capable of serving many game builds, but the default redirect route for the `satprotocol.online/game` route.
4) Click Build. Or Build and Run. In latter case ignore the opened browser tab with some localhost build. The standalone WebGL will not work without Flaks Server as the internal JS plugin functions communicate with server.

If the game build succeded and the game build is in the correct directory, refer to the **Running the Server** section to run the Flask Server to serve the new game build. 

# User Journey (Unless you have other plans)
1) Press Begin Trials
2) When redirected to authentication, press sign in as guest 
3) In childhood editor, upload a portrait photo of a child
4) Edit the child in the editor and press "Save & Play" when you ready to enter the game
5) Wait the game to download without altering tabs or other apps (or it will seem that the game is never going to download) 
6) Press quick start
7) In Game wait for child to spawn
8) Press E and begin SAT using the "Begin SAT" button
9) Press buttons that are related to different emotions of Self-Attachment Therapy.
10) (optional) Perform an unplanned diversified penetration test of the website and report the found vulnerabilities to andr10xp@gmail.com

# Game Controls:
### Looking around: Mouse
### Moving: W,A,S,D (or arrow keys)
### Press E
to toggle between UI buttons select with mouse and In-game First Person motion modes.
### Press space
to jump
### Mouse click
to select buttons in the UI

## Usage Notes
* When creating the Child Avatar, some human trial participants discovered that the "Generate Child Avatar" button can be pressed but not do the action. The button counts as pressed if the button got disabled afterwards. This happened to maybe 1 out of 15 cases. Press the button for the second time if nothing happens.
* Refresh the page when suspecting the bug
* Childhood-self editor a.k. Metaperson Creator freezes when user clicks outside the frame.
* The game downloads 150MB, so waiting time may take 10-40 seconds depending on Wi-Fi strength.