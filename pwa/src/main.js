import { createApp } from 'vue'
import App from './App.vue'
import './registerServiceWorker'
import router from './router'
import store from './store'
import PrimeVue from 'primevue/config';
import VueChartkick from 'vue-chartkick'
import 'chartkick/chart.js'

const app = createApp(App);
app.use(store);
app.use(router);
app.use(PrimeVue);
app.use(VueChartkick)
app.mount('#app');

import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
app.component('DataTable', DataTable);
app.component('Column', Column);

import Button from 'primevue/button';
app.component('Button', Button);

import Card from "primevue/card";
app.component('Card', Card);
import InputText from 'primevue/inputtext';
app.component('InputText', InputText);

import 'primevue/resources/themes/saga-blue/theme.css'; //theme
import 'primevue/resources/primevue.min.css'; //core css
import 'primeicons/primeicons.css'; //icons

import './assets/leaflet.css'; //leaflet