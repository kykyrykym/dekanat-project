<!-- eslint-disable -->
<template>
  <v-container>
    <v-btn @click="$router.back()" class="mb-4">
      <v-icon left>mdi-arrow-left</v-icon> Назад к группам
    </v-btn>

    <h1 class="text-h4 mb-4">Студенты группы {{ groupName }}</h1>

    <!-- Фильтры -->
    <v-row>
      <v-col cols="12" md="4">
        <v-select
          v-model="filters.gender"
          :items="genders"
          label="Пол"
          clearable
          @change="resetPageAndLoad"
        />
      </v-col>
      <v-col cols="12" md="4">
        <v-select
          v-model="filters.status"
          :items="statuses"
          item-text="text"
          item-value="value"
          label="Статус"
          clearable
          @change="resetPageAndLoad"
        />
      </v-col>
    </v-row>

    <!-- Индикатор загрузки -->
    <v-row v-if="loading">
      <v-col cols="12" class="text-center">
        <v-progress-circular indeterminate color="primary" size="64"></v-progress-circular>
        <p class="mt-2">Загрузка студентов...</p>
      </v-col>
    </v-row>

    <!-- Таблица -->
    <v-row v-else>
      <v-col cols="12">
        <v-card>
          <v-card-title>
            Найдено студентов: {{ students.length }} (всего: {{ totalStudents }})
          </v-card-title>
          <v-card-text>
            <v-data-table
              :headers="headers"
              :items="students"
              :loading="loading"
              class="elevation-1"
              hide-default-footer
            >
              <template v-slot:item.дата_Рождения="{ item }">
                {{ formatDate(item.дата_Рождения) }}
              </template>

              <template v-slot:item.actions="{ item }">
                <v-btn small color="primary" @click="openEditDialog(item)">
                  Редактировать
                </v-btn>
              </template>

              <template v-slot:no-data>
                <v-alert type="info" class="ma-4">Нет студентов в этой группе</v-alert>
              </template>
            </v-data-table>

            <!-- Пагинация -->
            <div class="text-center mt-4" v-if="totalStudents > 0">
              <v-pagination
                v-model="pagination.page"
                :length="totalPages"
                :total-visible="7"
                @input="loadStudents"
                circle
              ></v-pagination>
            </div>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <!-- МОДАЛЬНОЕ ОКНО ДЛЯ РЕДАКТИРОВАНИЯ -->
    <v-dialog v-model="dialog" max-width="500px">
      <v-card>
        <v-card-title>
          <span class="text-h5">Редактирование студента</span>
        </v-card-title>

        <v-card-text>
          <v-container>
            <v-row>
              <v-col cols="12">
                <v-text-field
                  v-model="editForm.fullName"
                  label="ФИО"
                  required
                ></v-text-field>
              </v-col>
              <v-col cols="12">
                <v-select
                  v-model="editForm.gender"
                  :items="genders"
                  label="Пол"
                ></v-select>
              </v-col>
              <v-col cols="12">
                <v-text-field
                  v-model="editForm.birthDate"
                  label="Дата рождения"
                  placeholder="ДД.ММ.ГГГГ"
                  maxlength="10"
                  @input="formatInputDate"
                ></v-text-field>
              </v-col>
              <v-col cols="12">
                <v-text-field
                  v-model="editForm.recordBook"
                  label="Номер зачётки"
                ></v-text-field>
              </v-col>
              <v-col cols="12">
                <v-text-field
                  v-model="editForm.averageGrade"
                  label="Средний балл"
                  type="number"
                  step="0.01"
                ></v-text-field>
              </v-col>
              <v-col cols="12">
                <v-select
                  v-model="editForm.status"
                  :items="statuses"
                  item-text="text"
                  item-value="value"
                  label="Статус"
                ></v-select>
              </v-col>
            </v-row>
          </v-container>
        </v-card-text>

        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="blue darken-1" text @click="dialog = false">Отмена</v-btn>
          <v-btn color="blue darken-1" text @click="saveStudent">Сохранить</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script>
import groupService from '@/services/groupService';

