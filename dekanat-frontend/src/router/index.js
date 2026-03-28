import Vue from 'vue'
import VueRouter from 'vue-router'
import GroupsPage from '@/views/GroupsPage.vue'
import StudentsPage from '@/views/StudentsPage.vue'

Vue.use(VueRouter)

const routes = [
  {
    path: '/',
    name: 'groups',
    component: GroupsPage
  },
  {
    path: '/groups/:groupId/students',
    name: 'students',
    component: StudentsPage
  }
]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
})

export default router