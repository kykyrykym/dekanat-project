import Vue from 'vue'
import VueRouter from 'vue-router'
import GroupsPage from '@/views/GroupsPage.vue'

Vue.use(VueRouter)

const routes = [
  {
    path: '/',
    name: 'groups',
    component: GroupsPage
  }
]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
})

export default router