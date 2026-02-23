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
                @change="loadGroups"
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
                @change="loadGroups"
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
                @change="loadGroups"
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
                @change="loadGroups"
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
                @change="loadGroups"
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

    <!-- Таблица с данными -->
    <v-row v-else>
      <v-col cols="12">
        <v-card>
          <v-card-title>
            Найдено групп: {{ groups.length }}
          </v-card-title>
          <v-card-text>
            <v-simple-table>
              <template v-slot:default>
                <thead>
                  <tr>
                    <th>Название группы</th>
                    <th>Специальность</th>
                    <th>Курс</th>
                    <th>Учебный год</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="group in groups" :key="group.id">
                    <td>{{ group.groupName }}</td>
                    <td>{{ group.specialty }}</td>
                    <td>{{ group.course }}</td>
                    <td>{{ group.academicYear }}</td>
                  </tr>
                  <tr v-if="groups.length === 0">
                    <td colspan="4" class="text-center">Нет данных для отображения</td>
                  </tr>
                </tbody>
              </template>
            </v-simple-table>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import axios from 'axios';

export default {
  name: 'GroupsPage',
  
  data() {
    return {
      groups: [],
      faculties: ['Факультет информатики', 'Факультет экономики', 'Факультет медицины'],
      studyForms: [
        { code: 1, name: 'Очная' },
        { code: 2, name: 'Заочная' },
        { code: 3, name: 'Очно-заочная' }
      ],
      educationLevels: [
        { code: 1, name: 'Специалитет' },
        { code: 2, name: 'Бакалавриат' },
        { code: 3, name: 'Магистратура' }
      ],
      academicYears: ['2023-2024', '2024-2025', '2025-2026'],
      
      filters: {
        faculty: null,
        studyForm: null,
        educationLevel: null,
        courses: [],
        academicYear: null,
      },
      
      loading: false,
    };
  },

  created() {
    this.loadGroups();
  },

  methods: {
    async loadGroups() {
      this.loading = true;
      
      try {
        // Простой запрос без параметров
        const response = await axios.get('http://localhost:5299/api/groups');
        this.groups = response.data;
        console.log('Загружено групп:', this.groups.length);
      } catch (error) {
        console.error('Ошибка загрузки:', error);
      } finally {
        this.loading = false;
      }
    },

    resetFilters() {
      this.filters = {
        faculty: null,
        studyForm: null,
        educationLevel: null,
        courses: [],
        academicYear: null,
      };
      this.loadGroups();
    },
  },
};
</script>