export default {
  name: 'StudentsPage',
  
  data() {
    return {
      groupId: null,
      groupName: '',
      students: [],
      totalStudents: 0,
      loading: false,
      
      filters: { gender: null, status: null },
      pagination: { page: 1, itemsPerPage: 10 },
      
      dialog: false,
      editingStudent: null,
      editForm: {
        fullName: '',
        gender: '',
        birthDate: '',
        recordBook: '',
        averageGrade: 0,
        status: 1
      },
      
      genders: ['Муж', 'Жен'],
      statuses: [
        { value: 0, text: 'Отчислен' },
        { value: 1, text: 'Учится' },
        { value: 2, text: 'Выпускник' },
        { value: 3, text: 'Академ' }
      ],
      
      headers: [
        { text: 'ФИО', value: 'фио', sortable: true },
        { text: 'Пол', value: 'пол', sortable: true },
        { text: 'Дата рождения', value: 'дата_Рождения', sortable: true },
        { text: 'Номер зачётки', value: 'номер_Зачетки', sortable: true },
        { text: 'Средний балл', value: 'средний_Балл', sortable: true },
        { text: 'Статус', value: 'статусТекст', sortable: true },
        { text: 'Действия', value: 'actions', sortable: false }
      ]
    };
  },

  computed: {
    totalPages() {
      return Math.ceil(this.totalStudents / this.pagination.itemsPerPage) || 1;
    }
  },

  mounted() {
    this.groupId = this.$route.params.groupId;
    console.log('groupId из URL:', this.groupId);
    
    if (!this.groupId) {
      console.error('Ошибка: groupId не передан!');
      return;
    }
    
    this.loadGroupName();
    this.loadStudents();
  },

  methods: {
    formatDate(dateString) {
      if (!dateString) return '';
      const date = new Date(dateString);
      const day = String(date.getDate()).padStart(2, '0');
      const month = String(date.getMonth() + 1).padStart(2, '0');
      const year = date.getFullYear();
      return `${day}.${month}.${year}`;
    },

    formatInputDate(value) {
      let cleaned = value.replace(/\D/g, '');
      if (cleaned.length > 8) cleaned = cleaned.slice(0, 8);
      let formatted = '';
      if (cleaned.length > 0) {
        formatted += cleaned.slice(0, 2);
      }
      if (cleaned.length >= 3) {
        formatted += '.' + cleaned.slice(2, 4);
      }
      if (cleaned.length >= 5) {
        formatted += '.' + cleaned.slice(4, 8);
      }
      if (formatted.length > 10) {
        formatted = formatted.slice(0, 10);
      }
      this.editForm.birthDate = formatted;
    },

    async loadGroupName() {
      try {
        const response = await groupService.getGroups({ page: 1, pageSize: 1000 });
        const group = response.data.find(g => g.id === parseInt(this.groupId));
        this.groupName = group ? group.groupName : 'неизвестной группы';
      } catch (error) {
        console.error('Ошибка загрузки названия группы:', error);
      }
    },

    resetPageAndLoad() {
      this.pagination.page = 1;
      this.loadStudents();
    },

    async loadStudents() {
      if (this.loading) return;
      
      this.loading = true;
      try {
        const params = {
          page: this.pagination.page,
          pageSize: this.pagination.itemsPerPage
        };
        
        if (this.filters.gender) params.gender = this.filters.gender;
        if (this.filters.status !== null && this.filters.status !== undefined) {
          params.status = parseInt(this.filters.status);
        }
        
        const response = await groupService.getStudentsByGroup(this.groupId, params);
        this.students = response.data;
        
        const totalCount = response.headers['x-total-count'];
        if (totalCount) {
          this.totalStudents = parseInt(totalCount);
        } else {
          this.totalStudents = this.students.length;
        }
        
      } catch (error) {
        console.error('Ошибка загрузки студентов:', error);
      } finally {
        this.loading = false;
      }
    },

    openEditDialog(student) {
      this.editingStudent = student;
      this.editForm = {
        fullName: student.фио || '',
        gender: student.пол || '',
        birthDate: student.дата_Рождения ? this.formatDate(student.дата_Рождения) : '',
        recordBook: student.номер_Зачетки || '',
        averageGrade: student.средний_Балл || 0,
        status: student.статус !== undefined ? student.статус : 1
      };
      this.dialog = true;
    },

    async saveStudent() {
      try {
        let birthDateForServer = '';
        if (this.editForm.birthDate) {
          const parts = this.editForm.birthDate.split('.');
          if (parts.length === 3) {
            birthDateForServer = `${parts[2]}-${parts[1]}-${parts[0]}`;
          }
        }
        
        const updateData = {
          ФИО: this.editForm.fullName,
          Пол: this.editForm.gender,
          Дата_Рождения: birthDateForServer,
          Номер_Зачетки: this.editForm.recordBook,
          Средний_Балл: parseFloat(this.editForm.averageGrade),
          Статус: parseInt(this.editForm.status)
        };
        
        console.log('Сохраняем:', updateData);
        
        const response = await groupService.updateStudent(this.editingStudent.id, updateData);
        console.log('Ответ сервера:', response.status);
        
        this.dialog = false;
        this.loadStudents();
      } catch (error) {
        console.error('Ошибка сохранения:', error);
        alert('Не удалось сохранить изменения');
      }
    }
  }
};
</script>