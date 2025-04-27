import { createRouter, createWebHistory } from "vue-router";
import Login from "@/pages/Login.vue";

const routes = [
  {
    path: "/home",
    name: "Home",
    component: () => import("@/pages/Home.vue"),
  },
  {
    path: "/login",
    name: "Login",
    component: Login,
  },
  {
    path: "/logindashboard",
    name: "DashboardLogin",
    component: () => import("@/pages/LoginDashboard.vue"),
  },
  {
    path: "/backoffice",
    name: "Backoffice",
    component: () => import("@/pages/Backoffice.vue"),
    meta: { requiresAuth: true },
  },
  {
    path: "/dashboard",
    name: "Dashboard",
    component: () => import("@/pages/Dashboard.vue"),
    meta: { requiresAuth: true },
  },
  {
    path: "/createaccount",
    name: "CreateAccount",
    component: () => import("@/pages/CreateAccount.vue"),
    meta: { requiresAuth: true },
  },
  {
    path: "/",
    redirect: "/home",
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach((to, from, next) => {
  const token = localStorage.getItem("token");
  if (to.meta.requiresAuth && !token) {
    next("/home");
  } else {
    next();
  }
});

export default router;
