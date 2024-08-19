async function fetchCredentials() {
  const response = await fetch("/api/avatarsdk/clientcreds", {
    method: "POST",
    headers: {
      "Content-Type": "text/plain",
      "Accept": "application/json",
      "Content-Length": 0
    },
    body: ""
  });
  return response.json();
}

async function sendAuthMessage(evt) {
  const credentialsForAvatarSDK = await credentials;
  let authenticationMessage = {
    "eventName": "authenticate",
    "clientId": credentialsForAvatarSDK.client_id,
    "clientSecret": credentialsForAvatarSDK.client_secret,
  };
  evt.source.postMessage(authenticationMessage, "*");
}

function setUIParameters(evt) {
  let uiParametersMessage = {
    "eventName": "set_ui_parameters",
    "isExportButtonVisible": false,
    "isLoginButtonVisible": false,
    "outfitsBlackList": [
      "dress_Shaki", "LORI", "Akna", "ARARAT", "SEVAN",
      "jacket_Arteni", "jacket_Tavush", "top_Urasar",
      "jacket_Oroklini", "shirt_Olympus",
      "pants_Arteni", "shorts_Vedi",
      "shoes_LORI",
    ],
    // Desktop version specific parameters
    "closeExportDialogWhenExportCompleted": true,
    "isLanguageSelectionVisible": true,
    "language": "EN"
  };
  evt.source.postMessage(uiParametersMessage, "*");
}

function processAuthenticationEvent(evt) {
  if (!evt.data.isAuthenticated) {
    if (appContext.DEBUG_MODE_ON) {
      alert(evt.data.errorMessage)
    }
  }
}

function processAvatarGeneratedEvent(_evt) {
  displayLoadingCompleted();
  setTimeout(removeInactiveOverlay, 1500);
}

function processModelExportedEvent(evt) {
  avatarData.avatarCode = evt.data.avatarCode;
  avatarData.downloadURL = evt.data.url;
  avatarExportSemaphore.release();
}

function processActionAvailabilityChangedEvent(evt) {
  const data = evt.data;
  if (data.actionName === "avatar_generation") {
    setButtonClickability(Buttons.generateAvatar, ClickAbility.SET(data.isAvailable));
  } else if (data.actionName === "avatar_export") {
    setButtonClickability(Buttons.saveAndPlay, ClickAbility.SET(data.isAvailable));
  } else if (data.actionName === "avatar_screenshot") {
    // We don't care about this here atm
  }
}

function setExportParameters(evt) {
  const exportParametersMessage = {
    "eventName": "set_export_parameters",
    "format": "glb",
    "lod": 1,
    "textureProfile": "1K.jpg",
    "useZip": false
  };
  evt.source.postMessage(exportParametersMessage, "*");
}

let eventSourceOfAvatarSDK = null;

function onWindowMessage(evt) {
  if (evt.type === "message") {
    if (evt.data?.source === "metaperson_creator") {
      eventSourceOfAvatarSDK = evt.source;
      let data = evt.data;
      let evtName = data?.eventName;
      switch (evtName) {
        case "metaperson_creator_loaded":
          sendAuthMessage(evt);
          setUIParameters(evt);
          setExportParameters(evt);
          break;
        case "authentication_status":
          processAuthenticationEvent(evt);
          break;
        case "model_generated":
          processAvatarGeneratedEvent(evt);
          break;
        case "model_exported":
          console.log(evt);
          processModelExportedEvent(evt);
          break;
        case "action_availability_changed":
          processActionAvailabilityChangedEvent(evt);
          break;
      }
    }
  }
}

function removeInactiveOverlay() {
  const section = document.getElementById('avatar-inactive-section');
  const overlay = section.querySelector('.avatar-inactive-overlay');
  if (overlay.style.display === 'block') {
    overlay.style.display = 'none';
  } else {
    // overlay.style.display = 'block';
  }
}

async function notifyServerAvatarExported() {
  return fetch("/api/avatarsdk/isready", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      'avatar_code': avatarData.avatarCode,
      'download_url': avatarData.downloadURL
    })
  });
}

function onExportClicked() {
  let iframe = document.getElementById("editor_iframe");
  let exportAvatarMessage = {
    "eventName": "export_avatar"
  };
  iframe.contentWindow.postMessage(exportAvatarMessage, "*");
  iframe.focus();
}

function displayLoadingStarted() {
  noticeBoard.textContent = "Picture is sent. Preparing Avatar..."
  // alert('displayLoadingStarted')
  }
  
  function displayLoadingCompleted() {
  noticeBoard.textContent = "Childhood-self is ready!"
  // alert('displayLoadingCompleted')
}

function setButtonClickability(button, clickabilitySetting) {
  button.disabled = !clickabilitySetting(button);
}

const ClickAbility = {
  ENABLE: _ => true,
  DISABLE: _ => false,
  TOGGLE: btn => !btn.disabled,
  SET: flag => (_ => flag),
}

async function onSaveAndPlayButtonClicked(event) {
  setButtonClickability(event.target, ClickAbility.DISABLE);
  displayLoadingStarted();
  onExportClicked();
  await avatarExportSemaphore.aquire();
  console.log("Contacting The server!", avatarData);
  const responsePromise = notifyServerAvatarExported();
  displayLoadingCompleted();
  if (appContext.DEBUG_MODE_ON) {
    const response = await responsePromise
    if (response.ok) {
      console.log("Successful Notification: ", avatarData);
      // alert("Nice")
    }
    // return;
  }
  window.location.href = "/game"
}

function onExitButtonClicked() {
  location.href = "/";
}

function onRestartButtonClicked() {
  location.replace(location.href);
}

const avatarData = {
  avatarCode: null,
  downloadURL: null,
};

let avatarExportSemaphore;

const credentials = fetchCredentials();

const Buttons = {
  saveAndPlay: null,
  exit: null,
  restart: null,
  generateAvatar: null,
}
let noticeBoard;

// The Main Code
//
document.addEventListener('DOMContentLoaded', function onDocumentReady() {
  avatarExportSemaphore = Semaphore.newInstance();
  window.addEventListener("message", onWindowMessage);
  document.getElementById('avatar-generation-form').addEventListener('submit', onGenerateAvatarClicked);

  Buttons.saveAndPlay = document.getElementById('avatar-editor-play-button');
  setButtonClickability(Buttons.saveAndPlay, ClickAbility.DISABLE);
  Buttons.saveAndPlay.addEventListener('click', onSaveAndPlayButtonClicked);

  Buttons.exit = document.getElementById('avatar-editor-exit-button');
  Buttons.exit.addEventListener('click', onExitButtonClicked);

  Buttons.restart = document.getElementById('avatar-editor-restart-button');
  Buttons.restart.addEventListener('click', onRestartButtonClicked);

  Buttons.generateAvatar = document.getElementById('avatar-generation-form-submit');

  noticeBoard = document.getElementById("avatar-notice-label");

  // Funky UI part...
  //
  setTimeout(() => {
    document.getElementById('avatar-overlay-shadow').classList.toggle('avatar-overlay-inset-shadow-applied');
  }, 1500);
});