import { createRouter, createWebHistory } from 'vue-router'
import Register from '../views/Register.vue'
import Login from '../views/Login.vue'
import Index from '../views/Index.vue'
import UserProfile from '../views/UserProfile.vue';



const routes = [
  {
    path: '/register',
    name: 'Register',
    component: Register
  },
  {
    path: '/login',
    name: 'Login',
    component: Login
  },
  {
    path: '/',
    name: 'Index',
    component: Index
  },
  {
    path: '/profile',
    name: 'UserProfile',
    component: UserProfile
  },

]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes
})

export default router
