function formReadGender() {
  let ret = "male";
  const genderSpecified = document.getElementById("avatar-gender-select").value;
  if (genderSpecified) {
    ret = genderSpecified;
  }
  return ret;
}

function readFileAsBase64(file) {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = e => resolve(e.target.result.split(',')[1]);
    reader.onerror = reject;
    reader.readAsDataURL(file);
  });
}

async function formReadImage() {
  const file = document.getElementById('avatar-image-input').files[0];
  if (!file) {
    return null;
  }
  return await readFileAsBase64(file);
}

async function onGenerateAvatarClicked(event) {
  event.preventDefault();
  const generateAvatarMessage = {
    "eventName": "generate_avatar",
    "gender": formReadGender(),
    "age": "teen12",
    "image": await formReadImage()
  };
  if (eventSourceOfAvatarSDK && generateAvatarMessage["image"] && generateAvatarMessage["gender"]) {
    console.log("Message Posted")
    eventSourceOfAvatarSDK.postMessage(generateAvatarMessage, "*");
  } else if (appContext.DEBUG_MODE_ON) {
    console.log("Message is not posted:");
    console.log("event source of avatar sdk", eventSourceOfAvatarSDK)
    console.log("message obj: ", generateAvatarMessage);
  }
  displayLoadingStarted()
}
