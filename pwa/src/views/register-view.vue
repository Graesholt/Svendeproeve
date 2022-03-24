<template>
    <div>Register</div>
    <label>Username</label><br />
    <input type="text" v-model="refUsername" name="Username" /><br />
    <label>Password</label><br />
    <input type="password" v-model="refPassword" name="Password" /><br />
    <label>Confirm password</label><br />
    <input type="password" v-model="refPasswordConfirm" name="PasswordConfirm" /><br />
    <div v-if="refErrorDiv">{{ refErrorDiv }}</div><br />
    <input type="button" value="Register" @click="attemptRegistration" /><br />
    <router-link to="/">Login</router-link>
</template>

<script async setup>
import axios from "axios";
import { ref } from "vue";
import { useRouter } from "vue-router";
const router = useRouter();

var refUsername = ref('');
var refPassword = ref('');
var refPasswordConfirm = ref('');
var refErrorDiv = ref('');

async function attemptRegistration() {
    if (refPassword.value != refPasswordConfirm.value)
    {
        //Password Error
        return
    }
    let user = {username: refUsername.value,password: refPassword.value}
    try
    {
    await axios .post("http://localhost:5268/api/User/Register", user)
          router.push('/');
    }
    catch(exception)
    {
        console.log(exception)
          if (exception.response.status == 409)
          {
              refErrorDiv.value = exception.response.data;
          }
    }
}

</script>

<style>
</style>