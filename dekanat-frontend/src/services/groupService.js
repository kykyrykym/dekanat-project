import axios from 'axios';

// Базовый URL вашего backend API
const API_BASE_URL = 'http://localhost:5299/api';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export default {
  // Получение всех групп с фильтрацией
  getGroups(params = {}) {
    return apiClient.get('/groups', { params });
  },
  
  // Получение списка факультетов
  getFaculties() {
    return apiClient.get('/groups/faculties');
  },
  
  // Получение списка форм обучения
  getStudyForms() {
    return apiClient.get('/groups/studyforms');
  },
  
  // Получение списка уровней образования
  getEducationLevels() {
    return apiClient.get('/groups/educationlevels');
  },
  
  // Получение списка учебных годов
  getAcademicYears() {
    return apiClient.get('/groups/academicyears');
  },
  
  // Проверка подключения
  testConnection() {
    return apiClient.get('/groups/test');
  }
};