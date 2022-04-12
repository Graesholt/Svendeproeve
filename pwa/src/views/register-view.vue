<template>
  <HeaderComponent />
  <Card>
    <template #title>
      <p class="center-text card-title">Registrer ny bruger</p>
    </template>
    <template #content>
      <p class="center-text">Brugernavn</p>
      <InputText type="text" class="center-input-field" v-model="refUsername" name="Username" v-on:keyup.enter="attemptRegistration" :disabled="refInputsDisabled" />
      <p class="center-text">Kodeord</p>
      <InputText type="password" class="center-input-field" v-model="refPassword" name="Password" v-on:keyup.enter="attemptRegistration" :disabled="refInputsDisabled" />
      <p class="center-text">Gentag kodeord</p>
      <InputText type="password" class="center-input-field" v-model="refPasswordConfirm" name="PasswordConfirm" v-on:keyup.enter="attemptRegistration" :disabled="refInputsDisabled" />
      <p v-if="refErrorDiv" class="registration-error-text-height center-text" v-bind:class="{ 'error-text': refErrorDivStatus == 'error', 'error-text-success': refErrorDivStatus == 'success' }">
        {{ refErrorDiv }}
      </p>
      <div v-else class="error-text-height"></div>
      <div class="center-div">
        <Button label="Registrer" value="Registrer" class="p-button-success left-button" @click="attemptRegistration" />
        <Button label="Log ind" value="Log ind" class="p-button-success p-button-outlined right-button" @click="router.push('/')" />
      </div>
    </template>
    <template #footer> </template>
  </Card>
</template>

<script async setup>
import axios from "axios";
import { ref } from "vue";
import { useRouter } from "vue-router";

const router = useRouter();

//Vue databindings
var refUsername = ref("");
var refPassword = ref("");
var refPasswordConfirm = ref("");
var refInputsDisabled = ref("");
var refErrorDiv = ref("");
var refErrorDivStatus = ref("");

refInputsDisabled.value = false; //Ready for input
refErrorDivStatus.value = "error"; //Set error div to error mode (red text)

async function attemptRegistration() {
  refInputsDisabled.value = true; //Lock input
  refErrorDiv.value = ""; //Wipe previous error message

  //User info validation happens again on server with the same checks
  //The checks below are faster for the user, instead of making a call for every attempt
  //They also enable the app to show valid information about why the info is not accepted
  //Server side is for extra security

  //Username validation
  if (refUsername.value == "") {
    //Blank username input
    refErrorDiv.value = "Brugernavn ikke indtastet";
    refInputsDisabled.value = false; //Unlock input
    return;
  }
  if (refUsername.value.length < 6) {
    //Username too short
    refErrorDiv.value = "Brugernavn skal være mindst 6 karakterer langt";
    refInputsDisabled.value = false; //Unlock input
    return;
  }
  if (refUsername.value.length > 30) {
    //Username too long
    refErrorDiv.value = "Brugernavn må ikke være længere end 30 karakterer";
    refInputsDisabled.value = false; //Unlock input
    return;
  }
  if (!refUsername.value.match(/^[a-zA-Z0-9-@._!?]+$/)) {
    //Username contains illegal character
    refErrorDiv.value = "Brugernavn må kun indeholde store og små bogstaver, tal, og tilladte specialtegn (som . - _ @ ! ? )";
    refInputsDisabled.value = false; //Unlock input
    return;
  }

  //Password validation
  if (refPassword.value == "") {
    //Blank password
    refErrorDiv.value = "Kodeord ikke indtastet";
    refInputsDisabled.value = false; //Unlock input
    return;
  }
  if (refPassword.value.length < 8) {
    //Password too short
    refErrorDiv.value = "Kodeord skal være mindst 8 karakterer langt";
    refInputsDisabled.value = false; //Unlock input
    return;
  }
  if (refPassword.value.length > 99) {
    //Password too long
    refErrorDiv.value = "Kodeord må ikke være længere end 99 karakterer";
    refInputsDisabled.value = false; //Unlock input
    return;
  }
  if (!refPassword.value.match(/.*[A-Z].*/)) {
    //Password does not include upper case letter
    refErrorDiv.value = "Kodeord skal inkludere mindst et stort bogstav";
    refInputsDisabled.value = false; //Unlock input
    return;
  }
  if (!refPassword.value.match(/.*[a-z].*/)) {
    //Password does not include lower case letter
    refErrorDiv.value = "Kodeord skal inkludere mindst et lille bogstav";
    refInputsDisabled.value = false; //Unlock input
    return;
  }
  if (!refPassword.value.match(/.*[0-9-@._!?].*/)) {
    //Password does not include capital letter
    refErrorDiv.value = "Kodeord skal inkludere mindst et tal eller specialtegn (som . - _ @ ! ? )";
    refInputsDisabled.value = false; //Unlock input
    return;
  }
  if (!refPassword.value.match(/^[a-zA-Z0-9-@._!?]+$/)) {
    //Password contains illegal character
    refErrorDiv.value = "Kodeord må kun indeholde store og små bogstaver, tal, og tilladte specialtegn (som . - _ @ ! ? )";
    refInputsDisabled.value = false; //Unlock input
    return;
  }
  if (refPassword.value != refPasswordConfirm.value) {
    //Passwords do not match
    refErrorDiv.value = "Kodeord er ikke ens";
    refInputsDisabled.value = false; //Unlock input
    return;
  }
  let user = { username: refUsername.value, password: refPassword.value };
  try {
    await axios.post(process.env.VUE_APP_API_URL + "api/User/Register", user);
    refErrorDivStatus.value = "success"; //Set error div to success mode (green text)
    refErrorDiv.value = "Bruger registreret!";
    await new Promise((r) => setTimeout(r, 2500)); //Wait a moment so user has time to see success message
    router.push("/"); //Send user back to Login page
  } catch (exception) {
    console.log(exception);
    if (exception.response.status == 409) {
      //If username already in use
      refErrorDiv.value = "Brugernavn er allerede registreret";
      refInputsDisabled.value = false; //Unlock input
    }
  }
}
</script>

<style scoped>
.page-header {
  margin-top: 4.5vh !important;
  margin-bottom: 0px !important;
}

.p-card {
  margin: auto;
  width: 50%;
  min-width: 300px;

  margin-top: 6vh;
}

.registration-error-text-height {
  /*height not locked, since error can be multi-line*/
  padding: 0;
  margin-top: 16px;
  margin-bottom: 16px;
}

.error-text-success {
  color: #22c55e;
}
</style>
