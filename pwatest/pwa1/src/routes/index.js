import { createRouter, createWebHistory } from "vue-router";
import login from "../views/login-view.vue";

const routes = [
    {
        path: "/",
        name: "login-view",
        component: login
    },
    {
        path: "/register-view",
        name: "register-view",
        component: () => import(/* webpackChunkName: "Runs" */ "../views/register-view.vue")
    },
    {
        path: "/runs-view",
        name: "runs-view",
        component: () => import(/* webpackChunkName: "Runs" */ "../views/runs-view.vue")
    },
    {
        path: "/run-view",
        name: "run-view",
        component: () => import(/* webpackChunkName: "Runs" */ "../views/run-view.vue")
    }
];

const router = createRouter({
    history: createWebHistory(process.env.BASE_URL),
    routes
});

//router.beforeEach((to, from, next) => {
//    let unProtectedRoutes = ["Auth"];
//    if (!unProtectedRoutes.includes(to.name) && !state.user.token) next({ name: "Auth" });
//    else next();
//});

export default router;