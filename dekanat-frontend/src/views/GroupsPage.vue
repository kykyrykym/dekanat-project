<template>
  <v-container fluid>
    <v-row>
      <v-col cols="12">
        <h1 class="text-h4 mb-4">Список учебных групп</h1>
      </v-col>
    </v-row>

    <!-- Фильтры -->
    <v-row>
      <v-col cols="12">
        <v-card class="pa-4 mb-4">
          <v-row>
            <v-col cols="12" md="3">
              <v-autocomplete
                v-model="filters.faculty"
                :items="faculties"
                label="Факультет"
                clearable
                dense
                outlined
                @change="resetPageAndLoad"
              ></v-autocomplete>
            </v-col>

            <v-col cols="12" md="2">
              <v-select
                v-model="filters.studyForm"
                :items="studyForms"
                item-text="name"
                item-value="code"
                label="Форма обучения"
                clearable
                dense
                outlined
                @change="resetPageAndLoad"
              ></v-select>
            </v-col>

            <v-col cols="12" md="2">
              <v-select
                v-model="filters.educationLevel"
                :items="educationLevels"
                item-text="name"
                item-value="code"
                label="Уровень образования"
                clearable
                dense
                outlined
                @change="resetPageAndLoad"
              ></v-select>
            </v-col>

            <v-col cols="12" md="2">
              <v-select
                v-model="filters.courses"
                :items="[1, 2, 3, 4, 5, 6]"
                label="Курс"
                multiple
                clearable
                dense
                outlined
                chips
                @change="resetPageAndLoad"
              ></v-select>
            </v-col>

            <v-col cols="12" md="3">
              <v-select
                v-model="filters.academicYear"
                :items="academicYears"
                label="Учебный год"
                clearable
                dense
                outlined
                @change="resetPageAndLoad"
              ></v-select>
            </v-col>
          </v-row>

          <v-row>
            <v-col cols="12" class="text-right">
              <v-btn color="primary" @click="resetFilters">
                <v-icon left>mdi-filter-remove</v-icon>
                Сбросить фильтры
              </v-btn>
            </v-col>
          </v-row>
        </v-card>
      </v-col>
    </v-row>

    <!-- Индикатор загрузки -->
    <v-row v-if="loading">
      <v-col cols="12" class="text-center">
        <v-progress-circular indeterminate color="primary" size="64"></v-progress-circular>
        <p class="mt-2">Загрузка данных...</p>
      </v-col>
    </v-row>

    <!-- Таблица и пагинация -->
    <v-row v-else>
      <v-col cols="12">
        <v-card>
          <v-card-title>
            Найдено групп: {{ groups.length }} (всего: {{ totalGroups }})
          </v-card-title>
          <v-card-text>
            <v-simple-table>
              <thead>
                <tr>
                  <th>Название группы</th>
                  <th>Факультет</th>
                  <th>Специальность</th>
                  <th>Курс</th>
                  <th>Форма обучения</th>
                  <th>Учебный год</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="group in groups" :key="group.id">
                  <td>{{ group.groupName }}</td>
                  <td>{{ group.facultyName || 'Не указан' }}</td>
                  <td>{{ group.specialty }}</td>
                  <td>{{ group.course }}</td>
                  <td>{{ group.studyFormName || 'Не указана' }}</td>
                  <td>{{ group.academicYear }}</td>
                </tr>
                <tr v-if="groups.length === 0">
                  <td colspan="6" class="text-center">Нет данных</td>
                </tr>
              </tbody>
            </v-simple-table>

            <!-- КНОПКИ ПАГИНАЦИИ -->
            <div class="text-center mt-4" v-if="totalGroups > 0">
              <v-pagination
                v-model="pagination.page"
                :length="totalPages"
                :total-visible="7"
                @input="loadGroups"
                circle
              ></v-pagination>
              <div class="text-caption text-grey mt-2">
                Страница {{ pagination.page }} из {{ totalPages }} 
                (показано {{ groups.length }} из {{ totalGroups }} групп)
              </div>
            </div>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import axios from 'axios';
import groupService from '@/services/groupService';

export default {
  name: 'GroupsPage',
  
  data() {
    return {
      groups: [],
      faculties: [],
      studyForms: [],
      educationLevels: [],
      academicYears: [],
      
      filters: {
        faculty: null,
        studyForm: null,
        educationLevel: null,
        courses: [],
        academicYear: null,
      },
      
      pagination: {
        page: 1,
        itemsPerPage: 50
      },
      totalGroups: 0,
      error: '',
      
      loading: false,
    };
  },

  computed: {
    // Общее количество страниц
    totalPages() {
      return Math.ceil(this.totalGroups / this.pagination.itemsPerPage) || 1;
    }
  },

  created() {
    this.loadFilterData();
    this.loadGroups();
  },

  methods: {
    async loadFilterData() {
      try {
        const [facultiesRes, studyFormsRes, educationLevelsRes, yearsRes] = await Promise.all([
          axios.get('http://localhost:5299/api/groups/faculties'),
          axios.get('http://localhost:5299/api/groups/studyforms'),
          axios.get('http://localhost:5299/api/groups/educationlevels'),
          axios.get('http://localhost:5299/api/groups/academicyears'),
        ]);
        
        this.faculties = facultiesRes.data;
        this.studyForms = studyFormsRes.data;
        this.educationLevels = educationLevelsRes.data;
        this.academicYears = yearsRes.data;
        
      } catch (error) {
        console.error('Ошибка загрузки фильтров:', error);
      }
    },

    async loadGroups() {
      this.loading = true;
      
      try {
        const params = {
          page: this.pagination.page,
          pageSize: this.pagination.itemsPerPage
        };
        
        if (this.filters.faculty) params.faculty = this.filters.faculty;
        if (this.filters.studyForm) params.studyForm = this.filters.studyForm;
        if (this.filters.educationLevel) params.educationLevel = this.filters.educationLevel;
        if (this.filters.courses?.length) params.courses = this.filters.courses.join(',');
        if (this.filters.academicYear) params.academicYear = this.filters.academicYear;
        
        const response = await groupService.getGroups(params);
        this.groups = response.data;
        
        const totalCount = response.headers['x-total-count'] || response.headers['X-Total-Count'];
        if (totalCount) {
          this.totalGroups = parseInt(totalCount);
        } else {
          this.totalGroups = this.groups.length;
        }
        
      } catch (error) {
        console.error('Ошибка загрузки групп:', error);
        this.error = 'Ошибка при загрузке групп';
      } finally {
        this.loading = false;
      }
    },

    // Сброс на первую страницу при изменении фильтров
    resetPageAndLoad() {
      this.pagination.page = 1;
      this.loadGroups();
    },

    resetFilters() {
      this.filters = {
        faculty: null,
        studyForm: null,
        educationLevel: null,
        courses: [],
        academicYear: null,
      };
      this.pagination.page = 1;
      this.loadGroups();
    },
  },
};
</script>

<style scoped>
/* Стили для пагинации */
.v-pagination {
  display: flex;
  justify-content: center;
}
</style>
