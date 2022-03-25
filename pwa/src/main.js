import { createApp } from 'vue'
import App from './App.vue'
import './registerServiceWorker'
import router from './routes'
import Vuex from 'vuex'
//import Worker from 'web-worker';
//import axios from 'axios';

createApp(App).use(router).use(Vuex).mount('#app')


/* 
const worker = new Worker('data:,postMessage("hello")');

worker.onmessage = () => {
    setInterval(async() => {
        let user = { username: new Date().toUTCString(), password: "123qwe123" };
          await axios.post("https://webapi-uc0.conveyor.cloud/api/User/Register", user);
      }, 10000)
};
*/

