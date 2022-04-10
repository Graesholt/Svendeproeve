import { createRouter, createWebHistory } from 'vue-router'
import login from "../views/login-view.vue";

const routes = [
  {
    path: "/",
    name: "login",
    component: login
},
{
    path: "/register",
    name: "register",
    component: () => import(/* webpackChunkName: "Runs" */ "../views/register-view.vue")
},
{
    path: "/newrun",
    name: "newrun",
    component: () => import(/* webpackChunkName: "Runs" */ "../views/newrun-view.vue")
},
{
    path: "/run/:runId",
    name: "run",
    component: () => import(/* webpackChunkName: "Runs" */ "../views/run-view.vue")
},
{
    path: "/runs",
    name: "runs",
    component: () => import(/* webpackChunkName: "Runs" */ "../views/runs-view.vue")
}
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})

//router.beforeEach((to, from, next) => {
//    let unProtectedRoutes = ["Auth"];
//    if (!unProtectedRoutes.includes(to.name) && !state.user.token) next({ name: "Auth" });
//    else next();
//});

export default router
