/* eslint-disable no-console */

import { register } from 'register-service-worker'
//import axios from "axios";

//if (process.env.NODE_ENV === 'production') {
  register(`${process.env.BASE_URL}service-worker.js`, {
    ready () {
      console.log(
        'App is being served from cache by a service worker.\n' +
        'For more details, visit https://goo.gl/AFskqB'
      )
    },
    registered () {
      console.log('Service worker has been registered.')
      /*
        setInterval(async() => {
        let user = { username: new Date().toUTCString(), password: "123qwe123" };
          await axios.post("https://webapi-uc0.conveyor.cloud/api/User/Register", user);
      }, 10000)
      */
    },
    cached () {
      console.log('Content has been cached for offline use.')
    },
    updatefound () {
      console.log('New content is downloading.')
    },
    updated () {
      console.log('New content is available; please refresh.')
    },
    offline () {
      console.log('No internet connection found. App is running in offline mode.')
    },
    error (error) {
      console.error('Error during service worker registration:', error)
    }
  })
//}
