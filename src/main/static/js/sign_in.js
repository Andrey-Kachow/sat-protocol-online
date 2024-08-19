const VISIBILITY_TRANSITION_DURATION = 1200;

const signInDialogUI = {};

function changeVisibility(classListActionAttr) {
  signInDialogUI.root.classList.toggle('signin-dialog-root-appear');
  setTimeout(() => {
    const isDisable = classListActionAttr === "remove";
    signInDialogUI.accountButton.disabled = isDisable;
    signInDialogUI.guestButton.disabled = isDisable;
  }, VISIBILITY_TRANSITION_DURATION);
}

const SigninPlugin = {

  onAccountButtonClick: (event) => {
    changeVisibility("remove");
    setTimeout(() => {
      signInDialogUI.root.remove();
    }, VISIBILITY_TRANSITION_DURATION + 800);
  },

  onGuestButtonClick: async (event) => {
    changeVisibility("remove");
    window.location.replace('/auth/guest')
  },

  appear: () => {
    changeVisibility("add");
  },
};

//
//
document.addEventListener('DOMContentLoaded', function () {
  signInDialogUI.root = document.getElementsByClassName('signin-dialog-root')[0];

  signInDialogUI.accountButton = document.getElementById('signin-btn-account');
  signInDialogUI.accountButton.addEventListener('click', SigninPlugin.onAccountButtonClick);

  signInDialogUI.guestButton = document.getElementById('signin-btn-guest');
  signInDialogUI.guestButton.addEventListener('click', SigninPlugin.onGuestButtonClick);

  setTimeout(SigninPlugin.appear, 500);
});