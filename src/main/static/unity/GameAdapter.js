let GameAdapter = null;

const CreateGameAdapter = () => {
  let __unityInstance = null;
  const encapsulatedGameAdapter = {
    setup: gameAdapterSetup,
    setUnityInstance:
      (unityInstance) => {
        __unityInstance = unityInstance;
      },
    getUnityInstance:
      () => __unityInstance,
  };
  GameAdapter = encapsulatedGameAdapter;
  console.log('Initialized Game Adapter successfully!')
  return GameAdapter;
};

/*
  Basic Interface with Unity
*/
function invokeUnity(objectName, methodName, value) {
  GameAdapter.getUnityInstance().SendMessage(objectName, methodName, value);
}

async function getDataJson(url) {
  const response = await fetch(url, {
    method: "POST",
    headers: {
      "Content-Type": "text/plain",
      "Content-Length": 0,
    },
    body: ""
  });
  if (!response.ok) {
    console.error('Network response was not ok');
    return "null";
  }
  return await response.text();
}

/*
  This funciton is called inside GamePlugin of WebGL build.
*/
async function checkSessionDetails() {
  const sessionDataSerialized = await getDataJson('/api/unity/session');
  console.log(sessionDataSerialized);
  if (sessionDataSerialized) {
    invokeUnity('SceneLoader', 'SessionDetailsResponseCallback', sessionDataSerialized)
  }
}

function getQualitySettingsFromSpecs() {
  const memory = navigator.deviceMemory || 4;
  const cores = navigator.hardwareConcurrency || 4;
  let qualityLevel = 'Medium';
  if (memory < 4 || cores < 4) {
    qualityLevel = 'Low';
  } else if (memory >= 8 && cores >= 8) {
    qualityLevel = 'High';
  }
  console.log("Quality settings for the game are: " + qualityLevel);
  return qualityLevel;
}

/*
  All browser-responsibility message invocations to Unity side.
*/
function gameAdapterSetup() {
  invokeUnity('StartupScreenSceneManager', 'SetQualitySettings', getQualitySettingsFromSpecs());
}